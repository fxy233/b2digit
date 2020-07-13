namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class up : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Activities", "verrouille", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Activities", "verrouille", c => c.Boolean(nullable: false));
        }
    }
}
