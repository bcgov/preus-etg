using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.11.06")]
    public partial class v021106 : ExtendedDbMigration
    {
        public override void Up()
        {
			PostDeployment();
        }
        
        public override void Down()
        {
        }
    }
}
