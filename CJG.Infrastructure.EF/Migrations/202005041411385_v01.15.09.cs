namespace CJG.Infrastructure.EF.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class v011509 : ExtendedDbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.GrantApplications", "AlternateSalutation", c => c.String(maxLength: 250));
			AddColumn("dbo.GrantApplications", "AlternateFirstName", c => c.String(maxLength: 250));
			AddColumn("dbo.GrantApplications", "AlternateLastName", c => c.String(maxLength: 250));
			AddColumn("dbo.GrantApplications", "AlternatePhoneNumber", c => c.String(maxLength: 20));
			AddColumn("dbo.GrantApplications", "AlternatePhoneExtension", c => c.String(maxLength: 20));
			AddColumn("dbo.GrantApplications", "AlternateJobTitle", c => c.String(maxLength: 500));
			AddColumn("dbo.GrantApplications", "AlternatePhysicalAddressId", c => c.Int());
			AddColumn("dbo.GrantApplications", "AlternateMailingAddressId", c => c.Int());
			AddColumn("dbo.GrantApplications", "AlternateEmail", c => c.String(maxLength: 500));
			CreateIndex("dbo.GrantApplications", "AlternateMailingAddressId");
			AddForeignKey("dbo.GrantApplications", "AlternateMailingAddressId", "dbo.ApplicationAddresses", "Id");
			AddForeignKey("dbo.GrantApplications", "AlternatePhysicalAddressId", "dbo.ApplicationAddresses", "Id");
		}

		public override void Down()
		{
			DropForeignKey("dbo.GrantApplications", "AlternatePhysicalAddressId", "dbo.ApplicationAddresses");
			DropForeignKey("dbo.GrantApplications", "AlternateMailingAddressId", "dbo.ApplicationAddresses");
			DropIndex("dbo.GrantApplications", new[] { "AlternateMailingAddressId" });
			DropColumn("dbo.GrantApplications", "AlternateEmail");
			DropColumn("dbo.GrantApplications", "AlternateMailingAddressId");
			DropColumn("dbo.GrantApplications", "AlternatePhysicalAddressId");
			DropColumn("dbo.GrantApplications", "AlternateJobTitle");
			DropColumn("dbo.GrantApplications", "AlternatePhoneExtension");
			DropColumn("dbo.GrantApplications", "AlternatePhoneNumber");
			DropColumn("dbo.GrantApplications", "AlternateLastName");
			DropColumn("dbo.GrantApplications", "AlternateFirstName");
			DropColumn("dbo.GrantApplications", "AlternateSalutation");
		}
	}
}
