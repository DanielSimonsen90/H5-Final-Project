namespace SmartWeightLib.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SmartWeightLib.Database.SmartWeightDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(SmartWeightLib.Database.SmartWeightDbContext context)
        {

        }
    }
}
