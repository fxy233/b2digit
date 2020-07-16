namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class info : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Infoes",
                c => new
                    {
                        InfoID = c.Int(nullable: false, identity: true),
                        TVA = c.Single(nullable: false),
                        Mention = c.String(),
                    })
                .PrimaryKey(t => t.InfoID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Infoes");
        }
    }
}
