namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ownerId : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProfitCenters", "Owner", c => c.Int(nullable: false));
            AlterColumn("dbo.ProfitCenters", "PartOwner", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProfitCenters", "PartOwner", c => c.String());
            AlterColumn("dbo.ProfitCenters", "Owner", c => c.String());
        }
    }
}
