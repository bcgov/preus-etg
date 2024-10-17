namespace CJG.Infrastructure.EF.Migrations
{
    using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v01.15.09.02")]
	public partial class v01150902 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantApplications", "IsAlternateContact", c => c.Boolean());
        }

        public override void Down()
        {
            DropColumn("dbo.GrantApplications", "IsAlternateContact");
        }
    }
}
