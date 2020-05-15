namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MajIndexUniqueTableConsultant : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Consultants", "Consultant_Unique_Index");
            CreateIndex("dbo.Consultants", "Email", unique: true, name: "Consultant_Unique_Index");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Consultants", "Consultant_Unique_Index");
            CreateIndex("dbo.Consultants", new[] { "FirstName", "LastName", "Email", "Status" }, unique: true, name: "Consultant_Unique_Index");
        }
    }
}
