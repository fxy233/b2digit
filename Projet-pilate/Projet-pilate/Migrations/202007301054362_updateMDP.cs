namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateMDP : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subsidiaries", "motdepasse", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subsidiaries", "motdepasse");
        }
    }
}
