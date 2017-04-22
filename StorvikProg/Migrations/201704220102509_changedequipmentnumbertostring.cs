namespace StorvikProg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedequipmentnumbertostring : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Equipments", "Number", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Equipments", "Number", c => c.Int(nullable: false));
        }
    }
}
