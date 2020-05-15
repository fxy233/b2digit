namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutProprietesNomPrenomTableSubsidiaries : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subsidiaries", "ManagerFirstName", c => c.String());
            AddColumn("dbo.Subsidiaries", "ManagerLastName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subsidiaries", "ManagerLastName");
            DropColumn("dbo.Subsidiaries", "ManagerFirstName");
        }
    }
}
