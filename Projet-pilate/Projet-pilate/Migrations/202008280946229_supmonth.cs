namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supmonth : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.MonthClotures");
        }
        
        public override void Down()
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
    }
}
