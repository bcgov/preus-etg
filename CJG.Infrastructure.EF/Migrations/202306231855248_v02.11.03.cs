using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.11.03")]
    public partial class v021103 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantApplications", "UsePIFInvitations", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantApplications", "UsePIFInvitations");
        }
    }
}
