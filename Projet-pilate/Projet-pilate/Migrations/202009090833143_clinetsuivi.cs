namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class clinetsuivi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suivis", "client", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suivis", "client");
        }
    }
}
