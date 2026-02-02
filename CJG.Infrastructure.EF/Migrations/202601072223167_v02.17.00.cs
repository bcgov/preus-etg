using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
    [Description("v02.17.00")]
	public partial class v021700 : ExtendedDbMigration
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
