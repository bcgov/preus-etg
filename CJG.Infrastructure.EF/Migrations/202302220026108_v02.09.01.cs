using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.09.01")]
	public partial class v020901 : ExtendedDbMigration
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
