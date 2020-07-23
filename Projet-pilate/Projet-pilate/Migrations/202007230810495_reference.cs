namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reference : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Missions", "Reference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Missions", "Reference");
        }
    }
}
