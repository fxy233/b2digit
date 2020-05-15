namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MajIndexUniqueTableManager : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Managers", "Manager_Unique_Index");
            CreateIndex("dbo.Managers", "Email", unique: true, name: "Manager_Unique_Index");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Managers", "Manager_Unique_Index");
            CreateIndex("dbo.Managers", new[] { "FirstName", "LastName", "Email" }, unique: true, name: "Manager_Unique_Index");
        }
    }
}
