namespace SmartWeightLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveWeightFromPartialMeasurement : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Measurements", "WeightId", "dbo.Weights");
            DropIndex("dbo.Measurements", new[] { "WeightId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Measurements", "WeightId");
            AddForeignKey("dbo.Measurements", "WeightId", "dbo.Weights", "Id", cascadeDelete: true);
        }
    }
}
