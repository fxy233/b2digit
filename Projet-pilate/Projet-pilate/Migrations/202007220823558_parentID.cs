namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class parentID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Factures", "parentID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Factures", "parentID");
        }
    }
}
