namespace CJG.Infrastructure.EF.Migrations
{
	using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v01.15.17")]
	public partial class v011517 : ExtendedDbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GrantApplications", "AlternateMailingAddressId", "dbo.ApplicationAddresses");
            DropForeignKey("dbo.GrantApplications", "AlternatePhysicalAddressId", "dbo.ApplicationAddresses");
            DropIndex("dbo.GrantApplications", new[] { "AlternatePhysicalAddressId" });
            DropIndex("dbo.GrantApplications", new[] { "AlternateMailingAddressId" });
            DropColumn("dbo.GrantApplications", "AlternatePhysicalAddressId");
            DropColumn("dbo.GrantApplications", "AlternateMailingAddressId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GrantApplications", "AlternateMailingAddressId", c => c.Int());
            AddColumn("dbo.GrantApplications", "AlternatePhysicalAddressId", c => c.Int());
            CreateIndex("dbo.GrantApplications", "AlternateMailingAddressId");
            CreateIndex("dbo.GrantApplications", "AlternatePhysicalAddressId");
            AddForeignKey("dbo.GrantApplications", "AlternatePhysicalAddressId", "dbo.ApplicationAddresses", "Id");
            AddForeignKey("dbo.GrantApplications", "AlternateMailingAddressId", "dbo.ApplicationAddresses", "Id");
        }
    }
}
