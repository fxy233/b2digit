namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSuivi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suivis", "mensuelConsultant", c => c.Single(nullable: false));
            AddColumn("dbo.Suivis", "mensuelManager", c => c.Single(nullable: false));
            AddColumn("dbo.Suivis", "fraisConsultant", c => c.Single(nullable: false));
            AddColumn("dbo.Suivis", "fraisManager", c => c.Single(nullable: false));
            DropColumn("dbo.Suivis", "cout");
            DropColumn("dbo.Suivis", "CJ");
            DropColumn("dbo.Suivis", "Frais");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Suivis", "Frais", c => c.Single(nullable: false));
            AddColumn("dbo.Suivis", "CJ", c => c.Single(nullable: false));
            AddColumn("dbo.Suivis", "cout", c => c.Single(nullable: false));
            DropColumn("dbo.Suivis", "fraisManager");
            DropColumn("dbo.Suivis", "fraisConsultant");
            DropColumn("dbo.Suivis", "mensuelManager");
            DropColumn("dbo.Suivis", "mensuelConsultant");
        }
    }
}
