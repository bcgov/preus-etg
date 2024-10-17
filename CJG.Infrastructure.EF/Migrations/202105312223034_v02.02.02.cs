namespace CJG.Infrastructure.EF.Migrations
{
    using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v02.02.02")]
	public partial class v020202 : ExtendedDbMigration
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
