namespace CJG.Infrastructure.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v020601 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantStreamEligibilityAnswers", "RationaleAnswer", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantStreamEligibilityAnswers", "RationaleAnswer");
        }
    }
}
