namespace StorvikProg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedProjectNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "Number", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "Number");
        }
    }
}
