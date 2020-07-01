namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MonthCloture : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MonthClotures",
                c => new
                    {
                        MonthClotureID = c.Int(nullable: false, identity: true),
                        Periode = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MonthClotureID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MonthClotures");
        }
    }
}
