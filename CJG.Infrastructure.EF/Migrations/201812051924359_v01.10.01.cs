namespace CJG.Infrastructure.EF.Migrations
{
	using System.ComponentModel;
	
	[Description("v01.10.01")]
	public partial class v011001 : ExtendedDbMigration
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
