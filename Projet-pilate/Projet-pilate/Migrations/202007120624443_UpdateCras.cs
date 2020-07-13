namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCras : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cras", "deverrouille", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cras", "deverrouille");
        }
    }
}
