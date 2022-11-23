using Microsoft.AspNetCore.Mvc;
using SmartWeightAPI.Controllers.Base;
using SmartWeightLib.Database;
using SmartWeightLib.Models.Data;

namespace SmartWeightAPI.Controllers.Measurements
{
    [Route("api/measurements")]
    public class MeasurementsController : BaseModelController<Measurement>
    {
        public MeasurementsController(SmartWeightDbContext context) : base(context)
        {

        }

        protected override void AddEntity(Measurement entity) => _context.Measurements.Add(entity);
        protected override bool EntityExists(Measurement entity) => _context.Measurements.Any(m =>
            m.WeightId == entity.WeightId
            && m.Value == entity.Value
            && m.Date == entity.Date
            && m.UserId == entity.UserId);
        protected override List<Measurement> GetEntities() => _context.Measurements.ToList();
        protected override Measurement? GetEntity(int id) => _context.Measurements.Find(id);
        protected override void DeleteEntity(Measurement entity) => _context.Measurements.Remove(entity);

        [HttpPost("collection")]
        public async Task<IActionResult> CreateMany([FromBody] Measurement[] measurements)
        {
            if (!ModelState.IsValid) return BadRequest($"Provided measurements are invalid.");

            _context.Measurements.AddRange(measurements);
            await _context.SaveChangesAsync();

            return Created($"Measurements created", measurements);
        }
        
        [HttpGet("overview/{userId}")]
        public IActionResult Overview(int userId)
        {
            List<Measurement>? measurements = _context.Measurements
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.Date)
                .ToList();
            measurements.Reverse();

            return measurements is null || measurements.Count == 0 ?
                NotFound($"No measurements found for user {userId}") :
                Ok(measurements);
        }

        [HttpGet("all")]
        public ActionResult<List<Measurement>> GetAll(MeasurementFilter filter = MeasurementFilter.ALL)
        {
            return Ok(GetEntities().Where(measurement => filter switch
            {
                MeasurementFilter.FULL => measurement.UserId is not null,
                MeasurementFilter.PARTIALS => measurement.UserId is null,
                _ => true
            }).ToList());
        }

        [HttpDelete("all")]
        public ActionResult DeleteAll(MeasurementFilter filter = MeasurementFilter.ALL)
        {
            ActionResult? getAllResult = GetAll(filter).Result;
            if (getAllResult is null || getAllResult is not OkObjectResult resolvedAll) return NotFound($"GET /all did not return expected result");

            List<Measurement> measurements = resolvedAll.Value as List<Measurement> ?? new List<Measurement>();
            string? filterName = Enum.GetName(typeof(MeasurementFilter), filter) ?? "unknown filter";

            if (!measurements.Any()) return NotFound($"No {nameof(Measurement)}s to delete using filter {filterName}.");

            _context.Measurements.RemoveRange(measurements);
            _context.SaveChanges();

            return Ok($"Deleted all {filterName} {nameof(Measurement)}s.");
        }
    }
}
