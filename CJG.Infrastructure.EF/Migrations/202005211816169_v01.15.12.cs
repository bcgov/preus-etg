namespace CJG.Infrastructure.EF.Migrations
{
	using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v01.15.12")]
	public partial class v011512 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "RiskFlag", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "Notes");
            DropColumn("dbo.Organizations", "RiskFlag");
        }
    }
}
