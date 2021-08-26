using CJG.Application.Business.Models.DocumentTemplate;
using CJG.Application.Services.Helpers;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using NLog;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="GrantProgramService"/> class, provides a way to manage grant programs.
	/// </summary>
	public class GrantProgramService : Service, IGrantProgramService
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly ApplicationUserManager _applicationUserManager;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantProgramService"/> object.
		/// </summary>
		/// <param name="applicationUserManager"></param>
		/// <param name="staticDataService"></param>
		/// <param name="dbContext"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public GrantProgramService(
			ApplicationUserManager applicationUserManager,
			IStaticDataService staticDataService,
			IDataContext dbContext,
			HttpContextBase httpContext,
			ILogger logger
			) : base(dbContext, httpContext, logger)
		{
			_applicationUserManager = applicationUserManager;
			_staticDataService = staticDataService;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the grant program for the specified 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public GrantProgram Get(int id)
		{
			return Get<GrantProgram>(id);
		}

		/// <summary>
		/// GEt a list of all the grant programs.
		/// </summary>
		/// <param name="state">State filter.</param>
		/// <returns></returns>
		public IEnumerable<GrantProgram> GetAll(GrantProgramStates? state = null)
		{
			return _dbContext.GrantPrograms.Where(gp => state == null || gp.State == state).ToArray();
		}

		/// <summary>
		/// Get an array of grant programs that have grant openings in the specified fiscal year.
		/// </summary>
		/// <param name="year"></param>
		/// <returns></returns>
		public IEnumerable<GrantProgram> GetForFiscalYear(int? year)
		{
			var programIds = _dbContext.GrantOpenings.Where(x => x.TrainingPeriod.FiscalYearId == year).Select(x => x.GrantStream.GrantProgramId).ToArray();
			return _dbContext.GrantPrograms.Where(x => year == null || programIds.Contains(x.Id)).OrderBy(gp => gp.Name).ToArray();
		}

		/// <summary>
		/// Get an array of grant programs that are implemented and have active grant streams.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<GrantProgram> GetWithActiveStreams()
		{
			return _dbContext.GrantPrograms.Where(gp => gp.GrantStreams.Any(gs => gs.IsActive) && gp.State == GrantProgramStates.Implemented).ToArray();
		}

		/// <summary>
		/// Get the current users prefered grant programs.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<GrantProgram> GetUserProgramPreferences()
		{
			var userId = _httpContext.User.GetUserId().Value;
			return (from gpp in _dbContext.UserGrantProgramPreferences
					where gpp.IsSelected
						&& gpp.GrantProgram.State == GrantProgramStates.Implemented
						&& gpp.UserId == userId
					select gpp.GrantProgram).ToArray();
		}

		/// <summary>
		/// Add the specified grant program to the datasource.
		/// By default it will assign the first document templates in the datasource.
		/// By default it will assign the first claim type in the datasource.
		/// </summary>
		/// <param name="grantProgram"></param>
		/// <returns></returns>
		public GrantProgram Add(GrantProgram grantProgram)
		{
			grantProgram.State = GrantProgramStates.NotImplemented;

			if (grantProgram.ApplicantDeclarationTemplate == null)
				grantProgram.ApplicantDeclarationTemplate = new DocumentTemplate()
				{
					DocumentType = DocumentTypes.ApplicantDeclaration,
					Title = $"{grantProgram.Name} - Applicant Declaration",
					Body = "Default",
					IsActive = true
				};

			if (grantProgram.ApplicantCoverLetterTemplate == null)
				grantProgram.ApplicantCoverLetterTemplate = new DocumentTemplate()
				{
					DocumentType = DocumentTypes.GrantAgreementCoverLetter,
					Title = $"{grantProgram.Name} - Cover Letter",
					Body = "Default",
					IsActive = true
				};

			if (grantProgram.ApplicantScheduleATemplate == null)
				grantProgram.ApplicantScheduleATemplate = new DocumentTemplate()
				{
					DocumentType = DocumentTypes.GrantAgreementScheduleA,
					Title = $"{grantProgram.Name} - Schedule A",
					Body = "Default",
					IsActive = true
				};

			if (grantProgram.ApplicantScheduleBTemplate == null)
				grantProgram.ApplicantScheduleBTemplate = new DocumentTemplate()
				{
					DocumentType = DocumentTypes.GrantAgreementScheduleB,
					Title = $"{grantProgram.Name} - Schedule B",
					Body = "Default",
					IsActive = true
				};

			if (grantProgram.ParticipantConsentTemplate == null)
				grantProgram.ParticipantConsentTemplate = new DocumentTemplate()
				{
					DocumentType = DocumentTypes.ParticipantConsent,
					Title = $"{grantProgram.Name} - Participant Consent",
					Body = "Default",
					IsActive = true
				};

			if (grantProgram.ProgramConfiguration == null)
			{
				grantProgram.ProgramConfiguration = grantProgram.ProgramConfigurationId > 0
					? Get<ProgramConfiguration>(grantProgram.ProgramConfigurationId)
					: new ProgramConfiguration(grantProgram.Name, ClaimTypes.MultipleClaimsWithoutAmendments);
			}

			_dbContext.GrantPrograms.Add(grantProgram);
			_dbContext.CommitTransaction();

			return grantProgram;
		}

		/// <summary>
		/// Update the specified grant program in the datasource.
		/// </summary>
		/// <param name="grantProgram"></param>
		/// <returns></returns>
		public GrantProgram Update(GrantProgram grantProgram)
		{
			// Add new notification type template
			if (grantProgram.GrantProgramNotificationTypes.Any(nt => nt.NotificationTemplate.Id == 0))
			{
				foreach (var notificationType in grantProgram.GrantProgramNotificationTypes.Where(nt => nt.NotificationTemplate.Id == 0))
				{
					_dbContext.NotificationTemplates.Add(notificationType.NotificationTemplate);
				}
			}

			_dbContext.Update<GrantProgram>(grantProgram);
			_dbContext.CommitTransaction();

			return grantProgram;
		}

		/// <summary>
		/// Delete the specified grant program from the datasource.
		/// </summary>
		/// <param name="grantProgram"></param>
		public void Delete(GrantProgram grantProgram)
		{
			if (_dbContext.GrantStreams.Any(t => t.GrantProgramId == grantProgram.Id))
			{
				throw new InvalidOperationException();
			}

			if (grantProgram.AccountCodeId.HasValue)
			{
				var accountCodeId = grantProgram.AccountCodeId.Value;
				var accountCode = _dbContext.AccountCodes.Where(t => t.Id == accountCodeId).FirstOrDefault();
				_dbContext.AccountCodes.Remove(accountCode);
			}


			if (_dbContext.GrantPrograms.Count(t => t.ApplicantDeclarationTemplateId == grantProgram.ApplicantDeclarationTemplateId
			&& t.Id != grantProgram.Id) == 0)
			{
				var template = _dbContext.DocumentTemplates.Find(grantProgram.ApplicantDeclarationTemplateId);
				_dbContext.DocumentTemplates.Remove(template);
			}

			if (_dbContext.GrantPrograms.Count(t => t.ApplicantCoverLetterTemplateId == grantProgram.ApplicantCoverLetterTemplateId
		  && t.Id != grantProgram.Id) == 0)
			{
				var template = _dbContext.DocumentTemplates.Find(grantProgram.ApplicantCoverLetterTemplateId);
				_dbContext.DocumentTemplates.Remove(template);
			}

			if (_dbContext.GrantPrograms.Count(t => t.ApplicantScheduleATemplateId == grantProgram.ApplicantScheduleATemplateId
		  && t.Id != grantProgram.Id) == 0)
			{
				var template = _dbContext.DocumentTemplates.Find(grantProgram.ApplicantScheduleATemplateId);
				_dbContext.DocumentTemplates.Remove(template);
			}

			if (_dbContext.GrantPrograms.Count(t => t.ApplicantScheduleBTemplateId == grantProgram.ApplicantScheduleBTemplateId
		  && t.Id != grantProgram.Id) == 0)
			{
				var template = _dbContext.DocumentTemplates.Find(grantProgram.ApplicantScheduleBTemplateId);
				_dbContext.DocumentTemplates.Remove(template);
			}

			if (_dbContext.GrantPrograms.Count(t => t.ParticipantConsentTemplateId == grantProgram.ParticipantConsentTemplateId
		&& t.Id != grantProgram.Id) == 0)
			{
				var template = _dbContext.DocumentTemplates.Find(grantProgram.ParticipantConsentTemplateId);
				_dbContext.DocumentTemplates.Remove(template);
			}


			var notifications = _dbContext.GrantProgramNotificationTypes.Where(t => t.GrantProgramId == grantProgram.Id).ToList();
			notifications.ForEach(t => _dbContext.GrantProgramNotificationTypes.Remove(t));

			var program = _dbContext.GrantPrograms.Find(grantProgram.Id);
			_dbContext.GrantPrograms.Remove(program);

			CommitTransaction();
		}

		/// <summary>
		/// Implement the specified grant program.
		/// </summary>
		/// <param name="grantProgram"></param>
		/// <returns></returns>
		public void Implement(GrantProgram grantProgram)
		{
			if (grantProgram.State == GrantProgramStates.Implemented)
			{
				throw new InvalidOperationException($"A Grant Program cannot be implemented with the state '{grantProgram.State.GetDescription()}'.");
			}

			if (_dbContext.DocumentTemplates
				.Where(t => t.Id == grantProgram.ApplicantDeclarationTemplateId
				|| t.Id == grantProgram.ApplicantCoverLetterTemplateId
				|| t.Id == grantProgram.ApplicantScheduleATemplateId
				|| t.Id == grantProgram.ApplicantScheduleBTemplateId
				|| t.Id == grantProgram.ParticipantConsentTemplateId)
				.Any(t => t.IsActive != true))
			{
				throw new InvalidOperationException("A Grant Program cannot be implemented unless all document templates associated to it are active.");
			}

			if (grantProgram.ShowMessage && string.IsNullOrEmpty(grantProgram.Message))
			{
				throw new InvalidOperationException("A Grant Program cannot be implemented if the home page message is empty.");
			}

			grantProgram.State = GrantProgramStates.Implemented;
			grantProgram.DateImplemented = AppDateTime.UtcNow;
			_dbContext.Update<GrantProgram>(grantProgram);
			CommitTransaction();
		}

		/// <summary>
		/// Terminate the specified grant program.
		/// </summary>
		/// <param name="grantProgram"></param>
		/// <returns></returns>
		public void Terminate(GrantProgram grantProgram)
		{
			if (grantProgram.State == GrantProgramStates.NotImplemented)
			{
				throw new InvalidOperationException($"A Grant Program cannot be terminated with the state '{grantProgram.State.GetDescription()}'.");
			}

			// If there are any active Grant Streams associated with the Grant Program.
			if (_dbContext.GrantStreams.Where(t => t.GrantProgramId == grantProgram.Id && t.IsActive).Any())
			{
				throw new InvalidOperationException("A Grant Program cannot be terminated if there are active Grant Streams.");
			}

			grantProgram.State = GrantProgramStates.NotImplemented;
			_dbContext.Update<GrantProgram>(grantProgram);
			CommitTransaction();
		}

		/// <summary>
		/// Get a list of all valid users who can be expense authorities.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<InternalUser> GetExpenseAuthorities()
		{
			string[] includePrivileges = { "AM3" };

			var roleIds = _dbContext.ApplicationClaims.Where(
					c => c.ClaimType == AppClaimTypes.Privilege && includePrivileges.Any(i => i.Equals(c.ClaimValue)))
				.SelectMany(x => x.ApplicationRoles)
				.Select(x => x.Id)
				.Distinct();

			return _applicationUserManager.Users.Where(u => u.Roles.Any(r => roleIds.Contains(r.RoleId))).OrderBy(u => u.InternalUser.LastName).ThenBy(u => u.InternalUser.FirstName).Select(a => a.InternalUser).AsEnumerable();
		}

		/// <summary>
		/// Get the report rates for the specified grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		public IEnumerable<ReportRate> GetReportRates(int grantProgramId)
		{
			return _dbContext.ReportRates.Where(rr => rr.GrantProgramId == grantProgramId);
		}

		#region Templates by Grant Program
		/// <summary>
		/// Generates the document text for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public string GenerateApplicantDeclarationBody(GrantApplication grantApplication)
		{
			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantDeclarationTemplate == null)
				throw new InvalidOperationException($"Cannot find applicant declartion document template '{DocumentTypes.ApplicantDeclaration}'.");

			GrantApplicationTemplateModel model = new GrantApplicationTemplateModel(grantApplication, SetupGrantApplicationModel);

			return ParseDocumentTemplate(model, grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantDeclarationTemplate.Body);
		}

		/// <summary>
		/// Generates the document for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public Document GenerateApplicantDeclaration(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (String.IsNullOrEmpty(grantApplication.FileNumber))
				throw new ArgumentNullException(nameof(grantApplication), "The FileNumber cannot be null or empty.");

			var docTemplate = grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantDeclarationTemplate;
			var body = GenerateApplicantDeclarationBody(grantApplication);
			return new Document($"Agreement Applicant Declaration for {grantApplication.FileNumber}", body, docTemplate);
		}

		/// <summary>
		/// Generates the document text for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public string GenerateApplicantCoverLetterBody(GrantApplication grantApplication)
		{
			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantCoverLetterTemplate == null)
				throw new InvalidOperationException($"Cannot find grant agreement cover letter document template '{DocumentTypes.GrantAgreementCoverLetter}'.");

			GrantApplicationTemplateModel model = new GrantApplicationTemplateModel(grantApplication, SetupGrantApplicationModelWithReimbursementRate);

			return ParseDocumentTemplate(model, grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantCoverLetterTemplate.Body);
		}

		/// <summary>
		/// Generates the document for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public Document GenerateApplicantCoverLetter(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (String.IsNullOrEmpty(grantApplication.FileNumber))
				throw new ArgumentNullException(nameof(grantApplication), "The FileNumber cannot be null or empty.");

			var docTemplate = grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantCoverLetterTemplate;
			var body = GenerateApplicantCoverLetterBody(grantApplication);
			return new Document($"Agreement Cover Letter for {grantApplication.FileNumber}", body, docTemplate);
		}

		/// <summary>
		/// Generates the document text for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public string GenerateAgreementScheduleABody(GrantApplication grantApplication)
		{
			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantScheduleATemplate == null)
				throw new InvalidOperationException($"Cannot find grant agreement schedule A document template '{DocumentTypes.GrantAgreementScheduleA}'.");

			GrantApplicationTemplateModel model = new GrantApplicationTemplateModel(grantApplication, SetupGrantApplicationModelWithReimbursementRate);

			return ParseDocumentTemplate(model, grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantScheduleATemplate.Body);
		}

		/// <summary>
		/// Generates the document for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public Document GenerateAgreementScheduleA(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (String.IsNullOrEmpty(grantApplication.FileNumber))
				throw new ArgumentNullException(nameof(grantApplication), "The FileNumber cannot be null or empty.");

			var docTemplate = grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantScheduleATemplate;
			var body = GenerateAgreementScheduleABody(grantApplication);
			return new Document($"Agreement Schedule A for {grantApplication.FileNumber}", body, docTemplate);
		}

		/// <summary>
		/// Generates the document text for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public string GenerateAgreementScheduleBBody(GrantApplication grantApplication)
		{
			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantScheduleBTemplate == null)
				throw new InvalidOperationException($"Cannot find grant agreement schedule B document template '{DocumentTypes.GrantAgreementScheduleB}'.");

			GrantApplicationTemplateModel model = new GrantApplicationTemplateModel(grantApplication, SetupGrantApplicationModelWithReimbursementRate);

			return ParseDocumentTemplate(model, grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantScheduleBTemplate.Body);
		}

		/// <summary>
		/// Generates the document for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public Document GenerateAgreementScheduleB(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (String.IsNullOrEmpty(grantApplication.FileNumber))
				throw new ArgumentNullException(nameof(grantApplication), "The FileNumber cannot be null or empty.");

			var docTemplate = grantApplication.GrantOpening.GrantStream.GrantProgram.ApplicantScheduleBTemplate;
			var body = GenerateAgreementScheduleBBody(grantApplication);
			return new Document($"Agreement Schedule B for {grantApplication.FileNumber}", body, docTemplate);
		}

		/// <summary>
		/// Generates the document text for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public string GenerateParticipantConsentBody(GrantApplication grantApplication)
		{
			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ParticipantConsentTemplate == null)
				throw new InvalidOperationException($"Cannot find participant consent document template '{DocumentTypes.ParticipantConsent}'.");

			GrantApplicationTemplateModel model = new GrantApplicationTemplateModel(grantApplication, SetupGrantApplicationModel);

			return ParseDocumentTemplate(model, grantApplication.GrantOpening.GrantStream.GrantProgram.ParticipantConsentTemplate.Body);
		}

		/// <summary>
		/// Generates the document for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public Document GenerateParticipantConsent(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (String.IsNullOrEmpty(grantApplication.FileNumber))
				throw new ArgumentNullException(nameof(grantApplication), "The FileNumber cannot be null or empty.");

			var docTemplate = grantApplication.GrantOpening.GrantStream.GrantProgram.ParticipantConsentTemplate;
			var body = GenerateParticipantConsentBody(grantApplication);
			return new Document($"Agreement Participant Consent for {grantApplication.FileNumber}", body, docTemplate);
		}

		/// <summary>
		/// Validates all the template to determine if they will correctly be parsed.
		/// </summary>
		/// <param name="grantProgram"></param>
		public void ValidateTemplates(GrantProgram grantProgram)
		{
			if (grantProgram == null) throw new ArgumentNullException(nameof(grantProgram));

			ValidateTemplate(grantProgram, DocumentTypes.ApplicantDeclaration);
			ValidateTemplate(grantProgram, DocumentTypes.GrantAgreementCoverLetter);
			ValidateTemplate(grantProgram, DocumentTypes.GrantAgreementScheduleA);
			ValidateTemplate(grantProgram, DocumentTypes.GrantAgreementScheduleB);
			ValidateTemplate(grantProgram, DocumentTypes.ParticipantConsent);
		}

		public string ValidateTemplate(GrantProgram grantProgram, DocumentTypes documentType)
		{
			// Grab a random grant application, or create one if one does not exist.
			var grantApplication = _dbContext.GrantApplications.OrderBy(ga => ga.DateSubmitted).FirstOrDefault(ga => ga.GrantOpening.GrantStream.GrantProgramId == grantProgram.Id && (ga.ApplicationStateInternal >= ApplicationStateInternal.CancelledByMinistry || ga.ApplicationStateInternal == ApplicationStateInternal.OfferIssued || ga.ApplicationStateInternal == ApplicationStateInternal.AgreementAccepted)) ?? _dbContext.CreateTestApplication(grantProgram);
			var model = new GrantApplicationTemplateModel(grantApplication, SetupGrantApplicationModel);

			try
			{
				switch (documentType)
				{
					case (DocumentTypes.ApplicantDeclaration):
						return ParseDocumentTemplate(model, grantProgram.ApplicantDeclarationTemplate.Body);
					case (DocumentTypes.GrantAgreementCoverLetter):
						return ParseDocumentTemplate(model, grantProgram.ApplicantCoverLetterTemplate.Body);
					case (DocumentTypes.GrantAgreementScheduleA):
						return ParseDocumentTemplate(model, grantProgram.ApplicantScheduleATemplate.Body);
					case (DocumentTypes.GrantAgreementScheduleB):
						return ParseDocumentTemplate(model, grantProgram.ApplicantScheduleBTemplate.Body);
					case (DocumentTypes.ParticipantConsent):
						return ParseDocumentTemplate(model, grantProgram.ParticipantConsentTemplate.Body);
				}
			}
			catch (TemplateCompilationException ex)
			{
				throw new TemplateException(documentType, ex);
			}
			catch (TemplateParsingException ex)
			{
				throw new TemplateException(documentType, ex);
			}

			return null;
		}

		private void SetupGrantApplicationModel(GrantApplicationTemplateModel model, GrantApplication grantApplication)
		{
			model.BaseURL = _httpContext?.Request?.Url.GetLeftPart(UriPartial.Authority) ?? "EMPTY";
		}

		private void SetupGrantApplicationModelWithReimbursementRate(GrantApplicationTemplateModel model, GrantApplication grantApplication)
		{
			var reimbursementRate = grantApplication.ReimbursementRate;
			var rateFormat = _staticDataService.GetRateFormat(reimbursementRate);

			model.ReimbursementRate = (rateFormat == null) ? (reimbursementRate * 100).ToString("0.##\\%") : rateFormat.Format;
			SetupGrantApplicationModel(model, grantApplication);
		}
		#endregion

		#region Program Configuration Expense Types
		/// <summary>
		/// Add the eligible expense type to the specified grant program.
		/// </summary>
		/// <param name="grantProgramid"></param>
		/// <param name="eligibleExpenseType"></param>
		/// <returns></returns>
		public EligibleExpenseType AddEligibleExpenseType(int grantProgramid, EligibleExpenseType eligibleExpenseType)
		{
			var config = Get<GrantProgram>(grantProgramid).ProgramConfiguration;
			eligibleExpenseType.ProgramConfigurations.Add(config);
			_dbContext.EligibleExpenseTypes.Add(eligibleExpenseType);

			_dbContext.CommitTransaction();
			return eligibleExpenseType;
		}

		/// <summary>
		/// Get an array of all the active eligible expense types in the specified grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		public IEnumerable<EligibleExpenseType> GetAllActiveEligibleExpenseTypes(int grantProgramId)
		{
			return Get<GrantProgram>(grantProgramId).ProgramConfiguration.EligibleExpenseTypes.Where(t => t.IsActive).ToArray();
		}

		/// <summary>
		/// Get an array of all the auto included active eligible expense types in the specified grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		public IEnumerable<EligibleExpenseType> GetAutoIncludeActiveEligibleExpenseTypes(int grantProgramId)
		{
			return Get<GrantProgram>(grantProgramId).ProgramConfiguration.EligibleExpenseTypes.Where(t => t.IsActive && t.AutoInclude).ToArray();
		}

		/// <summary>
		/// Get an array of all the eligible expense types within the specified grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		public IEnumerable<EligibleExpenseType> GetAllEligibleExpenseTypes(int grantProgramId)
		{
			return Get<GrantProgram>(grantProgramId).ProgramConfiguration.EligibleExpenseTypes.ToArray();
		}
		#endregion

		/// <summary>
		/// Count the number of applicants with applications in the specified grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		public int CountApplicantsWithApplications(int grantProgramId)
		{
			return (
				from bcr in _dbContext.BusinessContactRoles
				join ga in _dbContext.GrantApplications on bcr.GrantApplicationId equals ga.Id
				join go in _dbContext.GrantOpenings on ga.GrantOpeningId equals go.Id
				join gs in _dbContext.GrantStreams on go.GrantStreamId equals gs.Id
				where gs.GrantProgramId == grantProgramId
				select bcr.UserId
				).Distinct().Count();
		}

		/// <summary>
		/// Count the number of applicants interested in the specified grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		public int CountSubscribedApplicants(int grantProgramId)
		{
			return (
				from ugpp in _dbContext.UserGrantProgramPreferences
				where ugpp.GrantProgramId == grantProgramId
				select ugpp.UserId
				).Distinct().Count();
		}

		/// <summary>
		/// Get an array of email addresses for applicant recipients that have applications in the specified grant programs.
		/// </summary>
		/// <param name="grantProgramIds"></param>
		/// <returns></returns>
		public IEnumerable<string> GetApplicantEmailsFor(int[] grantProgramIds)
		{
			if (grantProgramIds == null) throw new ArgumentNullException(nameof(grantProgramIds));

			return (
				from u in _dbContext.Users
				join bcr in _dbContext.BusinessContactRoles on u.Id equals bcr.UserId
				join ga in _dbContext.GrantApplications on bcr.GrantApplicationId equals ga.Id
				join go in _dbContext.GrantOpenings on ga.GrantOpeningId equals go.Id
				join gs in _dbContext.GrantStreams on go.GrantStreamId equals gs.Id
				where grantProgramIds.Contains(gs.GrantProgramId)
				select u.EmailAddress
				).Distinct().ToArray();
		}

		/// <summary>
		/// Get an array of email addresses for the applicants who have subscribed to the specified grant programs.
		/// </summary>
		/// <param name="grantProgramIds"></param>
		/// <returns></returns>
		public IEnumerable<string> GetSubscriberEmailsFor(int[] grantProgramIds)
		{
			if (grantProgramIds == null) throw new ArgumentNullException(nameof(grantProgramIds));
			return (
				from u in _dbContext.Users
				join ugpp in _dbContext.UserGrantProgramPreferences on u.Id equals ugpp.UserId
				where grantProgramIds.Contains(ugpp.GrantProgramId)
				select u.EmailAddress
				).Distinct().ToArray();
		}
		#endregion
	}
}