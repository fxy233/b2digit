namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supauit : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Consultants", "QuitDate");
            DropColumn("dbo.Managers", "QuitDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Managers", "QuitDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Consultants", "QuitDate", c => c.DateTime(nullable: false));
        }
    }
}
