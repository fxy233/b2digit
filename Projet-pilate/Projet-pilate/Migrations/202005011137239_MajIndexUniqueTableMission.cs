namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MajIndexUniqueTableMission : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Missions", "Mission_Unique_Index");
            DropIndex("dbo.Missions", new[] { "ConsultantID" });
            DropIndex("dbo.Missions", new[] { "CompanyContactID" });
            DropIndex("dbo.Missions", new[] { "ProfitCenterID" });
            CreateIndex("dbo.Missions", new[] { "Name", "Start", "End", "ConsultantID", "CompanyContactID", "ProfitCenterID" }, unique: true, name: "Mission_Unique_Index");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Missions", "Mission_Unique_Index");
            CreateIndex("dbo.Missions", "ProfitCenterID");
            CreateIndex("dbo.Missions", "CompanyContactID");
            CreateIndex("dbo.Missions", "ConsultantID");
            CreateIndex("dbo.Missions", new[] { "Name", "Start", "End" }, unique: true, name: "Mission_Unique_Index");
        }
    }
}
