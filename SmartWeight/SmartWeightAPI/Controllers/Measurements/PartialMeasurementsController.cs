using MessagePack.Formatters;
using Microsoft.AspNetCore.Mvc;
using SmartWeightAPI.Controllers.Base;
using SmartWeightLib.Database;
using SmartWeightLib.Models.Data;
using System.Diagnostics.Metrics;
using System.Timers;

namespace SmartWeightAPI.Controllers.Measurements
{
    [Route("api/measurements/partials")]
    public class PartialMeasurementsController : BaseModelController<PartialMeasurement>
    {
        private static readonly int MEASUREMENTS_TIME_MS = 1000 * 60 * 10; // Every 10 minutes
        public PartialMeasurementsController(SmartWeightDbContext context) : base(context)
        {
            var timer = new System.Timers.Timer(MEASUREMENTS_TIME_MS);
            timer.Elapsed += OnIdleTick;
            timer.Start();
        }

        [HttpDelete("{aggregated}")]
        public IActionResult DeleteMany(string aggregated)
        {
            IEnumerable<int> ids = aggregated.Split('-').Select(id => int.Parse(id));
            IEnumerable<PartialMeasurement> partialMeasurements = GetEntities()
                .Where(m => ids.Contains(m.Id));

            if (!partialMeasurements.Any()) return NotFound($"No ids matching collection {aggregated}. Did you forget to seperate with \"-\"?");

            _context.Measurements.RemoveRange(partialMeasurements as IEnumerable<Measurement>);
            _context.SaveChanges();

            return Ok($"Partial measurements deleted.");
        }

        protected override void AddEntity(PartialMeasurement entity) => _context.Measurements.Add(new Measurement(entity, null));
        protected override bool EntityExists(PartialMeasurement entity) => _context.Measurements.Any(m => 
            m.WeightId == entity.WeightId
            && m.Value == entity.Value
            && m.Date == entity.Date);
        protected override List<PartialMeasurement> GetEntities() => _context.Measurements
            .Where(m => m.UserId == null)
            .ToList<PartialMeasurement>();
        protected override PartialMeasurement? GetEntity(int id) => _context.Measurements.Find(id);
        protected override void DeleteEntity(PartialMeasurement entity) => _context.Measurements.Remove(GetEntity(entity.Id) as Measurement);

        public override async Task<IActionResult> Create([FromBody] PartialMeasurement entity)
        {
            if (entity is null) return BadRequest($"{nameof(PartialMeasurement)} has invalid values.");
            else if (_context.Weights.Find(entity.WeightId) is null) return NotFound($"Weight {entity.WeightId} does not exist.");

            // Use base create method. If it fails, return error
            IActionResult result = await base.Create(entity);

            return result is CreatedResult ?
                await HandleMeasurement(entity.WeightId, MeasurementPartialTypes.PARTIAL_MEASUREMENT, result) :
                result;
        }

        private void OnIdleTick(object? state, ElapsedEventArgs args)
        {
            // No partial measurements
            if (GetEntities().Count == 0) return;

            // Select partial entries that are 10+ minutes old
            List<Measurement> partials = _context.Measurements
                .Where(m => m.Date < DateTime.Now.AddMilliseconds(MEASUREMENTS_TIME_MS))
                .ToList();

            // Partials saved are less than 10 minutes old, and therefore don't need to be removed just yet
            if (!partials.Any()) return;

            // Remove all partials, that exceed limit
            _context.Measurements.RemoveRange(partials);
            _context.SaveChangesAsync();
        }
    }
}
