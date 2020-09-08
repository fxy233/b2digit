namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class factureCon2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FactureHistoriques", "ConsultantId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FactureHistoriques", "ConsultantId");
        }
    }
}
