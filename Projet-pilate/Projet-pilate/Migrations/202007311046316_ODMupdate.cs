namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ODMupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrdreDeMissions", "environnement", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrdreDeMissions", "environnement");
        }
    }
}
