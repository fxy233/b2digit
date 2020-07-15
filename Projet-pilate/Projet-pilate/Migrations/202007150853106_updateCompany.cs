namespace Projet_pilate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "MailFacturation", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "MailFacturation");
        }
    }
}
