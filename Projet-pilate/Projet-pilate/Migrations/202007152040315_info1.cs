namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class info1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Infoes", "TVA", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Infoes", "TVA", c => c.Single(nullable: false));
        }
    }
}
