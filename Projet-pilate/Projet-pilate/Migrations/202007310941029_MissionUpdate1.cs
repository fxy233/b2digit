namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MissionUpdate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Missions", "avoirOdm", c => c.Boolean(nullable: false));
            AddColumn("dbo.Missions", "DateFinOdm", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Missions", "DateFinOdm");
            DropColumn("dbo.Missions", "avoirOdm");
        }
    }
}
