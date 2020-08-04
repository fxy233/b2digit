namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSuivi2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suivis", "statu", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suivis", "statu");
        }
    }
}
