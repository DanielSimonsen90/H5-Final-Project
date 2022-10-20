namespace SmartWeightLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WeightName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Weights", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Weights", "Name");
        }
    }
}
