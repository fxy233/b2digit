namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upManagers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Managers", "role", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Managers", "role");
        }
    }
}
