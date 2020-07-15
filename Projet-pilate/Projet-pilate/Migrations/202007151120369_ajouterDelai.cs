namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ajouterDelai : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Missions", "Delai", c => c.Int(nullable: false));
            AddColumn("dbo.Missions", "DesignationFacturation", c => c.String());
            AddColumn("dbo.Factures", "Delai", c => c.Int(nullable: false));
            AddColumn("dbo.Factures", "DesignationFacturation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Factures", "DesignationFacturation");
            DropColumn("dbo.Factures", "Delai");
            DropColumn("dbo.Missions", "DesignationFacturation");
            DropColumn("dbo.Missions", "Delai");
        }
    }
}
