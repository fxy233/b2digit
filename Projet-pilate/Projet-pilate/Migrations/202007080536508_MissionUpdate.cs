namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MissionUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Missions", "InfoFacturation", c => c.String());
            AddColumn("dbo.Missions", "PrincipalBCID", c => c.Int(nullable: false));
            AddColumn("dbo.Missions", "InterBC1ID", c => c.Int(nullable: false));
            AddColumn("dbo.Missions", "TJInterBC1", c => c.Single(nullable: false));
            AddColumn("dbo.Missions", "InterBC2ID", c => c.Int(nullable: false));
            AddColumn("dbo.Missions", "TJInterBC2", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Missions", "TJInterBC2");
            DropColumn("dbo.Missions", "InterBC2ID");
            DropColumn("dbo.Missions", "TJInterBC1");
            DropColumn("dbo.Missions", "InterBC1ID");
            DropColumn("dbo.Missions", "PrincipalBCID");
            DropColumn("dbo.Missions", "InfoFacturation");
        }
    }
}
