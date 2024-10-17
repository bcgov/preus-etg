namespace CJG.Infrastructure.EF.Migrations
{
	using System.ComponentModel;

	[Description("v01.12.01")]
	public partial class v011201 : ExtendedDbMigration
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
