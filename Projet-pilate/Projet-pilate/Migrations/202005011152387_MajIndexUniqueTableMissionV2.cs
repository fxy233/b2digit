namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MajIndexUniqueTableMissionV2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Missions", "Mission_Unique_Index");
            CreateIndex("dbo.Missions", "Name", unique: true, name: "MissionName_Unique_Index");
            CreateIndex("dbo.Missions", new[] { "Start", "End", "ConsultantID", "CompanyContactID", "ProfitCenterID" }, unique: true, name: "Mission_Unique_Index");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Missions", "Mission_Unique_Index");
            DropIndex("dbo.Missions", "MissionName_Unique_Index");
            CreateIndex("dbo.Missions", new[] { "Name", "Start", "End", "ConsultantID", "CompanyContactID", "ProfitCenterID" }, unique: true, name: "Mission_Unique_Index");
        }
    }
}
