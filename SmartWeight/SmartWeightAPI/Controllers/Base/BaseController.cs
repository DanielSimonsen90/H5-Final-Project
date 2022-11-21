using Microsoft.AspNetCore.Mvc;
using SmartWeightLib.Database;
using SmartWeightLib.Models;
using SmartWeightLib.Models.Api;
using SmartWeightLib.Models.Data;

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
                type == MeasurementPartialTypes.PARTIAL_MEASUREMENT ? _context.Connections.FirstOrDefault(c => c.WeightId == id && c.IsConnected) :
                null;
            if (conn is null) return result;

            List<Measurement> measurements = _context.Measurements.ToList();
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

            foreach (Measurement measurement in partialMeasurements.Cast<Measurement>())
            {
                measurement.UserId = conn.UserId;
                _context.Entry(_context.Measurements.Find(measurement.Id)).CurrentValues.SetValues(measurement);
            }

            await _context.SaveChangesAsync();

            return Ok("Measurement saved.");
        }
    }
}
