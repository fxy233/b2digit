namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatehistorique8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Infoes", "Historique", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Infoes", "Historique");
        }
    }
}
