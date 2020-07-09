namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class factureupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Factures", "mission", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Factures", "mission");
        }
    }
}
