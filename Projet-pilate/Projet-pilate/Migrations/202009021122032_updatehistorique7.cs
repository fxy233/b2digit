namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatehistorique7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FactureHistoriques", "FactureID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FactureHistoriques", "FactureID");
        }
    }
}
