namespace SmartWeightLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Connections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        WeightId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Weights", t => t.WeightId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.WeightId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Measurements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        WeightId = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Weights", t => t.WeightId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.WeightId);
            
            CreateTable(
                "dbo.Weights",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Connections", "WeightId", "dbo.Weights");
            DropForeignKey("dbo.Connections", "UserId", "dbo.Users");
            DropForeignKey("dbo.Measurements", "WeightId", "dbo.Weights");
            DropForeignKey("dbo.Measurements", "UserId", "dbo.Users");
            DropIndex("dbo.Measurements", new[] { "WeightId" });
            DropIndex("dbo.Measurements", new[] { "UserId" });
            DropIndex("dbo.Connections", new[] { "WeightId" });
            DropIndex("dbo.Connections", new[] { "UserId" });
            DropTable("dbo.Weights");
            DropTable("dbo.Measurements");
            DropTable("dbo.Users");
            DropTable("dbo.Connections");
        }
    }
}
