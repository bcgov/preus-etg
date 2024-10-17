namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("v01.03.00")]
    public partial class v010300 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            AddColumn("dbo.Logs", "Application", c => c.String(maxLength: 150));

            PostDeployment();
        }
        
        public override void Down()
        {
            DropColumn("dbo.Logs", "Application");
        }
    }
}
