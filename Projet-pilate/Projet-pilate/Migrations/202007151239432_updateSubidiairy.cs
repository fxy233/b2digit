namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSubidiairy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subsidiaries", "email", c => c.String());
            AddColumn("dbo.Subsidiaries", "IBAN", c => c.String(maxLength: 27));
            AddColumn("dbo.Subsidiaries", "BIC", c => c.String(maxLength: 11));
            AddColumn("dbo.Subsidiaries", "TVAIntra", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subsidiaries", "TVAIntra");
            DropColumn("dbo.Subsidiaries", "BIC");
            DropColumn("dbo.Subsidiaries", "IBAN");
            DropColumn("dbo.Subsidiaries", "email");
        }
    }
}
