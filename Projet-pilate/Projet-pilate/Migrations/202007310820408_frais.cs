namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class frais : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrdreDeMissions", "fraisAlloue", c => c.String());
            DropColumn("dbo.OrdreDeMissions", "typeProjet");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrdreDeMissions", "typeProjet", c => c.String());
            DropColumn("dbo.OrdreDeMissions", "fraisAlloue");
        }
    }
}
