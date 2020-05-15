namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreationSchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        ActivityID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Morning = c.String(),
                        Afternoon = c.String(),
                        CraID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityID)
                .ForeignKey("dbo.Cras", t => t.CraID)
                .Index(t => t.CraID);
            
            CreateTable(
                "dbo.Cras",
                c => new
                    {
                        CraID = c.Int(nullable: false, identity: true),
                        Changeable = c.Boolean(nullable: false),
                        Month = c.String(),
                        year = c.String(),
                        Satisfaction = c.String(),
                        Comment = c.String(),
                        ConsultantID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CraID)
                .ForeignKey("dbo.Consultants", t => t.ConsultantID)
                .Index(t => t.ConsultantID);
            
            CreateTable(
                "dbo.Consultants",
                c => new
                    {
                        ConsultantID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Email = c.String(maxLength: 256),
                        Status = c.String(maxLength: 50),
                        EntryDate = c.DateTime(nullable: false),
                        DateOfDeparture = c.DateTime(),
                        DailyCost = c.Double(nullable: false),
                        ProfitCenterID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ConsultantID)
                .ForeignKey("dbo.ProfitCenters", t => t.ProfitCenterID)
                .Index(t => new { t.FirstName, t.LastName, t.Email, t.Status }, unique: true, name: "Consultant_Unique_Index")
                .Index(t => t.ProfitCenterID);
            
            CreateTable(
                "dbo.Missions",
                c => new
                    {
                        MissionID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Fee = c.Single(nullable: false),
                        FreeDay = c.Int(nullable: false),
                        Periodicity = c.String(),
                        Comment = c.String(),
                        Creator = c.String(),
                        ConsultantID = c.Int(nullable: false),
                        CompanyContactID = c.Int(nullable: false),
                        ProfitCenterID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MissionID)
                .ForeignKey("dbo.ProfitCenters", t => t.ProfitCenterID, cascadeDelete: true)
                .ForeignKey("dbo.CompanyContacts", t => t.CompanyContactID)
                .ForeignKey("dbo.Consultants", t => t.ConsultantID)
                .Index(t => new { t.Name, t.Start, t.End }, unique: true, name: "Mission_Unique_Index")
                .Index(t => t.ConsultantID)
                .Index(t => t.CompanyContactID)
                .Index(t => t.ProfitCenterID);
            
            CreateTable(
                "dbo.CompanyContacts",
                c => new
                    {
                        CompanyContactID = c.Int(nullable: false, identity: true),
                        Mail = c.String(maxLength: 50),
                        CompanyName = c.String(maxLength: 50),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Position = c.String(),
                        PhoneNumber = c.String(),
                        CompanyID = c.Int(nullable: false),
                        ManagerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyContactID)
                .ForeignKey("dbo.Companies", t => t.CompanyID)
                .ForeignKey("dbo.Managers", t => t.ManagerID)
                .Index(t => new { t.Mail, t.CompanyName, t.FirstName, t.LastName }, unique: true, name: "Contact_Unique_Index")
                .Index(t => t.CompanyID)
                .Index(t => t.ManagerID);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Address = c.String(),
                        PostalCode = c.String(),
                        City = c.String(),
                    })
                .PrimaryKey(t => t.CompanyID)
                .Index(t => t.Name, unique: true, name: "Name_Unique_Index");
            
            CreateTable(
                "dbo.Managers",
                c => new
                    {
                        ManagerID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Email = c.String(maxLength: 50),
                        EntryDate = c.DateTime(nullable: false),
                        DateOfDeparture = c.DateTime(),
                        SubsidiaryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ManagerID)
                .ForeignKey("dbo.Subsidiaries", t => t.SubsidiaryID)
                .Index(t => new { t.FirstName, t.LastName, t.Email }, unique: true, name: "Manager_Unique_Index")
                .Index(t => t.SubsidiaryID);
            
            CreateTable(
                "dbo.ProfitCenters",
                c => new
                    {
                        ProfitCenterID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Cost = c.Double(nullable: false),
                        Turnover = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ProfitCenterID)
                .Index(t => t.Name, unique: true, name: "Index_Name");
            
            CreateTable(
                "dbo.Subsidiaries",
                c => new
                    {
                        SubsidiaryID = c.Int(nullable: false, identity: true),
                        Siren = c.String(maxLength: 50),
                        Name = c.String(maxLength: 50),
                        Address = c.String(),
                        PostaleCode = c.String(),
                        City = c.String(),
                    })
                .PrimaryKey(t => t.SubsidiaryID)
                .Index(t => new { t.Siren, t.Name }, unique: true, name: "Subsidiary_Unique_Index");
            
            CreateTable(
                "dbo.MonthActivations",
                c => new
                    {
                        MonthActivationID = c.Int(nullable: false, identity: true),
                        Periode = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MonthActivationID);
            
            CreateTable(
                "dbo.ManagerProfitCenters",
                c => new
                    {
                        Manager_ManagerID = c.Int(nullable: false),
                        ProfitCenter_ProfitCenterID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Manager_ManagerID, t.ProfitCenter_ProfitCenterID })
                .ForeignKey("dbo.Managers", t => t.Manager_ManagerID, cascadeDelete: true)
                .ForeignKey("dbo.ProfitCenters", t => t.ProfitCenter_ProfitCenterID, cascadeDelete: true)
                .Index(t => t.Manager_ManagerID)
                .Index(t => t.ProfitCenter_ProfitCenterID);
            
            AddColumn("dbo.User", "FirstName", c => c.String());
            AddColumn("dbo.User", "LastName", c => c.String());
            AddColumn("dbo.User", "Position", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Activities", "CraID", "dbo.Cras");
            DropForeignKey("dbo.Cras", "ConsultantID", "dbo.Consultants");
            DropForeignKey("dbo.Consultants", "ProfitCenterID", "dbo.ProfitCenters");
            DropForeignKey("dbo.Missions", "ConsultantID", "dbo.Consultants");
            DropForeignKey("dbo.Missions", "CompanyContactID", "dbo.CompanyContacts");
            DropForeignKey("dbo.CompanyContacts", "ManagerID", "dbo.Managers");
            DropForeignKey("dbo.Managers", "SubsidiaryID", "dbo.Subsidiaries");
            DropForeignKey("dbo.ManagerProfitCenters", "ProfitCenter_ProfitCenterID", "dbo.ProfitCenters");
            DropForeignKey("dbo.ManagerProfitCenters", "Manager_ManagerID", "dbo.Managers");
            DropForeignKey("dbo.Missions", "ProfitCenterID", "dbo.ProfitCenters");
            DropForeignKey("dbo.CompanyContacts", "CompanyID", "dbo.Companies");
            DropIndex("dbo.ManagerProfitCenters", new[] { "ProfitCenter_ProfitCenterID" });
            DropIndex("dbo.ManagerProfitCenters", new[] { "Manager_ManagerID" });
            DropIndex("dbo.Subsidiaries", "Subsidiary_Unique_Index");
            DropIndex("dbo.ProfitCenters", "Index_Name");
            DropIndex("dbo.Managers", new[] { "SubsidiaryID" });
            DropIndex("dbo.Managers", "Manager_Unique_Index");
            DropIndex("dbo.Companies", "Name_Unique_Index");
            DropIndex("dbo.CompanyContacts", new[] { "ManagerID" });
            DropIndex("dbo.CompanyContacts", new[] { "CompanyID" });
            DropIndex("dbo.CompanyContacts", "Contact_Unique_Index");
            DropIndex("dbo.Missions", new[] { "ProfitCenterID" });
            DropIndex("dbo.Missions", new[] { "CompanyContactID" });
            DropIndex("dbo.Missions", new[] { "ConsultantID" });
            DropIndex("dbo.Missions", "Mission_Unique_Index");
            DropIndex("dbo.Consultants", new[] { "ProfitCenterID" });
            DropIndex("dbo.Consultants", "Consultant_Unique_Index");
            DropIndex("dbo.Cras", new[] { "ConsultantID" });
            DropIndex("dbo.Activities", new[] { "CraID" });
            DropColumn("dbo.User", "Position");
            DropColumn("dbo.User", "LastName");
            DropColumn("dbo.User", "FirstName");
            DropTable("dbo.ManagerProfitCenters");
            DropTable("dbo.MonthActivations");
            DropTable("dbo.Subsidiaries");
            DropTable("dbo.ProfitCenters");
            DropTable("dbo.Managers");
            DropTable("dbo.Companies");
            DropTable("dbo.CompanyContacts");
            DropTable("dbo.Missions");
            DropTable("dbo.Consultants");
            DropTable("dbo.Cras");
            DropTable("dbo.Activities");
        }
    }
}
