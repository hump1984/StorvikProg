namespace StorvikProg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHourModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hours",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        RegDate = c.DateTime(nullable: false),
                        Hours = c.Single(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        EquipmentId = c.Int(nullable: false),
                        Comment = c.String(),
                        Controlled = c.Boolean(nullable: false),
                        Approved = c.Boolean(nullable: false),
                        Billed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Equipments", t => t.EquipmentId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.ProjectId)
                .Index(t => t.EquipmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hours", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Hours", "EquipmentId", "dbo.Equipments");
            DropForeignKey("dbo.Hours", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Hours", new[] { "EquipmentId" });
            DropIndex("dbo.Hours", new[] { "ProjectId" });
            DropIndex("dbo.Hours", new[] { "EmployeeId" });
            DropTable("dbo.Hours");
        }
    }
}
