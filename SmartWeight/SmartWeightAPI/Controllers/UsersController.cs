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
        protected override bool EntityExists(User entity) => _context.Users.Any(u => 
            u.Username == entity.Username
            && u.Password == entity.Password);
        protected override List<User> GetEntities() => _context.Users.ToList();
        protected override User? GetEntity(int id) => _context.Users.Find(id);
        protected override void DeleteEntity(User entity) => _context.Users.Remove(entity);

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            if (!ModelState.IsValid) return BadRequest($"User model is invalid");
            
            User? user = _context.Users.FirstOrDefault(u => 
                u.Username == login.Username 
                && u.Password == login.Password);
            
            return user is not null ? Ok(user) : NotFound("Invalid username or password");
        }
        
        [HttpDelete("login/{userId}")]
        public IActionResult Logout(int userId)
        {
            IQueryable<Connection> conn = _context.Connections.Where(c => c.UserId == userId);
            if (conn.Any()) _context.Connections.RemoveRange(conn);

            return Ok("Logged out");
        }
    }
}
