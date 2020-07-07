namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Suivi : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Suivis",
                c => new
                    {
                        SuiviID = c.Int(nullable: false, identity: true),
                        MoisDeFacturation = c.DateTime(nullable: false),
                        SubsidiaryID = c.Int(nullable: false),
                        ProfitCenterID = c.Int(nullable: false),
                        NomMission = c.String(),
                        Consultant = c.String(),
                        NombredUO = c.Single(nullable: false),
                        TJ = c.Single(nullable: false),
                        cout = c.Single(nullable: false),
                        CJ = c.Single(nullable: false),
                        Frais = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.SuiviID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Suivis");
        }
    }
}
