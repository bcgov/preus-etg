namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("v01.02.03")]
    public partial class v010203 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            DropIndex("dbo.GrantApplications", "IX_GrantApplication");
            CreateIndex("dbo.GrantApplications", new[] { "ApplicationStateInternal", "ApplicationStateExternal", "GrantOpeningId", "AssessorId", "OrganizationLegalName", "ApplicationTypeId", "PrioritySectorId" }, name: "IX_GrantApplication");

            DropColumn("dbo.GrantApplications", "WithdrawReason");
            DropColumn("dbo.GrantApplications", "DenyReason");
            DropColumn("dbo.GrantApplications", "CancellationReason");
            DropColumn("dbo.TrainingPrograms", "ReasonForTrainingProviderChange");
            DropColumn("dbo.GrantApplications", "ApplicationStateExternalChanged");
            DropColumn("dbo.GrantApplications", "ApplicationStateInternalChanged");
            AddColumn("dbo.TrainingPrograms", "UsedDeliveryPartner", c => c.Boolean());

            PostDeployment();
        }

        public override void Down()
        {
            DropColumn("dbo.TrainingPrograms", "UsedDeliveryPartner");
            AddColumn("dbo.GrantApplications", "CancellationReason", c => c.String(maxLength: 800));
            AddColumn("dbo.GrantApplications", "DenyReason", c => c.String(maxLength: 800));
            AddColumn("dbo.GrantApplications", "WithdrawReason", c => c.String());
            AddColumn("dbo.TrainingPrograms", "ReasonForTrainingProviderChange", c => c.String());
            AddColumn("dbo.GrantApplications", "ApplicationStateInternalChanged", c => c.DateTime());
            AddColumn("dbo.GrantApplications", "ApplicationStateExternalChanged", c => c.DateTime());
            DropIndex("dbo.GrantApplications", "IX_GrantApplication");
            CreateIndex("dbo.GrantApplications", new[] { "ApplicationStateInternal", "ApplicationStateExternal", "GrantOpeningId", "AssessorId", "OrganizationLegalName", "ApplicationTypeId", "ApplicationStateInternalChanged", "ApplicationStateExternalChanged", "PrioritySectorId" }, name: "IX_GrantApplication");
        }
    }
}
