using SmartWeightLib.Models.Data;
using System.Data.Entity;
using System.Text;
#nullable disable

namespace SmartWeightLib.Database
{
    public class SmartWeightDbContext : DbContext
    {
        public SmartWeightDbContext() : base(new List<string>
        {
            "Server=(localdb)\\MSSqlLocalDb",
            "Database=SmartWeightDB",
            "Trusted_Connection=true",
            "MultipleActiveResultSets=true"
        }.Aggregate((acc, cur) => acc += cur + ';')) {}
        public SmartWeightDbContext(string connectionString) : base(connectionString) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Weight> Weights { get; set; }
    }
}
