namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutProprietesTableManager : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Managers", "DailyCost", c => c.Double(nullable: false));
            AddColumn("dbo.Managers", "MonthlyCost", c => c.Double(nullable: false));
            AddColumn("dbo.Managers", "MealCost", c => c.Double(nullable: false));
            AddColumn("dbo.Managers", "TravelPackage", c => c.Double(nullable: false));
            AddColumn("dbo.Managers", "ExceptionalCost", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Managers", "ExceptionalCost");
            DropColumn("dbo.Managers", "TravelPackage");
            DropColumn("dbo.Managers", "MealCost");
            DropColumn("dbo.Managers", "MonthlyCost");
            DropColumn("dbo.Managers", "DailyCost");
        }
    }
}
