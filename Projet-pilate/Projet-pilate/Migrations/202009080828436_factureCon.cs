namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class factureCon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Factures", "ConsultantId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Factures", "ConsultantId");
        }
    }
}
