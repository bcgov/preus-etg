namespace CJG.Infrastructure.EF.Migrations
{
	using System.ComponentModel;

	[Description("v01.12.00")]
	public partial class v011200 : ExtendedDbMigration
	{
		public override void Up()
		{
			PreDeployment();

			DropIndex("dbo.DeliveryPartners", "IX_Caption");
			DropIndex("dbo.DeliveryPartnerServices", "IX_Caption");
			DropIndex("dbo.ParticipantForms", "IX_NocId");
			DropForeignKey("dbo.Notes", "NoteTypeId", "dbo.NoteTypes");
			DropForeignKey("dbo.ParticipantForms", "NocId", "dbo.NationalOccupationalClassifications");
			DropPrimaryKey("dbo.NoteTypes");
			DropForeignKey("dbo.NotificationTypes", "NotificationTemplateId", "dbo.NotificationTemplates");
			DropIndex("dbo.NotificationTypes", new[] { "NotificationTemplateId" });
			RenameColumn(table: "dbo.ParticipantForms", name: "NocId", newName: "FutureNocId");
			RenameIndex(table: "dbo.NotificationTypes", name: "IX_NotificationType", newName: "IX_Active");

			CreateTable(
				"dbo.ParticipantFormEligibleCostBreakdowns",
				c => new
				{
					ParticipantFormId = c.Int(nullable: false),
					EligibleCostBreakdownId = c.Int(nullable: false),
				})
				.PrimaryKey(t => new { t.ParticipantFormId, t.EligibleCostBreakdownId })
				.ForeignKey("dbo.ParticipantForms", t => t.ParticipantFormId)
				.ForeignKey("dbo.EligibleCostBreakdowns", t => t.EligibleCostBreakdownId)
				.Index(t => t.ParticipantFormId)
				.Index(t => t.EligibleCostBreakdownId);

			AddColumn("dbo.GrantPrograms", "IncludeDeliveryPartner", c => c.Boolean(nullable: false));
			AddColumn("dbo.GrantApplications", "OrganizationBusinessLicenseNumber", c => c.String(maxLength: 20));
			AddColumn("dbo.Notifications", "InternalUserId", c => c.Int());
			AddColumn("dbo.Notifications", "Metadata", c => c.String(maxLength: 1000));
			AddColumn("dbo.Notifications", "HadError", c => c.Boolean(nullable: false));
			AddColumn("dbo.NotificationScheduleQueue", "NotificationTemplateId", c => c.Int());
			AlterColumn("dbo.NotificationTypes", "NotificationTypeName", c => c.String(nullable: false, maxLength: 250));
			AlterColumn("dbo.InternalUserFilterAttributes", "Value", c => c.String(maxLength: 500));
			RenameColumn("dbo.NotificationTypes", "NotificationTypeName", "Caption");
			AddColumn("dbo.NotificationTypes", "RowSequence", c => c.Int(nullable: false));
			AddColumn("dbo.GrantProgramNotifications", "NotificationTemplateId", c => c.Int(nullable: false));
			AddColumn("dbo.Organizations", "BusinessLicenseNumber", c => c.String(maxLength: 20));
			AddColumn("dbo.ParticipantForms", "CurrentNocId", c => c.Int(nullable: false, defaultValue: 0));
			AddColumn("dbo.CompletionReportGroups", "RowSequence", c => c.Int(nullable: false));
			AddColumn("dbo.CompletionReportGroups", "ProgramTypeId", c => c.Int());
			AddColumn("dbo.TrainingProviderTypes", "PrivateSectorValidationType", c => c.Int(nullable: false));
			AddColumn("dbo.ProgramConfigurations", "UserGuidanceCostEstimates", c => c.String(maxLength: 1000));
			AddColumn("dbo.ProgramConfigurations", "UserGuidanceClaims", c => c.String(maxLength: 1000));
			AddColumn("dbo.GrantStreams", "HasParticipantOutcomeReporting", c => c.Boolean(nullable: false));
			AddColumn("dbo.DeliveryPartners", "GrantProgramId", c => c.Int(nullable: false));
			AddColumn("dbo.DeliveryPartnerServices", "GrantProgramId", c => c.Int(nullable: false));
			AddColumn("dbo.Settings", "InternalUserId", c => c.Int());
			AlterColumn("dbo.NoteTypes", "Id", c => c.Int(nullable: false));
			AlterColumn("dbo.Settings", "Value", c => c.String(maxLength: 2000));
			RenameColumn("dbo.GrantApplications", "CanReportParticipants", "CanApplicantReportParticipants");
			RenameColumn("dbo.GrantStreams", "CanReportParticipants", "CanApplicantReportParticipants");
			RenameColumn("dbo.GrantPrograms", "CanReportParticipants", "CanApplicantReportParticipants");
			AddColumn("dbo.GrantApplications", "CanReportParticipants", c => c.Boolean(nullable: false));

			PrePostDeployment();

			AddColumn("dbo.ParticipantForms", "NocId", c => c.Int());
			AddColumn("dbo.ParticipantForms", "NaicsId", c => c.Int());
			AddColumn("dbo.ParticipantForms", "EmployerName", c => c.String(maxLength: 250));
			AlterColumn("dbo.NotificationScheduleQueue", "NotificationTemplateId", c => c.Int(nullable: false));

			AddPrimaryKey("dbo.NoteTypes", "Id");
			CreateIndex("dbo.DeliveryPartners", "GrantProgramId");
			CreateIndex("dbo.DeliveryPartnerServices", "GrantProgramId");
			CreateIndex("dbo.Notifications", "InternalUserId");
			CreateIndex("dbo.NotificationScheduleQueue", "NotificationTemplateId");
			CreateIndex("dbo.NotificationTypes", "Caption", unique: true);
			CreateIndex("dbo.GrantProgramNotifications", "NotificationTemplateId");
			CreateIndex("dbo.ParticipantForms", "CurrentNocId");
			CreateIndex("dbo.ParticipantForms", "FutureNocId");
			CreateIndex("dbo.ParticipantForms", "NocId");
			CreateIndex("dbo.ParticipantForms", "NaicsId");
			CreateIndex("dbo.CompletionReportGroups", "ProgramTypeId");
			CreateIndex("dbo.Settings", "InternalUserId");
			AddForeignKey("dbo.DeliveryPartners", "GrantProgramId", "dbo.GrantPrograms", "Id", cascadeDelete: true);
			AddForeignKey("dbo.DeliveryPartnerServices", "GrantProgramId", "dbo.GrantPrograms", "Id", cascadeDelete: true);
			AddForeignKey("dbo.NotificationScheduleQueue", "NotificationTemplateId", "dbo.NotificationTemplates", "Id", cascadeDelete: true);
			AddForeignKey("dbo.GrantProgramNotifications", "NotificationTemplateId", "dbo.NotificationTemplates", "Id", cascadeDelete: true);
			AddForeignKey("dbo.Notifications", "InternalUserId", "dbo.InternalUsers", "Id");
			AddForeignKey("dbo.ParticipantForms", "NocId", "dbo.NationalOccupationalClassifications", "Id", cascadeDelete: false);
			AddForeignKey("dbo.ParticipantForms", "CurrentNocId", "dbo.NationalOccupationalClassifications", "Id", cascadeDelete: false);
			AddForeignKey("dbo.ParticipantForms", "FutureNocId", "dbo.NationalOccupationalClassifications", "Id", cascadeDelete: false);
			AddForeignKey("dbo.ParticipantForms", "NaicsId", "dbo.NaIndustryClassificationSystems", "Id", cascadeDelete: false);
			AddForeignKey("dbo.CompletionReportGroups", "ProgramTypeId", "dbo.ProgramTypes", "Id");
			AddForeignKey("dbo.Settings", "InternalUserId", "dbo.InternalUsers", "Id");
			AddForeignKey("dbo.Notes", "NoteTypeId", "dbo.NoteTypes", "Id", cascadeDelete: true);
			DropColumn("dbo.NotificationTypes", "NotificationTemplateId");
			DropColumn("dbo.NotificationTypes", "RecipientType");

			PostDeployment();
		}

		public override void Down()
		{
			AddColumn("dbo.GrantStreams", "CanReportParticipants", c => c.Boolean(nullable: false));
			AddColumn("dbo.NotificationTypes", "RecipientType", c => c.Int(nullable: false));
			AddColumn("dbo.NotificationTypes", "NotificationTypeName", c => c.String(nullable: false, maxLength: 128));
			AddColumn("dbo.NotificationTypes", "NotificationTemplateId", c => c.Int(nullable: false));
			AddColumn("dbo.GrantPrograms", "CanReportParticipants", c => c.Boolean(nullable: false));
			DropForeignKey("dbo.Notes", "NoteTypeId", "dbo.NoteTypes");
			DropForeignKey("dbo.Settings", "InternalUserId", "dbo.InternalUsers");
			DropForeignKey("dbo.CompletionReportGroups", "ProgramTypeId", "dbo.ProgramTypes");
			DropForeignKey("dbo.ParticipantForms", "NaicsId", "dbo.NaIndustryClassificationSystems");
			DropForeignKey("dbo.ParticipantForms", "FutureNocId", "dbo.NationalOccupationalClassifications");
			DropForeignKey("dbo.ParticipantFormEligibleCostBreakdowns", "EligibleCostBreakdownId", "dbo.EligibleCostBreakdowns");
			DropForeignKey("dbo.ParticipantFormEligibleCostBreakdowns", "ParticipantFormId", "dbo.ParticipantForms");
			DropForeignKey("dbo.ParticipantForms", "CurrentNocId", "dbo.NationalOccupationalClassifications");
			DropForeignKey("dbo.Notifications", "InternalUserId", "dbo.InternalUsers");
			DropForeignKey("dbo.GrantProgramNotifications", "NotificationTemplateId", "dbo.NotificationTemplates");
			DropForeignKey("dbo.NotificationScheduleQueue", "NotificationTemplateId", "dbo.NotificationTemplates");
			DropForeignKey("dbo.DeliveryPartnerServices", "GrantProgramId", "dbo.GrantPrograms");
			DropForeignKey("dbo.DeliveryPartners", "GrantProgramId", "dbo.GrantPrograms");
			DropIndex("dbo.ParticipantFormEligibleCostBreakdowns", new[] { "EligibleCostBreakdownId" });
			DropIndex("dbo.ParticipantFormEligibleCostBreakdowns", new[] { "ParticipantFormId" });
			DropIndex("dbo.Settings", new[] { "InternalUserId" });
			DropIndex("dbo.CompletionReportGroups", new[] { "ProgramTypeId" });
			DropIndex("dbo.ParticipantForms", new[] { "NaicsId" });
			DropIndex("dbo.ParticipantForms", new[] { "NocId" });
			DropIndex("dbo.ParticipantForms", new[] { "FutureNocId" });
			DropIndex("dbo.ParticipantForms", new[] { "CurrentNocId" });
			DropIndex("dbo.GrantProgramNotifications", new[] { "NotificationTemplateId" });
			DropIndex("dbo.NotificationTypes", new[] { "Caption" });
			DropIndex("dbo.NotificationScheduleQueue", new[] { "NotificationTemplateId" });
			DropIndex("dbo.Notifications", new[] { "InternalUserId" });
			DropIndex("dbo.DeliveryPartnerServices", new[] { "GrantProgramId" });
			DropIndex("dbo.DeliveryPartners", new[] { "GrantProgramId" });
			DropPrimaryKey("dbo.NoteTypes");
			AlterColumn("dbo.Settings", "Value", c => c.String(nullable: false, maxLength: 500));
			AlterColumn("dbo.InternalUserFilterAttributes", "Value", c => c.String(nullable: false, maxLength: 500));
			AlterColumn("dbo.NoteTypes", "Id", c => c.Int(nullable: false, identity: true));
			AlterColumn("dbo.ParticipantForms", "NocId", c => c.Int(nullable: false));
			DropColumn("dbo.Settings", "InternalUserId");
			DropColumn("dbo.DeliveryPartnerServices", "GrantProgramId");
			DropColumn("dbo.DeliveryPartners", "GrantProgramId");
			DropColumn("dbo.GrantStreams", "HasParticipantOutcomeReporting");
			DropColumn("dbo.GrantStreams", "CanApplicantReportParticipants");
			DropColumn("dbo.ProgramConfigurations", "UserGuidanceClaims");
			DropColumn("dbo.ProgramConfigurations", "UserGuidanceCostEstimates");
			DropColumn("dbo.TrainingProviderTypes", "PrivateSectorValidationType");
			DropColumn("dbo.CompletionReportGroups", "ProgramTypeId");
			DropColumn("dbo.CompletionReportGroups", "RowSequence");
			DropColumn("dbo.ParticipantForms", "EmployerName");
			DropColumn("dbo.ParticipantForms", "NaicsId");
			DropColumn("dbo.ParticipantForms", "FutureNocId");
			DropColumn("dbo.ParticipantForms", "CurrentNocId");
			DropColumn("dbo.Organizations", "BusinessLicenseNumber");
			DropColumn("dbo.GrantProgramNotifications", "NotificationTemplateId");
			DropColumn("dbo.NotificationTypes", "RowSequence");
			DropColumn("dbo.NotificationTypes", "Caption");
			DropColumn("dbo.NotificationScheduleQueue", "NotificationTemplateId");
			DropColumn("dbo.Notifications", "HadError");
			DropColumn("dbo.Notifications", "Metadata");
			DropColumn("dbo.Notifications", "InternalUserId");
			DropColumn("dbo.GrantApplications", "CanApplicantReportParticipants");
			DropColumn("dbo.GrantApplications", "OrganizationBusinessLicenseNumber");
			DropColumn("dbo.GrantPrograms", "IncludeDeliveryPartner");
			DropColumn("dbo.GrantPrograms", "CanApplicantReportParticipants");
			DropTable("dbo.ParticipantFormEligibleCostBreakdowns");
			AddPrimaryKey("dbo.NoteTypes", "Id");
			RenameIndex(table: "dbo.NotificationTypes", name: "IX_Active", newName: "IX_NotificationType");
			CreateIndex("dbo.ParticipantForms", "NocId");
			CreateIndex("dbo.NotificationTypes", "NotificationTemplateId");
			AddForeignKey("dbo.Notes", "NoteTypeId", "dbo.NoteTypes", "Id", cascadeDelete: true);
			AddForeignKey("dbo.NotificationTypes", "NotificationTemplateId", "dbo.NotificationTemplates", "Id", cascadeDelete: true);
		}
	}
}
