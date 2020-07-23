namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reference2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Factures", "reference", c => c.String());
            AddColumn("dbo.Factures", "referenceBancaire", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Factures", "referenceBancaire");
            DropColumn("dbo.Factures", "reference");
        }
    }
}
