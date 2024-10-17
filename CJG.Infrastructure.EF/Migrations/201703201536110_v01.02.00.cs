namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("v01.02.00")]
    public partial class v120 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();
            DropColumn("dbo.GrantAgreements", "IncompleteReason");
            PostDeployment();
        }
        
        public override void Down()
        {
            AddColumn("dbo.GrantAgreements", "IncompleteReason", c => c.String());
        }
    }
}
