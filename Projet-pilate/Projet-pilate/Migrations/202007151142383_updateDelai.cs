namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDelai : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Missions", "Delai", c => c.String());
            AlterColumn("dbo.Factures", "Delai", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Factures", "Delai", c => c.Int(nullable: false));
            AlterColumn("dbo.Missions", "Delai", c => c.Int(nullable: false));
        }
    }
}
