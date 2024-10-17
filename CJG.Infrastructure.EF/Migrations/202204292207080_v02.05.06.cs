namespace CJG.Infrastructure.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v020506 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GrantApplicationStateChanges", "Reason", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GrantApplicationStateChanges", "Reason", c => c.String(maxLength: 2000));
        }
    }
}
