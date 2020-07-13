namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCras1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cras", "verrouille", c => c.Boolean(nullable: false));
            DropColumn("dbo.Cras", "deverrouille");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cras", "deverrouille", c => c.Boolean(nullable: false));
            DropColumn("dbo.Cras", "verrouille");
        }
    }
}
