namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatehistorique5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FactureHistoriques",
                c => new
                    {
                        FactureHistoriqueID = c.Int(nullable: false, identity: true),
                        DernierEnregistrer = c.DateTime(nullable: false),
                        MoisDeFacturation = c.DateTime(nullable: false),
                        DateRegelement = c.DateTime(nullable: false),
                        NomFacture = c.String(maxLength: 50),
                        mission = c.String(),
                        InfoFacturation = c.String(),
                        PrincipalBC = c.String(),
                        AdresseBC = c.String(),
                        Client = c.String(),
                        AdresseFacturation = c.String(),
                        NombredUO = c.Single(nullable: false),
                        TJ = c.Single(nullable: false),
                        TVA = c.Single(nullable: false),
                        MontantHT = c.Single(nullable: false),
                        Delai = c.String(),
                        DesignationFacturation = c.String(),
                        type = c.String(),
                        parentID = c.Int(nullable: false),
                        reference = c.String(),
                        referenceBancaire = c.String(),
                        mention = c.String(),
                        CraId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FactureHistoriqueID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FactureHistoriques");
        }
    }
}
