namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class odmAjouter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrdreDeMissions", "Mission", c => c.String());
            AddColumn("dbo.OrdreDeMissions", "manager", c => c.String());
            AddColumn("dbo.OrdreDeMissions", "dateDebut", c => c.DateTime(nullable: false));
            AddColumn("dbo.OrdreDeMissions", "dataFin", c => c.DateTime(nullable: false));
            AddColumn("dbo.OrdreDeMissions", "nomConsultant", c => c.String());
            AddColumn("dbo.OrdreDeMissions", "prenomConsultant", c => c.String());
            AddColumn("dbo.OrdreDeMissions", "nomClient", c => c.String());
            AddColumn("dbo.OrdreDeMissions", "contactClient", c => c.String());
            AddColumn("dbo.OrdreDeMissions", "missionAdresse", c => c.String());
            DropColumn("dbo.OrdreDeMissions", "MissionID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrdreDeMissions", "MissionID", c => c.Int(nullable: false));
            DropColumn("dbo.OrdreDeMissions", "missionAdresse");
            DropColumn("dbo.OrdreDeMissions", "contactClient");
            DropColumn("dbo.OrdreDeMissions", "nomClient");
            DropColumn("dbo.OrdreDeMissions", "prenomConsultant");
            DropColumn("dbo.OrdreDeMissions", "nomConsultant");
            DropColumn("dbo.OrdreDeMissions", "dataFin");
            DropColumn("dbo.OrdreDeMissions", "dateDebut");
            DropColumn("dbo.OrdreDeMissions", "manager");
            DropColumn("dbo.OrdreDeMissions", "Mission");
        }
    }
}
