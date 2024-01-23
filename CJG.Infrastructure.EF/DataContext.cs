using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace CJG.Infrastructure.EF
{
	public class DataContext : IDataContext
	{
		#region Variables
		private readonly CJGContext _context;
		#endregion

		#region Properties
		public DbChangeTracker ChangeTracker => _context.ChangeTracker;

		public Database Database => _context.Database;

		public DbContext Context => _context;

		public HttpContextBase HttpContext
		{
			get { return _context.HttpContext; }
			set { _context.HttpContext = value; }
		}

		#region Users
		public DbSet<User> Users => _context.Users;
		public DbSet<UserPreference> UserPreferences => _context.UserPreferences;
		public DbSet<UserGrantProgramPreference> UserGrantProgramPreferences => _context.UserGrantProgramPreferences;
		public DbSet<InternalUser> InternalUsers => _context.InternalUsers;
		public DbSet<InternalUserFilter> InternalUserFilters => _context.InternalUserFilters;
		public DbSet<InternalUserFilterAttribute> InternalUserFilterAttributes => _context.InternalUserFilterAttributes;
		public DbSet<ApplicationClaim> ApplicationClaims => _context.ApplicationClaims;
		public DbSet<ApplicationRole> ApplicationRoles => _context.ApplicationRoles;
		public DbSet<BusinessContactRole> BusinessContactRoles => _context.BusinessContactRoles;
		#endregion

		#region Notifications
		public DbSet<NotificationType> NotificationTypes => _context.NotificationTypes;
		public DbSet<NotificationTemplate> NotificationTemplates => _context.NotificationTemplates;
		public DbSet<NotificationQueue> NotificationQueue => _context.NotificationQueue;
		public DbSet<NotificationTrigger> NotificationTriggers => _context.NotificationTriggers;
		#endregion

		#region Organizations
		public DbSet<Organization> Organizations => _context.Organizations;
		public DbSet<OrganizationType> OrganizationTypes => _context.OrganizationTypes;
		public DbSet<LegalStructure> LegalStructures => _context.LegalStructures;
		public DbSet<PrioritySector> PrioritySectors => _context.PrioritySectors;
		public DbSet<ApplicantOrganizationType> ApplicantOrganizationTypes => _context.ApplicantOrganizationTypes;
		#endregion

		#region Attachments
		public DbSet<Attachment> Attachments => _context.Attachments;
		public DbSet<VersionedAttachment> VersionedAttachments => _context.VersionedAttachments;
		public DbSet<DocumentTemplate> DocumentTemplates => _context.DocumentTemplates;
		public DbSet<Document> Documents => _context.Documents;
		public DbSet<VersionedDocument> VersionedDocuments => _context.VersionedDocuments;
		#endregion

		#region Addresses
		public DbSet<Address> Addresses => _context.Addresses;
		public DbSet<Region> Regions => _context.Regions;
		public DbSet<Country> Countries => _context.Countries;
		#endregion

		#region Administrative
		public DbSet<FiscalYear> FiscalYears => _context.FiscalYears;
		public DbSet<TrainingPeriod> TrainingPeriods => _context.TrainingPeriods;
		public DbSet<GrantProgram> GrantPrograms => _context.GrantPrograms;
		public DbSet<GrantProgramNotificationType> GrantProgramNotificationTypes => _context.GrantProgramNotificationTypes;
		public DbSet<GrantStream> GrantStreams => _context.GrantStreams;
		public DbSet<GrantOpening> GrantOpenings => _context.GrantOpenings;
		public DbSet<GrantStreamEligibilityQuestion> GrantStreamEligibilityQuestions => _context.GrantStreamEligibilityQuestions;
		public DbSet<GrantStreamEligibilityAnswer> GrantStreamEligibilityAnswers => _context.GrantStreamEligibilityAnswers;

		public DbSet<GrantOpeningFinancial> GrantOpeningFinancials => _context.GrantOpeningFinancials;
		public DbSet<GrantOpeningIntake> GrantOpeningIntakes => _context.GrantOpeningIntakes;
		public DbSet<ReportRate> ReportRates => _context.ReportRates;
		public DbSet<ProgramType> ProgramTypes => _context.ProgramTypes;
		public DbSet<RiskClassification> RiskClassifications => _context.RiskClassifications;
		public DbSet<ServiceType> ServiceTypes => _context.ServiceTypes;
		public DbSet<ServiceCategory> ServiceCategories => _context.ServiceCategories;
		public DbSet<ServiceLine> ServiceLines => _context.ServiceLines;
		public DbSet<ServiceLineBreakdown> ServiceLineBreakdowns => _context.ServiceLineBreakdowns;
		public DbSet<ProgramDescription> ProgramDescriptions => _context.ProgramDescriptions;
		public DbSet<UnderRepresentedPopulation> UnderRepresentedPopulations => _context.UnderRepresentedPopulations;
		public DbSet<VulnerableGroup> VulnerableGroups => _context.VulnerableGroups;
		public DbSet<ParticipantEmploymentStatus> ParticipantEmploymentStatuses => _context.ParticipantEmploymentStatuses;
		public DbSet<Community> Communities => _context.Communities;
		public DbSet<ExpenseType> ExpenseTypes => _context.ExpenseTypes;
		public DbSet<ProgramConfiguration> ProgramConfigurations => _context.ProgramConfigurations;
		public DbSet<ProgramNotification> ProgramNotifications => _context.ProgramNotifications;
		public DbSet<ProgramNotificationRecipient> ProgramNotificationRecipients => _context.ProgramNotificationRecipients;
		public DbSet<TrainingPeriodBudgetRate> TrainingPeriodBudgetRates => _context.TrainingPeriodBudgetRates;

		#endregion

		#region Grant Applications
		public DbSet<GrantApplication> GrantApplications => _context.GrantApplications;
		public DbSet<ApplicationType> ApplicationTypes => _context.ApplicationTypes;
		public DbSet<ApplicationAddress> ApplicationAddresses => _context.ApplicationAddresses;
		public DbSet<Note> Notes => _context.Notes;
		public DbSet<NoteType> NoteTypes => _context.NoteTypes;
		public DbSet<NaIndustryClassificationSystem> NaIndustryClassificationSystems => _context.NaIndustryClassificationSystems;
		public DbSet<GrantAgreement> GrantAgreements => _context.GrantAgreements;
		public DbSet<GrantApplicationStateChange> GrantApplicationStateChanges => _context.GrantApplicationStateChanges;
		public DbSet<GrantApplicationInternalState> GrantApplicationInternalStates => _context.GrantApplicationInternalStates;
		public DbSet<GrantApplicationExternalState> GrantApplicationExternalStates => _context.GrantApplicationExternalStates;
		public DbSet<TrainingCost> TrainingCosts => _context.TrainingCosts;
		public DbSet<EligibleCost> EligibleCosts => _context.EligibleCosts;
		public DbSet<EligibleCostBreakdown> EligibleCostBreakdowns => _context.EligibleCostBreakdowns;
		public DbSet<EligibleExpenseType> EligibleExpenseTypes => _context.EligibleExpenseTypes;
		public DbSet<EligibleExpenseBreakdown> EligibleExpenseBreakdowns => _context.EligibleExpenseBreakdowns;
		public DbSet<DenialReason> DenialReasons => _context.DenialReasons;
		public DbSet<PrioritizationScoreBreakdown> PrioritizationScoreBreakdowns => _context.PrioritizationScoreBreakdowns;
		public DbSet<PrioritizationScoreBreakdownAnswer> PrioritizationScoreBreakdownAnswers => _context.PrioritizationScoreBreakdownAnswers;

		#endregion

		#region Training Providers
		public DbSet<TrainingProvider> TrainingProviders => _context.TrainingProviders;
		public DbSet<TrainingProviderInventory> TrainingProviderInventory => _context.TrainingProviderInventory;
		public DbSet<TrainingProviderType> TrainingProviderTypes => _context.TrainingProviderTypes;
		#endregion

		#region Training Programs
		public DbSet<TrainingProgram> TrainingPrograms => _context.TrainingPrograms;
		public DbSet<UnderRepresentedGroup> UnderRepresentedGroups => _context.UnderRepresentedGroups;
		public DbSet<DeliveryMethod> DeliveryMethods => _context.DeliveryMethods;
		public DbSet<ExpectedQualification> ExpectedQualifications => _context.ExpectedQualifications;
		public DbSet<InDemandOccupation> InDemandOccupations => _context.InDemandOccupations;
		public DbSet<SkillsFocus> SkillsFocuses => _context.SkillsFocuses;
		public DbSet<SkillLevel> SkillLevels => _context.SkillLevels;
		public DbSet<TrainingLevel> TrainingLevels => _context.TrainingLevels;
		public DbSet<DeliveryPartner> DeliveryPartners => _context.DeliveryPartners;
		public DbSet<DeliveryPartnerService> DeliveryPartnerServices => _context.DeliveryPartnerServices;
		#endregion

		#region Participants
		public DbSet<ParticipantInvitation> ParticipantInvitations => _context.ParticipantInvitations;
		public DbSet<ParticipantForm> ParticipantForms => _context.ParticipantForms;
		public DbSet<Participant> Participants => _context.Participants;
		public DbSet<ParticipantCost> ParticipantCosts => _context.ParticipantCosts;
		public DbSet<NationalOccupationalClassification> NationalOccupationalClassifications => _context.NationalOccupationalClassifications;
		public DbSet<CanadianStatus> CanadianStatuses => _context.CanadianStatuses;
		public DbSet<AboriginalBand> AboriginalBands => _context.AboriginalBands;
		public DbSet<EducationLevel> EducationLevels => _context.EducationLevels;
		public DbSet<EmploymentType> EmploymentTypes => _context.EmploymentTypes;
		public DbSet<EmploymentStatus> EmploymentStatuses => _context.EmploymentStatuses;
		public DbSet<TrainingResult> TrainingResults => _context.TrainingResults;
		public DbSet<EIBenefit> EIBenefits => _context.EIBenefits;
		public DbSet<MaritalStatus> MaritalStatuses => _context.MaritalStatuses;
		public DbSet<FederalOfficialLanguage> FederalOfficialLanguages => _context.FederalOfficialLanguages;
		#endregion

		#region Claims
		public DbSet<Claim> Claims => _context.Claims;
		public DbSet<ClaimId> ClaimIds => _context.ClaimIds;
		public DbSet<ClaimEligibleCost> ClaimEligibleCosts => _context.ClaimEligibleCosts;
		public DbSet<ClaimBreakdownCost> ClaimBreakdownCosts => _context.ClaimBreakdownCosts;
		public DbSet<GrantApplicationClaimState> ClaimStates => _context.ClaimStates;
		public DbSet<ClaimType> ClaimTypes => _context.ClaimTypes;
		#endregion

		#region Misc
		public DbSet<Log> Logs => _context.Logs;
		public DbSet<TempData> TempData => _context.TempData;
		public DbSet<Setting> Settings => _context.Settings;
		public DbSet<RateFormat> RateFormats => _context.RateFormats;
		public DbSet<PrioritizationThreshold> PrioritizationThresholds => _context.PrioritizationThresholds;
		public DbSet<PrioritizationIndustryScore> PrioritizationIndustryScores => _context.PrioritizationIndustryScores;
		public DbSet<PrioritizationHighOpportunityOccupationScore> PrioritizationHighOpportunityOccupationScores => _context.PrioritizationHighOpportunityOccupationScores;
		public DbSet<PrioritizationRegion> PrioritizationRegions => _context.PrioritizationRegions;
		public DbSet<PrioritizationPostalCode> PrioritizationPostalCodes => _context.PrioritizationPostalCodes;
		#endregion

		#region Completion Report
		public DbSet<CompletionReport> CompletionReports => _context.CompletionReports;
		public DbSet<CompletionReportQuestion> CompletionReportQuestions => _context.CompletionReportQuestions;
		public DbSet<CompletionReportOption> CompletionReportOptions => _context.CompletionReportOptions;
		public DbSet<ParticipantCompletionReportAnswer> ParticipantCompletionReportAnswers => _context.ParticipantCompletionReportAnswers;
		public DbSet<CompletionReportGroup> CompletionReportGroups => _context.CompletionReportGroups;
		public DbSet<EmployerCompletionReportAnswer> EmployerCompletionReportAnswers => _context.EmployerCompletionReportAnswers;
		#endregion

		#region Payment Requests
		public DbSet<PaymentRequest> PaymentRequests => _context.PaymentRequests;
		public DbSet<PaymentRequestBatch> PaymentRequestBatches => _context.PaymentRequestBatches;
		public DbSet<AccountCode> AccountCodes => _context.AccountCodes;
		public DbSet<ReconciliationReport> ReconciliationReports => _context.ReconciliationReports;
		public DbSet<ReconciliationPayment> ReconciliationPayments => _context.ReconciliationPayments;

		public DbSet<ClaimParticipant> ClaimParticipants => _context.ClaimParticipants; // throw new NotImplementedException();
		#endregion

		#region CIPSCodes
		public DbSet<ClassificationOfInstructionalProgram> ClassificationOfInstructionalPrograms=> _context.ClassificationOfInstructionalPrograms;
		#endregion

		#endregion

		#region Constructors
		public DataContext(CJGContext context)
		{
			_context = context;
		}

		public DataContext(bool init)
		{
			_context = new CJGContext(init);
		}

		public DataContext() : this("CJG")
		{

		}

		public DataContext(string connectionString)
		{
			_context = new CJGContext(connectionString);
		}
		#endregion

		#region Methods
		public DbSet<TEntity> Set<TEntity>()
			where TEntity : class
		{
			return _context.Set<TEntity>();
		}

		public DbEntityEntry Entry(object entity)
		{
			return _context.Entry(entity);
		}

		public DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity)
			where TEntity : class
		{
			return _context.Entry<TEntity>(entity);
		}

		public TEntity Find<TEntity>(int id) where TEntity : class
		{
			return _context.Set<TEntity>().Find(id);
		}

		public IEnumerable<ValidationResult> Validate<TEntity>(TEntity entity)
			where TEntity : class
		{
			return _context.Validate(entity);
		}

		public TEntity Convert<TEntity>(object source, bool ignoreCase = false)
			where TEntity : class
		{
			return _context.Convert<TEntity>(source, ignoreCase);
		}

		public TEntity Convert<TEntity>(object source, TEntity result, bool ignoreCase = false)
			where TEntity : class
		{
			return _context.Convert(source, result, ignoreCase);
		}

		public TEntity ConvertAndValidate<TEntity>(object source, ICollection<ValidationResult> validationResults, bool ignoreCase = false)
			where TEntity : class
		{
			return _context.ConvertAndValidate<TEntity>(source, validationResults, ignoreCase);
		}

		public void ConvertAndValidate<TEntity>(object source, TEntity result, ICollection<ValidationResult> validationResults, bool ignoreCase = false)
			where TEntity : class
		{
			_context.ConvertAndValidate(source, result, validationResults, ignoreCase);
		}

		public void CopyTo<TEntity>(TEntity source, TEntity destination)
			where TEntity : class
		{
			_context.CopyTo(source, destination);
		}

		public int SaveChanges()
		{
			return _context.SaveChanges();
		}

		public IEnumerable<DbEntityValidationResult> GetValidationErrors()
		{
			return _context.GetValidationErrors();
		}

		public void Dispose()
		{
			_context.Dispose();
		}

		/// <summary>
		/// Commit the unit of work to the datasource.
		/// </summary>
		public int Commit()
		{
			if (_context.ChangeTracker.HasChanges()) {
				// Get all the updated entries and update their DateAdded or DateUpdated based on their state.
				foreach (var entry in _context.ChangeTracker.Entries().Where(e => !(new[] { EntityState.Unchanged }).Contains(e.State))) {
					UpdateRowVersion(entry);
					if (typeof(EntityBase).IsAssignableFrom(entry.Entity.GetType())) {
						switch (entry.State) {
							case (EntityState.Added):
								((EntityBase)entry.Entity).DateAdded = AppDateTime.UtcNow;
								break;
							case (EntityState.Modified):
								((EntityBase)entry.Entity).DateUpdated = AppDateTime.UtcNow;
								break;
						}
					}
				}
			}

			return _context.SaveChanges();
		}

		/// <summary>
		/// Commit the unit of work in a transaction to the datasource.
		/// </summary>
		public int CommitTransaction()
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					var result = Commit();
					transaction.Commit();
					return result;
				}
				catch
				{
					transaction.Rollback();
					throw;
				}
			}
		}

		/// <summary>
		/// Call the specified 'update' function and then save as a single transaction.
		/// </summary>
		/// <param name="update">Function to call before saving changes to DB.</param>
		/// <returns></returns>
		public int CommitTransaction(Func<int> update)
		{
			using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable)) {
				try {
					var result = update();
					_context.SaveChanges();
					transaction.Commit();
					return result;
				} catch {
					transaction.Rollback();
					throw;
				}
			}
		}

		/// <summary>
		/// We have to manually do this because we are manually setting the State.
		/// This will ensure optimistic concurrency is working.
		/// </summary>
		/// <param name="entry"></param>
		public void UpdateRowVersion(DbEntityEntry entry)
		{
			// Only entries that have been modified need to be handled.
			if (entry.State == EntityState.Modified) {
				var obj = entry.Entity as EntityBase;
				entry.OriginalValues[nameof(EntityBase.RowVersion)] = obj.RowVersion;
			}
		}

		/// <summary>
		/// Update the entity in the datasource.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
			var entry = _context.Entry(entity);
			entry.State = EntityState.Modified;
		}

		public void SetModified(object entity)
		{
			Entry(entity).State = EntityState.Modified;
		}

		/// <summary>
		/// Returns the original property value for the specified entity object.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public object OriginalValue<TEntity>(TEntity entity, string propertyName) where TEntity : class
		{
			return Entry<TEntity>(entity).OriginalValues[propertyName];
		}
		#endregion
	}
}
