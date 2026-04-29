using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.20.01")]
	public partial class v022001 : ExtendedDbMigration
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
