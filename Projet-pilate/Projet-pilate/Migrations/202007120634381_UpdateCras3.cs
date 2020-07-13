namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCras3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activities", "verrouille", c => c.Boolean(nullable: false));
            DropColumn("dbo.Cras", "verrouille");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cras", "verrouille", c => c.Boolean(nullable: false));
            DropColumn("dbo.Activities", "verrouille");
        }
    }
}
