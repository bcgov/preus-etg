namespace CJG.Infrastructure.EF.Migrations
{
	using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v01.15.15")]
	public partial class v011515 : ExtendedDbMigration
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
