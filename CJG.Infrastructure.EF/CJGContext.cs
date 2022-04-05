using CJG.Core.Entities;
using CJG.Core.Entities.Attributes;
using CJG.Infrastructure.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NLog;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Web;

namespace CJG.Infrastructure.EF
{
	/// <summary>
	/// CJGContext class, provides the DbContext for the CJG datasource.
	/// </summary>
	public class CJGContext : IdentityDbContext<ApplicationUser>
	{
		#region Variables
		private readonly ILogger _logger;
		#endregion

		#region Properties
		public HttpContextBase HttpContext { get; set; }
		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of a CJGContext object.
		/// This is only used for mocking purposes.
		/// </summary>
		/// <param name="init"></param>
		internal CJGContext(bool init)
		{
			Database.SetInitializer<CJGContext>(null);
			_logger = LogManager.GetCurrentClassLogger();
		}

		/// <summary>
		/// Creates a new instance of a CJGContext object.
		/// Initializes with the 'CJG' data connection string.
		/// </summary>
		public CJGContext() : this("CJG")
		{
			_logger = LogManager.GetCurrentClassLogger();
		}

		/// <summary>
		/// This constructor is used for integration tests to set unique localdb names for different test sessions
		/// </summary>
		/// <param name="nameOrConnectionString">Connection string or name of connection string in configuration file</param>
		internal CJGContext(string nameOrConnectionString) : base(nameOrConnectionString)
		{
			//this.Database.Log = m => System.Diagnostics.Debug.WriteLine(m);

			((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += (sender, e) => DateTimeKindAttribute.Apply(e.Entity);
			_logger = LogManager.GetCurrentClassLogger();
		}
		#endregion

		#region Properties

		#region Users
		public new virtual DbSet<User> Users { get; set; }
		public virtual DbSet<UserPreference> UserPreferences { get; set; }
		public virtual DbSet<UserGrantProgramPreference> UserGrantProgramPreferences { get; set; }
		public virtual DbSet<NotificationType> NotificationTypes { get; set; }
		public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }
		public virtual DbSet<InternalUser> InternalUsers { get; set; }
		public virtual DbSet<InternalUserFilter> InternalUserFilters { get; set; }
		public virtual DbSet<InternalUserFilterAttribute> InternalUserFilterAttributes { get; set; }
		#endregion

		#region Organizations
		public virtual DbSet<ApplicantOrganizationType> ApplicantOrganizationTypes { get; set; }
		public virtual DbSet<Organization> Organizations { get; set; }
		public virtual DbSet<OrganizationType> OrganizationTypes { get; set; }
		public virtual DbSet<LegalStructure> LegalStructures { get; set; }
		public virtual DbSet<PrioritySector> PrioritySectors { get; set; }
		#endregion

		#region Attachments
		public virtual DbSet<Attachment> Attachments { get; set; }
		public virtual DbSet<VersionedAttachment> VersionedAttachments { get; set; }

		public virtual DbSet<DocumentTemplate> DocumentTemplates { get; set; }
		public virtual DbSet<Document> Documents { get; set; }
		public virtual DbSet<VersionedDocument> VersionedDocuments { get; set; }
		#endregion

		#region Addresses
		public virtual DbSet<Address> Addresses { get; set; }
		public virtual DbSet<Region> Regions { get; set; }
		public virtual DbSet<Country> Countries { get; set; }
		#endregion

		#region Administrative
		public virtual DbSet<FiscalYear> FiscalYears { get; set; }
		public virtual DbSet<TrainingPeriod> TrainingPeriods { get; set; }
		public virtual DbSet<GrantStream> GrantStreams { get; set; }
		public virtual DbSet<GrantOpening> GrantOpenings { get; set; }
		public virtual DbSet<GrantStreamEligibilityQuestion> GrantStreamEligibilityQuestions { get; set; }
		public virtual DbSet<GrantStreamEligibilityAnswer> GrantStreamEligibilityAnswers { get; set; }
		public virtual DbSet<GrantOpeningFinancial> GrantOpeningFinancials { get; set; }
		public virtual DbSet<GrantOpeningIntake> GrantOpeningIntakes { get; set; }
		public virtual DbSet<ReportRate> ReportRates { get; set; }
		public virtual DbSet<ExpenseType> ExpenseTypes { get; set; }
		public virtual DbSet<UnderRepresentedPopulation> UnderRepresentedPopulations { get; set; }
		public virtual DbSet<VulnerableGroup> VulnerableGroups { get; set; }
		public virtual DbSet<ParticipantEmploymentStatus> ParticipantEmploymentStatuses { get; set; }
		#endregion

		#region Grant Applications
		public virtual DbSet<GrantApplication> GrantApplications { get; set; }
		public virtual DbSet<ApplicationType> ApplicationTypes { get; set; }
		public virtual DbSet<ApplicationAddress> ApplicationAddresses { get; set; }
		public virtual DbSet<Note> Notes { get; set; }
		public virtual DbSet<NoteType> NoteTypes { get; set; }
		public virtual DbSet<BusinessContactRole> BusinessContactRoles { get; set; }
		public virtual DbSet<NaIndustryClassificationSystem> NaIndustryClassificationSystems { get; set; }
		public virtual DbSet<GrantAgreement> GrantAgreements { get; set; }
		public virtual DbSet<GrantApplicationStateChange> GrantApplicationStateChanges { get; set; }
		public virtual DbSet<GrantApplicationInternalState> GrantApplicationInternalStates { get; set; }
		public virtual DbSet<GrantApplicationExternalState> GrantApplicationExternalStates { get; set; }
		public virtual DbSet<ProgramDescription> ProgramDescriptions { get; set; }
		public virtual DbSet<Community> Communities { get; set; }
		public virtual DbSet<EligibleCost> EligibleCosts { get; set; }
		public virtual DbSet<EligibleCostBreakdown> EligibleCostBreakdowns { get; set; }
		public virtual DbSet<DenialReason> DenialReasons { get; set; }
		#endregion

		#region Grant Program
		public virtual DbSet<ProgramType> ProgramTypes { get; set; }
		public virtual DbSet<GrantProgram> GrantPrograms { get; set; }
		public virtual DbSet<GrantProgramNotificationType> GrantProgramNotificationTypes { get; set; }
		public virtual DbSet<ProgramConfiguration> ProgramConfigurations { get; set; }
		public virtual DbSet<ServiceType> ServiceTypes { get; set; }
		public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
		public virtual DbSet<ServiceLine> ServiceLines { get; set; }
		public virtual DbSet<ServiceLineBreakdown> ServiceLineBreakdowns { get; set; }
		public virtual DbSet<EligibleExpenseType> EligibleExpenseTypes { get; set; }
		public virtual DbSet<EligibleExpenseBreakdown> EligibleExpenseBreakdowns { get; set; }
		public virtual DbSet<RiskClassification> RiskClassifications { get; set; }
		public virtual DbSet<TrainingCost> TrainingCosts { get; set; }
		#endregion

		#region Training Providers
		public virtual DbSet<TrainingProvider> TrainingProviders { get; set; }
		public virtual DbSet<TrainingProviderInventory> TrainingProviderInventory { get; set; }
		public virtual DbSet<TrainingProviderType> TrainingProviderTypes { get; set; }
		#endregion

		#region Training Programs
		public virtual DbSet<TrainingProgram> TrainingPrograms { get; set; }
		public virtual DbSet<UnderRepresentedGroup> UnderRepresentedGroups { get; set; }
		public virtual DbSet<DeliveryMethod> DeliveryMethods { get; set; }
		public virtual DbSet<ExpectedQualification> ExpectedQualifications { get; set; }
		public virtual DbSet<InDemandOccupation> InDemandOccupations { get; set; }
		public virtual DbSet<SkillsFocus> SkillsFocuses { get; set; }
		public virtual DbSet<SkillLevel> SkillLevels { get; set; }
		public virtual DbSet<TrainingLevel> TrainingLevels { get; set; }
		public virtual DbSet<DeliveryPartner> DeliveryPartners { get; set; }
		public virtual DbSet<DeliveryPartnerService> DeliveryPartnerServices { get; set; }
		#endregion

		#region Participants
		public virtual DbSet<ParticipantForm> ParticipantForms { get; set; }
		public virtual DbSet<Participant> Participants { get; set; }
		public virtual DbSet<ParticipantCost> ParticipantCosts { get; set; }
		public virtual DbSet<NationalOccupationalClassification> NationalOccupationalClassifications { get; set; }
		public virtual DbSet<CanadianStatus> CanadianStatuses { get; set; }
		public virtual DbSet<AboriginalBand> AboriginalBands { get; set; }
		public virtual DbSet<EducationLevel> EducationLevels { get; set; }
		public virtual DbSet<EmploymentType> EmploymentTypes { get; set; }
		public virtual DbSet<EmploymentStatus> EmploymentStatuses { get; set; }
		public virtual DbSet<TrainingResult> TrainingResults { get; set; }
		public virtual DbSet<EIBenefit> EIBenefits { get; set; }
		public virtual DbSet<MaritalStatus> MaritalStatuses { get; set; }
		public virtual DbSet<FederalOfficialLanguage> FederalOfficialLanguages { get; set; }
		#endregion

		#region Claims
		public virtual DbSet<Claim> Claims { get; set; }
		public virtual DbSet<ClaimId> ClaimIds { get; set; }
		public virtual DbSet<ClaimEligibleCost> ClaimEligibleCosts { get; set; }
		public virtual DbSet<ClaimBreakdownCost> ClaimBreakdownCosts { get; set; }
		public virtual DbSet<GrantApplicationClaimState> ClaimStates { get; set; }
		public virtual DbSet<ClaimType> ClaimTypes { get; set; }
		public virtual DbSet<ClaimParticipant> ClaimParticipants { get; set; }
		#endregion

		#region Misc
		public virtual DbSet<Log> Logs { get; set; }
		public virtual DbSet<TempData> TempData { get; set; }
		public virtual DbSet<Setting> Settings { get; set; }
		public virtual DbSet<ApplicationClaim> ApplicationClaims { get; set; }
		public virtual DbSet<ApplicationRole> ApplicationRoles { get; set; }
		public virtual DbSet<RateFormat> RateFormats { get; set; }
		#endregion

		#region Completion Report
		public virtual DbSet<CompletionReport> CompletionReports { get; set; }
		public virtual DbSet<CompletionReportQuestion> CompletionReportQuestions { get; set; }
		public virtual DbSet<CompletionReportOption> CompletionReportOptions { get; set; }
		public virtual DbSet<CompletionReportGroup> CompletionReportGroups { get; set; }
		public virtual DbSet<ParticipantCompletionReportAnswer> ParticipantCompletionReportAnswers { get; set; }
		public virtual DbSet<EmployerCompletionReportAnswer> EmployerCompletionReportAnswers { get; set; }
		#endregion

		#region Payment Requests
		public virtual DbSet<PaymentRequest> PaymentRequests { get; set; }
		public virtual DbSet<PaymentRequestBatch> PaymentRequestBatches { get; set; }
		public virtual DbSet<AccountCode> AccountCodes { get; set; }
		public virtual DbSet<ReconciliationReport> ReconciliationReports { get; set; }
		public virtual DbSet<ReconciliationPayment> ReconciliationPayments { get; set; }
		#endregion

		#region Notification
		public virtual DbSet<NotificationTrigger> NotificationTriggers { get; set; }
		public virtual DbSet<NotificationQueue> NotificationQueue { get; set; }
		public virtual DbSet<ProgramNotification> ProgramNotifications { get; set; }
		public virtual DbSet<ProgramNotificationRecipient> ProgramNotificationRecipients { get; set; }
		#endregion

		#region CIPSCodes
		public virtual DbSet<ClassificationOfInstructionalProgram> ClassificationOfInstructionalPrograms { get; set; }
		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// Configure the DbContext model objects.
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

			#region Log
			modelBuilder.Entity<Log>().Property(l => l.Level).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Logs") { Order = 1 }));
			modelBuilder.Entity<Log>().Property(l => l.DateAdded).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Logs") { Order = 2 }));
			#endregion

			#region Identity
			modelBuilder.Entity<ApplicationUser>().ToTable("ApplicationUsers")
												  .Ignore(x => x.EmailConfirmed)
												  .Ignore(x => x.TwoFactorEnabled)
												  .Ignore(x => x.PasswordHash)
												  .Ignore(x => x.PhoneNumber)
												  .Ignore(x => x.PhoneNumberConfirmed)
												  .Ignore(x => x.LockoutEnabled)
												  .Ignore(x => x.LockoutEndDateUtc)
												  .Ignore(x => x.AccessFailedCount)
												  .Property(p => p.Id).HasColumnName("ApplicationUserId");

			modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id).ToTable("ApplicationRoles");
			modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId }).ToTable("ApplicationUserRoles");
			modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId).ToTable("ApplicationUserLogins");
			modelBuilder.Entity<IdentityUserClaim>().HasKey<int>(l => l.Id).ToTable("ApplicationUserClaims");

			modelBuilder.Entity<ApplicationRole>()
				.HasMany(tp => tp.ApplicationClaims)
				.WithMany(dm => dm.ApplicationRoles)
				.Map(m =>
				{
					m.MapLeftKey("RoleId");
					m.MapRightKey("ClaimId");
					m.ToTable("ApplicationRoleClaims");
				});

			modelBuilder.Entity<Organization>()
				.HasOptional(o => o.Naics)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Organization>()
				.HasOptional(o => o.BusinessLicenseDocuments)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Organization>()
				.HasMany(ga => ga.BusinessLicenseDocuments)
				.WithMany()
				.Map(m =>
				{
					m.ToTable("OrganizationBusinessLicenseDocuments");
					m.MapLeftKey("OrganizationId");
					m.MapRightKey("BusinessLicenseDocumentId");
				});
			#endregion

			#region Address
			modelBuilder.Entity<Region>()
				.Property(p => p.Id).HasMaxLength(10);

			modelBuilder.Entity<Country>()
				.Property(p => p.Id).HasMaxLength(20);

			modelBuilder.Entity<Address>()
				.HasRequired(aa => aa.Region)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Address>()
				.HasRequired(aa => aa.Country)
				.WithMany()
				.WillCascadeOnDelete(false);
			#endregion

			#region GrantProgram
			modelBuilder.Entity<GrantProgram>()
				.HasRequired(gp => gp.ApplicantDeclarationTemplate)
				.WithMany(dt => dt.ApplicantDeclarationTemplates)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantProgram>()
				.HasRequired(gp => gp.ApplicantCoverLetterTemplate)
				.WithMany(dt => dt.ApplicantCoverLetterTemplates)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantProgram>()
				.HasRequired(gp => gp.ApplicantScheduleATemplate)
				.WithMany(dt => dt.ApplicantScheduleATemplates)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantProgram>()
				.HasRequired(gp => gp.ApplicantScheduleBTemplate)
				.WithMany(dt => dt.ApplicantScheduleBTemplates)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantProgram>()
				.HasRequired(gp => gp.ParticipantConsentTemplate)
				.WithMany(dt => dt.ParticipantConsentTemplates)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantProgram>()
				.HasRequired(gp => gp.ProgramConfiguration)
				.WithMany(pc => pc.GrantPrograms)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ProgramConfiguration>()
				.HasMany(pc => pc.EligibleExpenseTypes)
				.WithMany(eet => eet.ProgramConfigurations)
				.Map(pc =>
				{
					pc.ToTable("ProgramConfigurationEligibleExpenseTypes");
					pc.MapLeftKey("ProgramConfigurationId");
					pc.MapRightKey("EligibleExpenseTypeId");
				});
			#endregion

			#region GrantStream
			modelBuilder.Entity<GrantStream>()
				.HasRequired(gs => gs.ProgramConfiguration)
				.WithMany(pc => pc.GrantStreams)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantStream>()
				.HasMany(ga => ga.GrantStreamEligibilityQuestions)
				.WithRequired(tp => tp.GrantStream)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ReportRate>()
				.HasRequired(rr => rr.GrantStream)
				.WithMany()
				.WillCascadeOnDelete(false);
			modelBuilder.Entity<GrantStreamEligibilityQuestion>()
					.HasRequired(rr => rr.GrantStream)
					.WithMany()
					.WillCascadeOnDelete(false);
			modelBuilder.Entity<GrantStreamEligibilityAnswer>()
					.HasRequired(rr => rr.GrantApplication)
					.WithMany()
					.WillCascadeOnDelete(false);

			#endregion

			#region GrantApplication
			modelBuilder.Entity<GrantApplication>()
				.HasOptional(ga => ga.OrganizationType)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantApplication>()
				.HasRequired(ga => ga.ApplicantPhysicalAddress)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantApplication>()
				.HasOptional(ga => ga.ApplicantMailingAddress)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantApplication>()
				.HasOptional(ga => ga.OrganizationAddress)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantApplication>()
				.HasOptional(ga => ga.OrganizationLegalStructure)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantApplication>()
				.HasMany(ga => ga.TrainingProviders)
				.WithOptional(tp => tp.GrantApplication)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantApplication>()
				.HasMany(ga => ga.TrainingPrograms)
				.WithRequired(tp => tp.GrantApplication)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantApplication>()
				.HasMany(ga => ga.GrantStreamEligibilityAnswers)
				.WithRequired(tp => tp.GrantApplication)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantApplication>()
				   .HasMany(tp => tp.DeliveryPartnerServices)
				   .WithMany()
				   .Map(m =>
				   {
					   m.MapLeftKey("GrantApplicationId");
					   m.MapRightKey("DeliveryPartnerServiceId");
					   m.ToTable("GrantApplicationDeliveryPartnerServices");
				   });

			modelBuilder.Entity<GrantApplication>()
				.HasOptional(ga => ga.Assessor)
				.WithMany(ui => ui.AssessorApplications)
				.Map(m => m.MapKey("AssessorId"));

			modelBuilder.Entity<ApplicationAddress>()
				.HasRequired(aa => aa.Region)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ApplicationAddress>()
				.HasRequired(aa => aa.Country)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantApplication>()
				.HasMany(ga => ga.Attachments)
				.WithMany(a => a.GrantApplications)
				.Map(m =>
				{
					m.ToTable("GrantApplicationAttachments");
					m.MapLeftKey("GrantApplicationId");
					m.MapRightKey("AttachmentId");
				});

			modelBuilder.Entity<BusinessContactRole>()
				.HasRequired(bcr => bcr.User)
				.WithMany(u => u.BusinessContactRoles)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GrantApplication>()
				.HasMany<DenialReason>(r => r.GrantApplicationDenialReasons)
				.WithMany()
				.Map(m =>
				   {
					   m.MapLeftKey("GrantApplicationId");
					   m.MapRightKey("DenialReasonId");
					   m.ToTable("GrantApplicationDenialReasons");
				   });

			#endregion

			#region WDA Services
			modelBuilder.Entity<ServiceCategory>().ToTable("ServiceCategories");
			#endregion

			#region TrainingProgram
			modelBuilder.Entity<TrainingProgram>()
				.HasMany(tp => tp.DeliveryMethods)
				.WithMany()
				.Map(m =>
				{
					m.MapLeftKey("TrainingProgramId");
					m.MapRightKey("DeliveryMethodId");
					m.ToTable("TrainingProgramDeliveryMethods");
				});

			modelBuilder.Entity<TrainingProgram>()
				.HasMany(tp => tp.UnderRepresentedGroups)
				.WithMany()
				.Map(m =>
				{
					m.MapLeftKey("TrainingProgramId");
					m.MapRightKey("UnderRepresentedGroupId");
					m.ToTable("TrainingProgramUnderRepresentedGroups");
				});

			modelBuilder.Entity<TrainingProgram>()
				.HasMany(tp => tp.TrainingProviders)
				.WithMany(tp => tp.TrainingPrograms)
				.Map(m =>
				{
					m.ToTable("TrainingProgramTrainingProviders");
					m.MapLeftKey("TrainingProgramId");
					m.MapRightKey("TrainingProviderId");
				});

			modelBuilder.Entity<TrainingProgram>().Property(tp => tp.DateAdded).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_TrainingPrograms") { Order = 3 }));
			#endregion

			#region TrainingProvider

			modelBuilder.Entity<TrainingProvider>()
				.HasRequired(tp => tp.TrainingAddress)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<TrainingProvider>()
				.HasOptional(m => m.OriginalTrainingProvider)
				.WithMany(m => m.RequestedTrainingProviders)
				.HasForeignKey(m => m.OriginalTrainingProviderId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<TrainingProvider>().Property(tp => tp.DateAdded).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_TrainingProviders") { Order = 7 }));
			#endregion

			#region TrainingCosts

			modelBuilder.Entity<EligibleCostBreakdown>()
				.HasRequired(ecb => ecb.EligibleExpenseBreakdown)
				.WithMany(eeb => eeb.EligibleCostBreakdowns)
				.WillCascadeOnDelete(false);
			#endregion

			#region Participants
			modelBuilder.Entity<ParticipantForm>()
				.HasRequired(pf => pf.CurrentNoc)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ParticipantForm>()
				.HasRequired(pf => pf.FutureNoc)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ParticipantForm>()
				.HasRequired(pf => pf.Region)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ParticipantForm>()
				.HasRequired(pf => pf.Country)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ParticipantForm>()
				.HasMany(ga => ga.EligibleCostBreakdowns)
				.WithMany(a => a.ParticipantForms)
				.Map(m =>
				{
					m.ToTable("ParticipantFormEligibleCostBreakdowns");
					m.MapLeftKey("ParticipantFormId");
					m.MapRightKey("EligibleCostBreakdownId");
				});

			modelBuilder.Entity<MaritalStatus>()
				.ToTable("MartialStatus");  // Fixing Typo but retaining table name
			#endregion

			#region Claim
			modelBuilder.Entity<Claim>()
				.HasMany(c => c.Receipts)
				.WithMany()
				.Map(m =>
				{
					m.MapLeftKey("ClaimId", "ClaimVersion");
					m.MapRightKey("AttachmentId");
					m.ToTable("ClaimReceipts");
				});

			modelBuilder.Entity<Claim>()
				.HasRequired(c => c.GrantApplication)
				.WithMany(tp => tp.Claims)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ClaimBreakdownCost>()
				.HasRequired(cbc => cbc.EligibleExpenseBreakdown)
				.WithMany(eeb => eeb.ClaimBreakdownCosts)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Claim>()
				.HasMany(c => c.ParticipantForms)
				.WithMany(pf => pf.Claims)
				.Map(m =>
				{
					m.ToTable("ClaimParticipants");
					m.MapLeftKey("ClaimId", "ClaimVersion");
					m.MapRightKey("ParticipantFormId");
				});
			#endregion

			#region Documents
			modelBuilder.Entity<VersionedDocument>()
				.HasOptional(vd => vd.DocumentTemplate)
				.WithMany()
				.WillCascadeOnDelete(false);
			#endregion

			#region Notifications
			modelBuilder.Entity<NotificationQueue>()
				.ToTable("NotificationQueue");

			modelBuilder.Entity<NotificationType>()
				.HasRequired(n => n.NotificationTemplate)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<NotificationType>()
				.HasRequired(n => n.NotificationTrigger)
				.WithMany(t => t.NotificationTypes)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<NotificationType>()
				.HasMany(n => n.NotificationQueue)
				.WithOptional(q => q.NotificationType)
				.WillCascadeOnDelete(false);
			#endregion

			#region Completion Report
			modelBuilder.Entity<CompletionReportOption>()
				.HasRequired(cro => cro.Question)
				.WithMany(u => u.Options)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ParticipantCompletionReportAnswer>()
			.HasMany<CompletionReportOption>(a => a.MultAnswers)
				.WithMany()
				.Map(m =>
				{
					m.MapLeftKey(new[] { "ParticipantFormId", "GrantApplicationId", "ParticipantCompletionAnswerId" });
					m.MapRightKey("CompletionReportOptionId");
					m.ToTable("ParticipantCompletionReportMultAnswers");
				});

			modelBuilder.Entity<GrantApplication>()
				.HasRequired(ga => ga.CompletionReport)
				.WithMany(cr => cr.GrantApplications)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<CompletionReportGroup>()
				.HasRequired(crg => crg.CompletionReport)
				.WithMany(cr => cr.Groups)
				.WillCascadeOnDelete(false);
			#endregion

			#region Payment Requests
			modelBuilder.Entity<PaymentRequestBatch>()
				.HasRequired(pr => pr.GrantProgram)
				.WithMany(gp => gp.PaymentRequestBatches)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ReconciliationReport>()
				.HasMany(rr => rr.Payments)
				.WithMany(rp => rp.Reports)
				.Map(m =>
				{
					m.MapLeftKey("ReconciliationReportId");
					m.MapRightKey("ReconciliationPaymentId");
					m.ToTable("ReconciliationReportPayments");
				});

			modelBuilder.Entity<ReconciliationPayment>()
				.HasOptional(rp => rp.PaymentRequest)
				.WithMany(pr => pr.ReconciliationPayments)
				.HasForeignKey(rp => new { rp.PaymentRequestBatchId, rp.ClaimId, rp.ClaimVersion })
				.WillCascadeOnDelete(false);
			#endregion

			#region ProgramDescription
			modelBuilder.Entity<ProgramDescription>()
				.HasMany<Community>(p => p.Communities)
				.WithMany()
				.Map(pc =>
				{
					pc.MapLeftKey(nameof(ProgramDescription.GrantApplicationId));
					pc.MapRightKey($"{nameof(Community)}{nameof(Community.Id)}");
					pc.ToTable("ProgramDescriptionCommunities");
				});

			modelBuilder.Entity<ProgramDescription>()
				.HasMany<VulnerableGroup>(p => p.VulnerableGroups)
				.WithMany()
				.Map(pc =>
				{
					pc.MapLeftKey(nameof(ProgramDescription.GrantApplicationId));
					pc.MapRightKey($"{nameof(VulnerableGroup)}{nameof(VulnerableGroup.Id)}");
					pc.ToTable("ProgramDescriptionVulnerableGroups");
				});

			modelBuilder.Entity<ProgramDescription>()
				.HasMany<UnderRepresentedPopulation>(p => p.UnderRepresentedPopulations)
				.WithMany()
				.Map(pc =>
				{
					pc.MapLeftKey(nameof(ProgramDescription.GrantApplicationId));
					pc.MapRightKey($"{nameof(UnderRepresentedPopulation)}{nameof(UnderRepresentedPopulation.Id)}");
					pc.ToTable("ProgramDescriptionUnderRepresentedPopulations");
				});

			modelBuilder.Entity<ProgramDescription>()
				.HasMany<ParticipantEmploymentStatus>(pd => pd.ParticipantEmploymentStatuses)
				.WithMany()
				.Map(m =>
				{
					m.MapLeftKey(nameof(ProgramDescription.GrantApplicationId));
					m.MapRightKey(nameof(ParticipantEmploymentStatus.Id));
					m.ToTable("ApplicationParticipantEmploymentStatuses");
				});

			#endregion

			#region Configuration
			modelBuilder.Entity<Community>().ToTable("Communities");
			#endregion

			base.OnModelCreating(modelBuilder);
		}

		/// <summary>
		/// To improve validation we need to pass the current <typeparamref name="DbContext"/> and the <typeparamref name="DbEntityEntry"/> object to the Validate method.
		/// </summary>
		/// <param name="entityEntry"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
		{
			items["DbContext"] = new DataContext(this);
			items["Entry"] = entityEntry;
			items["Logger"] = _logger;
			items["HttpContext"] = this.HttpContext;
			return base.ValidateEntity(entityEntry, items);
		}

		public static CJGContext Create()
		{
			return new CJGContext(false);
		}
		#endregion
	}
}