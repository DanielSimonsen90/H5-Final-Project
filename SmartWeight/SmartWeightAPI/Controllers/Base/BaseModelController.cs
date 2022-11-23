using Microsoft.AspNetCore.Mvc;
using SmartWeightLib.Database;
using SmartWeightLib.Models.Data;

namespace SmartWeightAPI.Controllers.Base
{
    //[Route("api/[controller]")]
    [ApiController]
    public abstract class BaseModelController<Entity> : BaseController where Entity : IDbItem
    {
        private readonly string entityName = nameof(Entity).ToLower();

        protected BaseModelController(SmartWeightDbContext context) : base(context) { }

        protected abstract void AddEntity(Entity entity);
        protected abstract bool EntityExists(Entity entity);
        protected abstract Entity? GetEntity(int id);
        protected abstract List<Entity> GetEntities();
        protected abstract void DeleteEntity(Entity entity);

        [HttpPost]
        public async virtual Task<IActionResult> Create([FromBody] Entity entity)
        {
            if (!ModelState.IsValid) return BadRequest($"Provided {entityName} {nameof(entity)} is invalid.");
            else if (EntityExists(entity)) return BadRequest($"Provided {entityName} {nameof(entity)} already exists.");

            AddEntity(entity);
            await _context.SaveChangesAsync();

            return Created($"{nameof(entity)} created", entity);
        }

        [HttpGet]
        public async virtual Task<ActionResult<List<Entity>>> GetAll() => await Task.FromResult<ActionResult<List<Entity>>>(Ok(GetEntities()));

        [HttpGet("{id}")]
        public async virtual Task<IActionResult> GetOne(int id)
        {
            Entity? entity = GetEntity(id);

            return await Task.FromResult<IActionResult>(entity is null ?
                NotFound($"No {entityName} found with id {id}") :
                Ok(entity));
        }


        [HttpPut("{id}")]
        public async virtual Task<IActionResult> Update(int id, [FromBody] Entity entity)
        {
            if (!ModelState.IsValid) return BadRequest($"Provided {entityName} {nameof(entity)} is invalid.");
            else if (entity.Id != id) return BadRequest($"Id mismatch between {entityName} {entity.Id} and parameter {id}.");

            Entity? oldEntity = GetEntity(id);

            if (oldEntity is null) return NotFound($"No {entityName} with that id");

            _context.Entry(oldEntity).CurrentValues.SetValues(entity);
            _context.SaveChanges();

            return await Task.FromResult<IActionResult>(Ok($"{entityName} updated."));
        }

        [HttpDelete("{id}")]
        public async virtual Task<IActionResult> Delete(int id)
        {
            Entity? entity = GetEntity(id);

            if (entity is null) return NotFound($"No {entityName} with that id");

            DeleteEntity(entity);
            _context.SaveChanges();

            return await Task.FromResult<IActionResult>(Ok($"{entityName} deleted."));
        }
    }
}
