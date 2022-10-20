using Microsoft.AspNetCore.Mvc;
using SmartWeightAPI.Controllers.Base;
using SmartWeightLib.Database;
using SmartWeightLib.Models.Data;

namespace SmartWeightAPI.Controllers
{
    [Route("api/users")]
    public class UsersController : BaseModelController<User>
    {
        public UsersController(SmartWeightDbContext context) : base(context) {}

        protected override void AddEntity(User entity) => _context.Users.Add(entity);
        protected override List<User> GetEntities() => _context.Users.ToList();
        protected override User? GetEntity(int id) => _context.Users.Find(id);
        protected override void DeleteEntity(User entity) => _context.Users.Remove(entity);

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user) => 
            !ModelState.IsValid ? BadRequest($"User model is invalid") :
            !_context.Users.Any(u => u.Username == user.Username && u.Password == user.Password) ? NotFound("Invalid username or password") :
            Ok("Successful login.");

        [HttpDelete("login/{userId}")]
        public IActionResult Logout(int userId)
        {
            IQueryable<Connection> conn = _context.Connections.Where(c => c.UserId == userId);
            if (conn.Any()) _context.Connections.RemoveRange(conn);

            return Ok("Logged out");
        }
    }
}
