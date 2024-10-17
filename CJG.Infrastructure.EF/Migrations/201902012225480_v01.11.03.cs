namespace CJG.Infrastructure.EF.Migrations
{
	using System.ComponentModel;

	[Description("v01.11.03")]
	public partial class v011103 : ExtendedDbMigration
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
