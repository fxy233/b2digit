namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class frais1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrdreDeMissions", "MissionID", c => c.Int(nullable: false));
            DropColumn("dbo.OrdreDeMissions", "NomMission");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrdreDeMissions", "NomMission", c => c.String());
            DropColumn("dbo.OrdreDeMissions", "MissionID");
        }
    }
}
