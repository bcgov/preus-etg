namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("v01.09.00")]
    public partial class v010900 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            RenameTable(name: "dbo.ApplicationRoles", newName: "AspNetRoles");
            RenameTable(name: "dbo.ApplicationUserRoles", newName: "AspNetUserRoles");
            RenameTable(name: "dbo.ApplicationUsers", newName: "AspNetUsers");
            RenameTable(name: "dbo.ApplicationUserClaims", newName: "AspNetUserClaims");
            RenameTable(name: "dbo.ApplicationUserLogins", newName: "AspNetUserLogins");
            DropForeignKey("dbo.BusinessContactRoles", "EmployerEnrollmentId", "dbo.EmployerEnrollments");
            DropForeignKey("dbo.EmployerEnrollments", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.ParticipantEnrollments", "EmployerEnrollmentId", "dbo.EmployerEnrollments");
            DropForeignKey("dbo.ParticipantEnrollments", "ParticipantId", "dbo.Participants");
            DropForeignKey("dbo.EligibleCosts", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.PaymentRequests", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.TrainingProviders", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.Claims", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.ParticipantCosts", "ParticipantEnrollmentId", "dbo.ParticipantEnrollments");
            DropForeignKey("dbo.ParticipantEnrollments", "ParticipantFormId", "dbo.ParticipantForms");
            DropForeignKey("dbo.ParticipantForms", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.EmployerEnrollments", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.EmployerCompletionReportAnswers", "EmployerEnrollmentId", "dbo.EmployerEnrollments");
            DropForeignKey("dbo.ParticipantCompletionReportAnswers", "ParticipantEnrollmentId", "dbo.ParticipantEnrollments");
            DropForeignKey("dbo.PaymentRequests", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims");
            DropForeignKey("dbo.TrainingPrograms", "SkillFocusId", "dbo.SkillsFocus");
            DropIndex("dbo.AboriginalBands", new[] { "Caption" });
            DropIndex("dbo.GrantApplications", "IX_GrantApplication");
            DropIndex("dbo.GrantApplications", new[] { "NaicsId" });
            DropIndex("dbo.ApplicationTypes", new[] { "Caption" });
            DropIndex("dbo.BusinessContactRoles", "IX_BusinessContactRole");
            DropIndex("dbo.EmployerEnrollments", new[] { "TrainingProgramId" });
            DropIndex("dbo.EmployerEnrollments", new[] { "OrganizationId" });
            DropIndex("dbo.LegalStructures", new[] { "Caption" });
            DropIndex("dbo.OrganizationTypes", new[] { "Caption" });
            DropIndex("dbo.ParticipantEnrollments", "IX_ParticipantEnrollment");
            DropIndex("dbo.ParticipantCosts", new[] { "ParticipantEnrollmentId" });
            DropIndex("dbo.Claims", "IX_Claim");
            DropIndex("dbo.Claims", new[] { "TrainingProgramId" });
            DropIndex("dbo.PaymentRequests", new[] { "TrainingProgramId" });
            DropIndex("dbo.PaymentRequests", new[] { "ClaimId", "ClaimVersion" });
            DropIndex("dbo.PaymentRequestBatches", new[] { "GrantProgramId" });
            DropIndex("dbo.TrainingPrograms", "IX_TrainingPrograms");
            DropIndex("dbo.DeliveryMethods", new[] { "Caption" });
            DropIndex("dbo.DeliveryPartners", new[] { "Caption" });
            DropIndex("dbo.DeliveryPartnerServices", new[] { "Caption" });
            DropIndex("dbo.EligibleCosts", new[] { "TrainingProgramId" });
            DropIndex("dbo.EligibleExpenseTypes", new[] { "Caption" });
            DropIndex("dbo.ExpectedQualifications", new[] { "Caption" });
            DropIndex("dbo.InDemandOccupations", new[] { "Caption" });
            DropIndex("dbo.SkillsFocus", new[] { "Caption" });
            DropIndex("dbo.SkillLevels", new[] { "Caption" });
            DropIndex("dbo.TrainingLevels", new[] { "Caption" });
            DropIndex("dbo.TrainingProviders", "IX_TrainingProviders");
            DropIndex("dbo.TrainingProviderTypes", new[] { "Caption" });
            DropIndex("dbo.UnderRepresentedGroups", new[] { "Caption" });
            DropIndex("dbo.ParticipantForms", "IX_ParticipantForm");
            DropIndex("dbo.CanadianStatus", new[] { "Caption" });
            DropIndex("dbo.EducationLevels", new[] { "Caption" });
            DropIndex("dbo.EIBenefits", new[] { "Caption" });
            DropIndex("dbo.EmploymentStatus", new[] { "Caption" });
            DropIndex("dbo.EmploymentTypes", new[] { "Caption" });
            DropIndex("dbo.RecentPeriods", new[] { "Caption" });
            DropIndex("dbo.TrainingResults", new[] { "Caption" });
            DropIndex("dbo.GrantAgreements", "IX_GrantAgreement");
            DropIndex("dbo.GrantOpenings", "IX_GrantOpening");
            DropIndex("dbo.TrainingPeriods", "IX_TrainingPeriod");
            DropIndex("dbo.FiscalYears", "IX_FiscalYear_Dates");
            DropIndex("dbo.NoteTypes", new[] { "Caption" });
            DropIndex("dbo.PrioritySectors", new[] { "Caption" });
            DropIndex("dbo.ClaimStates", new[] { "Caption" });
            DropIndex("dbo.EmployerCompletionReportAnswers", new[] { "EmployerEnrollmentId" });
            DropIndex("dbo.GrantApplicationExternalStates", new[] { "Caption" });
            DropIndex("dbo.GrantApplicationInternalStates", new[] { "Caption" });
            DropIndex("dbo.Logs", "IX_Logs");
            DropIndex("dbo.ParticipantCompletionReportAnswers", new[] { "ParticipantEnrollmentId" });
            DropPrimaryKey("dbo.PaymentRequests");
            DropPrimaryKey("dbo.EmployerCompletionReportAnswers");
            DropPrimaryKey("dbo.ParticipantCompletionReportAnswers");
            CreateTable(
                "dbo.UserGrantProgramPreferences",
                c => new
                {
                    UserId = c.Int(nullable: false),
                    GrantProgramId = c.Int(nullable: false),
                    IsSelected = c.Boolean(nullable: false),
                    DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => new { t.UserId, t.GrantProgramId })
                .ForeignKey("dbo.GrantPrograms", t => t.GrantProgramId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => new { t.UserId, t.GrantProgramId, t.IsSelected }, name: "IX_UserGrantProgramPreference");

            CreateTable(
                "dbo.UserPreferences",
                c => new
                {
                    UserId = c.Int(nullable: false),
                    GrantProgramPreferencesUpdated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.ClaimTypes",
                c => new
                {
                    Id = c.Int(nullable: false),
                    Description = c.String(maxLength: 1000),
                    IsAmendable = c.Boolean(nullable: false),
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
                "dbo.ProgramConfigurations",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Caption = c.String(nullable: false, maxLength: 200),
                    Description = c.String(maxLength: 1000),
                    ClaimTypeId = c.Int(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    SkillsTrainingMaxEstimatedParticipantCosts = c.Decimal(nullable: false, precision: 18, scale: 2),
                    ESSMaxEstimatedParticipantCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClaimTypes", t => t.ClaimTypeId, cascadeDelete: true)
                .Index(t => t.Caption, unique: true, name: "IX_ClaimType_Caption")
                .Index(t => t.ClaimTypeId, name: "IX_ProgramConfiguration_ClaimTypeId")
                .Index(t => t.IsActive, name: "IX_ProgramConfiguration_IsActive");

            CreateTable(
                "dbo.EligibleExpenseBreakdowns",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Description = c.String(),
                    EligibleExpenseTypeId = c.Int(nullable: false),
                    ServiceLineId = c.Int(),
                    EnableCost = c.Boolean(nullable: false),
                    Caption = c.String(nullable: false, maxLength: 250),
                    IsActive = c.Boolean(nullable: false),
                    RowSequence = c.Int(nullable: false),
                    DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceLines", t => t.ServiceLineId)
                .ForeignKey("dbo.EligibleExpenseTypes", t => t.EligibleExpenseTypeId, cascadeDelete: true)
                .Index(t => new { t.Caption, t.EligibleExpenseTypeId }, unique: true, name: "IX_Caption")
                .Index(t => t.ServiceLineId)
                .Index(t => t.IsActive, name: "IX_Active");

            CreateTable(
                "dbo.ClaimBreakdownCosts",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    EligibleExpenseBreakdownId = c.Int(nullable: false),
                    ClaimEligibleCostId = c.Int(nullable: false),
                    ClaimCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    AssessedCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClaimEligibleCosts", t => t.ClaimEligibleCostId, cascadeDelete: true)
                .ForeignKey("dbo.EligibleExpenseBreakdowns", t => t.EligibleExpenseBreakdownId)
                .Index(t => t.EligibleExpenseBreakdownId)
                .Index(t => t.ClaimEligibleCostId);

            CreateTable(
                "dbo.EligibleCostBreakdowns",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    EligibleCostId = c.Int(nullable: false),
                    EligibleExpenseBreakdownId = c.Int(nullable: false),
                    EstimatedCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    AssessedCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EligibleCosts", t => t.EligibleCostId, cascadeDelete: true)
                .ForeignKey("dbo.EligibleExpenseBreakdowns", t => t.EligibleExpenseBreakdownId)
                .Index(t => t.EligibleCostId)
                .Index(t => t.EligibleExpenseBreakdownId);

            CreateTable(
                "dbo.ServiceLines",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Description = c.String(maxLength: 1000),
                    BreakdownCaption = c.String(maxLength: 250),
                    ServiceCategoryId = c.Int(nullable: false),
                    EnableCost = c.Boolean(nullable: false),
                    Caption = c.String(nullable: false, maxLength: 250),
                    IsActive = c.Boolean(nullable: false),
                    RowSequence = c.Int(nullable: false),
                    DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceCategories", t => t.ServiceCategoryId, cascadeDelete: true)
                .Index(t => t.ServiceCategoryId)
                .Index(t => t.Caption, unique: true)
                .Index(t => t.IsActive, name: "IX_Active");

            CreateTable(
                "dbo.ServiceCategories",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Description = c.String(maxLength: 1000),
                    ServiceTypeId = c.Int(nullable: false),
                    Rate = c.Double(),
                    AutoInclude = c.Boolean(nullable: false),
                    AllowMultiple = c.Boolean(nullable: false),
                    MinProviders = c.Int(nullable: false),
                    MaxProviders = c.Int(nullable: false),
                    MinPrograms = c.Int(nullable: false),
                    MaxPrograms = c.Int(nullable: false),
                    CompletionReport = c.Boolean(nullable: false),
                    Caption = c.String(nullable: false, maxLength: 250),
                    IsActive = c.Boolean(nullable: false),
                    RowSequence = c.Int(nullable: false),
                    DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceTypes", t => t.ServiceTypeId, cascadeDelete: true)
                .Index(t => t.ServiceTypeId)
                .Index(t => t.Caption, unique: true)
                .Index(t => t.IsActive, name: "IX_Active");

            CreateTable(
                "dbo.ServiceTypes",
                c => new
                {
                    Id = c.Int(nullable: false),
                    Description = c.String(maxLength: 1000),
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
                "dbo.ServiceLineBreakdowns",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Description = c.String(maxLength: 1000),
                    ServiceLineId = c.Int(nullable: false),
                    Caption = c.String(nullable: false, maxLength: 250),
                    IsActive = c.Boolean(nullable: false),
                    RowSequence = c.Int(nullable: false),
                    DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceLines", t => t.ServiceLineId, cascadeDelete: true)
                .Index(t => t.ServiceLineId)
                .Index(t => t.Caption, unique: true)
                .Index(t => t.IsActive, name: "IX_Active");

            CreateTable(
                "dbo.TrainingCosts",
                c => new
                {
                    GrantApplicationId = c.Int(nullable: false),
                    TrainingCostState = c.Int(nullable: false),
                    EstimatedParticipants = c.Int(nullable: false),
                    TotalEstimatedCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    TotalEstimatedReimbursement = c.Decimal(nullable: false, precision: 18, scale: 2),
                    AgreedParticipants = c.Int(nullable: false),
                    TotalAgreedMaxCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    AgreedCommitment = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.GrantApplicationId)
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
                .Index(t => t.GrantApplicationId);

            CreateTable(
                "dbo.ExpenseTypes",
                c => new
                {
                    Id = c.Int(nullable: false),
                    Description = c.String(maxLength: 1000),
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
                "dbo.ProgramDescriptions",
                c => new
                {
                    GrantApplicationId = c.Int(nullable: false),
                    Description = c.String(nullable: false, maxLength: 2000),
                    DescriptionState = c.Int(nullable: false),
                    SupportingEmployers = c.Int(nullable: false),
                    ParticipantEmploymentStatusId = c.Int(),
                    TargetNAICSId = c.Int(),
                    TargetNOCId = c.Int(),
                    ApplicantOrganizationTypeId = c.Int(),
                    ApplicantOrganizationTypeInfo = c.String(maxLength: 200),
                    DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.GrantApplicationId)
                .ForeignKey("dbo.ApplicantOrganizationTypes", t => t.ApplicantOrganizationTypeId)
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
                .ForeignKey("dbo.NationalOccupationalClassifications", t => t.TargetNOCId)
                .ForeignKey("dbo.ParticipantEmploymentStatus", t => t.ParticipantEmploymentStatusId)
                .ForeignKey("dbo.NaIndustryClassificationSystems", t => t.TargetNAICSId)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.ParticipantEmploymentStatusId)
                .Index(t => t.TargetNAICSId)
                .Index(t => t.TargetNOCId)
                .Index(t => t.ApplicantOrganizationTypeId);

            CreateTable(
                "dbo.ApplicantOrganizationTypes",
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
                "dbo.Communities",
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
                "dbo.ParticipantEmploymentStatus",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
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
                "dbo.UnderRepresentedPopulations",
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
                "dbo.VulnerableGroups",
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
                "dbo.RiskClassifications",
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
                "dbo.ProgramTypes",
                c => new
                {
                    Id = c.Int(nullable: false),
                    Description = c.String(maxLength: 1000),
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
                "dbo.GrantApplicationAttachments",
                c => new
                {
                    GrantApplicationId = c.Int(nullable: false),
                    AttachmentId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.GrantApplicationId, t.AttachmentId })
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
                .ForeignKey("dbo.Attachments", t => t.AttachmentId)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.AttachmentId);

            CreateTable(
                "dbo.TrainingProgramTrainingProviders",
                c => new
                {
                    TrainingProgramId = c.Int(nullable: false),
                    TrainingProviderId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.TrainingProgramId, t.TrainingProviderId })
                .ForeignKey("dbo.TrainingPrograms", t => t.TrainingProgramId)
                .ForeignKey("dbo.TrainingProviders", t => t.TrainingProviderId)
                .Index(t => t.TrainingProgramId)
                .Index(t => t.TrainingProviderId);

            CreateTable(
                "dbo.ProgramConfigurationEligibleExpenseTypes",
                c => new
                {
                    ProgramConfigurationId = c.Int(nullable: false),
                    EligibleExpenseTypeId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.ProgramConfigurationId, t.EligibleExpenseTypeId })
                .ForeignKey("dbo.ProgramConfigurations", t => t.ProgramConfigurationId)
                .ForeignKey("dbo.EligibleExpenseTypes", t => t.EligibleExpenseTypeId)
                .Index(t => t.ProgramConfigurationId)
                .Index(t => t.EligibleExpenseTypeId);

            CreateTable(
                "dbo.ProgramDescriptionCommunities",
                c => new
                {
                    GrantApplicationId = c.Int(nullable: false),
                    CommunityId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.GrantApplicationId, t.CommunityId })
                .ForeignKey("dbo.ProgramDescriptions", t => t.GrantApplicationId)
                .ForeignKey("dbo.Communities", t => t.CommunityId)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.CommunityId);

            CreateTable(
                "dbo.ProgramDescriptionUnderRepresentedPopulations",
                c => new
                {
                    GrantApplicationId = c.Int(nullable: false),
                    UnderRepresentedPopulationId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.GrantApplicationId, t.UnderRepresentedPopulationId })
                .ForeignKey("dbo.ProgramDescriptions", t => t.GrantApplicationId)
                .ForeignKey("dbo.UnderRepresentedPopulations", t => t.UnderRepresentedPopulationId)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.UnderRepresentedPopulationId);

            CreateTable(
                "dbo.ProgramDescriptionVulnerableGroups",
                c => new
                {
                    GrantApplicationId = c.Int(nullable: false),
                    VulnerableGroupId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.GrantApplicationId, t.VulnerableGroupId })
                .ForeignKey("dbo.ProgramDescriptions", t => t.GrantApplicationId)
                .ForeignKey("dbo.VulnerableGroups", t => t.VulnerableGroupId)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.VulnerableGroupId);

            AddColumn("dbo.GrantPrograms", "UseFIFOReservation", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantPrograms", "DateImplemented", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.GrantPrograms", "ProgramConfigurationId", c => c.Int(nullable: false));
            AddColumn("dbo.GrantPrograms", "ProgramTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.GrantApplications", "RiskClassificationId", c => c.Int());
            AddColumn("dbo.GrantApplications", "EligibilityConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantApplications", "CanReportParticipants", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantApplications", "StartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.GrantApplications", "EndDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AddColumn("dbo.GrantApplications", "InvitationKey", c => c.Guid(nullable: false));
            AddColumn("dbo.GrantApplications", "InvitationExpiresOn", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.GrantApplications", "HoldPaymentRequests", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantProgramNotifications", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.ParticipantCosts", "ParticipantFormId", c => c.Int(nullable: false));
            AddColumn("dbo.ClaimEligibleCosts", "ClaimReimbursementCost", c => c.Double(nullable: false));
            AddColumn("dbo.ClaimEligibleCosts", "AssessedReimbursementCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Claims", "GrantApplicationId", c => c.Int(nullable: false));
            AddColumn("dbo.Claims", "ClaimTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.PaymentRequests", "GrantApplicationId", c => c.Int(nullable: false));
            AddColumn("dbo.PaymentRequestBatches", "IssuedByName", c => c.String(nullable: false, maxLength: 250));
            AddColumn("dbo.PaymentRequestBatches", "ExpenseAuthorityName", c => c.String(nullable: false, maxLength: 250));
            AddColumn("dbo.PaymentRequestBatches", "BatchRequestDescription", c => c.String(maxLength: 2000));
            AddColumn("dbo.TrainingPrograms", "ServiceLineId", c => c.Int());
            AddColumn("dbo.TrainingPrograms", "ServiceLineBreakdownId", c => c.Int());
            AddColumn("dbo.TrainingPrograms", "EligibleCostBreakdownId", c => c.Int());
            AddColumn("dbo.TrainingPrograms", "EligibleExpenseBreakdown_Id", c => c.Int());
            AddColumn("dbo.EligibleCosts", "GrantApplicationId", c => c.Int(nullable: false));
            AddColumn("dbo.EligibleExpenseTypes", "ExpenseTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.EligibleExpenseTypes", "Rate", c => c.Double());
            AddColumn("dbo.EligibleExpenseTypes", "AutoInclude", c => c.Boolean(nullable: false));
            AddColumn("dbo.EligibleExpenseTypes", "AllowMultiple", c => c.Boolean(nullable: false));
            AddColumn("dbo.EligibleExpenseTypes", "ServiceCategoryId", c => c.Int());
            AddColumn("dbo.EligibleExpenseTypes", "MinProviders", c => c.Int(nullable: false));
            AddColumn("dbo.EligibleExpenseTypes", "MaxProviders", c => c.Int(nullable: false));
            AddColumn("dbo.TrainingProviders", "EligibleCostId", c => c.Int());
            AddColumn("dbo.ParticipantForms", "GrantApplicationId", c => c.Int());
            AddColumn("dbo.ParticipantForms", "ParticipantId", c => c.Int());
            AddColumn("dbo.ParticipantForms", "ClaimReported", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantStreams", "AttachmentsIsEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantStreams", "AttachmentsHeader", c => c.String(maxLength: 200));
            AddColumn("dbo.GrantStreams", "AttachmentsUserGuidance", c => c.String(maxLength: 2500));
            AddColumn("dbo.GrantStreams", "AttachmentsMaximum", c => c.Int(nullable: false));
            AddColumn("dbo.GrantStreams", "AttachmentsRequired", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantStreams", "EligibilityRequirements", c => c.String(maxLength: 2000));
            AddColumn("dbo.GrantStreams", "EligibilityEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantStreams", "EligibilityQuestion", c => c.String(maxLength: 2000));
            AddColumn("dbo.GrantStreams", "EligibilityRequired", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantStreams", "CanReportParticipants", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantStreams", "ProgramConfigurationId", c => c.Int(nullable: false));
            AddColumn("dbo.EmployerCompletionReportAnswers", "GrantApplicationId", c => c.Int(nullable: false));
            AddColumn("dbo.ParticipantCompletionReportAnswers", "ParticipantFormId", c => c.Int(nullable: false));
            AddColumn("dbo.ParticipantCompletionReportAnswers", "GrantApplicationId", c => c.Int(nullable: false));
            AlterColumn("dbo.AboriginalBands", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.AboriginalBands", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.AboriginalBands", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.AccountCodes", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.AccountCodes", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantPrograms", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantPrograms", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DocumentTemplates", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DocumentTemplates", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.InternalUsers", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.InternalUsers", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantApplications", "DateSubmitted", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantApplications", "DateCancelled", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantApplications", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantApplications", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ApplicationAddresses", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ApplicationAddresses", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Countries", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Countries", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Regions", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Regions", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ApplicationTypes", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.ApplicationTypes", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ApplicationTypes", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.BusinessContactRoles", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.BusinessContactRoles", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Organizations", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Organizations", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Addresses", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Addresses", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.LegalStructures", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.LegalStructures", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.LegalStructures", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NaIndustryClassificationSystems", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NaIndustryClassificationSystems", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.OrganizationTypes", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.OrganizationTypes", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.OrganizationTypes", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Users", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Users", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Notifications", "ExpiryDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Notifications", "EmailSentDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Notifications", "AlertClearedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Notifications", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Notifications", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NotificationScheduleQueue", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NotificationScheduleQueue", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NotificationTypes", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NotificationTypes", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantProgramNotifications", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantProgramNotifications", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NotificationTemplates", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NotificationTemplates", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Participants", "BirthDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Participants", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Participants", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ParticipantCosts", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ParticipantCosts", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ClaimEligibleCosts", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ClaimEligibleCosts", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Claims", "DateSubmitted", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Claims", "DateAssessed", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Claims", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Claims", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PaymentRequests", "ClaimId", c => c.Int(nullable: false));
            AlterColumn("dbo.PaymentRequests", "ClaimVersion", c => c.Int(nullable: false));
            AlterColumn("dbo.PaymentRequests", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PaymentRequests", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PaymentRequestBatches", "IssuedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PaymentRequestBatches", "GrantProgramId", c => c.Int(nullable: false));
            AlterColumn("dbo.PaymentRequestBatches", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PaymentRequestBatches", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingPrograms", "SkillFocusId", c => c.Int());
            AlterColumn("dbo.TrainingPrograms", "StartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingPrograms", "EndDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingPrograms", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingPrograms", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DeliveryMethods", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.DeliveryMethods", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DeliveryMethods", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DeliveryPartners", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.DeliveryPartners", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DeliveryPartners", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DeliveryPartnerServices", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.DeliveryPartnerServices", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.DeliveryPartnerServices", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EligibleCosts", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EligibleCosts", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EligibleExpenseTypes", "Description", c => c.String(maxLength: 1000));
            AlterColumn("dbo.EligibleExpenseTypes", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.EligibleExpenseTypes", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EligibleExpenseTypes", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ExpectedQualifications", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.ExpectedQualifications", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ExpectedQualifications", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.InDemandOccupations", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.InDemandOccupations", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.InDemandOccupations", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SkillsFocus", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.SkillsFocus", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SkillsFocus", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SkillLevels", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.SkillLevels", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.SkillLevels", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingLevels", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.TrainingLevels", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingLevels", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingProviders", "GrantApplicationId", c => c.Int());
            AlterColumn("dbo.TrainingProviders", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingProviders", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Attachments", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Attachments", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.VersionedAttachments", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.VersionedAttachments", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingProviderInventory", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingProviderInventory", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingProviderTypes", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.TrainingProviderTypes", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingProviderTypes", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.UnderRepresentedGroups", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.UnderRepresentedGroups", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.UnderRepresentedGroups", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Payments", "DatePaid", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Payments", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Payments", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ParticipantForms", "ProgramStartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ParticipantForms", "BirthDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ParticipantForms", "ConsentDateEntered", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ParticipantForms", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ParticipantForms", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CanadianStatus", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.CanadianStatus", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CanadianStatus", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EducationLevels", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.EducationLevels", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EducationLevels", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EIBenefits", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.EIBenefits", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EIBenefits", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EmploymentStatus", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.EmploymentStatus", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EmploymentStatus", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EmploymentTypes", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.EmploymentTypes", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EmploymentTypes", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NationalOccupationalClassifications", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NationalOccupationalClassifications", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RecentPeriods", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.RecentPeriods", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RecentPeriods", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingResults", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.TrainingResults", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingResults", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantAgreements", "ParticipantReportingDueDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantAgreements", "ReimbursementClaimDueDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantAgreements", "CompletionReportingDueDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantAgreements", "DateAccepted", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantAgreements", "StartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantAgreements", "EndDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantAgreements", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantAgreements", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Documents", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Documents", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.VersionedDocuments", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.VersionedDocuments", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantOpenings", "PublishDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantOpenings", "OpeningDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantOpenings", "ClosingDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantOpenings", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantOpenings", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantOpeningFinancials", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantOpeningFinancials", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantOpeningIntakes", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantOpeningIntakes", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantStreams", "DateFirstUsed", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantStreams", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantStreams", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingPeriods", "StartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingPeriods", "EndDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingPeriods", "DefaultPublishDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingPeriods", "DefaultOpeningDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingPeriods", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TrainingPeriods", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.FiscalYears", "StartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.FiscalYears", "EndDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.FiscalYears", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.FiscalYears", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ReportRates", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ReportRates", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Notes", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Notes", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NoteTypes", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.NoteTypes", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.NoteTypes", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PrioritySectors", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.PrioritySectors", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.PrioritySectors", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantApplicationStateChanges", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantApplicationStateChanges", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.InternalUserFilters", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.InternalUserFilters", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.InternalUserFilterAttributes", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.InternalUserFilterAttributes", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ApplicationClaims", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ApplicationClaims", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ClaimIds", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ClaimIds", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ClaimStates", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.ClaimStates", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ClaimStates", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CompletionReportOptions", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CompletionReportOptions", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CompletionReportQuestions", "AnswerTableHeadings", c => c.String(maxLength: 500));
            AlterColumn("dbo.CompletionReportQuestions", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CompletionReportQuestions", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CompletionReports", "EffectiveDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CompletionReports", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CompletionReports", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CompletionReportGroups", "Title", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.CompletionReportGroups", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CompletionReportGroups", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EmployerCompletionReportAnswers", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.EmployerCompletionReportAnswers", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantApplicationExternalStates", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.GrantApplicationExternalStates", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantApplicationExternalStates", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantApplicationInternalStates", "Caption", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.GrantApplicationInternalStates", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.GrantApplicationInternalStates", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Logs", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Logs", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ParticipantCompletionReportAnswers", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ParticipantCompletionReportAnswers", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RateFormats", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RateFormats", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Settings", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Settings", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TempData", "DateAdded", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TempData", "DateUpdated", c => c.DateTime(precision: 7, storeType: "datetime2"));

            DeployScripts("PrePostDeploy");

            AddPrimaryKey("dbo.PaymentRequests", new[] { "PaymentRequestBatchId", "GrantApplicationId" });
            AddPrimaryKey("dbo.EmployerCompletionReportAnswers", new[] { "GrantApplicationId", "QuestionId" });
            AddPrimaryKey("dbo.ParticipantCompletionReportAnswers", new[] { "ParticipantFormId", "GrantApplicationId", "QuestionId" });
            CreateIndex("dbo.AboriginalBands", "Caption", unique: true);
            CreateIndex("dbo.AboriginalBands", "IsActive", name: "IX_Active");
            CreateIndex("dbo.GrantPrograms", "ProgramConfigurationId");
            CreateIndex("dbo.GrantPrograms", "ProgramTypeId");
            CreateIndex("dbo.GrantApplications", new[] { "ApplicationStateInternal", "ApplicationStateExternal", "GrantOpeningId", "AssessorId", "OrganizationLegalName", "ApplicationTypeId", "StartDate", "EndDate", "InvitationKey", "InvitationExpiresOn" }, name: "IX_GrantApplication");
            CreateIndex("dbo.GrantApplications", "RiskClassificationId");
            CreateIndex("dbo.GrantApplications", "PrioritySectorId");
            CreateIndex("dbo.GrantApplications", "NAICSId");
            CreateIndex("dbo.ApplicationTypes", "Caption", unique: true);
            CreateIndex("dbo.ApplicationTypes", "IsActive", name: "IX_Active");
            CreateIndex("dbo.BusinessContactRoles", new[] { "UserId", "GrantApplicationId" }, unique: true, name: "IX_BusinessContactRole");
            CreateIndex("dbo.LegalStructures", "Caption", unique: true);
            CreateIndex("dbo.LegalStructures", "IsActive", name: "IX_Active");
            CreateIndex("dbo.OrganizationTypes", "Caption", unique: true);
            CreateIndex("dbo.OrganizationTypes", "IsActive", name: "IX_Active");
            CreateIndex("dbo.Claims", new[] { "AssessorId", "ClaimState", "ClaimNumber", "DateSubmitted", "DateAssessed" }, name: "IX_Claim");
            CreateIndex("dbo.Claims", "GrantApplicationId");
            CreateIndex("dbo.Claims", "ClaimTypeId");
            CreateIndex("dbo.EligibleExpenseTypes", "Caption");
            CreateIndex("dbo.EligibleExpenseTypes", new[] { "ServiceCategoryId", "Caption" }, name: "IX_EligibleExpenseType");
            CreateIndex("dbo.EligibleExpenseTypes", "ExpenseTypeId");
            CreateIndex("dbo.EligibleExpenseTypes", "IsActive", name: "IX_Active");
            CreateIndex("dbo.EligibleCosts", "GrantApplicationId");
            CreateIndex("dbo.TrainingPrograms", new[] { "TrainingProgramState", "DateAdded", "GrantApplicationId", "StartDate", "EndDate", "InDemandOccupationId", "SkillLevelId", "SkillFocusId", "ExpectedQualificationId", "TrainingLevelId", "DeliveryPartnerId" }, name: "IX_TrainingPrograms");
            CreateIndex("dbo.TrainingPrograms", "ServiceLineId");
            CreateIndex("dbo.TrainingPrograms", "ServiceLineBreakdownId");
            CreateIndex("dbo.TrainingPrograms", "EligibleCostBreakdownId");
            CreateIndex("dbo.TrainingPrograms", "EligibleExpenseBreakdown_Id");
            CreateIndex("dbo.DeliveryMethods", "Caption", unique: true);
            CreateIndex("dbo.DeliveryMethods", "IsActive", name: "IX_Active");
            CreateIndex("dbo.DeliveryPartners", "Caption", unique: true);
            CreateIndex("dbo.DeliveryPartners", "IsActive", name: "IX_Active");
            CreateIndex("dbo.DeliveryPartnerServices", "Caption", unique: true);
            CreateIndex("dbo.DeliveryPartnerServices", "IsActive", name: "IX_Active");
            CreateIndex("dbo.ExpectedQualifications", "Caption", unique: true);
            CreateIndex("dbo.ExpectedQualifications", "IsActive", name: "IX_Active");
            CreateIndex("dbo.InDemandOccupations", "Caption", unique: true);
            CreateIndex("dbo.InDemandOccupations", "IsActive", name: "IX_Active");
            CreateIndex("dbo.SkillsFocus", "Caption", unique: true);
            CreateIndex("dbo.SkillsFocus", "IsActive", name: "IX_Active");
            CreateIndex("dbo.SkillLevels", "Caption", unique: true);
            CreateIndex("dbo.SkillLevels", "IsActive", name: "IX_Active");
            CreateIndex("dbo.TrainingLevels", "Caption", unique: true);
            CreateIndex("dbo.TrainingLevels", "IsActive", name: "IX_Active");
            CreateIndexWithInclude("dbo.TrainingProviders", new[] { "TrainingProviderState", "GrantApplicationId", "TrainingProviderTypeId", "TrainingOutsideBC", "DateAdded", "ContactLastName", "ContactFirstName" }, includes: new[] { "Name" }, name: "IX_TrainingProviders");
            CreateIndex("dbo.TrainingProviders", "EligibleCostId");
            CreateIndex("dbo.TrainingProviderTypes", "Caption", unique: true);
            CreateIndex("dbo.TrainingProviderTypes", "IsActive", name: "IX_Active");
            CreateIndex("dbo.UnderRepresentedGroups", "Caption", unique: true);
            CreateIndex("dbo.UnderRepresentedGroups", "IsActive", name: "IX_Active");
            CreateIndex("dbo.ParticipantCosts", "ParticipantFormId");
            CreateIndex("dbo.ParticipantForms", new[] { "GrantApplicationId", "ParticipantId" }, name: "IX_ParticipantForm");
            CreateIndex("dbo.CanadianStatus", "Caption", unique: true);
            CreateIndex("dbo.CanadianStatus", "IsActive", name: "IX_Active");
            CreateIndex("dbo.EducationLevels", "Caption", unique: true);
            CreateIndex("dbo.EducationLevels", "IsActive", name: "IX_Active");
            CreateIndex("dbo.EIBenefits", "Caption", unique: true);
            CreateIndex("dbo.EIBenefits", "IsActive", name: "IX_Active");
            CreateIndex("dbo.EmploymentStatus", "Caption", unique: true);
            CreateIndex("dbo.EmploymentStatus", "IsActive", name: "IX_Active");
            CreateIndex("dbo.EmploymentTypes", "Caption", unique: true);
            CreateIndex("dbo.EmploymentTypes", "IsActive", name: "IX_Active");
            CreateIndex("dbo.ParticipantCompletionReportAnswers", "ParticipantFormId");
            CreateIndex("dbo.ParticipantCompletionReportAnswers", "GrantApplicationId");
            CreateIndex("dbo.RecentPeriods", "Caption", unique: true);
            CreateIndex("dbo.RecentPeriods", "IsActive", name: "IX_Active");
            CreateIndex("dbo.TrainingResults", "Caption", unique: true);
            CreateIndex("dbo.TrainingResults", "IsActive", name: "IX_Active");
            CreateIndex("dbo.GrantStreams", "ProgramConfigurationId");
            CreateIndex("dbo.GrantOpenings", new[] { "State", "PublishDate", "OpeningDate", "ClosingDate", "TrainingPeriodId", "GrantStreamId" }, name: "IX_GrantOpening");
            CreateIndex("dbo.TrainingPeriods", new[] { "FiscalYearId", "StartDate", "EndDate" }, unique: true, name: "IX_TrainingPeriod");
            CreateIndex("dbo.FiscalYears", new[] { "StartDate", "EndDate" }, unique: true, name: "IX_FiscalYear_Dates");
            CreateIndex("dbo.ReportRates", "GrantStreamId");
            CreateIndex("dbo.PaymentRequests", "GrantApplicationId");
            CreateIndex("dbo.PaymentRequests", new[] { "ClaimId", "ClaimVersion" });
            CreateIndex("dbo.PaymentRequestBatches", "BatchNumber", name: "IX_PaymentRequestBatch");
            CreateIndex("dbo.PaymentRequestBatches", "GrantProgramId");
            CreateIndex("dbo.EmployerCompletionReportAnswers", "GrantApplicationId");
            CreateIndex("dbo.GrantAgreements", new[] { "StartDate", "EndDate", "DateAccepted", "ParticipantReportingDueDate", "ReimbursementClaimDueDate", "CompletionReportingDueDate" }, name: "IX_GrantAgreement");
            CreateIndex("dbo.NoteTypes", "Caption", unique: true);
            CreateIndex("dbo.NoteTypes", "IsActive", name: "IX_Active");
            CreateIndex("dbo.PrioritySectors", "Caption", unique: true);
            CreateIndex("dbo.PrioritySectors", "IsActive", name: "IX_Active");
            CreateIndex("dbo.ClaimStates", "Caption", unique: true);
            CreateIndex("dbo.ClaimStates", "IsActive", name: "IX_Active");
            CreateIndex("dbo.GrantApplicationExternalStates", "Caption", unique: true);
            CreateIndex("dbo.GrantApplicationExternalStates", "IsActive", name: "IX_Active");
            CreateIndex("dbo.GrantApplicationInternalStates", "Caption", unique: true);
            CreateIndex("dbo.GrantApplicationInternalStates", "IsActive", name: "IX_Active");
            CreateIndex("dbo.Logs", new[] { "Level", "DateAdded" }, name: "IX_Logs");
            AddForeignKey("dbo.Claims", "ClaimTypeId", "dbo.ClaimTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TrainingPrograms", "EligibleCostBreakdownId", "dbo.EligibleCostBreakdowns", "Id");
            AddForeignKey("dbo.EligibleExpenseTypes", "ServiceCategoryId", "dbo.ServiceCategories", "Id");
            AddForeignKey("dbo.TrainingPrograms", "ServiceLineId", "dbo.ServiceLines", "Id");
            AddForeignKey("dbo.TrainingPrograms", "ServiceLineBreakdownId", "dbo.ServiceLineBreakdowns", "Id");
            AddForeignKey("dbo.TrainingProviders", "EligibleCostId", "dbo.EligibleCosts", "Id");
            AddForeignKey("dbo.EligibleCosts", "GrantApplicationId", "dbo.TrainingCosts", "GrantApplicationId", cascadeDelete: true);
            AddForeignKey("dbo.ParticipantForms", "GrantApplicationId", "dbo.GrantApplications", "Id");
            AddForeignKey("dbo.ParticipantForms", "ParticipantId", "dbo.Participants", "Id");
            AddForeignKey("dbo.ParticipantCompletionReportAnswers", "GrantApplicationId", "dbo.GrantApplications", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ParticipantCompletionReportAnswers", "ParticipantFormId", "dbo.ParticipantForms", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ParticipantCosts", "ParticipantFormId", "dbo.ParticipantForms", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TrainingPrograms", "EligibleExpenseBreakdown_Id", "dbo.EligibleExpenseBreakdowns", "Id");
            AddForeignKey("dbo.EligibleExpenseTypes", "ExpenseTypeId", "dbo.ExpenseTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ReportRates", "GrantStreamId", "dbo.GrantStreams", "Id");
            AddForeignKey("dbo.Claims", "GrantApplicationId", "dbo.GrantApplications", "Id");
            AddForeignKey("dbo.PaymentRequests", "GrantApplicationId", "dbo.GrantApplications", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EmployerCompletionReportAnswers", "GrantApplicationId", "dbo.GrantApplications", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GrantApplications", "RiskClassificationId", "dbo.RiskClassifications", "Id");
            AddForeignKey("dbo.PaymentRequests", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims", new[] { "Id", "ClaimVersion" }, cascadeDelete: true);
            AddForeignKey("dbo.TrainingPrograms", "SkillFocusId", "dbo.SkillsFocus", "Id");
            DropColumn("dbo.BusinessContactRoles", "EmployerEnrollmentId");
            DropColumn("dbo.Organizations", "EmployerTypeCode");
            DropColumn("dbo.ParticipantCosts", "ParticipantEnrollmentId");
            DropColumn("dbo.Claims", "TrainingProgramId");
            DropColumn("dbo.PaymentRequests", "TrainingProgramId");
            DropColumn("dbo.PaymentRequestBatches", "Status");
            DropColumn("dbo.TrainingPrograms", "TrainingCostState");
            DropColumn("dbo.TrainingPrograms", "EstimatedParticipants");
            DropColumn("dbo.TrainingPrograms", "TotalEstimatedCost");
            DropColumn("dbo.TrainingPrograms", "TotalEstimatedReimbursement");
            DropColumn("dbo.TrainingPrograms", "AgreedParticipants");
            DropColumn("dbo.TrainingPrograms", "TotalAgreedMaxCost");
            DropColumn("dbo.TrainingPrograms", "AgreedCommitment");
            DropColumn("dbo.TrainingPrograms", "HoldPaymentRequests");
            DropColumn("dbo.EligibleCosts", "TrainingProgramId");
            DropColumn("dbo.TrainingProviders", "TrainingProgramId");
            DropColumn("dbo.ParticipantForms", "TrainingProgramId");
            DropColumn("dbo.ParticipantForms", "ConsentNameEntered");
            DropColumn("dbo.GrantStreams", "Criteria");
            DropColumn("dbo.EmployerCompletionReportAnswers", "EmployerEnrollmentId");
            DropColumn("dbo.ParticipantCompletionReportAnswers", "ParticipantEnrollmentId");
            DropTable("dbo.EmployerEnrollments");
            DropTable("dbo.ParticipantEnrollments");

            PostDeployment();

            AddForeignKey("dbo.GrantPrograms", "ProgramConfigurationId", "dbo.ProgramConfigurations", "Id");
            AddForeignKey("dbo.GrantPrograms", "ProgramTypeId", "dbo.ProgramTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GrantStreams", "ProgramConfigurationId", "dbo.ProgramConfigurations", "Id");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.ParticipantEnrollments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    EmployerEnrollmentId = c.Int(nullable: false),
                    ParticipantFormId = c.Int(),
                    ParticipantId = c.Int(),
                    InvitationKey = c.Guid(nullable: false),
                    InvitationExpiresOn = c.DateTime(),
                    ReportedDate = c.DateTime(),
                    ClaimReported = c.Boolean(nullable: false),
                    DateAdded = c.DateTime(nullable: false),
                    DateUpdated = c.DateTime(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.EmployerEnrollments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    InvitationKey = c.Guid(nullable: false),
                    InvitationExpiresOn = c.DateTime(),
                    TrainingProgramId = c.Int(nullable: false),
                    OrganizationId = c.Int(nullable: false),
                    DateAdded = c.DateTime(nullable: false),
                    DateUpdated = c.DateTime(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.ParticipantCompletionReportAnswers", "ParticipantEnrollmentId", c => c.Int(nullable: false));
            AddColumn("dbo.EmployerCompletionReportAnswers", "EmployerEnrollmentId", c => c.Int(nullable: false));
            AddColumn("dbo.GrantStreams", "Criteria", c => c.String(maxLength: 2500));
            AddColumn("dbo.ParticipantForms", "ConsentNameEntered", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.ParticipantForms", "TrainingProgramId", c => c.Int(nullable: false));
            AddColumn("dbo.TrainingProviders", "TrainingProgramId", c => c.Int());
            AddColumn("dbo.EligibleCosts", "TrainingProgramId", c => c.Int(nullable: false));
            AddColumn("dbo.TrainingPrograms", "HoldPaymentRequests", c => c.Boolean(nullable: false));
            AddColumn("dbo.TrainingPrograms", "AgreedCommitment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TrainingPrograms", "TotalAgreedMaxCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TrainingPrograms", "AgreedParticipants", c => c.Int(nullable: false));
            AddColumn("dbo.TrainingPrograms", "TotalEstimatedReimbursement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TrainingPrograms", "TotalEstimatedCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TrainingPrograms", "EstimatedParticipants", c => c.Int(nullable: false));
            AddColumn("dbo.TrainingPrograms", "TrainingCostState", c => c.Int(nullable: false));
            AddColumn("dbo.PaymentRequestBatches", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.PaymentRequests", "TrainingProgramId", c => c.Int(nullable: false));
            AddColumn("dbo.Claims", "TrainingProgramId", c => c.Int(nullable: false));
            AddColumn("dbo.ParticipantCosts", "ParticipantEnrollmentId", c => c.Int(nullable: false));
            AddColumn("dbo.Organizations", "EmployerTypeCode", c => c.Int(nullable: false));
            AddColumn("dbo.BusinessContactRoles", "EmployerEnrollmentId", c => c.Int());
            DropForeignKey("dbo.TrainingPrograms", "SkillFocusId", "dbo.SkillsFocus");
            DropForeignKey("dbo.PaymentRequests", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims");
            DropForeignKey("dbo.GrantPrograms", "ProgramTypeId", "dbo.ProgramTypes");
            DropForeignKey("dbo.GrantPrograms", "ProgramConfigurationId", "dbo.ProgramConfigurations");
            DropForeignKey("dbo.GrantApplications", "RiskClassificationId", "dbo.RiskClassifications");
            DropForeignKey("dbo.ProgramDescriptionVulnerableGroups", "VulnerableGroupId", "dbo.VulnerableGroups");
            DropForeignKey("dbo.ProgramDescriptionVulnerableGroups", "GrantApplicationId", "dbo.ProgramDescriptions");
            DropForeignKey("dbo.ProgramDescriptionUnderRepresentedPopulations", "UnderRepresentedPopulationId", "dbo.UnderRepresentedPopulations");
            DropForeignKey("dbo.ProgramDescriptionUnderRepresentedPopulations", "GrantApplicationId", "dbo.ProgramDescriptions");
            DropForeignKey("dbo.ProgramDescriptions", "TargetNAICSId", "dbo.NaIndustryClassificationSystems");
            DropForeignKey("dbo.ProgramDescriptions", "ParticipantEmploymentStatusId", "dbo.ParticipantEmploymentStatus");
            DropForeignKey("dbo.ProgramDescriptions", "TargetNOCId", "dbo.NationalOccupationalClassifications");
            DropForeignKey("dbo.ProgramDescriptions", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.ProgramDescriptionCommunities", "CommunityId", "dbo.Communities");
            DropForeignKey("dbo.ProgramDescriptionCommunities", "GrantApplicationId", "dbo.ProgramDescriptions");
            DropForeignKey("dbo.ProgramDescriptions", "ApplicantOrganizationTypeId", "dbo.ApplicantOrganizationTypes");
            DropForeignKey("dbo.EmployerCompletionReportAnswers", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.PaymentRequests", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.Claims", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.GrantStreams", "ProgramConfigurationId", "dbo.ProgramConfigurations");
            DropForeignKey("dbo.ReportRates", "GrantStreamId", "dbo.GrantStreams");
            DropForeignKey("dbo.ProgramConfigurationEligibleExpenseTypes", "EligibleExpenseTypeId", "dbo.EligibleExpenseTypes");
            DropForeignKey("dbo.ProgramConfigurationEligibleExpenseTypes", "ProgramConfigurationId", "dbo.ProgramConfigurations");
            DropForeignKey("dbo.EligibleExpenseTypes", "ExpenseTypeId", "dbo.ExpenseTypes");
            DropForeignKey("dbo.TrainingPrograms", "EligibleExpenseBreakdown_Id", "dbo.EligibleExpenseBreakdowns");
            DropForeignKey("dbo.EligibleExpenseBreakdowns", "EligibleExpenseTypeId", "dbo.EligibleExpenseTypes");
            DropForeignKey("dbo.ClaimBreakdownCosts", "EligibleExpenseBreakdownId", "dbo.EligibleExpenseBreakdowns");
            DropForeignKey("dbo.ParticipantCosts", "ParticipantFormId", "dbo.ParticipantForms");
            DropForeignKey("dbo.ParticipantCompletionReportAnswers", "ParticipantFormId", "dbo.ParticipantForms");
            DropForeignKey("dbo.ParticipantCompletionReportAnswers", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.ParticipantForms", "ParticipantId", "dbo.Participants");
            DropForeignKey("dbo.ParticipantForms", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.TrainingCosts", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.EligibleCosts", "GrantApplicationId", "dbo.TrainingCosts");
            DropForeignKey("dbo.TrainingProgramTrainingProviders", "TrainingProviderId", "dbo.TrainingProviders");
            DropForeignKey("dbo.TrainingProgramTrainingProviders", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.TrainingProviders", "EligibleCostId", "dbo.EligibleCosts");
            DropForeignKey("dbo.TrainingPrograms", "ServiceLineBreakdownId", "dbo.ServiceLineBreakdowns");
            DropForeignKey("dbo.TrainingPrograms", "ServiceLineId", "dbo.ServiceLines");
            DropForeignKey("dbo.ServiceLineBreakdowns", "ServiceLineId", "dbo.ServiceLines");
            DropForeignKey("dbo.ServiceCategories", "ServiceTypeId", "dbo.ServiceTypes");
            DropForeignKey("dbo.ServiceLines", "ServiceCategoryId", "dbo.ServiceCategories");
            DropForeignKey("dbo.EligibleExpenseTypes", "ServiceCategoryId", "dbo.ServiceCategories");
            DropForeignKey("dbo.EligibleExpenseBreakdowns", "ServiceLineId", "dbo.ServiceLines");
            DropForeignKey("dbo.TrainingPrograms", "EligibleCostBreakdownId", "dbo.EligibleCostBreakdowns");
            DropForeignKey("dbo.EligibleCostBreakdowns", "EligibleExpenseBreakdownId", "dbo.EligibleExpenseBreakdowns");
            DropForeignKey("dbo.EligibleCostBreakdowns", "EligibleCostId", "dbo.EligibleCosts");
            DropForeignKey("dbo.ClaimBreakdownCosts", "ClaimEligibleCostId", "dbo.ClaimEligibleCosts");
            DropForeignKey("dbo.ProgramConfigurations", "ClaimTypeId", "dbo.ClaimTypes");
            DropForeignKey("dbo.Claims", "ClaimTypeId", "dbo.ClaimTypes");
            DropForeignKey("dbo.UserPreferences", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserGrantProgramPreferences", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserGrantProgramPreferences", "GrantProgramId", "dbo.GrantPrograms");
            DropForeignKey("dbo.GrantApplicationAttachments", "AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.GrantApplicationAttachments", "GrantApplicationId", "dbo.GrantApplications");
            DropIndex("dbo.ProgramDescriptionVulnerableGroups", new[] { "VulnerableGroupId" });
            DropIndex("dbo.ProgramDescriptionVulnerableGroups", new[] { "GrantApplicationId" });
            DropIndex("dbo.ProgramDescriptionUnderRepresentedPopulations", new[] { "UnderRepresentedPopulationId" });
            DropIndex("dbo.ProgramDescriptionUnderRepresentedPopulations", new[] { "GrantApplicationId" });
            DropIndex("dbo.ProgramDescriptionCommunities", new[] { "CommunityId" });
            DropIndex("dbo.ProgramDescriptionCommunities", new[] { "GrantApplicationId" });
            DropIndex("dbo.ProgramConfigurationEligibleExpenseTypes", new[] { "EligibleExpenseTypeId" });
            DropIndex("dbo.ProgramConfigurationEligibleExpenseTypes", new[] { "ProgramConfigurationId" });
            DropIndex("dbo.TrainingProgramTrainingProviders", new[] { "TrainingProviderId" });
            DropIndex("dbo.TrainingProgramTrainingProviders", new[] { "TrainingProgramId" });
            DropIndex("dbo.GrantApplicationAttachments", new[] { "AttachmentId" });
            DropIndex("dbo.GrantApplicationAttachments", new[] { "GrantApplicationId" });
            DropIndex("dbo.Logs", "IX_Logs");
            DropIndex("dbo.GrantApplicationInternalStates", "IX_Active");
            DropIndex("dbo.GrantApplicationInternalStates", new[] { "Caption" });
            DropIndex("dbo.GrantApplicationExternalStates", "IX_Active");
            DropIndex("dbo.GrantApplicationExternalStates", new[] { "Caption" });
            DropIndex("dbo.ClaimStates", "IX_Active");
            DropIndex("dbo.ClaimStates", new[] { "Caption" });
            DropIndex("dbo.ProgramTypes", "IX_Active");
            DropIndex("dbo.ProgramTypes", new[] { "Caption" });
            DropIndex("dbo.RiskClassifications", "IX_Active");
            DropIndex("dbo.RiskClassifications", new[] { "Caption" });
            DropIndex("dbo.VulnerableGroups", "IX_Active");
            DropIndex("dbo.VulnerableGroups", new[] { "Caption" });
            DropIndex("dbo.UnderRepresentedPopulations", "IX_Active");
            DropIndex("dbo.UnderRepresentedPopulations", new[] { "Caption" });
            DropIndex("dbo.ParticipantEmploymentStatus", "IX_Active");
            DropIndex("dbo.ParticipantEmploymentStatus", new[] { "Caption" });
            DropIndex("dbo.Communities", "IX_Active");
            DropIndex("dbo.Communities", new[] { "Caption" });
            DropIndex("dbo.ApplicantOrganizationTypes", "IX_Active");
            DropIndex("dbo.ApplicantOrganizationTypes", new[] { "Caption" });
            DropIndex("dbo.ProgramDescriptions", new[] { "ApplicantOrganizationTypeId" });
            DropIndex("dbo.ProgramDescriptions", new[] { "TargetNOCId" });
            DropIndex("dbo.ProgramDescriptions", new[] { "TargetNAICSId" });
            DropIndex("dbo.ProgramDescriptions", new[] { "ParticipantEmploymentStatusId" });
            DropIndex("dbo.ProgramDescriptions", new[] { "GrantApplicationId" });
            DropIndex("dbo.PrioritySectors", "IX_Active");
            DropIndex("dbo.PrioritySectors", new[] { "Caption" });
            DropIndex("dbo.NoteTypes", "IX_Active");
            DropIndex("dbo.NoteTypes", new[] { "Caption" });
            DropIndex("dbo.GrantAgreements", "IX_GrantAgreement");
            DropIndex("dbo.EmployerCompletionReportAnswers", new[] { "GrantApplicationId" });
            DropIndex("dbo.PaymentRequestBatches", new[] { "GrantProgramId" });
            DropIndex("dbo.PaymentRequestBatches", "IX_PaymentRequestBatch");
            DropIndex("dbo.PaymentRequests", new[] { "ClaimId", "ClaimVersion" });
            DropIndex("dbo.PaymentRequests", new[] { "GrantApplicationId" });
            DropIndex("dbo.ReportRates", new[] { "GrantStreamId" });
            DropIndex("dbo.FiscalYears", "IX_FiscalYear_Dates");
            DropIndex("dbo.TrainingPeriods", "IX_TrainingPeriod");
            DropIndex("dbo.GrantOpenings", "IX_GrantOpening");
            DropIndex("dbo.GrantStreams", new[] { "ProgramConfigurationId" });
            DropIndex("dbo.ExpenseTypes", "IX_Active");
            DropIndex("dbo.ExpenseTypes", new[] { "Caption" });
            DropIndex("dbo.TrainingResults", "IX_Active");
            DropIndex("dbo.TrainingResults", new[] { "Caption" });
            DropIndex("dbo.RecentPeriods", "IX_Active");
            DropIndex("dbo.RecentPeriods", new[] { "Caption" });
            DropIndex("dbo.ParticipantCompletionReportAnswers", new[] { "GrantApplicationId" });
            DropIndex("dbo.ParticipantCompletionReportAnswers", new[] { "ParticipantFormId" });
            DropIndex("dbo.EmploymentTypes", "IX_Active");
            DropIndex("dbo.EmploymentTypes", new[] { "Caption" });
            DropIndex("dbo.EmploymentStatus", "IX_Active");
            DropIndex("dbo.EmploymentStatus", new[] { "Caption" });
            DropIndex("dbo.EIBenefits", "IX_Active");
            DropIndex("dbo.EIBenefits", new[] { "Caption" });
            DropIndex("dbo.EducationLevels", "IX_Active");
            DropIndex("dbo.EducationLevels", new[] { "Caption" });
            DropIndex("dbo.CanadianStatus", "IX_Active");
            DropIndex("dbo.CanadianStatus", new[] { "Caption" });
            DropIndex("dbo.ParticipantForms", "IX_ParticipantForm");
            DropIndex("dbo.ParticipantCosts", new[] { "ParticipantFormId" });
            DropIndex("dbo.TrainingCosts", new[] { "GrantApplicationId" });
            DropIndex("dbo.UnderRepresentedGroups", "IX_Active");
            DropIndex("dbo.UnderRepresentedGroups", new[] { "Caption" });
            DropIndex("dbo.TrainingProviderTypes", "IX_Active");
            DropIndex("dbo.TrainingProviderTypes", new[] { "Caption" });
            DropIndex("dbo.TrainingProviders", new[] { "EligibleCostId" });
            DropIndex("dbo.TrainingProviders", "IX_TrainingProviders");
            DropIndex("dbo.TrainingLevels", "IX_Active");
            DropIndex("dbo.TrainingLevels", new[] { "Caption" });
            DropIndex("dbo.SkillLevels", "IX_Active");
            DropIndex("dbo.SkillLevels", new[] { "Caption" });
            DropIndex("dbo.SkillsFocus", "IX_Active");
            DropIndex("dbo.SkillsFocus", new[] { "Caption" });
            DropIndex("dbo.ServiceLineBreakdowns", "IX_Active");
            DropIndex("dbo.ServiceLineBreakdowns", new[] { "Caption" });
            DropIndex("dbo.ServiceLineBreakdowns", new[] { "ServiceLineId" });
            DropIndex("dbo.ServiceTypes", "IX_Active");
            DropIndex("dbo.ServiceTypes", new[] { "Caption" });
            DropIndex("dbo.ServiceCategories", "IX_Active");
            DropIndex("dbo.ServiceCategories", new[] { "Caption" });
            DropIndex("dbo.ServiceCategories", new[] { "ServiceTypeId" });
            DropIndex("dbo.ServiceLines", "IX_Active");
            DropIndex("dbo.ServiceLines", new[] { "Caption" });
            DropIndex("dbo.ServiceLines", new[] { "ServiceCategoryId" });
            DropIndex("dbo.InDemandOccupations", "IX_Active");
            DropIndex("dbo.InDemandOccupations", new[] { "Caption" });
            DropIndex("dbo.ExpectedQualifications", "IX_Active");
            DropIndex("dbo.ExpectedQualifications", new[] { "Caption" });
            DropIndex("dbo.DeliveryPartnerServices", "IX_Active");
            DropIndex("dbo.DeliveryPartnerServices", new[] { "Caption" });
            DropIndex("dbo.DeliveryPartners", "IX_Active");
            DropIndex("dbo.DeliveryPartners", new[] { "Caption" });
            DropIndex("dbo.DeliveryMethods", "IX_Active");
            DropIndex("dbo.DeliveryMethods", new[] { "Caption" });
            DropIndex("dbo.TrainingPrograms", new[] { "EligibleExpenseBreakdown_Id" });
            DropIndex("dbo.TrainingPrograms", new[] { "EligibleCostBreakdownId" });
            DropIndex("dbo.TrainingPrograms", new[] { "ServiceLineBreakdownId" });
            DropIndex("dbo.TrainingPrograms", new[] { "ServiceLineId" });
            DropIndex("dbo.TrainingPrograms", "IX_TrainingPrograms");
            DropIndex("dbo.EligibleCostBreakdowns", new[] { "EligibleExpenseBreakdownId" });
            DropIndex("dbo.EligibleCostBreakdowns", new[] { "EligibleCostId" });
            DropIndex("dbo.EligibleCosts", new[] { "GrantApplicationId" });
            DropIndex("dbo.ClaimBreakdownCosts", new[] { "ClaimEligibleCostId" });
            DropIndex("dbo.ClaimBreakdownCosts", new[] { "EligibleExpenseBreakdownId" });
            DropIndex("dbo.EligibleExpenseBreakdowns", "IX_Active");
            DropIndex("dbo.EligibleExpenseBreakdowns", new[] { "ServiceLineId" });
            DropIndex("dbo.EligibleExpenseBreakdowns", "IX_Caption");
            DropIndex("dbo.EligibleExpenseTypes", "IX_Active");
            DropIndex("dbo.EligibleExpenseTypes", new[] { "ExpenseTypeId" });
            DropIndex("dbo.EligibleExpenseTypes", "IX_EligibleExpenseType");
            DropIndex("dbo.EligibleExpenseTypes", new[] { "Caption" });
            DropIndex("dbo.ProgramConfigurations", "IX_ProgramConfiguration_IsActive");
            DropIndex("dbo.ProgramConfigurations", "IX_ProgramConfiguration_ClaimTypeId");
            DropIndex("dbo.ProgramConfigurations", "IX_ClaimType_Caption");
            DropIndex("dbo.ClaimTypes", "IX_Active");
            DropIndex("dbo.ClaimTypes", new[] { "Caption" });
            DropIndex("dbo.Claims", new[] { "ClaimTypeId" });
            DropIndex("dbo.Claims", new[] { "GrantApplicationId" });
            DropIndex("dbo.Claims", "IX_Claim");
            DropIndex("dbo.UserPreferences", new[] { "UserId" });
            DropIndex("dbo.UserGrantProgramPreferences", "IX_UserGrantProgramPreference");
            DropIndex("dbo.OrganizationTypes", "IX_Active");
            DropIndex("dbo.OrganizationTypes", new[] { "Caption" });
            DropIndex("dbo.LegalStructures", "IX_Active");
            DropIndex("dbo.LegalStructures", new[] { "Caption" });
            DropIndex("dbo.BusinessContactRoles", "IX_BusinessContactRole");
            DropIndex("dbo.ApplicationTypes", "IX_Active");
            DropIndex("dbo.ApplicationTypes", new[] { "Caption" });
            DropIndex("dbo.GrantApplications", new[] { "NAICSId" });
            DropIndex("dbo.GrantApplications", new[] { "PrioritySectorId" });
            DropIndex("dbo.GrantApplications", new[] { "RiskClassificationId" });
            DropIndex("dbo.GrantApplications", "IX_GrantApplication");
            DropIndex("dbo.GrantPrograms", new[] { "ProgramTypeId" });
            DropIndex("dbo.GrantPrograms", new[] { "ProgramConfigurationId" });
            DropIndex("dbo.AboriginalBands", "IX_Active");
            DropIndex("dbo.AboriginalBands", new[] { "Caption" });
            DropPrimaryKey("dbo.ParticipantCompletionReportAnswers");
            DropPrimaryKey("dbo.EmployerCompletionReportAnswers");
            DropPrimaryKey("dbo.PaymentRequests");
            AlterColumn("dbo.TempData", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.TempData", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Settings", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Settings", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RateFormats", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.RateFormats", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ParticipantCompletionReportAnswers", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.ParticipantCompletionReportAnswers", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Logs", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Logs", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantApplicationInternalStates", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.GrantApplicationInternalStates", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantApplicationInternalStates", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.GrantApplicationExternalStates", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.GrantApplicationExternalStates", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantApplicationExternalStates", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.EmployerCompletionReportAnswers", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.EmployerCompletionReportAnswers", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CompletionReportGroups", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.CompletionReportGroups", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CompletionReportGroups", "Title", c => c.String());
            AlterColumn("dbo.CompletionReports", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.CompletionReports", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CompletionReports", "EffectiveDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CompletionReportQuestions", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.CompletionReportQuestions", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CompletionReportQuestions", "AnswerTableHeadings", c => c.String());
            AlterColumn("dbo.CompletionReportOptions", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.CompletionReportOptions", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ClaimStates", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.ClaimStates", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ClaimStates", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.ClaimIds", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.ClaimIds", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ApplicationClaims", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.ApplicationClaims", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.InternalUserFilterAttributes", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.InternalUserFilterAttributes", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.InternalUserFilters", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.InternalUserFilters", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantApplicationStateChanges", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.GrantApplicationStateChanges", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PrioritySectors", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.PrioritySectors", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PrioritySectors", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.NoteTypes", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.NoteTypes", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.NoteTypes", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.Notes", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Notes", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ReportRates", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.ReportRates", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.FiscalYears", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.FiscalYears", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.FiscalYears", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.FiscalYears", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingPeriods", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.TrainingPeriods", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingPeriods", "DefaultOpeningDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingPeriods", "DefaultPublishDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingPeriods", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingPeriods", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantStreams", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.GrantStreams", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantStreams", "DateFirstUsed", c => c.DateTime());
            AlterColumn("dbo.GrantOpeningIntakes", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.GrantOpeningIntakes", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantOpeningFinancials", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.GrantOpeningFinancials", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantOpenings", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.GrantOpenings", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantOpenings", "ClosingDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantOpenings", "OpeningDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantOpenings", "PublishDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.VersionedDocuments", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.VersionedDocuments", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Documents", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Documents", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantAgreements", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.GrantAgreements", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantAgreements", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantAgreements", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantAgreements", "DateAccepted", c => c.DateTime());
            AlterColumn("dbo.GrantAgreements", "CompletionReportingDueDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantAgreements", "ReimbursementClaimDueDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantAgreements", "ParticipantReportingDueDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingResults", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.TrainingResults", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingResults", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.RecentPeriods", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.RecentPeriods", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RecentPeriods", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.NationalOccupationalClassifications", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.NationalOccupationalClassifications", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EmploymentTypes", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.EmploymentTypes", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EmploymentTypes", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.EmploymentStatus", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.EmploymentStatus", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EmploymentStatus", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.EIBenefits", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.EIBenefits", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EIBenefits", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.EducationLevels", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.EducationLevels", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EducationLevels", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.CanadianStatus", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.CanadianStatus", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CanadianStatus", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.ParticipantForms", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.ParticipantForms", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ParticipantForms", "ConsentDateEntered", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ParticipantForms", "BirthDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ParticipantForms", "ProgramStartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Payments", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Payments", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Payments", "DatePaid", c => c.DateTime());
            AlterColumn("dbo.UnderRepresentedGroups", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.UnderRepresentedGroups", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.UnderRepresentedGroups", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.TrainingProviderTypes", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.TrainingProviderTypes", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingProviderTypes", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.TrainingProviderInventory", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.TrainingProviderInventory", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.VersionedAttachments", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.VersionedAttachments", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Attachments", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Attachments", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingProviders", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.TrainingProviders", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingProviders", "GrantApplicationId", c => c.Int(nullable: false));
            AlterColumn("dbo.TrainingLevels", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.TrainingLevels", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingLevels", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.SkillLevels", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.SkillLevels", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SkillLevels", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.SkillsFocus", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.SkillsFocus", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SkillsFocus", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.InDemandOccupations", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.InDemandOccupations", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.InDemandOccupations", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.ExpectedQualifications", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.ExpectedQualifications", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ExpectedQualifications", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.EligibleExpenseTypes", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.EligibleExpenseTypes", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EligibleExpenseTypes", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.EligibleExpenseTypes", "Description", c => c.String());
            AlterColumn("dbo.EligibleCosts", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.EligibleCosts", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DeliveryPartnerServices", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.DeliveryPartnerServices", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DeliveryPartnerServices", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.DeliveryPartners", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.DeliveryPartners", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DeliveryPartners", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.DeliveryMethods", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.DeliveryMethods", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DeliveryMethods", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.TrainingPrograms", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.TrainingPrograms", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingPrograms", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingPrograms", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrainingPrograms", "SkillFocusId", c => c.Int(nullable: false));
            AlterColumn("dbo.PaymentRequestBatches", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.PaymentRequestBatches", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PaymentRequestBatches", "GrantProgramId", c => c.Int());
            AlterColumn("dbo.PaymentRequestBatches", "IssuedDate", c => c.DateTime());
            AlterColumn("dbo.PaymentRequests", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.PaymentRequests", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PaymentRequests", "ClaimVersion", c => c.Int());
            AlterColumn("dbo.PaymentRequests", "ClaimId", c => c.Int());
            AlterColumn("dbo.Claims", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Claims", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Claims", "DateAssessed", c => c.DateTime());
            AlterColumn("dbo.Claims", "DateSubmitted", c => c.DateTime());
            AlterColumn("dbo.ClaimEligibleCosts", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.ClaimEligibleCosts", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ParticipantCosts", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.ParticipantCosts", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Participants", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Participants", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Participants", "BirthDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.NotificationTemplates", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.NotificationTemplates", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantProgramNotifications", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.GrantProgramNotifications", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.NotificationTypes", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.NotificationTypes", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.NotificationScheduleQueue", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.NotificationScheduleQueue", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Notifications", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Notifications", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Notifications", "AlertClearedDate", c => c.DateTime());
            AlterColumn("dbo.Notifications", "EmailSentDate", c => c.DateTime());
            AlterColumn("dbo.Notifications", "ExpiryDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Users", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.OrganizationTypes", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.OrganizationTypes", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.OrganizationTypes", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.NaIndustryClassificationSystems", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.NaIndustryClassificationSystems", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.LegalStructures", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.LegalStructures", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.LegalStructures", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.Addresses", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Addresses", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Organizations", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Organizations", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.BusinessContactRoles", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.BusinessContactRoles", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ApplicationTypes", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.ApplicationTypes", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ApplicationTypes", "Caption", c => c.String(maxLength: 250));
            AlterColumn("dbo.Regions", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Regions", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Countries", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.Countries", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ApplicationAddresses", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.ApplicationAddresses", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantApplications", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.GrantApplications", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantApplications", "DateCancelled", c => c.DateTime());
            AlterColumn("dbo.GrantApplications", "DateSubmitted", c => c.DateTime());
            AlterColumn("dbo.InternalUsers", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.InternalUsers", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DocumentTemplates", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.DocumentTemplates", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.GrantPrograms", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.GrantPrograms", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AccountCodes", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.AccountCodes", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AboriginalBands", "DateUpdated", c => c.DateTime());
            AlterColumn("dbo.AboriginalBands", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AboriginalBands", "Caption", c => c.String(maxLength: 250));
            DropColumn("dbo.ParticipantCompletionReportAnswers", "GrantApplicationId");
            DropColumn("dbo.ParticipantCompletionReportAnswers", "ParticipantFormId");
            DropColumn("dbo.EmployerCompletionReportAnswers", "GrantApplicationId");
            DropColumn("dbo.GrantStreams", "ProgramConfigurationId");
            DropColumn("dbo.GrantStreams", "CanReportParticipants");
            DropColumn("dbo.GrantStreams", "EligibilityRequired");
            DropColumn("dbo.GrantStreams", "EligibilityQuestion");
            DropColumn("dbo.GrantStreams", "EligibilityEnabled");
            DropColumn("dbo.GrantStreams", "EligibilityRequirements");
            DropColumn("dbo.GrantStreams", "AttachmentsRequired");
            DropColumn("dbo.GrantStreams", "AttachmentsMaximum");
            DropColumn("dbo.GrantStreams", "AttachmentsUserGuidance");
            DropColumn("dbo.GrantStreams", "AttachmentsHeader");
            DropColumn("dbo.GrantStreams", "AttachmentsIsEnabled");
            DropColumn("dbo.ParticipantForms", "ClaimReported");
            DropColumn("dbo.ParticipantForms", "ParticipantId");
            DropColumn("dbo.ParticipantForms", "GrantApplicationId");
            DropColumn("dbo.TrainingProviders", "EligibleCostId");
            DropColumn("dbo.EligibleExpenseTypes", "MaxProviders");
            DropColumn("dbo.EligibleExpenseTypes", "MinProviders");
            DropColumn("dbo.EligibleExpenseTypes", "ServiceCategoryId");
            DropColumn("dbo.EligibleExpenseTypes", "AllowMultiple");
            DropColumn("dbo.EligibleExpenseTypes", "AutoInclude");
            DropColumn("dbo.EligibleExpenseTypes", "Rate");
            DropColumn("dbo.EligibleExpenseTypes", "ExpenseTypeId");
            DropColumn("dbo.EligibleCosts", "GrantApplicationId");
            DropColumn("dbo.TrainingPrograms", "EligibleExpenseBreakdown_Id");
            DropColumn("dbo.TrainingPrograms", "EligibleCostBreakdownId");
            DropColumn("dbo.TrainingPrograms", "ServiceLineBreakdownId");
            DropColumn("dbo.TrainingPrograms", "ServiceLineId");
            DropColumn("dbo.PaymentRequestBatches", "BatchRequestDescription");
            DropColumn("dbo.PaymentRequestBatches", "ExpenseAuthorityName");
            DropColumn("dbo.PaymentRequestBatches", "IssuedByName");
            DropColumn("dbo.PaymentRequests", "GrantApplicationId");
            DropColumn("dbo.Claims", "ClaimTypeId");
            DropColumn("dbo.Claims", "GrantApplicationId");
            DropColumn("dbo.ClaimEligibleCosts", "AssessedReimbursementCost");
            DropColumn("dbo.ClaimEligibleCosts", "ClaimReimbursementCost");
            DropColumn("dbo.ParticipantCosts", "ParticipantFormId");
            DropColumn("dbo.GrantProgramNotifications", "IsActive");
            DropColumn("dbo.GrantApplications", "HoldPaymentRequests");
            DropColumn("dbo.GrantApplications", "InvitationExpiresOn");
            DropColumn("dbo.GrantApplications", "InvitationKey");
            DropColumn("dbo.GrantApplications", "EndDate");
            DropColumn("dbo.GrantApplications", "StartDate");
            DropColumn("dbo.GrantApplications", "CanReportParticipants");
            DropColumn("dbo.GrantApplications", "EligibilityConfirmed");
            DropColumn("dbo.GrantApplications", "RiskClassificationId");
            DropColumn("dbo.GrantPrograms", "ProgramTypeId");
            DropColumn("dbo.GrantPrograms", "ProgramConfigurationId");
            DropColumn("dbo.GrantPrograms", "DateImplemented");
            DropColumn("dbo.GrantPrograms", "UseFIFOReservation");
            DropTable("dbo.ProgramDescriptionVulnerableGroups");
            DropTable("dbo.ProgramDescriptionUnderRepresentedPopulations");
            DropTable("dbo.ProgramDescriptionCommunities");
            DropTable("dbo.ProgramConfigurationEligibleExpenseTypes");
            DropTable("dbo.TrainingProgramTrainingProviders");
            DropTable("dbo.GrantApplicationAttachments");
            DropTable("dbo.ProgramTypes");
            DropTable("dbo.RiskClassifications");
            DropTable("dbo.VulnerableGroups");
            DropTable("dbo.UnderRepresentedPopulations");
            DropTable("dbo.ParticipantEmploymentStatus");
            DropTable("dbo.Communities");
            DropTable("dbo.ApplicantOrganizationTypes");
            DropTable("dbo.ProgramDescriptions");
            DropTable("dbo.ExpenseTypes");
            DropTable("dbo.TrainingCosts");
            DropTable("dbo.ServiceLineBreakdowns");
            DropTable("dbo.ServiceTypes");
            DropTable("dbo.ServiceCategories");
            DropTable("dbo.ServiceLines");
            DropTable("dbo.EligibleCostBreakdowns");
            DropTable("dbo.ClaimBreakdownCosts");
            DropTable("dbo.EligibleExpenseBreakdowns");
            DropTable("dbo.ProgramConfigurations");
            DropTable("dbo.ClaimTypes");
            DropTable("dbo.UserPreferences");
            DropTable("dbo.UserGrantProgramPreferences");
            AddPrimaryKey("dbo.ParticipantCompletionReportAnswers", new[] { "ParticipantEnrollmentId", "QuestionId" });
            AddPrimaryKey("dbo.EmployerCompletionReportAnswers", new[] { "EmployerEnrollmentId", "QuestionId" });
            AddPrimaryKey("dbo.PaymentRequests", new[] { "PaymentRequestBatchId", "TrainingProgramId" });
            CreateIndex("dbo.ParticipantCompletionReportAnswers", "ParticipantEnrollmentId");
            CreateIndex("dbo.Logs", new[] { "Level", "DateAdded" }, name: "IX_Logs");
            CreateIndex("dbo.GrantApplicationInternalStates", "Caption", unique: true);
            CreateIndex("dbo.GrantApplicationExternalStates", "Caption", unique: true);
            CreateIndex("dbo.EmployerCompletionReportAnswers", "EmployerEnrollmentId");
            CreateIndex("dbo.ClaimStates", "Caption", unique: true);
            CreateIndex("dbo.PrioritySectors", "Caption", unique: true);
            CreateIndex("dbo.NoteTypes", "Caption", unique: true);
            CreateIndex("dbo.FiscalYears", new[] { "StartDate", "EndDate" }, unique: true, name: "IX_FiscalYear_Dates");
            CreateIndex("dbo.TrainingPeriods", new[] { "FiscalYearId", "StartDate", "EndDate" }, unique: true, name: "IX_TrainingPeriod");
            CreateIndex("dbo.GrantOpenings", new[] { "State", "PublishDate", "OpeningDate", "ClosingDate", "TrainingPeriodId", "GrantStreamId" }, name: "IX_GrantOpening");
            CreateIndex("dbo.GrantAgreements", new[] { "StartDate", "EndDate", "DateAccepted", "ParticipantReportingDueDate", "ReimbursementClaimDueDate", "CompletionReportingDueDate" }, name: "IX_GrantAgreement");
            CreateIndex("dbo.TrainingResults", "Caption", unique: true);
            CreateIndex("dbo.RecentPeriods", "Caption", unique: true);
            CreateIndex("dbo.EmploymentTypes", "Caption", unique: true);
            CreateIndex("dbo.EmploymentStatus", "Caption", unique: true);
            CreateIndex("dbo.EIBenefits", "Caption", unique: true);
            CreateIndex("dbo.EducationLevels", "Caption", unique: true);
            CreateIndex("dbo.CanadianStatus", "Caption", unique: true);
            CreateIndex("dbo.ParticipantForms", "TrainingProgramId", name: "IX_ParticipantForm");
            CreateIndex("dbo.UnderRepresentedGroups", "Caption", unique: true);
            CreateIndex("dbo.TrainingProviderTypes", "Caption", unique: true);
            CreateIndex("dbo.TrainingProviders", new[] { "TrainingProviderState", "Name", "GrantApplicationId", "TrainingProgramId", "TrainingProviderTypeId", "TrainingOutsideBC", "DateAdded", "ContactLastName", "ContactFirstName" }, name: "IX_TrainingProviders");
            CreateIndex("dbo.TrainingLevels", "Caption", unique: true);
            CreateIndex("dbo.SkillLevels", "Caption", unique: true);
            CreateIndex("dbo.SkillsFocus", "Caption", unique: true);
            CreateIndex("dbo.InDemandOccupations", "Caption", unique: true);
            CreateIndex("dbo.ExpectedQualifications", "Caption", unique: true);
            CreateIndex("dbo.EligibleExpenseTypes", "Caption", unique: true);
            CreateIndex("dbo.EligibleCosts", "TrainingProgramId");
            CreateIndex("dbo.DeliveryPartnerServices", "Caption", unique: true);
            CreateIndex("dbo.DeliveryPartners", "Caption", unique: true);
            CreateIndex("dbo.DeliveryMethods", "Caption", unique: true);
            CreateIndex("dbo.TrainingPrograms", new[] { "TrainingProgramState", "TrainingCostState", "DateAdded", "GrantApplicationId", "StartDate", "EndDate", "InDemandOccupationId", "SkillLevelId", "SkillFocusId", "ExpectedQualificationId", "TrainingLevelId", "DeliveryPartnerId" }, name: "IX_TrainingPrograms");
            CreateIndex("dbo.PaymentRequestBatches", "GrantProgramId");
            CreateIndex("dbo.PaymentRequests", new[] { "ClaimId", "ClaimVersion" });
            CreateIndex("dbo.PaymentRequests", "TrainingProgramId");
            CreateIndex("dbo.Claims", "TrainingProgramId");
            CreateIndex("dbo.Claims", new[] { "AssessorId", "ClaimState", "ClaimNumber", "DateSubmitted", "DateAssessed" }, name: "IX_Claim");
            CreateIndex("dbo.ParticipantCosts", "ParticipantEnrollmentId");
            CreateIndex("dbo.ParticipantEnrollments", new[] { "EmployerEnrollmentId", "ParticipantFormId", "ParticipantId" }, name: "IX_ParticipantEnrollment");
            CreateIndex("dbo.OrganizationTypes", "Caption", unique: true);
            CreateIndex("dbo.LegalStructures", "Caption", unique: true);
            CreateIndex("dbo.EmployerEnrollments", "OrganizationId");
            CreateIndex("dbo.EmployerEnrollments", "TrainingProgramId");
            CreateIndex("dbo.BusinessContactRoles", new[] { "UserId", "GrantApplicationId", "EmployerEnrollmentId" }, unique: true, name: "IX_BusinessContactRole");
            CreateIndex("dbo.ApplicationTypes", "Caption", unique: true);
            CreateIndex("dbo.GrantApplications", "NaicsId");
            CreateIndex("dbo.GrantApplications", new[] { "ApplicationStateInternal", "ApplicationStateExternal", "GrantOpeningId", "AssessorId", "OrganizationLegalName", "ApplicationTypeId", "PrioritySectorId" }, name: "IX_GrantApplication");
            CreateIndex("dbo.AboriginalBands", "Caption", unique: true);
            AddForeignKey("dbo.TrainingPrograms", "SkillFocusId", "dbo.SkillsFocus", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PaymentRequests", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims", new[] { "Id", "ClaimVersion" });
            AddForeignKey("dbo.ParticipantCompletionReportAnswers", "ParticipantEnrollmentId", "dbo.ParticipantEnrollments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EmployerCompletionReportAnswers", "EmployerEnrollmentId", "dbo.EmployerEnrollments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EmployerEnrollments", "TrainingProgramId", "dbo.TrainingPrograms", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ParticipantForms", "TrainingProgramId", "dbo.TrainingPrograms", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ParticipantEnrollments", "ParticipantFormId", "dbo.ParticipantForms", "Id");
            AddForeignKey("dbo.ParticipantCosts", "ParticipantEnrollmentId", "dbo.ParticipantEnrollments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Claims", "TrainingProgramId", "dbo.TrainingPrograms", "Id");
            AddForeignKey("dbo.TrainingProviders", "TrainingProgramId", "dbo.TrainingPrograms", "Id");
            AddForeignKey("dbo.PaymentRequests", "TrainingProgramId", "dbo.TrainingPrograms", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EligibleCosts", "TrainingProgramId", "dbo.TrainingPrograms", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ParticipantEnrollments", "ParticipantId", "dbo.Participants", "Id");
            AddForeignKey("dbo.ParticipantEnrollments", "EmployerEnrollmentId", "dbo.EmployerEnrollments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EmployerEnrollments", "OrganizationId", "dbo.Organizations", "Id");
            AddForeignKey("dbo.BusinessContactRoles", "EmployerEnrollmentId", "dbo.EmployerEnrollments", "Id");
            RenameTable(name: "dbo.AspNetUserLogins", newName: "ApplicationUserLogins");
            RenameTable(name: "dbo.AspNetUserClaims", newName: "ApplicationUserClaims");
            RenameTable(name: "dbo.AspNetUsers", newName: "ApplicationUsers");
            RenameTable(name: "dbo.AspNetUserRoles", newName: "ApplicationUserRoles");
            RenameTable(name: "dbo.AspNetRoles", newName: "ApplicationRoles");
        }
    }
}
