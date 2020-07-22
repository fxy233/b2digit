namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class enleverunique : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Factures", "Index_NomFacture");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Factures", "NomFacture", unique: true, name: "Index_NomFacture");
        }
    }
}
