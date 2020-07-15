namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ajouterAdresseMission : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Missions", "AdresseMission", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Missions", "AdresseMission");
        }
    }
}
