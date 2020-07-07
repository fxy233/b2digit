namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Facture : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Factures",
                c => new
                    {
                        FactureID = c.Int(nullable: false, identity: true),
                        MoisDeFacturation = c.DateTime(nullable: false),
                        NomFacture = c.String(maxLength: 50),
                        InfoFacturation = c.String(),
                        PrincipalBC = c.String(),
                        AdresseBC = c.String(),
                        Client = c.String(),
                        AdresseFacturation = c.String(),
                        NombredUO = c.Single(nullable: false),
                        TJ = c.Single(nullable: false),
                        TVA = c.Single(nullable: false),
                        MontantHT = c.Single(nullable: false),
                        FAE = c.Boolean(nullable: false),
                        Emise = c.Boolean(nullable: false),
                        payee = c.Boolean(nullable: false),
                        annulee = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FactureID)
                .Index(t => t.NomFacture, unique: true, name: "Index_NomFacture");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Factures", "Index_NomFacture");
            DropTable("dbo.Factures");
        }
    }
}
