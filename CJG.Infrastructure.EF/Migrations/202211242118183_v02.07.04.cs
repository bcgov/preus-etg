using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.07.04")]
    public partial class v020704 : ExtendedDbMigration
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
