namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutIndexUniqueNomTableSubsidiaries : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Subsidiaries", "Name", unique: true, name: "Subsidiary_Unique_Name");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Subsidiaries", "Subsidiary_Unique_Name");
        }
    }
}
