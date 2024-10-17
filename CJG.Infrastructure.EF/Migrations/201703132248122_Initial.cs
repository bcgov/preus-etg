namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("Initial")]
    public partial class Initial : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();
            CreateTable(
                "dbo.AboriginalBands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        RegionId = c.String(nullable: false, maxLength: 10),
                        CountryId = c.String(nullable: false, maxLength: 20),
                        Id = c.Int(nullable: false, identity: true),
                        AddressLine1 = c.String(nullable: false, maxLength: 250),
                        AddressLine2 = c.String(maxLength: 250),
                        City = c.String(nullable: false, maxLength: 250),
                        PostalCode = c.String(nullable: false, maxLength: 20),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.Regions", t => new { t.RegionId, t.CountryId })
                .Index(t => new { t.RegionId, t.CountryId })
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 20),
                        Name = c.String(nullable: false, maxLength: 150),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_Country");
            
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 10),
                        CountryId = c.String(nullable: false, maxLength: 20),
                        Name = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.Id, t.CountryId })
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .Index(t => new { t.Name, t.CountryId }, unique: true, name: "IX_Region");
            
            CreateTable(
                "dbo.ApplicantDeclarations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Body = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.IsActive, name: "IX_ApplicantDeclaration");
            
            CreateTable(
                "dbo.GrantApplications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GrantOpeningId = c.Int(nullable: false),
                        ApplicationTypeId = c.Int(nullable: false),
                        AssessorId = c.Int(),
                        FileNumber = c.String(maxLength: 50),
                        ApplicationStateExternal = c.Int(nullable: false),
                        ApplicationStateExternalChanged = c.DateTime(storeType: "datetime2"),
                        ApplicationStateInternal = c.Int(nullable: false),
                        ApplicationStateInternalChanged = c.DateTime(storeType: "datetime2"),
                        HasAppliedForGrantBefore = c.Boolean(nullable: false),
                        WouldTrainEmployeesWithoutGrant = c.Boolean(nullable: false),
                        HostingTrainingProgram = c.Boolean(nullable: false),
                        ApplicantDeclarationId = c.Int(),
                        ApplicantBCeID = c.Guid(nullable: false),
                        ApplicantSalutation = c.String(maxLength: 250),
                        ApplicantFirstName = c.String(nullable: false, maxLength: 250),
                        ApplicantLastName = c.String(nullable: false, maxLength: 250),
                        ApplicantEmail = c.String(nullable: false, maxLength: 500),
                        ApplicantPhoneNumber = c.String(nullable: false, maxLength: 20),
                        ApplicantPhoneExtension = c.String(maxLength: 20),
                        ApplicantJobTitle = c.String(maxLength: 500),
                        ApplicantPhysicalAddressId = c.Int(nullable: false),
                        ApplicantMailingAddressId = c.Int(),
                        OrganizationId = c.Int(nullable: false),
                        OrganizationBCeID = c.Guid(),
                        OrganizationAddressId = c.Int(),
                        OrganizationTypeId = c.Int(),
                        OrganizationLegalStructureId = c.Int(),
                        OrganizationYearEstablished = c.Int(),
                        OrganizationNumberOfEmployeesWorldwide = c.Int(),
                        OrganizationAnnualTrainingBudget = c.Single(),
                        OrganizationAnnualEmployeesTrained = c.Int(),
                        OrganizationLegalName = c.String(maxLength: 250),
                        PrioritySectorId = c.Int(),
                        OrganizationDoingBusinessAs = c.String(maxLength: 500),
                        NaicsId = c.Int(),
                        DateSubmitted = c.DateTime(storeType: "datetime2"),
                        WithdrawReason = c.String(),
                        DenyReason = c.String(maxLength: 800),
                        AssessorNote = c.String(maxLength: 500),
                        MaxReimbursementAmt = c.Single(nullable: false),
                        ReimbursementRate = c.Single(nullable: false),
                        CancellationReason = c.String(maxLength: 800),
                        DateCancelled = c.DateTime(storeType: "datetime2"),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicantDeclarations", t => t.ApplicantDeclarationId)
                .ForeignKey("dbo.ApplicationAddresses", t => t.ApplicantMailingAddressId)
                .ForeignKey("dbo.ApplicationAddresses", t => t.ApplicantPhysicalAddressId)
                .ForeignKey("dbo.ApplicationTypes", t => t.ApplicationTypeId, cascadeDelete: true)
                .ForeignKey("dbo.InternalUsers", t => t.AssessorId)
                .ForeignKey("dbo.GrantOpenings", t => t.GrantOpeningId, cascadeDelete: true)
                .ForeignKey("dbo.NaIndustryClassificationSystems", t => t.NaicsId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.ApplicationAddresses", t => t.OrganizationAddressId)
                .ForeignKey("dbo.LegalStructures", t => t.OrganizationLegalStructureId)
                .ForeignKey("dbo.OrganizationTypes", t => t.OrganizationTypeId)
                .ForeignKey("dbo.PrioritySectors", t => t.PrioritySectorId)
                .Index(t => new { t.ApplicationStateInternal, t.ApplicationStateExternal, t.GrantOpeningId, t.AssessorId, t.OrganizationLegalName, t.ApplicationTypeId, t.ApplicationStateInternalChanged, t.ApplicationStateExternalChanged, t.PrioritySectorId }, name: "IX_GrantApplication")
                .Index(t => t.ApplicantDeclarationId)
                .Index(t => t.ApplicantPhysicalAddressId)
                .Index(t => t.ApplicantMailingAddressId)
                .Index(t => t.OrganizationId)
                .Index(t => t.OrganizationAddressId)
                .Index(t => t.OrganizationTypeId)
                .Index(t => t.OrganizationLegalStructureId)
                .Index(t => t.NaicsId);
            
            CreateTable(
                "dbo.ApplicationAddresses",
                c => new
                    {
                        RegionId = c.String(nullable: false, maxLength: 10),
                        CountryId = c.String(nullable: false, maxLength: 20),
                        Id = c.Int(nullable: false, identity: true),
                        AddressLine1 = c.String(nullable: false, maxLength: 250),
                        AddressLine2 = c.String(maxLength: 250),
                        City = c.String(nullable: false, maxLength: 250),
                        PostalCode = c.String(nullable: false, maxLength: 10),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.Regions", t => new { t.RegionId, t.CountryId })
                .Index(t => new { t.RegionId, t.CountryId })
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.ApplicationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.InternalUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 250),
                        LastName = c.String(nullable: false, maxLength: 250),
                        IDIR = c.String(nullable: false, maxLength: 100),
                        Salutation = c.String(maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 500),
                        PhoneNumber = c.String(maxLength: 20),
                        PhoneNumberExt = c.String(maxLength: 20),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.IDIR, unique: true, name: "IX_InternalUser");
            
            CreateTable(
                "dbo.Claims",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ClaimVersion = c.Int(nullable: false),
                        AssessorId = c.Int(),
                        TrainingProgramId = c.Int(nullable: false),
                        FileNumber = c.String(nullable: false, maxLength: 50),
                        ClaimState = c.Int(nullable: false),
                        TotalClaimReimbursement = c.Single(nullable: false),
                        TotalAssessedReimbursement = c.Single(nullable: false),
                        AssessorNotes = c.String(),
                        DateSubmitted = c.DateTime(storeType: "datetime2"),
                        DateAssessed = c.DateTime(storeType: "datetime2"),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.Id, t.ClaimVersion })
                .ForeignKey("dbo.InternalUsers", t => t.AssessorId)
                .ForeignKey("dbo.TrainingPrograms", t => t.TrainingProgramId)
                .Index(t => new { t.AssessorId, t.ClaimState, t.FileNumber, t.DateSubmitted, t.DateAssessed }, name: "IX_Claim")
                .Index(t => t.TrainingProgramId);
            
            CreateTable(
                "dbo.ClaimEligibleCosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimId = c.Int(nullable: false),
                        ClaimVersion = c.Int(nullable: false),
                        EligibleExpenseTypeId = c.Int(nullable: false),
                        EligibleCostId = c.Int(),
                        ClaimCost = c.Single(nullable: false),
                        ClaimParticipants = c.Int(nullable: false),
                        ClaimMaxParticipantCost = c.Single(nullable: false),
                        ClaimMaxReimbursementCost = c.Single(nullable: false),
                        ClaimEmployerContribution = c.Single(nullable: false),
                        AssessedCost = c.Single(nullable: false),
                        AssessedParticipants = c.Int(nullable: false),
                        AssessedMaxParticipantCost = c.Single(nullable: false),
                        AssessedMaxReimbursementCost = c.Single(nullable: false),
                        AssessedEmployerContribution = c.Single(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Claims", t => new { t.ClaimId, t.ClaimVersion }, cascadeDelete: true)
                .ForeignKey("dbo.EligibleCosts", t => t.EligibleCostId)
                .ForeignKey("dbo.EligibleExpenseTypes", t => t.EligibleExpenseTypeId, cascadeDelete: true)
                .Index(t => new { t.ClaimId, t.ClaimVersion })
                .Index(t => t.EligibleExpenseTypeId)
                .Index(t => t.EligibleCostId);
            
            CreateTable(
                "dbo.EligibleCosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrainingProgramId = c.Int(nullable: false),
                        EligibleExpenseTypeId = c.Int(nullable: false),
                        EstimatedCost = c.Single(nullable: false),
                        EstimatedParticipants = c.Int(nullable: false),
                        EstimatedParticipantCost = c.Single(nullable: false),
                        EstimatedReimbursement = c.Single(nullable: false),
                        EstimatedEmployerContribution = c.Single(nullable: false),
                        AgreedMaxCost = c.Single(nullable: false),
                        AgreedMaxParticipants = c.Int(nullable: false),
                        AgreedMaxParticipantCost = c.Single(nullable: false),
                        AgreedMaxReimbursement = c.Single(nullable: false),
                        AgreedEmployerContribution = c.Single(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EligibleExpenseTypes", t => t.EligibleExpenseTypeId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingPrograms", t => t.TrainingProgramId, cascadeDelete: true)
                .Index(t => t.TrainingProgramId)
                .Index(t => t.EligibleExpenseTypeId);
            
            CreateTable(
                "dbo.EligibleExpenseTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.TrainingPrograms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrainingProviderId = c.Int(nullable: false),
                        RequestedTrainingProviderId = c.Int(),
                        ReasonForTrainingProviderChange = c.String(),
                        GrantApplicationId = c.Int(nullable: false),
                        InDemandOccupationId = c.Int(),
                        SkillLevelId = c.Int(nullable: false),
                        SkillFocusId = c.Int(nullable: false),
                        ExpectedQualificationId = c.Int(nullable: false),
                        TrainingLevelId = c.Int(),
                        CourseTitle = c.String(nullable: false, maxLength: 500),
                        TrainingBusinessCase = c.String(),
                        StartDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        EndDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        TotalTrainingHours = c.Int(nullable: false),
                        TitleOfQualification = c.String(maxLength: 500),
                        HasOfferedThisTypeOfTrainingBefore = c.Boolean(nullable: false),
                        HasRequestedAdditionalFunding = c.Boolean(nullable: false),
                        DescriptionOfFundingRequested = c.String(),
                        MemberOfUnderRepresentedGroup = c.Boolean(),
                        TrainingProgramState = c.Int(nullable: false),
                        TrainingCostState = c.Int(nullable: false),
                        EstimatedParticipants = c.Int(nullable: false),
                        TotalEstimatedCost = c.Single(nullable: false),
                        AgreedParticipants = c.Int(nullable: false),
                        TotalAgreedMaxCost = c.Single(nullable: false),
                        DeliveryPartnerId = c.Int(),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DeliveryPartners", t => t.DeliveryPartnerId)
                .ForeignKey("dbo.ExpectedQualifications", t => t.ExpectedQualificationId, cascadeDelete: true)
                .ForeignKey("dbo.InDemandOccupations", t => t.InDemandOccupationId)
                .ForeignKey("dbo.TrainingProviders", t => t.TrainingProviderId)
                .ForeignKey("dbo.TrainingProviders", t => t.RequestedTrainingProviderId)
                .ForeignKey("dbo.SkillsFocus", t => t.SkillFocusId, cascadeDelete: true)
                .ForeignKey("dbo.SkillLevels", t => t.SkillLevelId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingLevels", t => t.TrainingLevelId)
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
                .Index(t => t.TrainingProviderId)
                .Index(t => t.RequestedTrainingProviderId)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.InDemandOccupationId)
                .Index(t => t.SkillLevelId)
                .Index(t => t.SkillFocusId)
                .Index(t => t.ExpectedQualificationId)
                .Index(t => t.TrainingLevelId)
                .Index(t => t.DeliveryPartnerId);
            
            CreateTable(
                "dbo.DeliveryMethods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.DeliveryPartners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.DeliveryPartnerServices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.EmployerEnrollments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvitationKey = c.Guid(nullable: false),
                        InvitationExpiresOn = c.DateTime(storeType: "datetime2"),
                        TrainingProgramId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.TrainingPrograms", t => t.TrainingProgramId, cascadeDelete: true)
                .Index(t => t.TrainingProgramId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.BusinessContactRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        GrantApplicationId = c.Int(nullable: false),
                        EmployerEnrollmentId = c.Int(),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmployerEnrollments", t => t.EmployerEnrollmentId)
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => new { t.UserId, t.GrantApplicationId, t.EmployerEnrollmentId }, unique: true, name: "IX_BusinessContactRole");
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BCeIDGuid = c.Guid(nullable: false),
                        AccountType = c.Int(nullable: false),
                        Salutation = c.String(maxLength: 250),
                        FirstName = c.String(nullable: false, maxLength: 250),
                        LastName = c.String(nullable: false, maxLength: 250),
                        EmailAddress = c.String(nullable: false, maxLength: 500),
                        JobTitle = c.String(maxLength: 500),
                        PhoneNumber = c.String(maxLength: 20),
                        PhoneExtension = c.String(maxLength: 20),
                        PhysicalAddressId = c.Int(),
                        MailingAddressId = c.Int(),
                        IsOrganizationProfileAdministrator = c.Boolean(nullable: false),
                        IsSubscriberToEmail = c.Boolean(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.MailingAddressId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.Addresses", t => t.PhysicalAddressId)
                .Index(t => t.BCeIDGuid, unique: true)
                .Index(t => t.PhysicalAddressId)
                .Index(t => t.MailingAddressId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NotificationTemplateId = c.Int(),
                        NotificationScheduleQueueId = c.Int(),
                        Viewed = c.Boolean(nullable: false),
                        ExpiryDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        SendEmail = c.Boolean(nullable: false),
                        EmailSentDate = c.DateTime(storeType: "datetime2"),
                        EmailSubject = c.String(nullable: false, maxLength: 500),
                        EmailBody = c.String(nullable: false),
                        EmailRecipients = c.String(nullable: false),
                        EmailSender = c.String(nullable: false),
                        AlertCaption = c.String(maxLength: 250),
                        AlertClearedDate = c.DateTime(storeType: "datetime2"),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NotificationScheduleQueue", t => t.NotificationScheduleQueueId)
                .ForeignKey("dbo.NotificationTemplates", t => t.NotificationTemplateId)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.NotificationTemplateId)
                .Index(t => t.NotificationScheduleQueueId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.NotificationScheduleQueue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GrantApplicationId = c.Int(nullable: false),
                        NotificationTypeId = c.Int(nullable: false),
                        SendStatus = c.Int(nullable: false),
                        NotificationTicketId = c.String(nullable: false, maxLength: 32),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.NotificationTypes", t => t.NotificationTypeId, cascadeDelete: true)
                .Index(t => t.GrantApplicationId)
                .Index(t => new { t.SendStatus, t.NotificationTypeId }, name: "IX_NotificationScheduleQueue");
            
            CreateTable(
                "dbo.NotificationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        NotificationTemplateId = c.Int(nullable: false),
                        MilestoneDateName = c.String(nullable: false, maxLength: 64),
                        MilestoneDateOffset = c.Int(nullable: false),
                        NotificationTypeName = c.String(nullable: false, maxLength: 128),
                        RecipientType = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NotificationTemplates", t => t.NotificationTemplateId, cascadeDelete: true)
                .Index(t => t.NotificationTemplateId);
            
            CreateTable(
                "dbo.NotificationTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AlertCaption = c.String(maxLength: 250),
                        EmailSubject = c.String(nullable: false, maxLength: 500),
                        EmailBody = c.String(nullable: false),
                        DefaultExpiryDays = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BCeIDGuid = c.Guid(nullable: false),
                        LegalName = c.String(nullable: false, maxLength: 250),
                        EmployerTypeCode = c.Int(nullable: false),
                        HeadOfficeAddressId = c.Int(),
                        OrganizationTypeId = c.Int(),
                        LegalStructureId = c.Int(),
                        OtherLegalStructure = c.String(maxLength: 250),
                        YearEstablished = c.Int(),
                        NumberOfEmployeesWorldwide = c.Int(),
                        NumberOfEmployeesInBC = c.Int(),
                        AnnualTrainingBudget = c.Single(),
                        AnnualEmployeesTrained = c.Int(),
                        DoingBusinessAs = c.String(maxLength: 500),
                        NaicsId = c.Int(),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.HeadOfficeAddressId)
                .ForeignKey("dbo.LegalStructures", t => t.LegalStructureId)
                .ForeignKey("dbo.NaIndustryClassificationSystems", t => t.NaicsId)
                .ForeignKey("dbo.OrganizationTypes", t => t.OrganizationTypeId)
                .Index(t => t.BCeIDGuid, unique: true, name: "IX_Organization_BCeID")
                .Index(t => new { t.LegalName, t.OrganizationTypeId, t.LegalStructureId }, name: "IX_Organization")
                .Index(t => t.HeadOfficeAddressId)
                .Index(t => t.NaicsId);
            
            CreateTable(
                "dbo.LegalStructures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.NaIndustryClassificationSystems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 10),
                        Description = c.String(nullable: false, maxLength: 250),
                        Level = c.Int(nullable: false),
                        ParentId = c.Int(),
                        Left = c.Int(nullable: false),
                        Right = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NaIndustryClassificationSystems", t => t.ParentId)
                .Index(t => new { t.Code, t.Left, t.Right, t.Level }, unique: true, name: "IX_NaIndustryClassificationSystem")
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.OrganizationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.ParticipantEnrollments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployerEnrollmentId = c.Int(nullable: false),
                        ParticipantFormId = c.Int(),
                        ParticipantId = c.Int(),
                        InvitationKey = c.Guid(nullable: false),
                        InvitationExpiresOn = c.DateTime(storeType: "datetime2"),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmployerEnrollments", t => t.EmployerEnrollmentId, cascadeDelete: true)
                .ForeignKey("dbo.Participants", t => t.ParticipantId)
                .ForeignKey("dbo.ParticipantForms", t => t.ParticipantFormId)
                .Index(t => new { t.EmployerEnrollmentId, t.ParticipantFormId, t.ParticipantId }, name: "IX_ParticipantEnrollment");
            
            CreateTable(
                "dbo.Participants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        BirthDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        Email = c.String(nullable: false, maxLength: 500),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ParticipantCosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimEligibleCostId = c.Int(nullable: false),
                        ParticipantEnrollmentId = c.Int(nullable: false),
                        ClaimParticipantCost = c.Single(nullable: false),
                        ClaimReimbursement = c.Single(nullable: false),
                        ClaimEmployerContribution = c.Single(nullable: false),
                        AssessedParticipantCost = c.Single(nullable: false),
                        AssessedReimbursement = c.Single(nullable: false),
                        AssessedEmployerContribution = c.Single(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClaimEligibleCosts", t => t.ClaimEligibleCostId, cascadeDelete: true)
                .ForeignKey("dbo.ParticipantEnrollments", t => t.ParticipantEnrollmentId, cascadeDelete: true)
                .Index(t => t.ClaimEligibleCostId)
                .Index(t => t.ParticipantEnrollmentId);
            
            CreateTable(
                "dbo.ParticipantForms",
                c => new
                    {
                        RegionId = c.String(nullable: false, maxLength: 10),
                        CountryId = c.String(nullable: false, maxLength: 20),
                        Id = c.Int(nullable: false, identity: true),
                        InvitationKey = c.Guid(nullable: false),
                        TrainingProgramId = c.Int(nullable: false),
                        CanadianStatusId = c.Int(nullable: false),
                        AboriginalBandId = c.Int(),
                        EducationLevelId = c.Int(),
                        EmploymentTypeId = c.Int(),
                        RecentPeriodId = c.Int(),
                        EmploymentStatusId = c.Int(nullable: false),
                        TrainingResultId = c.Int(),
                        ParticipantConsentId = c.Int(nullable: false),
                        ProgramSponsorName = c.String(nullable: false, maxLength: 500),
                        ProgramDescription = c.String(nullable: false),
                        ProgramStartDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        MiddleName = c.String(maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        BirthDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        SIN = c.String(nullable: false, maxLength: 11),
                        PhoneNumber1 = c.String(nullable: false, maxLength: 20),
                        PhoneExtension1 = c.String(maxLength: 20),
                        PhoneNumber2 = c.String(maxLength: 20),
                        PhoneExtension2 = c.String(maxLength: 20),
                        EmailAddress = c.String(nullable: false, maxLength: 500),
                        AddressLine1 = c.String(nullable: false, maxLength: 500),
                        AddressLine2 = c.String(maxLength: 500),
                        City = c.String(nullable: false, maxLength: 500),
                        PostalCode = c.String(nullable: false, maxLength: 10),
                        Gender = c.Int(nullable: false),
                        YouthInCare = c.Boolean(nullable: false),
                        PersonDisability = c.Int(nullable: false),
                        PersonAboriginal = c.Int(nullable: false),
                        LiveOnReserve = c.Boolean(nullable: false),
                        VisibleMinority = c.Int(nullable: false),
                        CanadaImmigrant = c.Boolean(nullable: false),
                        YearToCanada = c.Int(nullable: false),
                        CanadaRefugee = c.Boolean(nullable: false),
                        FromCountry = c.String(maxLength: 200),
                        EIBenefitId = c.Int(nullable: false),
                        MaternalPaternal = c.Boolean(nullable: false),
                        BceaClient = c.Boolean(nullable: false),
                        EmployedBySupportEmployer = c.Boolean(nullable: false),
                        BusinessOwner = c.Boolean(nullable: false),
                        Apprentice = c.Boolean(nullable: false),
                        ItaRegistered = c.Boolean(nullable: false),
                        OtherPrograms = c.Boolean(nullable: false),
                        HowLongYears = c.Int(),
                        HowLongMonths = c.Int(),
                        AvgHoursPerWeek = c.Int(),
                        HourlyWage = c.Single(),
                        PrimaryCity = c.String(maxLength: 250),
                        OtherProgramDesc = c.String(),
                        LastHighSchoolName = c.String(maxLength: 250),
                        LastHighSchoolCity = c.String(maxLength: 250),
                        NocId = c.Int(nullable: false),
                        ConsentNameEntered = c.String(nullable: false, maxLength: 500),
                        ConsentDateEntered = c.DateTime(nullable: false, storeType: "datetime2"),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AboriginalBands", t => t.AboriginalBandId)
                .ForeignKey("dbo.CanadianStatus", t => t.CanadianStatusId, cascadeDelete: true)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.EducationLevels", t => t.EducationLevelId)
                .ForeignKey("dbo.EIBenefits", t => t.EIBenefitId, cascadeDelete: true)
                .ForeignKey("dbo.EmploymentStatus", t => t.EmploymentStatusId, cascadeDelete: true)
                .ForeignKey("dbo.EmploymentTypes", t => t.EmploymentTypeId)
                .ForeignKey("dbo.NationalOccupationalClassifications", t => t.NocId)
                .ForeignKey("dbo.ParticipantConsents", t => t.ParticipantConsentId, cascadeDelete: true)
                .ForeignKey("dbo.RecentPeriods", t => t.RecentPeriodId)
                .ForeignKey("dbo.Regions", t => new { t.RegionId, t.CountryId })
                .ForeignKey("dbo.TrainingPrograms", t => t.TrainingProgramId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingResults", t => t.TrainingResultId)
                .Index(t => new { t.RegionId, t.CountryId })
                .Index(t => t.CountryId)
                .Index(t => t.TrainingProgramId, name: "IX_ParticipantForm")
                .Index(t => t.CanadianStatusId)
                .Index(t => t.AboriginalBandId)
                .Index(t => t.EducationLevelId)
                .Index(t => t.EmploymentTypeId)
                .Index(t => t.RecentPeriodId)
                .Index(t => t.EmploymentStatusId)
                .Index(t => t.TrainingResultId)
                .Index(t => t.ParticipantConsentId)
                .Index(t => t.EIBenefitId)
                .Index(t => t.NocId);
            
            CreateTable(
                "dbo.CanadianStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.EducationLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.EIBenefits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.EmploymentStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.EmploymentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.NationalOccupationalClassifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 10),
                        Description = c.String(nullable: false, maxLength: 250),
                        Level = c.Int(nullable: false),
                        ParentId = c.Int(),
                        Left = c.Int(nullable: false),
                        Right = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NationalOccupationalClassifications", t => t.ParentId)
                .Index(t => t.Code, unique: true, name: "IX_NaIndustryClassificationSystem")
                .Index(t => new { t.Left, t.Right, t.Level }, unique: true, name: "IX_NationalOccupationalClassification")
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.ParticipantConsents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.IsActive, name: "IX_ParticipantConsent");
            
            CreateTable(
                "dbo.RecentPeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.TrainingResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.ExpectedQualifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.InDemandOccupations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.TrainingProviders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 500),
                        GrantApplicationId = c.Int(nullable: false),
                        TrainingProviderTypeId = c.Int(nullable: false),
                        TrainingAddressId = c.Int(nullable: false),
                        TrainingProviderState = c.Int(nullable: false),
                        TrainingProviderInventoryId = c.Int(),
                        TrainingOutsideBC = c.Boolean(nullable: false),
                        BusinessCase = c.String(),
                        BusinessCaseDocumentId = c.Int(),
                        ProofOfQualificationsDocumentId = c.Int(),
                        CourseOutlineDocumentId = c.Int(),
                        ContactFirstName = c.String(nullable: false, maxLength: 128),
                        ContactLastName = c.String(nullable: false, maxLength: 128),
                        ContactEmail = c.String(nullable: false, maxLength: 500),
                        ContactPhoneNumber = c.String(nullable: false, maxLength: 20),
                        ContactPhoneExtension = c.String(maxLength: 20),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.BusinessCaseDocumentId)
                .ForeignKey("dbo.Attachments", t => t.CourseOutlineDocumentId)
                .ForeignKey("dbo.Attachments", t => t.ProofOfQualificationsDocumentId)
                .ForeignKey("dbo.ApplicationAddresses", t => t.TrainingAddressId)
                .ForeignKey("dbo.TrainingProviderInventory", t => t.TrainingProviderInventoryId)
                .ForeignKey("dbo.TrainingProviderTypes", t => t.TrainingProviderTypeId, cascadeDelete: true)
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.TrainingProviderTypeId)
                .Index(t => t.TrainingAddressId)
                .Index(t => t.TrainingProviderInventoryId)
                .Index(t => t.BusinessCaseDocumentId)
                .Index(t => t.ProofOfQualificationsDocumentId)
                .Index(t => t.CourseOutlineDocumentId);
            
            CreateTable(
                "dbo.Attachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VersionNumber = c.Int(nullable: false),
                        FileName = c.String(nullable: false, maxLength: 500),
                        Description = c.String(maxLength: 500),
                        FileExtension = c.String(nullable: false, maxLength: 50),
                        AttachmentData = c.Binary(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VersionedAttachments",
                c => new
                    {
                        AttachmentId = c.Int(nullable: false),
                        VersionNumber = c.Int(nullable: false),
                        FileName = c.String(nullable: false, maxLength: 500),
                        Description = c.String(maxLength: 500),
                        FileExtension = c.String(nullable: false, maxLength: 50),
                        AttachmentData = c.Binary(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.AttachmentId, t.VersionNumber })
                .ForeignKey("dbo.Attachments", t => t.AttachmentId, cascadeDelete: true)
                .Index(t => t.AttachmentId);
            
            CreateTable(
                "dbo.TrainingProviderInventory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        Notes = c.String(),
                        IsEligible = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.IsActive, t.IsEligible, t.Name }, name: "IX_TrainingProviderInventory");
            
            CreateTable(
                "dbo.TrainingProviderTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.SkillsFocus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.SkillLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.TrainingLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.UnderRepresentedGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        ClaimId = c.Int(nullable: false),
                        ClaimVersion = c.Int(nullable: false),
                        PaymentNumber = c.Int(nullable: false),
                        IsDuplicate = c.Boolean(nullable: false),
                        Amount = c.Single(nullable: false),
                        DatePaid = c.DateTime(storeType: "datetime2"),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.ClaimId, t.ClaimVersion, t.PaymentNumber })
                .ForeignKey("dbo.Claims", t => new { t.ClaimId, t.ClaimVersion }, cascadeDelete: true)
                .Index(t => new { t.ClaimId, t.ClaimVersion });
            
            CreateTable(
                "dbo.InternalUserFilters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InternalUserId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 1000),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InternalUsers", t => t.InternalUserId, cascadeDelete: true)
                .Index(t => t.InternalUserId);
            
            CreateTable(
                "dbo.InternalUserFilterAttributes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InternUserFilterId = c.Int(nullable: false),
                        Key = c.String(nullable: false, maxLength: 250),
                        Operator = c.Int(nullable: false),
                        Value = c.String(nullable: false, maxLength: 500),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InternalUserFilters", t => t.InternUserFilterId, cascadeDelete: true)
                .Index(t => t.InternUserFilterId);
            
            CreateTable(
                "dbo.Notes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NoteTypeId = c.Int(nullable: false),
                        GrantApplicationId = c.Int(nullable: false),
                        AttachmentId = c.Int(),
                        CreatorId = c.Int(),
                        Content = c.String(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.AttachmentId)
                .ForeignKey("dbo.InternalUsers", t => t.CreatorId)
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.NoteTypes", t => t.NoteTypeId, cascadeDelete: true)
                .Index(t => new { t.NoteTypeId, t.GrantApplicationId, t.CreatorId }, name: "IX_Note")
                .Index(t => t.AttachmentId);
            
            CreateTable(
                "dbo.NoteTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 500),
                        IsSystem = c.Boolean(nullable: false),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.GrantAgreements",
                c => new
                    {
                        GrantApplicationId = c.Int(nullable: false),
                        DirectorNotes = c.String(),
                        CoverLetterId = c.Int(),
                        CoverLetterConfirmed = c.Boolean(nullable: false),
                        ScheduleAId = c.Int(),
                        ScheduleAConfirmed = c.Boolean(nullable: false),
                        ScheduleBId = c.Int(),
                        ScheduleBConfirmed = c.Boolean(nullable: false),
                        ParticipantReportingDueDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        ReimbursementClaimDueDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        CompletionReportingDueDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        DateAccepted = c.DateTime(storeType: "datetime2"),
                        StartDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        EndDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        IncompleteReason = c.String(),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.GrantApplicationId)
                .ForeignKey("dbo.Documents", t => t.CoverLetterId)
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
                .ForeignKey("dbo.Documents", t => t.ScheduleAId)
                .ForeignKey("dbo.Documents", t => t.ScheduleBId)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.CoverLetterId)
                .Index(t => t.ScheduleAId)
                .Index(t => t.ScheduleBId)
                .Index(t => new { t.StartDate, t.EndDate, t.DateAccepted, t.ParticipantReportingDueDate, t.ReimbursementClaimDueDate, t.CompletionReportingDueDate }, name: "IX_GrantAgreement");
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VersionNumber = c.Int(nullable: false),
                        DocumentTemplateId = c.Int(),
                        Title = c.String(nullable: false, maxLength: 500),
                        Body = c.String(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DocumentTemplates", t => t.DocumentTemplateId)
                .Index(t => t.DocumentTemplateId);
            
            CreateTable(
                "dbo.DocumentTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentType = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 500),
                        Body = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.DocumentType, t.IsActive }, name: "IX_DocumentTemplate");
            
            CreateTable(
                "dbo.VersionedDocuments",
                c => new
                    {
                        DocumentId = c.Int(nullable: false),
                        VersionNumber = c.Int(nullable: false),
                        DocumentTemplateId = c.Int(),
                        Title = c.String(nullable: false, maxLength: 500),
                        Body = c.String(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.DocumentId, t.VersionNumber })
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .ForeignKey("dbo.DocumentTemplates", t => t.DocumentTemplateId)
                .Index(t => t.DocumentId)
                .Index(t => t.DocumentTemplateId);
            
            CreateTable(
                "dbo.GrantOpenings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        State = c.Int(nullable: false),
                        IntakeTargetAmt = c.Double(nullable: false),
                        BudgetAllocationAmt = c.Double(nullable: false),
                        PlanDeniedRate = c.Single(nullable: false),
                        PlanWithdrawnRate = c.Single(nullable: false),
                        PlanReductionRate = c.Single(nullable: false),
                        PlanSlippageRate = c.Single(nullable: false),
                        PlanCancellationRate = c.Single(nullable: false),
                        PublishDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        OpeningDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        ClosingDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        TrainingPeriodId = c.Int(nullable: false),
                        GrantStreamId = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GrantStreams", t => t.GrantStreamId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingPeriods", t => t.TrainingPeriodId, cascadeDelete: true)
                .Index(t => new { t.State, t.PublishDate, t.OpeningDate, t.ClosingDate, t.TrainingPeriodId, t.GrantStreamId }, name: "IX_GrantOpening");
            
            CreateTable(
                "dbo.GrantOpeningFinancials",
                c => new
                    {
                        GrantOpeningId = c.Int(nullable: false),
                        CurrentReservations = c.Double(nullable: false),
                        AssessedCommitments = c.Double(nullable: false),
                        OutstandingCommitments = c.Double(nullable: false),
                        Cancellations = c.Double(nullable: false),
                        ClaimsReceived = c.Double(nullable: false),
                        ClaimsAssessed = c.Double(nullable: false),
                        CurrentClaims = c.Double(nullable: false),
                        ClaimsDenied = c.Double(nullable: false),
                        PaymentRequests = c.Double(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.GrantOpeningId)
                .ForeignKey("dbo.GrantOpenings", t => t.GrantOpeningId)
                .Index(t => t.GrantOpeningId);
            
            CreateTable(
                "dbo.GrantOpeningIntakes",
                c => new
                    {
                        GrantOpeningId = c.Int(nullable: false),
                        NewCount = c.Int(nullable: false),
                        NewAmt = c.Double(nullable: false),
                        PendingAssessmentCount = c.Int(nullable: false),
                        PendingAssessmentAmt = c.Double(nullable: false),
                        UnderAssessmentCount = c.Int(nullable: false),
                        UnderAssessmentAmt = c.Double(nullable: false),
                        DeniedCount = c.Int(nullable: false),
                        DeniedAmt = c.Double(nullable: false),
                        WithdrawnCount = c.Int(nullable: false),
                        WithdrawnAmt = c.Double(nullable: false),
                        ReductionsCount = c.Int(nullable: false),
                        ReductionsAmt = c.Double(nullable: false),
                        CommitmentCount = c.Int(nullable: false),
                        CommitmentAmt = c.Double(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.GrantOpeningId)
                .ForeignKey("dbo.GrantOpenings", t => t.GrantOpeningId)
                .Index(t => t.GrantOpeningId);
            
            CreateTable(
                "dbo.GrantStreams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        DateFirstUsed = c.DateTime(storeType: "datetime2"),
                        StreamId = c.Int(nullable: false),
                        StreamCriteriaId = c.Int(nullable: false),
                        StreamObjectiveId = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Streams", t => t.StreamId, cascadeDelete: true)
                .ForeignKey("dbo.StreamCriterias", t => t.StreamCriteriaId, cascadeDelete: true)
                .ForeignKey("dbo.StreamObjectives", t => t.StreamObjectiveId, cascadeDelete: true)
                .Index(t => new { t.IsActive, t.StreamId }, name: "IX_GrantStream")
                .Index(t => t.StreamCriteriaId)
                .Index(t => t.StreamObjectiveId);
            
            CreateTable(
                "dbo.Streams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        MaxReimbursementAmt = c.Single(nullable: false),
                        ReimbursementRate = c.Single(nullable: false),
                        DefaultDeniedRate = c.Single(nullable: false),
                        DefaultWithdrawnRate = c.Single(nullable: false),
                        DefaultReductionRate = c.Single(nullable: false),
                        DefaultSlippageRate = c.Single(nullable: false),
                        DefaultCancellationRate = c.Single(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_Stream");
            
            CreateTable(
                "dbo.StreamFiscals",
                c => new
                    {
                        StreamId = c.Int(nullable: false),
                        FiscalYearId = c.Int(nullable: false),
                        ClaimSlippageRate = c.Single(nullable: false),
                        AgreementCancellationRate = c.Single(nullable: false),
                        AgreementSlippageRate = c.Single(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.StreamId, t.FiscalYearId })
                .ForeignKey("dbo.FiscalYears", t => t.FiscalYearId, cascadeDelete: true)
                .ForeignKey("dbo.Streams", t => t.StreamId, cascadeDelete: true)
                .Index(t => t.StreamId)
                .Index(t => t.FiscalYearId);
            
            CreateTable(
                "dbo.FiscalYears",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(nullable: false, maxLength: 250),
                        StartDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        EndDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        NextAgreementNumber = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.StartDate, t.EndDate }, unique: true, name: "IX_FiscalYear_Dates");
            
            CreateTable(
                "dbo.TrainingPeriods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(nullable: false, maxLength: 250),
                        StartDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        EndDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        DefaultPublishDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        DefaultOpeningDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        FiscalYearId = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FiscalYears", t => t.FiscalYearId, cascadeDelete: true)
                .Index(t => new { t.FiscalYearId, t.StartDate, t.EndDate }, unique: true, name: "IX_TrainingPeriod");
            
            CreateTable(
                "dbo.StreamCriterias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 2000),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.StreamObjectives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 2000),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.PrioritySectors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);
            
            CreateTable(
                "dbo.GrantApplicationStateChanges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GrantApplicationId = c.Int(nullable: false),
                        FromState = c.Int(nullable: false),
                        ToState = c.Int(nullable: false),
                        ChangedDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        Reason = c.String(maxLength: 1000),
                        ApplicationAdministratorId = c.Int(),
                        AssessorId = c.Int(),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.ApplicationAdministratorId)
                .ForeignKey("dbo.InternalUsers", t => t.AssessorId)
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId, cascadeDelete: true)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.ApplicationAdministratorId)
                .Index(t => t.AssessorId);
            
            CreateTable(
                "dbo.ApplicationClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(nullable: false),
                        ClaimValue = c.String(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ApplicationUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.ApplicationRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ClaimIds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Level = c.String(nullable: false, maxLength: 20),
                        Message = c.String(),
                        Exception = c.String(),
                        UserName = c.String(maxLength: 100),
                        ServerName = c.String(maxLength: 100),
                        Url = c.String(maxLength: 500),
                        RemoteAddress = c.String(maxLength: 500),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Level, t.DateAdded }, name: "IX_Logs");
            
            CreateTable(
                "dbo.RateFormats",
                c => new
                    {
                        Rate = c.Single(nullable: false),
                        Format = c.String(nullable: false, maxLength: 50),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Rate);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 50),
                        Value = c.String(nullable: false, maxLength: 500),
                        ValueType = c.String(nullable: false, maxLength: 500),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Key);
            
            CreateTable(
                "dbo.TempData",
                c => new
                    {
                        ParentType = c.String(nullable: false, maxLength: 50),
                        ParentId = c.Int(nullable: false),
                        DataType = c.String(nullable: false, maxLength: 150),
                        Data = c.String(nullable: false, storeType: "xml"),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.ParentType, t.ParentId, t.DataType });
            
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        InternalUserId = c.Int(),
                        Active = c.Boolean(),
                        Email = c.String(maxLength: 256),
                        SecurityStamp = c.String(),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.ApplicationUserId)
                .ForeignKey("dbo.InternalUsers", t => t.InternalUserId)
                .Index(t => t.InternalUserId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.ApplicationUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ApplicationUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TrainingProgramDeliveryMethods",
                c => new
                    {
                        TrainingProgramId = c.Int(nullable: false),
                        DeliveryMethodId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrainingProgramId, t.DeliveryMethodId })
                .ForeignKey("dbo.TrainingPrograms", t => t.TrainingProgramId)
                .ForeignKey("dbo.DeliveryMethods", t => t.DeliveryMethodId)
                .Index(t => t.TrainingProgramId)
                .Index(t => t.DeliveryMethodId);
            
            CreateTable(
                "dbo.TrainingProgramDeliveryPartnerServices",
                c => new
                    {
                        TrainingProgramId = c.Int(nullable: false),
                        DeliveryPartnerServiceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrainingProgramId, t.DeliveryPartnerServiceId })
                .ForeignKey("dbo.TrainingPrograms", t => t.TrainingProgramId)
                .ForeignKey("dbo.DeliveryPartnerServices", t => t.DeliveryPartnerServiceId)
                .Index(t => t.TrainingProgramId)
                .Index(t => t.DeliveryPartnerServiceId);
            
            CreateTable(
                "dbo.TrainingProgramUnderRepresentedGroups",
                c => new
                    {
                        TrainingProgramId = c.Int(nullable: false),
                        UnderRepresentedGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrainingProgramId, t.UnderRepresentedGroupId })
                .ForeignKey("dbo.TrainingPrograms", t => t.TrainingProgramId)
                .ForeignKey("dbo.UnderRepresentedGroups", t => t.UnderRepresentedGroupId)
                .Index(t => t.TrainingProgramId)
                .Index(t => t.UnderRepresentedGroupId);
            
            CreateTable(
                "dbo.ClaimReceipts",
                c => new
                    {
                        ClaimId = c.Int(nullable: false),
                        ClaimVersion = c.Int(nullable: false),
                        AttachmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ClaimId, t.ClaimVersion, t.AttachmentId })
                .ForeignKey("dbo.Claims", t => new { t.ClaimId, t.ClaimVersion })
                .ForeignKey("dbo.Attachments", t => t.AttachmentId)
                .Index(t => new { t.ClaimId, t.ClaimVersion })
                .Index(t => t.AttachmentId);
            
            CreateTable(
                "dbo.ApplicationRoleClaims",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        ClaimId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.ClaimId })
                .ForeignKey("dbo.ApplicationRoles", t => t.RoleId)
                .ForeignKey("dbo.ApplicationClaims", t => t.ClaimId)
                .Index(t => t.RoleId)
                .Index(t => t.ClaimId);

            PostDeployment();
        }

        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserRoles", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserLogins", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUsers", "InternalUserId", "dbo.InternalUsers");
            DropForeignKey("dbo.ApplicationUserClaims", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserRoles", "RoleId", "dbo.ApplicationRoles");
            DropForeignKey("dbo.ApplicationRoleClaims", "ClaimId", "dbo.ApplicationClaims");
            DropForeignKey("dbo.ApplicationRoleClaims", "RoleId", "dbo.ApplicationRoles");
            DropForeignKey("dbo.TrainingProviders", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.TrainingPrograms", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.GrantApplicationStateChanges", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.GrantApplicationStateChanges", "AssessorId", "dbo.InternalUsers");
            DropForeignKey("dbo.GrantApplicationStateChanges", "ApplicationAdministratorId", "dbo.Users");
            DropForeignKey("dbo.GrantApplications", "PrioritySectorId", "dbo.PrioritySectors");
            DropForeignKey("dbo.GrantApplications", "OrganizationTypeId", "dbo.OrganizationTypes");
            DropForeignKey("dbo.GrantApplications", "OrganizationLegalStructureId", "dbo.LegalStructures");
            DropForeignKey("dbo.GrantApplications", "OrganizationAddressId", "dbo.ApplicationAddresses");
            DropForeignKey("dbo.GrantApplications", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.GrantApplications", "NaicsId", "dbo.NaIndustryClassificationSystems");
            DropForeignKey("dbo.GrantStreams", "StreamObjectiveId", "dbo.StreamObjectives");
            DropForeignKey("dbo.GrantStreams", "StreamCriteriaId", "dbo.StreamCriterias");
            DropForeignKey("dbo.StreamFiscals", "StreamId", "dbo.Streams");
            DropForeignKey("dbo.GrantOpenings", "TrainingPeriodId", "dbo.TrainingPeriods");
            DropForeignKey("dbo.TrainingPeriods", "FiscalYearId", "dbo.FiscalYears");
            DropForeignKey("dbo.StreamFiscals", "FiscalYearId", "dbo.FiscalYears");
            DropForeignKey("dbo.GrantStreams", "StreamId", "dbo.Streams");
            DropForeignKey("dbo.GrantOpenings", "GrantStreamId", "dbo.GrantStreams");
            DropForeignKey("dbo.GrantOpeningIntakes", "GrantOpeningId", "dbo.GrantOpenings");
            DropForeignKey("dbo.GrantOpeningFinancials", "GrantOpeningId", "dbo.GrantOpenings");
            DropForeignKey("dbo.GrantApplications", "GrantOpeningId", "dbo.GrantOpenings");
            DropForeignKey("dbo.GrantAgreements", "ScheduleBId", "dbo.Documents");
            DropForeignKey("dbo.GrantAgreements", "ScheduleAId", "dbo.Documents");
            DropForeignKey("dbo.GrantAgreements", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.GrantAgreements", "CoverLetterId", "dbo.Documents");
            DropForeignKey("dbo.VersionedDocuments", "DocumentTemplateId", "dbo.DocumentTemplates");
            DropForeignKey("dbo.VersionedDocuments", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.Documents", "DocumentTemplateId", "dbo.DocumentTemplates");
            DropForeignKey("dbo.GrantApplications", "AssessorId", "dbo.InternalUsers");
            DropForeignKey("dbo.Notes", "NoteTypeId", "dbo.NoteTypes");
            DropForeignKey("dbo.Notes", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.Notes", "CreatorId", "dbo.InternalUsers");
            DropForeignKey("dbo.Notes", "AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.InternalUserFilters", "InternalUserId", "dbo.InternalUsers");
            DropForeignKey("dbo.InternalUserFilterAttributes", "InternUserFilterId", "dbo.InternalUserFilters");
            DropForeignKey("dbo.Claims", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.ClaimReceipts", "AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.ClaimReceipts", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims");
            DropForeignKey("dbo.Payments", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims");
            DropForeignKey("dbo.ClaimEligibleCosts", "EligibleExpenseTypeId", "dbo.EligibleExpenseTypes");
            DropForeignKey("dbo.ClaimEligibleCosts", "EligibleCostId", "dbo.EligibleCosts");
            DropForeignKey("dbo.TrainingProgramUnderRepresentedGroups", "UnderRepresentedGroupId", "dbo.UnderRepresentedGroups");
            DropForeignKey("dbo.TrainingProgramUnderRepresentedGroups", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.TrainingPrograms", "TrainingLevelId", "dbo.TrainingLevels");
            DropForeignKey("dbo.TrainingPrograms", "SkillLevelId", "dbo.SkillLevels");
            DropForeignKey("dbo.TrainingPrograms", "SkillFocusId", "dbo.SkillsFocus");
            DropForeignKey("dbo.TrainingPrograms", "RequestedTrainingProviderId", "dbo.TrainingProviders");
            DropForeignKey("dbo.TrainingProviders", "TrainingProviderTypeId", "dbo.TrainingProviderTypes");
            DropForeignKey("dbo.TrainingProviders", "TrainingProviderInventoryId", "dbo.TrainingProviderInventory");
            DropForeignKey("dbo.TrainingPrograms", "TrainingProviderId", "dbo.TrainingProviders");
            DropForeignKey("dbo.TrainingProviders", "TrainingAddressId", "dbo.ApplicationAddresses");
            DropForeignKey("dbo.TrainingProviders", "ProofOfQualificationsDocumentId", "dbo.Attachments");
            DropForeignKey("dbo.TrainingProviders", "CourseOutlineDocumentId", "dbo.Attachments");
            DropForeignKey("dbo.TrainingProviders", "BusinessCaseDocumentId", "dbo.Attachments");
            DropForeignKey("dbo.VersionedAttachments", "AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.TrainingPrograms", "InDemandOccupationId", "dbo.InDemandOccupations");
            DropForeignKey("dbo.TrainingPrograms", "ExpectedQualificationId", "dbo.ExpectedQualifications");
            DropForeignKey("dbo.EmployerEnrollments", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.ParticipantForms", "TrainingResultId", "dbo.TrainingResults");
            DropForeignKey("dbo.ParticipantForms", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.ParticipantForms", new[] { "RegionId", "CountryId" }, "dbo.Regions");
            DropForeignKey("dbo.ParticipantForms", "RecentPeriodId", "dbo.RecentPeriods");
            DropForeignKey("dbo.ParticipantEnrollments", "ParticipantFormId", "dbo.ParticipantForms");
            DropForeignKey("dbo.ParticipantForms", "ParticipantConsentId", "dbo.ParticipantConsents");
            DropForeignKey("dbo.ParticipantForms", "NocId", "dbo.NationalOccupationalClassifications");
            DropForeignKey("dbo.NationalOccupationalClassifications", "ParentId", "dbo.NationalOccupationalClassifications");
            DropForeignKey("dbo.ParticipantForms", "EmploymentTypeId", "dbo.EmploymentTypes");
            DropForeignKey("dbo.ParticipantForms", "EmploymentStatusId", "dbo.EmploymentStatus");
            DropForeignKey("dbo.ParticipantForms", "EIBenefitId", "dbo.EIBenefits");
            DropForeignKey("dbo.ParticipantForms", "EducationLevelId", "dbo.EducationLevels");
            DropForeignKey("dbo.ParticipantForms", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.ParticipantForms", "CanadianStatusId", "dbo.CanadianStatus");
            DropForeignKey("dbo.ParticipantForms", "AboriginalBandId", "dbo.AboriginalBands");
            DropForeignKey("dbo.ParticipantCosts", "ParticipantEnrollmentId", "dbo.ParticipantEnrollments");
            DropForeignKey("dbo.ParticipantCosts", "ClaimEligibleCostId", "dbo.ClaimEligibleCosts");
            DropForeignKey("dbo.ParticipantEnrollments", "ParticipantId", "dbo.Participants");
            DropForeignKey("dbo.ParticipantEnrollments", "EmployerEnrollmentId", "dbo.EmployerEnrollments");
            DropForeignKey("dbo.EmployerEnrollments", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.BusinessContactRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "PhysicalAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Users", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Organizations", "OrganizationTypeId", "dbo.OrganizationTypes");
            DropForeignKey("dbo.Organizations", "NaicsId", "dbo.NaIndustryClassificationSystems");
            DropForeignKey("dbo.NaIndustryClassificationSystems", "ParentId", "dbo.NaIndustryClassificationSystems");
            DropForeignKey("dbo.Organizations", "LegalStructureId", "dbo.LegalStructures");
            DropForeignKey("dbo.Organizations", "HeadOfficeAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Notifications", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Notifications", "NotificationTemplateId", "dbo.NotificationTemplates");
            DropForeignKey("dbo.Notifications", "NotificationScheduleQueueId", "dbo.NotificationScheduleQueue");
            DropForeignKey("dbo.NotificationScheduleQueue", "NotificationTypeId", "dbo.NotificationTypes");
            DropForeignKey("dbo.NotificationTypes", "NotificationTemplateId", "dbo.NotificationTemplates");
            DropForeignKey("dbo.NotificationScheduleQueue", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.Users", "MailingAddressId", "dbo.Addresses");
            DropForeignKey("dbo.BusinessContactRoles", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.BusinessContactRoles", "EmployerEnrollmentId", "dbo.EmployerEnrollments");
            DropForeignKey("dbo.EligibleCosts", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.TrainingProgramDeliveryPartnerServices", "DeliveryPartnerServiceId", "dbo.DeliveryPartnerServices");
            DropForeignKey("dbo.TrainingProgramDeliveryPartnerServices", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.TrainingPrograms", "DeliveryPartnerId", "dbo.DeliveryPartners");
            DropForeignKey("dbo.TrainingProgramDeliveryMethods", "DeliveryMethodId", "dbo.DeliveryMethods");
            DropForeignKey("dbo.TrainingProgramDeliveryMethods", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.EligibleCosts", "EligibleExpenseTypeId", "dbo.EligibleExpenseTypes");
            DropForeignKey("dbo.ClaimEligibleCosts", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims");
            DropForeignKey("dbo.Claims", "AssessorId", "dbo.InternalUsers");
            DropForeignKey("dbo.GrantApplications", "ApplicationTypeId", "dbo.ApplicationTypes");
            DropForeignKey("dbo.GrantApplications", "ApplicantPhysicalAddressId", "dbo.ApplicationAddresses");
            DropForeignKey("dbo.GrantApplications", "ApplicantMailingAddressId", "dbo.ApplicationAddresses");
            DropForeignKey("dbo.ApplicationAddresses", new[] { "RegionId", "CountryId" }, "dbo.Regions");
            DropForeignKey("dbo.ApplicationAddresses", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.GrantApplications", "ApplicantDeclarationId", "dbo.ApplicantDeclarations");
            DropForeignKey("dbo.Addresses", new[] { "RegionId", "CountryId" }, "dbo.Regions");
            DropForeignKey("dbo.Addresses", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Regions", "CountryId", "dbo.Countries");
            DropIndex("dbo.ApplicationRoleClaims", new[] { "ClaimId" });
            DropIndex("dbo.ApplicationRoleClaims", new[] { "RoleId" });
            DropIndex("dbo.ClaimReceipts", new[] { "AttachmentId" });
            DropIndex("dbo.ClaimReceipts", new[] { "ClaimId", "ClaimVersion" });
            DropIndex("dbo.TrainingProgramUnderRepresentedGroups", new[] { "UnderRepresentedGroupId" });
            DropIndex("dbo.TrainingProgramUnderRepresentedGroups", new[] { "TrainingProgramId" });
            DropIndex("dbo.TrainingProgramDeliveryPartnerServices", new[] { "DeliveryPartnerServiceId" });
            DropIndex("dbo.TrainingProgramDeliveryPartnerServices", new[] { "TrainingProgramId" });
            DropIndex("dbo.TrainingProgramDeliveryMethods", new[] { "DeliveryMethodId" });
            DropIndex("dbo.TrainingProgramDeliveryMethods", new[] { "TrainingProgramId" });
            DropIndex("dbo.ApplicationUserLogins", new[] { "UserId" });
            DropIndex("dbo.ApplicationUserClaims", new[] { "UserId" });
            DropIndex("dbo.ApplicationUsers", "UserNameIndex");
            DropIndex("dbo.ApplicationUsers", new[] { "InternalUserId" });
            DropIndex("dbo.Logs", "IX_Logs");
            DropIndex("dbo.ApplicationUserRoles", new[] { "RoleId" });
            DropIndex("dbo.ApplicationUserRoles", new[] { "UserId" });
            DropIndex("dbo.ApplicationRoles", "RoleNameIndex");
            DropIndex("dbo.GrantApplicationStateChanges", new[] { "AssessorId" });
            DropIndex("dbo.GrantApplicationStateChanges", new[] { "ApplicationAdministratorId" });
            DropIndex("dbo.GrantApplicationStateChanges", new[] { "GrantApplicationId" });
            DropIndex("dbo.PrioritySectors", new[] { "Caption" });
            DropIndex("dbo.StreamObjectives", new[] { "Caption" });
            DropIndex("dbo.StreamCriterias", new[] { "Caption" });
            DropIndex("dbo.TrainingPeriods", "IX_TrainingPeriod");
            DropIndex("dbo.FiscalYears", "IX_FiscalYear_Dates");
            DropIndex("dbo.StreamFiscals", new[] { "FiscalYearId" });
            DropIndex("dbo.StreamFiscals", new[] { "StreamId" });
            DropIndex("dbo.Streams", "IX_Stream");
            DropIndex("dbo.GrantStreams", new[] { "StreamObjectiveId" });
            DropIndex("dbo.GrantStreams", new[] { "StreamCriteriaId" });
            DropIndex("dbo.GrantStreams", "IX_GrantStream");
            DropIndex("dbo.GrantOpeningIntakes", new[] { "GrantOpeningId" });
            DropIndex("dbo.GrantOpeningFinancials", new[] { "GrantOpeningId" });
            DropIndex("dbo.GrantOpenings", "IX_GrantOpening");
            DropIndex("dbo.VersionedDocuments", new[] { "DocumentTemplateId" });
            DropIndex("dbo.VersionedDocuments", new[] { "DocumentId" });
            DropIndex("dbo.DocumentTemplates", "IX_DocumentTemplate");
            DropIndex("dbo.Documents", new[] { "DocumentTemplateId" });
            DropIndex("dbo.GrantAgreements", "IX_GrantAgreement");
            DropIndex("dbo.GrantAgreements", new[] { "ScheduleBId" });
            DropIndex("dbo.GrantAgreements", new[] { "ScheduleAId" });
            DropIndex("dbo.GrantAgreements", new[] { "CoverLetterId" });
            DropIndex("dbo.GrantAgreements", new[] { "GrantApplicationId" });
            DropIndex("dbo.NoteTypes", new[] { "Caption" });
            DropIndex("dbo.Notes", new[] { "AttachmentId" });
            DropIndex("dbo.Notes", "IX_Note");
            DropIndex("dbo.InternalUserFilterAttributes", new[] { "InternUserFilterId" });
            DropIndex("dbo.InternalUserFilters", new[] { "InternalUserId" });
            DropIndex("dbo.Payments", new[] { "ClaimId", "ClaimVersion" });
            DropIndex("dbo.UnderRepresentedGroups", new[] { "Caption" });
            DropIndex("dbo.TrainingLevels", new[] { "Caption" });
            DropIndex("dbo.SkillLevels", new[] { "Caption" });
            DropIndex("dbo.SkillsFocus", new[] { "Caption" });
            DropIndex("dbo.TrainingProviderTypes", new[] { "Caption" });
            DropIndex("dbo.TrainingProviderInventory", "IX_TrainingProviderInventory");
            DropIndex("dbo.VersionedAttachments", new[] { "AttachmentId" });
            DropIndex("dbo.TrainingProviders", new[] { "CourseOutlineDocumentId" });
            DropIndex("dbo.TrainingProviders", new[] { "ProofOfQualificationsDocumentId" });
            DropIndex("dbo.TrainingProviders", new[] { "BusinessCaseDocumentId" });
            DropIndex("dbo.TrainingProviders", new[] { "TrainingProviderInventoryId" });
            DropIndex("dbo.TrainingProviders", new[] { "TrainingAddressId" });
            DropIndex("dbo.TrainingProviders", new[] { "TrainingProviderTypeId" });
            DropIndex("dbo.TrainingProviders", new[] { "GrantApplicationId" });
            DropIndex("dbo.InDemandOccupations", new[] { "Caption" });
            DropIndex("dbo.ExpectedQualifications", new[] { "Caption" });
            DropIndex("dbo.TrainingResults", new[] { "Caption" });
            DropIndex("dbo.RecentPeriods", new[] { "Caption" });
            DropIndex("dbo.ParticipantConsents", "IX_ParticipantConsent");
            DropIndex("dbo.NationalOccupationalClassifications", new[] { "ParentId" });
            DropIndex("dbo.NationalOccupationalClassifications", "IX_NationalOccupationalClassification");
            DropIndex("dbo.NationalOccupationalClassifications", "IX_NaIndustryClassificationSystem");
            DropIndex("dbo.EmploymentTypes", new[] { "Caption" });
            DropIndex("dbo.EmploymentStatus", new[] { "Caption" });
            DropIndex("dbo.EIBenefits", new[] { "Caption" });
            DropIndex("dbo.EducationLevels", new[] { "Caption" });
            DropIndex("dbo.CanadianStatus", new[] { "Caption" });
            DropIndex("dbo.ParticipantForms", new[] { "NocId" });
            DropIndex("dbo.ParticipantForms", new[] { "EIBenefitId" });
            DropIndex("dbo.ParticipantForms", new[] { "ParticipantConsentId" });
            DropIndex("dbo.ParticipantForms", new[] { "TrainingResultId" });
            DropIndex("dbo.ParticipantForms", new[] { "EmploymentStatusId" });
            DropIndex("dbo.ParticipantForms", new[] { "RecentPeriodId" });
            DropIndex("dbo.ParticipantForms", new[] { "EmploymentTypeId" });
            DropIndex("dbo.ParticipantForms", new[] { "EducationLevelId" });
            DropIndex("dbo.ParticipantForms", new[] { "AboriginalBandId" });
            DropIndex("dbo.ParticipantForms", new[] { "CanadianStatusId" });
            DropIndex("dbo.ParticipantForms", "IX_ParticipantForm");
            DropIndex("dbo.ParticipantForms", new[] { "CountryId" });
            DropIndex("dbo.ParticipantForms", new[] { "RegionId", "CountryId" });
            DropIndex("dbo.ParticipantCosts", new[] { "ParticipantEnrollmentId" });
            DropIndex("dbo.ParticipantCosts", new[] { "ClaimEligibleCostId" });
            DropIndex("dbo.ParticipantEnrollments", "IX_ParticipantEnrollment");
            DropIndex("dbo.OrganizationTypes", new[] { "Caption" });
            DropIndex("dbo.NaIndustryClassificationSystems", new[] { "ParentId" });
            DropIndex("dbo.NaIndustryClassificationSystems", "IX_NaIndustryClassificationSystem");
            DropIndex("dbo.LegalStructures", new[] { "Caption" });
            DropIndex("dbo.Organizations", new[] { "NaicsId" });
            DropIndex("dbo.Organizations", new[] { "HeadOfficeAddressId" });
            DropIndex("dbo.Organizations", "IX_Organization");
            DropIndex("dbo.Organizations", "IX_Organization_BCeID");
            DropIndex("dbo.NotificationTypes", new[] { "NotificationTemplateId" });
            DropIndex("dbo.NotificationScheduleQueue", "IX_NotificationScheduleQueue");
            DropIndex("dbo.NotificationScheduleQueue", new[] { "GrantApplicationId" });
            DropIndex("dbo.Notifications", new[] { "User_Id" });
            DropIndex("dbo.Notifications", new[] { "NotificationScheduleQueueId" });
            DropIndex("dbo.Notifications", new[] { "NotificationTemplateId" });
            DropIndex("dbo.Users", new[] { "OrganizationId" });
            DropIndex("dbo.Users", new[] { "MailingAddressId" });
            DropIndex("dbo.Users", new[] { "PhysicalAddressId" });
            DropIndex("dbo.Users", new[] { "BCeIDGuid" });
            DropIndex("dbo.BusinessContactRoles", "IX_BusinessContactRole");
            DropIndex("dbo.EmployerEnrollments", new[] { "OrganizationId" });
            DropIndex("dbo.EmployerEnrollments", new[] { "TrainingProgramId" });
            DropIndex("dbo.DeliveryPartnerServices", new[] { "Caption" });
            DropIndex("dbo.DeliveryPartners", new[] { "Caption" });
            DropIndex("dbo.DeliveryMethods", new[] { "Caption" });
            DropIndex("dbo.TrainingPrograms", new[] { "DeliveryPartnerId" });
            DropIndex("dbo.TrainingPrograms", new[] { "TrainingLevelId" });
            DropIndex("dbo.TrainingPrograms", new[] { "ExpectedQualificationId" });
            DropIndex("dbo.TrainingPrograms", new[] { "SkillFocusId" });
            DropIndex("dbo.TrainingPrograms", new[] { "SkillLevelId" });
            DropIndex("dbo.TrainingPrograms", new[] { "InDemandOccupationId" });
            DropIndex("dbo.TrainingPrograms", new[] { "GrantApplicationId" });
            DropIndex("dbo.TrainingPrograms", new[] { "RequestedTrainingProviderId" });
            DropIndex("dbo.TrainingPrograms", new[] { "TrainingProviderId" });
            DropIndex("dbo.EligibleExpenseTypes", new[] { "Caption" });
            DropIndex("dbo.EligibleCosts", new[] { "EligibleExpenseTypeId" });
            DropIndex("dbo.EligibleCosts", new[] { "TrainingProgramId" });
            DropIndex("dbo.ClaimEligibleCosts", new[] { "EligibleCostId" });
            DropIndex("dbo.ClaimEligibleCosts", new[] { "EligibleExpenseTypeId" });
            DropIndex("dbo.ClaimEligibleCosts", new[] { "ClaimId", "ClaimVersion" });
            DropIndex("dbo.Claims", new[] { "TrainingProgramId" });
            DropIndex("dbo.Claims", "IX_Claim");
            DropIndex("dbo.InternalUsers", "IX_InternalUser");
            DropIndex("dbo.ApplicationTypes", new[] { "Caption" });
            DropIndex("dbo.ApplicationAddresses", new[] { "CountryId" });
            DropIndex("dbo.ApplicationAddresses", new[] { "RegionId", "CountryId" });
            DropIndex("dbo.GrantApplications", new[] { "NaicsId" });
            DropIndex("dbo.GrantApplications", new[] { "OrganizationLegalStructureId" });
            DropIndex("dbo.GrantApplications", new[] { "OrganizationTypeId" });
            DropIndex("dbo.GrantApplications", new[] { "OrganizationAddressId" });
            DropIndex("dbo.GrantApplications", new[] { "OrganizationId" });
            DropIndex("dbo.GrantApplications", new[] { "ApplicantMailingAddressId" });
            DropIndex("dbo.GrantApplications", new[] { "ApplicantPhysicalAddressId" });
            DropIndex("dbo.GrantApplications", new[] { "ApplicantDeclarationId" });
            DropIndex("dbo.GrantApplications", "IX_GrantApplication");
            DropIndex("dbo.ApplicantDeclarations", "IX_ApplicantDeclaration");
            DropIndex("dbo.Regions", "IX_Region");
            DropIndex("dbo.Countries", "IX_Country");
            DropIndex("dbo.Addresses", new[] { "CountryId" });
            DropIndex("dbo.Addresses", new[] { "RegionId", "CountryId" });
            DropIndex("dbo.AboriginalBands", new[] { "Caption" });
            DropTable("dbo.ApplicationRoleClaims");
            DropTable("dbo.ClaimReceipts");
            DropTable("dbo.TrainingProgramUnderRepresentedGroups");
            DropTable("dbo.TrainingProgramDeliveryPartnerServices");
            DropTable("dbo.TrainingProgramDeliveryMethods");
            DropTable("dbo.ApplicationUserLogins");
            DropTable("dbo.ApplicationUserClaims");
            DropTable("dbo.ApplicationUsers");
            DropTable("dbo.TempData");
            DropTable("dbo.Settings");
            DropTable("dbo.RateFormats");
            DropTable("dbo.Logs");
            DropTable("dbo.ClaimIds");
            DropTable("dbo.ApplicationUserRoles");
            DropTable("dbo.ApplicationRoles");
            DropTable("dbo.ApplicationClaims");
            DropTable("dbo.GrantApplicationStateChanges");
            DropTable("dbo.PrioritySectors");
            DropTable("dbo.StreamObjectives");
            DropTable("dbo.StreamCriterias");
            DropTable("dbo.TrainingPeriods");
            DropTable("dbo.FiscalYears");
            DropTable("dbo.StreamFiscals");
            DropTable("dbo.Streams");
            DropTable("dbo.GrantStreams");
            DropTable("dbo.GrantOpeningIntakes");
            DropTable("dbo.GrantOpeningFinancials");
            DropTable("dbo.GrantOpenings");
            DropTable("dbo.VersionedDocuments");
            DropTable("dbo.DocumentTemplates");
            DropTable("dbo.Documents");
            DropTable("dbo.GrantAgreements");
            DropTable("dbo.NoteTypes");
            DropTable("dbo.Notes");
            DropTable("dbo.InternalUserFilterAttributes");
            DropTable("dbo.InternalUserFilters");
            DropTable("dbo.Payments");
            DropTable("dbo.UnderRepresentedGroups");
            DropTable("dbo.TrainingLevels");
            DropTable("dbo.SkillLevels");
            DropTable("dbo.SkillsFocus");
            DropTable("dbo.TrainingProviderTypes");
            DropTable("dbo.TrainingProviderInventory");
            DropTable("dbo.VersionedAttachments");
            DropTable("dbo.Attachments");
            DropTable("dbo.TrainingProviders");
            DropTable("dbo.InDemandOccupations");
            DropTable("dbo.ExpectedQualifications");
            DropTable("dbo.TrainingResults");
            DropTable("dbo.RecentPeriods");
            DropTable("dbo.ParticipantConsents");
            DropTable("dbo.NationalOccupationalClassifications");
            DropTable("dbo.EmploymentTypes");
            DropTable("dbo.EmploymentStatus");
            DropTable("dbo.EIBenefits");
            DropTable("dbo.EducationLevels");
            DropTable("dbo.CanadianStatus");
            DropTable("dbo.ParticipantForms");
            DropTable("dbo.ParticipantCosts");
            DropTable("dbo.Participants");
            DropTable("dbo.ParticipantEnrollments");
            DropTable("dbo.OrganizationTypes");
            DropTable("dbo.NaIndustryClassificationSystems");
            DropTable("dbo.LegalStructures");
            DropTable("dbo.Organizations");
            DropTable("dbo.NotificationTemplates");
            DropTable("dbo.NotificationTypes");
            DropTable("dbo.NotificationScheduleQueue");
            DropTable("dbo.Notifications");
            DropTable("dbo.Users");
            DropTable("dbo.BusinessContactRoles");
            DropTable("dbo.EmployerEnrollments");
            DropTable("dbo.DeliveryPartnerServices");
            DropTable("dbo.DeliveryPartners");
            DropTable("dbo.DeliveryMethods");
            DropTable("dbo.TrainingPrograms");
            DropTable("dbo.EligibleExpenseTypes");
            DropTable("dbo.EligibleCosts");
            DropTable("dbo.ClaimEligibleCosts");
            DropTable("dbo.Claims");
            DropTable("dbo.InternalUsers");
            DropTable("dbo.ApplicationTypes");
            DropTable("dbo.ApplicationAddresses");
            DropTable("dbo.GrantApplications");
            DropTable("dbo.ApplicantDeclarations");
            DropTable("dbo.Regions");
            DropTable("dbo.Countries");
            DropTable("dbo.Addresses");
            DropTable("dbo.AboriginalBands");
        }
    }
}
