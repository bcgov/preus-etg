namespace CJG.Infrastructure.EF.Migrations
{
    using System;
	using System.ComponentModel;

	[Description("v01.13.00")]
	public partial class v011300 : ExtendedDbMigration
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
