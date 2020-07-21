namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ajouteFactureID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subsidiaries", "FactureID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subsidiaries", "FactureID");
        }
    }
}
