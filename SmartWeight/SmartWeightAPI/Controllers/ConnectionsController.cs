using Microsoft.AspNetCore.Mvc;
using SmartWeightAPI.Controllers.Base;
using SmartWeightLib.Database;
using SmartWeightLib.Models.Data;

namespace SmartWeightAPI.Controllers
{
    [Route("api/connections/{userId}")]
    [ApiController]
    public class ConnectionsController : BaseController
    {
        public ConnectionsController(SmartWeightDbContext context) : base(context) 
        {
            // Remove all connections on restart
            _context.Connections.RemoveRange(_context.Connections.ToList());
            _context.SaveChanges();
        }

        [HttpPost("{weightId}")]
        public async Task<IActionResult> Connect(int weightId, int userId)
        {
            // Arguments provided are existing entities
            Weight? weight = _context.Weights.Find(weightId);
            User? user = _context.Users.Find(userId);
            if (weight is null || user is null) return NotFound("One or more entities not found");

            // Get previous connections
            Connection? userConnection = _context.Connections.Find(userId);
            Connection? weightConnection = _context.Connections.Find(weightId);

            // Connections exist between entities
            if (userConnection == weightConnection && userConnection is not null) return Ok("Connection was already established.");

            // Remove any previous connections, if any
            if (userConnection is not null) _context.Connections.Remove(userConnection);
            if (weightConnection is not null) _context.Connections.Remove(weightConnection);

            // Create connection and save
            var conn = new Connection(user, weight);
            _context.Connections.Add(conn);
            await _context.SaveChangesAsync();

            CreatedResult result = Created("connections/userId/weightId", conn);
            return await HandleMeasurement(userId, MeasurementPartialTypes.USER, result);
        }

        [HttpGet("all")]
        public IActionResult GetAllConnections(int userId = -1)
        {
            return Ok(_context.Connections.ToList());
        }

        [HttpGet]
        public IActionResult GetConnection(int userId, bool fromApp)
        {
            if (!fromApp) return Forbid("You are not allowed to view this information.");

            Connection? conn = _context.Connections.Find(userId);
            if (conn is null) return NotFound("User is not connected to any weight.");

            return Ok(conn);
        }

        [HttpDelete]
        public IActionResult Disconnect(int userId, bool fromApp)
        {
            if (!fromApp) return Forbid("You are not allowed to delete this connection.");

            Connection? conn = _context.Connections.ToList().Find(c => c.UserId == userId);
            if (conn is null) return Ok("Connection already deleted");

            _context.Connections.Remove(conn);
            _context.SaveChangesAsync();
            return Ok("Connection deleted");
        }
    }
}
