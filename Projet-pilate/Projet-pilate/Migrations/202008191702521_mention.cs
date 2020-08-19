namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mention : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Factures", "mention", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Factures", "mention");
        }
    }
}
