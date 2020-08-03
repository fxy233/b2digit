namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class signature : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrdreDeMissions", "signature", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrdreDeMissions", "signature");
        }
    }
}
