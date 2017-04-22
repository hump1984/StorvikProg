namespace StorvikProg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedHoursinhourtodouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Hours", "Hours", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Hours", "Hours", c => c.Single(nullable: false));
        }
    }
}
