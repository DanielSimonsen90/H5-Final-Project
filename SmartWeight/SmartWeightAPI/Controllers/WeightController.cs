using Microsoft.AspNetCore.Mvc;
using SmartWeightAPI.Controllers.Base;
using SmartWeightLib.Database;
using SmartWeightLib.Models.Data;

namespace SmartWeightAPI.Controllers
{
    [Route("api/weights")]
    public class WeightController : BaseModelController<Weight>
    {
        public WeightController(SmartWeightDbContext context) : base(context) {}

        protected override void AddEntity(Weight entity) => _context.Weights.Add(entity);
        protected override List<Weight> GetEntities() => _context.Weights.ToList();
        protected override Weight? GetEntity(int id) => _context.Weights.Find(id);
        protected override void DeleteEntity(Weight entity) => _context.Weights.Remove(entity);
    }
}
