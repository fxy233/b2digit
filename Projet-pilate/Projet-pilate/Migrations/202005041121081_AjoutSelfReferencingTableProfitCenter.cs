namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutSelfReferencingTableProfitCenter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProfitCenters", "FatherProfitCenterID", c => c.Int());
            CreateIndex("dbo.ProfitCenters", "FatherProfitCenterID");
            AddForeignKey("dbo.ProfitCenters", "FatherProfitCenterID", "dbo.ProfitCenters", "ProfitCenterID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProfitCenters", "FatherProfitCenterID", "dbo.ProfitCenters");
            DropIndex("dbo.ProfitCenters", new[] { "FatherProfitCenterID" });
            DropColumn("dbo.ProfitCenters", "FatherProfitCenterID");
        }
    }
}
