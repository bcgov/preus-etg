namespace CJG.Infrastructure.EF.Migrations
{
	using System.ComponentModel;

	[Description("v01.14.01")]
	public partial class v011401 : ExtendedDbMigration
	{
		public override void Up()
		{
			PreDeployment();

			PostDeployment();
		}
		
		public override void Down()
		{
		}
	}
}
