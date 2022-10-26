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
            if (weight is null || user is null) return NotFound("One or more entities not found.");

            // Get previous connections
            List<Connection> connections = _context.Connections.ToList();
            Connection? conn = connections.Find(c => c.UserId == userId && c.WeightId == weightId);

            // Connections exist between entities
            if (conn is not null && conn.IsConnected) return Ok("Connection was already established.");

            // Remove any previous connections, if any
            Connection? userConnection = connections
                .Where(c => c.UserId == userId && c.IsConnected)
                .FirstOrDefault();
            Connection? weightConnection = connections
                .Where(c => c.WeightId == weightId && c.IsConnected)
                .FirstOrDefault();

            if (userConnection is not null) userConnection.IsConnected = false;
            if (weightConnection is not null) weightConnection.IsConnected = false;

            // Create connection and save
            if (conn is null)
            {
                conn = new Connection(user, weight, true);
                _context.Connections.Add(conn);
            }
            else conn.IsConnected = true;
            
            await _context.SaveChangesAsync();

            CreatedResult result = Created("connections/userId/weightId", conn);
            return await HandleMeasurement(userId, MeasurementPartialTypes.USER, result);
        }

        [HttpGet("all")]
        public IActionResult GetAllConnections(int userId = -1) => Ok(_context.Connections.ToList());

        [HttpGet]
        public IActionResult GetConnection(int userId, bool fromApp)
        {
            if (!fromApp) return Forbid("You are not allowed to view this information.");

            Connection? conn = _context.Connections.Find(userId);
            if (conn is null || !conn.IsConnected) return NotFound("User is not connected to any weight.");

            return Ok(conn);
        }

        [HttpDelete]
        public IActionResult Disconnect(int userId, bool fromApp, bool delete)
        {
            if (!fromApp) return Forbid("You are not allowed to delete this connection.");
            string response = "Connection already deleted";

            Connection? conn = _context.Connections.ToList().Find(c => c.UserId == userId);
            if (conn is null) return Ok("Connection already deleted.");
            else if (delete)
            {
                _context.Connections.Remove(conn);
                response = "Connection deleted.";
            }
            else
            {
                conn.IsConnected = false;
                response = $"Disconnected user {userId} from weight {conn.WeightId}.";
            }
            
            _context.SaveChangesAsync();
            return Ok(response);
        }
    }
}
