namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upSuivCra : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suivis", "craID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suivis", "craID");
        }
    }
}
