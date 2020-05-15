namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RetraitProprieteDailyCostTableManager : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Managers", "DailyCost");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Managers", "DailyCost", c => c.Double(nullable: false));
        }
    }
}
