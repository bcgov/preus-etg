using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Web;
using CJG.Core.Entities;
using CJG.Infrastructure.Identity;

namespace CJG.Infrastructure.Entities
{
	public interface IDataContext : IDisposable
	{
		#region Properties
		DbChangeTracker ChangeTracker { get; }

		Database Database { get; }

		DbContext Context { get; }

		HttpContextBase HttpContext { get; set; }

		#region Users
		DbSet<User> Users { get; }
		DbSet<UserPreference> UserPreferences { get; }
		DbSet<UserGrantProgramPreference> UserGrantProgramPreferences { get; }
		DbSet<InternalUser> InternalUsers { get; }
		DbSet<InternalUserFilter> InternalUserFilters { get; }
		DbSet<InternalUserFilterAttribute> InternalUserFilterAttributes { get; }
		DbSet<ApplicationClaim> ApplicationClaims { get; }
		DbSet<ApplicationRole> ApplicationRoles { get; }
		DbSet<BusinessContactRole> BusinessContactRoles { get; }
		#endregion

		#region Notifications
		DbSet<NotificationType> NotificationTypes { get; }
		DbSet<NotificationTemplate> NotificationTemplates { get; }
		DbSet<NotificationTrigger> NotificationTriggers { get; }
		DbSet<NotificationQueue> NotificationQueue { get; }

		#endregion

		#region Organizations
		DbSet<Organization> Organizations { get; }
		DbSet<OrganizationType> OrganizationTypes { get; }
		DbSet<LegalStructure> LegalStructures { get; }
		DbSet<PrioritySector> PrioritySectors { get; }
		DbSet<ApplicantOrganizationType> ApplicantOrganizationTypes { get; }
		#endregion

		#region Attachments
		DbSet<Attachment> Attachments { get; }
		DbSet<VersionedAttachment> VersionedAttachments { get; }

		DbSet<DocumentTemplate> DocumentTemplates { get; }
		DbSet<Document> Documents { get; }
		DbSet<VersionedDocument> VersionedDocuments { get; }
		#endregion

		#region Addresses
		DbSet<Address> Addresses { get; }
		DbSet<Region> Regions { get; }
		DbSet<Country> Countries { get; }
		#endregion

		#region Administrative
		DbSet<FiscalYear> FiscalYears { get; }
		DbSet<TrainingPeriod> TrainingPeriods { get; }
		DbSet<GrantProgram> GrantPrograms { get; }
		DbSet<GrantProgramNotificationType> GrantProgramNotificationTypes { get; }
		DbSet<GrantStream> GrantStreams { get; }
		DbSet<GrantOpening> GrantOpenings { get; }
		DbSet<GrantStreamEligibilityQuestion> GrantStreamEligibilityQuestions { get; }
		DbSet<GrantStreamEligibilityAnswer> GrantStreamEligibilityAnswers { get; }
		DbSet<GrantOpeningFinancial> GrantOpeningFinancials { get; }
		DbSet<GrantOpeningIntake> GrantOpeningIntakes { get; }
		DbSet<ReportRate> ReportRates { get; }
		DbSet<ProgramType> ProgramTypes { get; }
		DbSet<RiskClassification> RiskClassifications { get; }
		DbSet<ServiceType> ServiceTypes { get; }
		DbSet<ServiceCategory> ServiceCategories { get; }
		DbSet<ServiceLine> ServiceLines { get; }
		DbSet<ServiceLineBreakdown> ServiceLineBreakdowns { get; }
		DbSet<ProgramDescription> ProgramDescriptions { get; }
		DbSet<VulnerableGroup> VulnerableGroups { get; }
		DbSet<ParticipantEmploymentStatus> ParticipantEmploymentStatuses { get; }
		DbSet<UnderRepresentedPopulation> UnderRepresentedPopulations { get; }
		DbSet<Community> Communities { get; }
		DbSet<ExpenseType> ExpenseTypes { get; }
		DbSet<ProgramConfiguration> ProgramConfigurations { get; }
		DbSet<ProgramNotification> ProgramNotifications { get; }
		DbSet<ProgramNotificationRecipient> ProgramNotificationRecipients { get; }
		DbSet<TrainingPeriodBudgetRate> TrainingPeriodBudgetRates { get; }
		#endregion

		#region Grant Applications
		DbSet<GrantApplication> GrantApplications { get; }
		DbSet<ApplicationType> ApplicationTypes { get; }
		DbSet<ApplicationAddress> ApplicationAddresses { get; }
		DbSet<Note> Notes { get; }
		DbSet<NoteType> NoteTypes { get; }
		DbSet<NaIndustryClassificationSystem> NaIndustryClassificationSystems { get; }
		DbSet<GrantAgreement> GrantAgreements { get; }
		DbSet<GrantApplicationStateChange> GrantApplicationStateChanges { get; }
		DbSet<GrantApplicationInternalState> GrantApplicationInternalStates { get; }
		DbSet<GrantApplicationExternalState> GrantApplicationExternalStates { get; }
		DbSet<TrainingCost> TrainingCosts { get; }
		DbSet<EligibleCost> EligibleCosts { get; }
		DbSet<EligibleCostBreakdown> EligibleCostBreakdowns { get; }
		DbSet<EligibleExpenseType> EligibleExpenseTypes { get; }
		DbSet<EligibleExpenseBreakdown> EligibleExpenseBreakdowns { get; }
		DbSet<DenialReason> DenialReasons { get; }
		DbSet<PrioritizationScoreBreakdown> PrioritizationScoreBreakdowns { get; }
		DbSet<PrioritizationScoreBreakdownAnswer> PrioritizationScoreBreakdownAnswers { get; }
		#endregion

