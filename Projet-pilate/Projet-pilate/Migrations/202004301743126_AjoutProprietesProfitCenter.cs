namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutProprietesProfitCenter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProfitCenters", "Owner", c => c.String());
            AddColumn("dbo.ProfitCenters", "PartOwner", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProfitCenters", "PartOwner");
            DropColumn("dbo.ProfitCenters", "Owner");
        }
    }
}
