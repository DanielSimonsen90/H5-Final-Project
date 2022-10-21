using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using SmartWeightAPI.Controllers.Measurements;
using SmartWeightLib.Database;
using SmartWeightLib.Models;
using SmartWeightLib.Models.Api;
using SmartWeightLib.Models.Data;
using System.Net;
using System.Text;

namespace SmartWeightAPI.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly SmartWeightDbContext _context;
        protected readonly ApiClient _client = new();

        public BaseController(SmartWeightDbContext context)
        {
            _context = context;
        }

        protected Task<SimpleResponse> PostAsync<T>(Endpoints endpoint, string path, T entity) => _client.Post(endpoint, path, entity);
        protected Task<SimpleResponse> DeleteAsync(Endpoints endpoint, string path) => _client.Delete(endpoint, path);

        /// <summary>
        /// Checks if there's a connection between any ids, and if so, add the measurement to the database
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="type">user | weight</param>
        protected async Task<IActionResult> HandleMeasurement(int id, MeasurementPartialTypes type, IActionResult result)
        {
            // Is weight connected to a user, if not, then maybe user connects after weight was used
            Connection? conn =
                type == MeasurementPartialTypes.USER ? _context.Connections.FirstOrDefault(c => c.UserId == id) :
                type == MeasurementPartialTypes.PARTIAL_MEASUREMENT ? _context.Connections.FirstOrDefault(c => c.WeightId == id) :
                null;
            if (conn is null) return result;

            var measurements = _context.Measurements.ToList();
            // Get partial measurements, if any
            List<PartialMeasurement> partialMeasurements = measurements
                .Where(m => m.UserId == null 
                    && m.WeightId == conn.WeightId)
                .ToList<PartialMeasurement>();
            if (!partialMeasurements.Any()) return result;

            // Delete connection regardless of what happens next
            await DeleteAsync(Endpoints.CONNECTIONS, $"{conn.UserId}?fromApp=true");

            // Get user from connection
            if (_context.Users.Find(conn.UserId) is null) return StatusCode(
                StatusCodes.Status500InternalServerError, 
                "User not found - connection is corrupted.");

            SimpleResponse? posted = null;
            if (partialMeasurements.Count == 1)
            {
                PartialMeasurement partialMeasurement = partialMeasurements.First();

                // Save measurement
                posted = await PostAsync(Endpoints.MEASUREMENTS, string.Empty, new Measurement(
                    conn.UserId,
                    partialMeasurement.WeightId,
                    partialMeasurement.Value,
                    partialMeasurement.Date));

                // Delete partial measurement
                await DeleteAsync(Endpoints.MEASUREMENTS_PARTIALS, $"{partialMeasurement.Id}");
            }
            else
            {
                // Save measurements
                posted = await PostAsync(Endpoints.MEASUREMENTS, "collection", partialMeasurements.Select(m => new Measurement(
                    conn.UserId,
                    m.WeightId,
                    m.Value,
                    m.Date)).ToList());

                // From all partialMeasurements, select each Id and convert them into 1 string, where each id is seperated by "-": 1-2-3-
                string partialMeasurementIdStrings = partialMeasurements
                    .Select(m => m.Id)
                    .Aggregate(new StringBuilder(), (acc, id) => acc.Append($"{id}-"))
                    .ToString();
                await DeleteAsync(Endpoints.MEASUREMENTS_PARTIALS, $"collection/{partialMeasurementIdStrings}");
            }


            return posted.StatusCode == HttpStatusCode.Created ?
                Ok("Measurement saved.") :
                StatusCode(StatusCodes.Status500InternalServerError, 
                    $"Failed to save measurement:\n\n{posted.Message}");
        }
    }
}
