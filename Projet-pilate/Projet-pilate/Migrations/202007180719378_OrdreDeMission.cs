namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrdreDeMission : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrdreDeMissions",
                c => new
                    {
                        OrdreDeMissionID = c.Int(nullable: false, identity: true),
                        ValidationDeAdv = c.Boolean(nullable: false),
                        ValidationConsultant = c.Boolean(nullable: false),
                        NomMission = c.String(),
                        typeProjet = c.String(),
                    })
                .PrimaryKey(t => t.OrdreDeMissionID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OrdreDeMissions");
        }
    }
}
