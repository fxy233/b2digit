namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatehistorique6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FactureHistoriques", "FAE", c => c.Boolean(nullable: false));
            AddColumn("dbo.FactureHistoriques", "Emise", c => c.Boolean(nullable: false));
            AddColumn("dbo.FactureHistoriques", "payee", c => c.Boolean(nullable: false));
            AddColumn("dbo.FactureHistoriques", "annulee", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FactureHistoriques", "annulee");
            DropColumn("dbo.FactureHistoriques", "payee");
            DropColumn("dbo.FactureHistoriques", "Emise");
            DropColumn("dbo.FactureHistoriques", "FAE");
        }
    }
}
