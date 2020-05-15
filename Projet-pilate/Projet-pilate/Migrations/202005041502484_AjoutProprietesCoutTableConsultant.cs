namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutProprietesCoutTableConsultant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consultants", "MonthlyCost", c => c.Double(nullable: false));
            AddColumn("dbo.Consultants", "MealCost", c => c.Double(nullable: false));
            AddColumn("dbo.Consultants", "TravelPackage", c => c.Double(nullable: false));
            AddColumn("dbo.Consultants", "ExceptionalCost", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Consultants", "ExceptionalCost");
            DropColumn("dbo.Consultants", "TravelPackage");
            DropColumn("dbo.Consultants", "MealCost");
            DropColumn("dbo.Consultants", "MonthlyCost");
        }
    }
}
