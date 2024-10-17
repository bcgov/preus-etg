namespace CJG.Infrastructure.EF.Migrations
{
    using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v01.15.19")]
	public partial class v011519 : ExtendedDbMigration
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
