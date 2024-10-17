namespace CJG.Infrastructure.EF.Migrations
{
	using System.ComponentModel;

	[Description("v01.14.00")]
	public partial class v011400 : ExtendedDbMigration
	{
		public override void Up()
		{
			PreDeployment();

			DropForeignKey("dbo.Payments", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims");
			DropForeignKey("dbo.TrainingPrograms", "EligibleExpenseBreakdown_Id", "dbo.EligibleExpenseBreakdowns");
			DropForeignKey("dbo.ClaimParticipants", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims");
			DropForeignKey("dbo.ClaimParticipants", "ParticipantFormId", "dbo.ParticipantForms");
			DropForeignKey("dbo.GrantProgramNotifications", "NotificationTypeId", "dbo.NotificationTypes");
			DropTable("dbo.ClaimParticipants");
			RenameTable(name: "dbo.GrantProgramNotifications", newName: "GrantProgramNotificationTypes");
			DropForeignKey("dbo.NotificationScheduleQueue", "GrantApplicationId", "dbo.GrantApplications");
			DropForeignKey("dbo.NotificationScheduleQueue", "NotificationTemplateId", "dbo.NotificationTemplates");
			DropForeignKey("dbo.NotificationScheduleQueue", "NotificationTypeId", "dbo.NotificationTypes");
			DropForeignKey("dbo.Notifications", "NotificationScheduleQueueId", "dbo.NotificationScheduleQueue");
			DropForeignKey("dbo.Notifications", "NotificationTemplateId", "dbo.NotificationTemplates");
			DropForeignKey("dbo.Notifications", "InternalUserId", "dbo.InternalUsers");
			DropForeignKey("dbo.Notifications", "User_Id", "dbo.Users");
			DropIndex("dbo.Payments", new[] { "ClaimId", "ClaimVersion" });
			DropIndex("dbo.ServiceLines", "IX_Caption");
			DropIndex("dbo.ServiceLineBreakdowns", "IX_Caption");
			DropIndex("dbo.GrantApplications", "IX_GrantApplication");
			DropIndex("dbo.Notifications", new[] { "NotificationTemplateId" });
			DropIndex("dbo.Notifications", new[] { "NotificationScheduleQueueId" });
			DropIndex("dbo.Notifications", new[] { "InternalUserId" });
			DropIndex("dbo.Notifications", new[] { "User_Id" });
			DropIndex("dbo.NotificationScheduleQueue", new[] { "GrantApplicationId" });
			DropIndex("dbo.NotificationScheduleQueue", "IX_NotificationScheduleQueue");
			DropIndex("dbo.NotificationScheduleQueue", new[] { "NotificationTemplateId" });
			DropIndex("dbo.NotificationTypes", "IX_Active");
			DropIndex("dbo.TrainingProviderInventory", "IX_TrainingProviderInventory");
			DropIndex("dbo.TrainingPrograms", new[] { "EligibleExpenseBreakdown_Id" });
			DropPrimaryKey("dbo.Notifications");
			DropPrimaryKey("dbo.PaymentRequests");
			CreateTable(
				"dbo.FederalOfficialLanguages",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					Caption = c.String(nullable: false, maxLength: 250),
					IsActive = c.Boolean(nullable: false),
					RowSequence = c.Int(nullable: false),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
				})
				.PrimaryKey(t => t.Id)
				.Index(t => t.Caption, unique: true)
				.Index(t => t.IsActive, name: "IX_Active");

			CreateTable(
				"dbo.MartialStatus",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					Caption = c.String(nullable: false, maxLength: 250),
					IsActive = c.Boolean(nullable: false),
					RowSequence = c.Int(nullable: false),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
				})
				.PrimaryKey(t => t.Id)
				.Index(t => t.Caption, unique: true)
				.Index(t => t.IsActive, name: "IX_Active");

			CreateTable(
				"dbo.ReconciliationPayments",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					PaymentRequestBatchId = c.Int(),
					ClaimId = c.Int(),
					ClaimVersion = c.Int(),
					GrantApplicationId = c.Int(),
					DocumentNumber = c.String(nullable: false, maxLength: 75),
					SupplierName = c.String(nullable: false, maxLength: 250),
					SupplierNumber = c.String(maxLength: 30),
					BatchName = c.String(maxLength: 100),
					Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
					DateCreated = c.DateTime(precision: 7, storeType: "datetime2"),
					PaymentType = c.Int(nullable: false),
					ReconcilationState = c.Int(nullable: false),
					FromCAS = c.Boolean(nullable: false),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
				})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
				.ForeignKey("dbo.PaymentRequests", t => new { t.PaymentRequestBatchId, t.ClaimId, t.ClaimVersion })
				.ForeignKey("dbo.PaymentRequestBatches", t => t.PaymentRequestBatchId)
				.ForeignKey("dbo.Claims", t => new { t.ClaimId, t.ClaimVersion })
				.Index(t => new { t.ClaimId, t.ClaimVersion })
				.Index(t => new { t.PaymentRequestBatchId, t.ClaimId, t.ClaimVersion })
				.Index(t => new { t.Id, t.GrantApplicationId, t.DocumentNumber, t.SupplierName, t.BatchName, t.Amount, t.DateCreated, t.ReconcilationState }, name: "IX_ReconciliationPayment");

			CreateTable(
				"dbo.ReconciliationReports",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					InternalUserId = c.Int(nullable: false),
					DateRun = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					Requestor = c.String(nullable: false, maxLength: 100),
					PeriodFrom = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					PeriodTo = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					IsReconciled = c.Boolean(nullable: false),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion")
				})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.InternalUsers", t => t.InternalUserId, cascadeDelete: true)
				.Index(t => new { t.Id, t.IsReconciled, t.PeriodFrom, t.PeriodTo }, name: "IX_ReconciliationReport")
				.Index(t => t.InternalUserId);

			CreateTable(
				"dbo.NotificationQueue",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					NotificationTypeId = c.Int(),
					GrantApplicationId = c.Int(),
					OrganizationId = c.Int(nullable: false),
					BatchNumber = c.String(nullable: false, maxLength: 100),
					EmailSubject = c.String(nullable: false, maxLength: 500),
					EmailBody = c.String(nullable: false),
					EmailRecipients = c.String(nullable: false, maxLength: 500),
					EmailSender = c.String(nullable: false, maxLength: 500),
					State = c.Int(nullable: false),
					ErrorMessage = c.String(maxLength: 1000),
					SendDate = c.DateTime(precision: 7, storeType: "datetime2"),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
				})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
				.ForeignKey("dbo.NotificationTypes", t => t.NotificationTypeId)
				.ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
				.Index(t => t.NotificationTypeId)
				.Index(t => t.GrantApplicationId)
				.Index(t => t.OrganizationId)
				.Index(t => new { t.BatchNumber, t.State }, name: "IX_NotificationQueue");

			CreateTable(
				"dbo.NotificationTriggers",
				c => new
				{
					Id = c.Int(nullable: false, identity: false),
					Description = c.String(maxLength: 500),
					Caption = c.String(nullable: false, maxLength: 250),
					IsActive = c.Boolean(nullable: false),
					RowSequence = c.Int(nullable: false),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
				})
				.PrimaryKey(t => t.Id)
				.Index(t => t.Caption, unique: true)
				.Index(t => t.IsActive, name: "IX_Active");

			CreateTable(
				"dbo.ProgramNotificationRecipients",
				c => new
				{
					ProgramNotificationId = c.Int(nullable: false),
					GrantProgramId = c.Int(nullable: false),
					ApplicantOnly = c.Boolean(nullable: false),
					SubscriberOnly = c.Boolean(nullable: false),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
				})
				.PrimaryKey(t => new { t.ProgramNotificationId, t.GrantProgramId })
				.ForeignKey("dbo.GrantPrograms", t => t.GrantProgramId, cascadeDelete: true)
				.ForeignKey("dbo.ProgramNotifications", t => t.ProgramNotificationId, cascadeDelete: true)
				.Index(t => t.ProgramNotificationId)
				.Index(t => t.GrantProgramId);

			CreateTable(
				"dbo.ProgramNotifications",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					NotificationTemplateId = c.Int(nullable: false),
					SendDate = c.DateTime(precision: 7, storeType: "datetime2"),
					AllApplicants = c.Boolean(nullable: false),
					Caption = c.String(nullable: false, maxLength: 250),
					Description = c.String(maxLength: 500),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
				})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.NotificationTemplates", t => t.NotificationTemplateId, cascadeDelete: true)
				.Index(t => t.NotificationTemplateId)
				.Index(t => t.Caption, unique: true);

			CreateTable(
				"dbo.ClaimParticipants",
				c => new
				{
					ClaimId = c.Int(nullable: false),
					ClaimVersion = c.Int(nullable: false),
					ParticipantFormId = c.Int(nullable: false),
				})
				.PrimaryKey(t => new { t.ClaimId, t.ClaimVersion, t.ParticipantFormId })
				.ForeignKey("dbo.Claims", t => new { t.ClaimId, t.ClaimVersion })
				.ForeignKey("dbo.ParticipantForms", t => t.ParticipantFormId);

			CreateTable(
				"dbo.ReconciliationReportPayments",
				c => new
				{
					ReconciliationReportId = c.Int(nullable: false),
					ReconciliationPaymentId = c.Int(nullable: false),
				})
				.PrimaryKey(t => new { t.ReconciliationReportId, t.ReconciliationPaymentId })
				.ForeignKey("dbo.ReconciliationReports", t => t.ReconciliationReportId)
				.ForeignKey("dbo.ReconciliationPayments", t => t.ReconciliationPaymentId)
				.Index(t => t.ReconciliationReportId)
				.Index(t => t.ReconciliationPaymentId);

			AddColumn("dbo.GrantApplications", "CompletionReportId", c => c.Int(nullable: false));
			AddColumn("dbo.GrantApplications", "ScheduledNotificationsEnabled", c => c.Boolean(nullable: false, defaultValueSql: "1"));
			AddColumn("dbo.NotificationTemplates", "Caption", c => c.String(nullable: false, maxLength: 250));
			AddColumn("dbo.NotificationTypes", "NotificationTemplateId", c => c.Int());
			AddColumn("dbo.NotificationTypes", "PreviousApplicationState", c => c.Int());
			AddColumn("dbo.NotificationTypes", "CurrentApplicationState", c => c.Int());
			AddColumn("dbo.NotificationTypes", "NotificationTriggerId", c => c.Int());
			AddColumn("dbo.NotificationTypes", "MilestoneDateExpires", c => c.Int(nullable: false));
			AddColumn("dbo.NotificationTypes", "ResendDelayDays", c => c.Int(nullable: false));
			AddColumn("dbo.NotificationTypes", "ResendRule", c => c.Int());
			AddColumn("dbo.NotificationTypes", "ApprovalRule", c => c.Int());
			AddColumn("dbo.NotificationTypes", "ParticipantReportRule", c => c.Int());
			AddColumn("dbo.NotificationTypes", "ClaimReportRule", c => c.Int());
			AddColumn("dbo.NotificationTypes", "CompletionReportRule", c => c.Int());
			AddColumn("dbo.NotificationTypes", "RecipientRule", c => c.Int());
			AddColumn("dbo.Organizations", "DoingBusinessAsMinistry", c => c.String(maxLength: 500));
			AddColumn("dbo.ParticipantForms", "FederalOfficialLanguageId", c => c.Int());
			AddColumn("dbo.ParticipantForms", "MartialStatusId", c => c.Int());
			AddColumn("dbo.ParticipantForms", "NumberOfDependents", c => c.Int(nullable: false));
			AddColumn("dbo.TrainingProviderInventory", "Acronym", c => c.String(maxLength: 10));
			AddColumn("dbo.CompletionReportGroups", "CompletionReportId", c => c.Int(nullable: false));
			AlterColumn("dbo.NotificationTypes", "Id", c => c.Int(nullable: false, identity: true));
			AlterColumn("dbo.NotificationTypes", "MilestoneDateName", c => c.String(maxLength: 64));
			AlterColumn("dbo.ClaimEligibleCosts", "ClaimReimbursementCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
			AlterColumn("dbo.ReconciliationPayments", "SupplierName", c => c.String(nullable: false, maxLength: 250));
			RenameColumn("dbo.PaymentRequests", "ReconcileToCAS", "IsReconciled");
			AddPrimaryKey("dbo.PaymentRequests", new[] { "PaymentRequestBatchId", "ClaimId", "ClaimVersion" });

			PrePostDeployment();

			CreateIndex("dbo.GrantApplications", new[] { "ApplicationStateInternal", "ApplicationStateExternal", "GrantOpeningId", "AssessorId", "OrganizationLegalName", "ApplicationTypeId", "StartDate", "EndDate", "InvitationKey", "InvitationExpiresOn", "DeliveryPartnerId", "ScheduledNotificationsEnabled" }, name: "IX_GrantApplication");
			CreateIndex("dbo.GrantApplications", "CompletionReportId");
			CreateIndex("dbo.ParticipantForms", "FederalOfficialLanguageId");
			CreateIndex("dbo.ParticipantForms", "MartialStatusId");
			CreateIndex("dbo.CompletionReportGroups", "CompletionReportId");
			CreateIndex("dbo.TrainingProviderInventory", new[] { "IsActive", "IsEligible", "Name", "Acronym" }, name: "IX_TrainingProviderInventory");
			CreateIndex("dbo.PaymentRequests", new[] { "DocumentNumber", "IsReconciled", "PaymentAmount" }, name: "IX_PaymentRequest");
			CreateIndex("dbo.ServiceLines", new[] { "Caption", "ServiceCategoryId" }, name: "IX_Caption", unique: true);
			CreateIndex("dbo.ServiceLineBreakdowns", new[] { "Caption", "ServiceLineId" }, name: "IX_Caption", unique: true);
			AddForeignKey("dbo.ParticipantForms", "FederalOfficialLanguageId", "dbo.FederalOfficialLanguages", "Id");
			AddForeignKey("dbo.ParticipantForms", "MartialStatusId", "dbo.MartialStatus", "Id");
			AddForeignKey("dbo.CompletionReportGroups", "CompletionReportId", "dbo.CompletionReports", "Id");
			AddForeignKey("dbo.GrantApplications", "CompletionReportId", "dbo.CompletionReports", "Id");
			AddForeignKey("dbo.NotificationTypes", "NotificationTemplateId", "dbo.NotificationTemplates", "Id");
			AddForeignKey("dbo.NotificationTypes", "NotificationTriggerId", "dbo.NotificationTriggers", "Id");
			AddForeignKey("dbo.GrantProgramNotificationTypes", "NotificationTypeId", "dbo.NotificationTypes", "Id", cascadeDelete: true);
			DropColumn("dbo.NotificationTemplates", "AlertCaption");
			DropColumn("dbo.NotificationTemplates", "DefaultExpiryDays");
			DropColumn("dbo.TrainingPrograms", "EligibleExpenseBreakdown_Id");
			DropTable("dbo.Notifications");
			DropTable("dbo.NotificationScheduleQueue");
			DropTable("dbo.Payments");

			PostDeployment();

			CreateIndex("dbo.NotificationTypes", "NotificationTemplateId");
			CreateIndex("dbo.NotificationTypes", new[] { "NotificationTriggerId", "IsActive", "PreviousApplicationState", "CurrentApplicationState" }, name: "IX_NotificationType");

		}

		public override void Down()
		{
			CreateTable(
				"dbo.ClaimParticipants",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					ClaimId = c.Int(nullable: false),
					ClaimVersion = c.Int(nullable: false),
					ParticipantFormId = c.Int(nullable: false),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
				})
				.PrimaryKey(t => t.Id);

			CreateTable(
				"dbo.NotificationScheduleQueue",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					GrantApplicationId = c.Int(nullable: false),
					NotificationTypeId = c.Int(nullable: false),
					NotificationTemplateId = c.Int(nullable: false),
					SendStatus = c.Int(nullable: false),
					NotificationTicketId = c.String(nullable: false, maxLength: 32),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
				})
				.PrimaryKey(t => t.Id);

			CreateTable(
				"dbo.Notifications",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					NotificationTemplateId = c.Int(),
					NotificationScheduleQueueId = c.Int(),
					Viewed = c.Boolean(nullable: false),
					ExpiryDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					SendEmail = c.Boolean(nullable: false),
					EmailSentDate = c.DateTime(precision: 7, storeType: "datetime2"),
					EmailSubject = c.String(nullable: false, maxLength: 500),
					EmailBody = c.String(nullable: false),
					EmailRecipients = c.String(nullable: false),
					EmailSender = c.String(nullable: false),
					AlertCaption = c.String(maxLength: 250),
					AlertClearedDate = c.DateTime(precision: 7, storeType: "datetime2"),
					InternalUserId = c.Int(),
					Metadata = c.String(maxLength: 1000),
					HadError = c.Boolean(nullable: false),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
					User_Id = c.Int(),
				})
				.PrimaryKey(t => t.Id);

			AddColumn("dbo.NotificationTemplates", "DefaultExpiryDays", c => c.Int(nullable: false));
			AddColumn("dbo.NotificationTemplates", "AlertCaption", c => c.String(maxLength: 250));
			DropForeignKey("dbo.GrantProgramNotificationTypes", "NotificationTypeId", "dbo.NotificationTypes");
			DropForeignKey("dbo.ProgramNotificationRecipients", "ProgramNotificationId", "dbo.ProgramNotifications");
			DropForeignKey("dbo.ProgramNotifications", "NotificationTemplateId", "dbo.NotificationTemplates");
			DropForeignKey("dbo.ProgramNotificationRecipients", "GrantProgramId", "dbo.GrantPrograms");
			DropForeignKey("dbo.NotificationQueue", "OrganizationId", "dbo.Organizations");
			DropForeignKey("dbo.NotificationQueue", "NotificationTypeId", "dbo.NotificationTypes");
			DropForeignKey("dbo.NotificationTypes", "NotificationTriggerId", "dbo.NotificationTriggers");
			DropForeignKey("dbo.NotificationTypes", "NotificationTemplateId", "dbo.NotificationTemplates");
			DropForeignKey("dbo.NotificationQueue", "GrantApplicationId", "dbo.GrantApplications");
			DropForeignKey("dbo.GrantApplications", "CompletionReportId", "dbo.CompletionReports");
			DropForeignKey("dbo.ReconciliationReportPayments", "ReconciliationPaymentId", "dbo.ReconciliationPayments");
			DropForeignKey("dbo.ReconciliationReportPayments", "ReconciliationReportId", "dbo.ReconciliationReports");
			DropForeignKey("dbo.ReconciliationReports", "InternalUserId", "dbo.InternalUsers");
			DropForeignKey("dbo.ReconciliationPayments", "PaymentRequestBatchId", "dbo.PaymentRequestBatches");
			DropForeignKey("dbo.ReconciliationPayments", new[] { "PaymentRequestBatchId", "GrantApplicationId", "ClaimId" }, "dbo.PaymentRequests");
			DropForeignKey("dbo.ReconciliationPayments", "GrantApplicationId", "dbo.GrantApplications");
			DropForeignKey("dbo.ClaimParticipants", "ParticipantFormId", "dbo.ParticipantForms");
			DropForeignKey("dbo.ClaimParticipants", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims");
			DropForeignKey("dbo.CompletionReportGroups", "CompletionReportId", "dbo.CompletionReports");
			DropForeignKey("dbo.ParticipantForms", "MartialStatusId", "dbo.MartialStatus");
			DropForeignKey("dbo.ParticipantForms", "FederalOfficialLanguageId", "dbo.FederalOfficialLanguages");
			DropIndex("dbo.ReconciliationReportPayments", new[] { "ReconciliationPaymentId" });
			DropIndex("dbo.ReconciliationReportPayments", new[] { "ReconciliationReportId" });
			DropIndex("dbo.ProgramNotifications", new[] { "Caption" });
			DropIndex("dbo.ProgramNotifications", new[] { "NotificationTemplateId" });
			DropIndex("dbo.ProgramNotificationRecipients", new[] { "GrantProgramId" });
			DropIndex("dbo.ProgramNotificationRecipients", new[] { "ProgramNotificationId" });
			DropIndex("dbo.NotificationTriggers", "IX_Active");
			DropIndex("dbo.NotificationTriggers", new[] { "Caption" });
			DropIndex("dbo.NotificationTypes", new[] { "NotificationTriggerId" });
			DropIndex("dbo.NotificationTypes", new[] { "NotificationTemplateId" });
			DropIndex("dbo.NotificationTypes", "IX_NotificationType");
			DropIndex("dbo.NotificationQueue", "IX_NotificationQueue");
			DropIndex("dbo.NotificationQueue", new[] { "OrganizationId" });
			DropIndex("dbo.NotificationQueue", new[] { "GrantApplicationId" });
			DropIndex("dbo.NotificationQueue", new[] { "NotificationTypeId" });
			DropIndex("dbo.ReconciliationReports", new[] { "InternalUserId" });
			DropIndex("dbo.ReconciliationReports", "IX_ReconciliationReport");
			DropIndex("dbo.ReconciliationPayments", "IX_ReconciliationPayment");
			DropIndex("dbo.ReconciliationPayments", new[] { "PaymentRequestBatchId", "GrantApplicationId", "ClaimId" });
			DropIndex("dbo.PaymentRequests", "IX_PaymentRequest");
			DropIndex("dbo.TrainingProviderInventory", "IX_TrainingProviderInventory");
			DropIndex("dbo.CompletionReportGroups", new[] { "CompletionReportId" });
			DropIndex("dbo.MartialStatus", "IX_Active");
			DropIndex("dbo.MartialStatus", new[] { "Caption" });
			DropIndex("dbo.FederalOfficialLanguages", "IX_Active");
			DropIndex("dbo.FederalOfficialLanguages", new[] { "Caption" });
			DropIndex("dbo.ParticipantForms", new[] { "MartialStatusId" });
			DropIndex("dbo.ParticipantForms", new[] { "FederalOfficialLanguageId" });
			DropIndex("dbo.GrantApplications", new[] { "CompletionReportId" });
			DropIndex("dbo.GrantApplications", "IX_GrantApplication");
			DropPrimaryKey("dbo.NotificationTypes");
			AlterColumn("dbo.ClaimEligibleCosts", "ClaimReimbursementCost", c => c.Double(nullable: false));
			AlterColumn("dbo.NotificationTypes", "MilestoneDateName", c => c.String(nullable: false, maxLength: 64));
			AlterColumn("dbo.NotificationTypes", "Id", c => c.Int(nullable: false));
			DropColumn("dbo.PaymentRequests", "IsReconciled");
			DropColumn("dbo.CompletionReportGroups", "CompletionReportId");
			DropColumn("dbo.TrainingProviderInventory", "Acronym");
			DropColumn("dbo.ParticipantForms", "NumberOfDependents");
			DropColumn("dbo.ParticipantForms", "MartialStatusId");
			DropColumn("dbo.ParticipantForms", "FederalOfficialLanguageId");
			DropColumn("dbo.Organizations", "DoingBusinessAsMinistry");
			DropColumn("dbo.NotificationTypes", "RecipientRule");
			DropColumn("dbo.NotificationTypes", "CompletionReportRule");
			DropColumn("dbo.NotificationTypes", "ClaimReportRule");
			DropColumn("dbo.NotificationTypes", "ParticipantReportRule");
			DropColumn("dbo.NotificationTypes", "ApprovalRule");
			DropColumn("dbo.NotificationTypes", "ResendRule");
			DropColumn("dbo.NotificationTypes", "ResendDelayDays");
			DropColumn("dbo.NotificationTypes", "MilestoneDateExpires");
			DropColumn("dbo.NotificationTypes", "NotificationTriggerId");
			DropColumn("dbo.NotificationTypes", "CurrentApplicationState");
			DropColumn("dbo.NotificationTypes", "PreviousApplicationState");
			DropColumn("dbo.NotificationTypes", "NotificationTemplateId");
			DropColumn("dbo.NotificationTemplates", "Caption");
			DropColumn("dbo.GrantApplications", "ScheduledNotificationsEnabled");
			DropColumn("dbo.GrantApplications", "CompletionReportId");
			DropTable("dbo.ReconciliationReportPayments");
			DropTable("dbo.ClaimParticipants");
			DropTable("dbo.ProgramNotifications");
			DropTable("dbo.ProgramNotificationRecipients");
			DropTable("dbo.NotificationTriggers");
			DropTable("dbo.NotificationQueue");
			DropTable("dbo.ReconciliationReports");
			DropTable("dbo.ReconciliationPayments");
			DropTable("dbo.MartialStatus");
			DropTable("dbo.FederalOfficialLanguages");
			AddPrimaryKey("dbo.NotificationTypes", "Id");
			CreateIndex("dbo.TrainingProviderInventory", new[] { "IsActive", "IsEligible", "Name" }, name: "IX_TrainingProviderInventory");
			CreateIndex("dbo.NotificationTypes", "IsActive", name: "IX_Active");
			CreateIndex("dbo.NotificationScheduleQueue", "NotificationTemplateId");
			CreateIndex("dbo.NotificationScheduleQueue", new[] { "SendStatus", "NotificationTypeId" }, name: "IX_NotificationScheduleQueue");
			CreateIndex("dbo.NotificationScheduleQueue", "GrantApplicationId");
			CreateIndex("dbo.Notifications", "User_Id");
			CreateIndex("dbo.Notifications", "InternalUserId");
			CreateIndex("dbo.Notifications", "NotificationScheduleQueueId");
			CreateIndex("dbo.Notifications", "NotificationTemplateId");
			CreateIndex("dbo.GrantApplications", new[] { "ApplicationStateInternal", "ApplicationStateExternal", "GrantOpeningId", "AssessorId", "OrganizationLegalName", "ApplicationTypeId", "StartDate", "EndDate", "InvitationKey", "InvitationExpiresOn", "DeliveryPartnerId" }, name: "IX_GrantApplication");
			AddForeignKey("dbo.GrantProgramNotifications", "NotificationTypeId", "dbo.NotificationTypes", "Id", cascadeDelete: true);
			AddForeignKey("dbo.ClaimParticipants", "ParticipantFormId", "dbo.ParticipantForms", "Id", cascadeDelete: true);
			AddForeignKey("dbo.ClaimParticipants", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims", new[] { "Id", "ClaimVersion" }, cascadeDelete: true);
			AddForeignKey("dbo.Notifications", "User_Id", "dbo.Users", "Id");
			AddForeignKey("dbo.Notifications", "InternalUserId", "dbo.InternalUsers", "Id");
			AddForeignKey("dbo.Notifications", "NotificationTemplateId", "dbo.NotificationTemplates", "Id");
			AddForeignKey("dbo.Notifications", "NotificationScheduleQueueId", "dbo.NotificationScheduleQueue", "Id");
			AddForeignKey("dbo.NotificationScheduleQueue", "NotificationTypeId", "dbo.NotificationTypes", "Id", cascadeDelete: true);
			AddForeignKey("dbo.NotificationScheduleQueue", "NotificationTemplateId", "dbo.NotificationTemplates", "Id", cascadeDelete: true);
			AddForeignKey("dbo.NotificationScheduleQueue", "GrantApplicationId", "dbo.GrantApplications", "Id", cascadeDelete: true);
			RenameTable(name: "dbo.GrantProgramNotificationTypes", newName: "GrantProgramNotifications");
		}
	}
}
