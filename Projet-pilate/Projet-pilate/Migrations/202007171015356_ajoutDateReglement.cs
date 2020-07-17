namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ajoutDateReglement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Factures", "DateRegelement", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Factures", "DateRegelement");
        }
    }
}
