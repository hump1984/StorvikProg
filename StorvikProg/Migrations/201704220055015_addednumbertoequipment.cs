namespace StorvikProg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednumbertoequipment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Equipments", "Number", c => c.Int(nullable: false));
            AlterColumn("dbo.Equipments", "Serial", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Equipments", "Serial", c => c.String(nullable: false));
            DropColumn("dbo.Equipments", "Number");
        }
    }
}
