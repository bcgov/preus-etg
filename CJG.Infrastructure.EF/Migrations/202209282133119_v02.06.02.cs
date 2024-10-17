using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.06.02")]
    public partial class v020602 : ExtendedDbMigration
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
