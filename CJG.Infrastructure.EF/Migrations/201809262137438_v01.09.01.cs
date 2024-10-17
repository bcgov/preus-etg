namespace CJG.Infrastructure.EF.Migrations
{
	using System.ComponentModel;

	[Description("v01.09.01")]
	public partial class v010901 : ExtendedDbMigration
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
