namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upSuivi : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Suivis", "MoisDeFacturation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Suivis", "MoisDeFacturation", c => c.DateTime(nullable: false));
        }
    }
}
