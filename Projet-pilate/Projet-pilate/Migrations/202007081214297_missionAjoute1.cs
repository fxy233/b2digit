namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class missionAjoute1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Missions", "inexist", c => c.Boolean(nullable: false));
            DropColumn("dbo.Missions", "exist");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Missions", "exist", c => c.Boolean(nullable: false));
            DropColumn("dbo.Missions", "inexist");
        }
    }
}
