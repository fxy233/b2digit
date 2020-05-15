namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutRelationConsultantSubsidiary : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consultants", "SubsidiaryID", c => c.Int(nullable: false));
            CreateIndex("dbo.Consultants", "SubsidiaryID");
            AddForeignKey("dbo.Consultants", "SubsidiaryID", "dbo.Subsidiaries", "SubsidiaryID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Consultants", "SubsidiaryID", "dbo.Subsidiaries");
            DropIndex("dbo.Consultants", new[] { "SubsidiaryID" });
            DropColumn("dbo.Consultants", "SubsidiaryID");
        }
    }
}
