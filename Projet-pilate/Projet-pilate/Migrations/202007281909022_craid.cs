namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class craid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Factures", "CraId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Factures", "CraId");
        }
    }
}
