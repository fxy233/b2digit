namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateSuivi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suivis", "date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suivis", "date");
        }
    }
}
