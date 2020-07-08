namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class missionAjoute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Missions", "exist", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Missions", "exist");
        }
    }
}
