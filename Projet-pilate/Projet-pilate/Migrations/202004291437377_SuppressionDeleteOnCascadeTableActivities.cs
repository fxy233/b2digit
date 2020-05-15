namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SuppressionDeleteOnCascadeTableActivities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Activities", "CraID", "dbo.Cras");
            AddForeignKey("dbo.Activities", "CraID", "dbo.Cras", "CraID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Activities", "CraID", "dbo.Cras");
            AddForeignKey("dbo.Activities", "CraID", "dbo.Cras", "CraID");
        }
    }
}
