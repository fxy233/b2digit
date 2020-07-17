namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateFacture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Factures", "type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Factures", "type");
        }
    }
}
