namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class quitDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consultants", "quit", c => c.Boolean(nullable: false));
            AddColumn("dbo.Consultants", "QuitDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Managers", "quit", c => c.Boolean(nullable: false));
            AddColumn("dbo.Managers", "QuitDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Managers", "QuitDate");
            DropColumn("dbo.Managers", "quit");
            DropColumn("dbo.Consultants", "QuitDate");
            DropColumn("dbo.Consultants", "quit");
        }
    }
}
