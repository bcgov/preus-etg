namespace CJG.Infrastructure.EF.Migrations
{
	using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v01.15.03")]
	public partial class v011503 : ExtendedDbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Organizations", "IsNaicsUpdated", c => c.Boolean(nullable: false, defaultValueSql: "0"));			
			PreDeployment();
			PostDeployment();
		}

		public override void Down()
		{
			DropColumn("dbo.Organizations", "IsNaicsUpdated");
		}
	}
}