		#region Training Providers
		DbSet<TrainingProvider> TrainingProviders { get; }
		DbSet<TrainingProviderInventory> TrainingProviderInventory { get; }
		DbSet<TrainingProviderType> TrainingProviderTypes { get; }
		#endregion

		#region Training Programs
		DbSet<TrainingProgram> TrainingPrograms { get; }
		DbSet<UnderRepresentedGroup> UnderRepresentedGroups { get; }
		DbSet<DeliveryMethod> DeliveryMethods { get; }
		DbSet<ExpectedQualification> ExpectedQualifications { get; }
		DbSet<InDemandOccupation> InDemandOccupations { get; }
		DbSet<SkillsFocus> SkillsFocuses { get; }
		DbSet<SkillLevel> SkillLevels { get; }
		DbSet<TrainingLevel> TrainingLevels { get; }
		DbSet<DeliveryPartner> DeliveryPartners { get; }
		DbSet<DeliveryPartnerService> DeliveryPartnerServices { get; }
		#endregion

		#region Participants
		DbSet<ParticipantInvitation> ParticipantInvitations { get; }
		DbSet<ParticipantForm> ParticipantForms { get; }
		DbSet<ClaimParticipant> ClaimParticipants { get; }
		DbSet<Participant> Participants { get; }
		DbSet<ParticipantCost> ParticipantCosts { get; }
		DbSet<NationalOccupationalClassification> NationalOccupationalClassifications { get; }
		DbSet<CanadianStatus> CanadianStatuses { get; }
		DbSet<AboriginalBand> AboriginalBands { get; }
		DbSet<EducationLevel> EducationLevels { get; }
		DbSet<EmploymentType> EmploymentTypes { get; }
		DbSet<EmploymentStatus> EmploymentStatuses { get; }
		DbSet<TrainingResult> TrainingResults { get; }
		DbSet<EIBenefit> EIBenefits { get; }
		DbSet<MaritalStatus> MaritalStatuses { get; }
		DbSet<FederalOfficialLanguage> FederalOfficialLanguages { get; }
		#endregion

		#region Claims
		DbSet<Claim> Claims { get; }
		DbSet<ClaimId> ClaimIds { get; }
		DbSet<ClaimEligibleCost> ClaimEligibleCosts { get; }
		DbSet<ClaimBreakdownCost> ClaimBreakdownCosts { get; }
		DbSet<GrantApplicationClaimState> ClaimStates { get; }
		DbSet<ClaimType> ClaimTypes { get; }
		#endregion

		#region Misc
		DbSet<Log> Logs { get; }
		DbSet<TempData> TempData { get; }
		DbSet<Setting> Settings { get; }
		DbSet<RateFormat> RateFormats { get; }
		DbSet<PrioritizationThreshold> PrioritizationThresholds { get; }
		DbSet<PrioritizationIndustryScore> PrioritizationIndustryScores { get; }
		DbSet<PrioritizationHighOpportunityOccupationScore> PrioritizationHighOpportunityOccupationScores { get; }
		DbSet<PrioritizationRegion> PrioritizationRegions { get; }
		DbSet<PrioritizationPostalCode> PrioritizationPostalCodes { get; }
		#endregion

		#region Completion Report
		DbSet<CompletionReport> CompletionReports { get; }
		DbSet<CompletionReportQuestion> CompletionReportQuestions { get; }
		DbSet<CompletionReportOption> CompletionReportOptions { get; }
		DbSet<CompletionReportGroup> CompletionReportGroups { get; }
		DbSet<ParticipantCompletionReportAnswer> ParticipantCompletionReportAnswers { get; }
		DbSet<EmployerCompletionReportAnswer> EmployerCompletionReportAnswers { get; }
		#endregion

		#region Payment Requests
		DbSet<PaymentRequest> PaymentRequests { get; }
		DbSet<PaymentRequestBatch> PaymentRequestBatches { get; }
		DbSet<AccountCode> AccountCodes { get; }
		DbSet<ReconciliationReport> ReconciliationReports { get; }
		DbSet<ReconciliationPayment> ReconciliationPayments { get; }
		#endregion

		#region CIPSCodes
		DbSet<ClassificationOfInstructionalProgram> ClassificationOfInstructionalPrograms { get; }
		#endregion

		#endregion

		#region Methods
		DbSet<TEntity> Set<TEntity>() where TEntity : class;
		DbEntityEntry Entry(object entity);
		DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
		IEnumerable<ValidationResult> Validate<TEntity>(TEntity entity) where TEntity : class;
		TEntity Convert<TEntity>(object source, bool ignoreCase = false) where TEntity : class;
		TEntity Convert<TEntity>(object source, TEntity result, bool ignoreCase = false) where TEntity : class;
		TEntity ConvertAndValidate<TEntity>(object source, ICollection<ValidationResult> validationResults, bool ignoreCase = false) where TEntity : class;
		void ConvertAndValidate<TEntity>(object source, TEntity result, ICollection<ValidationResult> validationResults, bool ignoreCase = false) where TEntity : class;
		void CopyTo<TEntity>(TEntity source, TEntity destination) where TEntity : class;
		int SaveChanges();
		IEnumerable<DbEntityValidationResult> GetValidationErrors();
		int Commit();
		void UpdateRowVersion(DbEntityEntry entry);
		int CommitTransaction();
		int CommitTransaction(Func<int> update);
		void Update<TEntity>(TEntity entity) where TEntity : class;
		void SetModified(object entity);
		object OriginalValue<TEntity>(TEntity entity, string propertyName) where TEntity : class;
		#endregion
	}
}
