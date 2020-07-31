namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vcode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subsidiaries", "VCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subsidiaries", "VCode");
        }
    }
}
