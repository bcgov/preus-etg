using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class GrantAgreementService : Service, IGrantAgreementService
	{
		private readonly IGrantProgramService _grantProgramService;
		private readonly INotificationService _notificationService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IPrioritizationService _prioritizationService;
		private readonly INoteService _noteService;
		private readonly IUserService _userService;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="grantProgramService"></param>
		/// <param name="notificationService"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="prioritizationService"></param>
		/// <param name="noteService"></param>
		/// <param name="userService"></param>
		/// <param name="dbContext"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public GrantAgreementService(
			IGrantProgramService grantProgramService,
			INotificationService notificationService,
			IGrantOpeningService grantOpeningService,
			IPrioritizationService prioritizationService,
			INoteService noteService,
			IUserService userService,
			IDataContext dbContext,
			HttpContextBase httpContext,
			ILogger logger) : base(dbContext, httpContext, logger)
		{
			_grantProgramService = grantProgramService;
			_notificationService = notificationService;
			_grantOpeningService = grantOpeningService;
			_prioritizationService = prioritizationService;
			_noteService = noteService;
			_userService = userService;
		}

		public GrantAgreement Get(int id)
		{
			var agreement = Get<GrantAgreement>(id);

			if (!_httpContext.User.CanPerformAction(agreement.GrantApplication, ApplicationWorkflowTrigger.ViewApplication))
				throw new NotAuthorizedException($"User does not have permission to access application {id}.");

			return agreement;
		}

		public GrantAgreement Add(GrantAgreement grantAgreement)
		{
			if (grantAgreement == null)
				throw new ArgumentNullException(nameof(grantAgreement));

			if (grantAgreement.GrantApplication == null && grantAgreement.GrantApplicationId == 0)
				throw new ArgumentNullException(nameof(grantAgreement), "The Grant Agreement must be associated with a Grant Application.");

			_dbContext.GrantAgreements.Add(grantAgreement);
			_dbContext.Commit();

			return grantAgreement;
		}

		public GrantAgreement Update(GrantAgreement grantAgreement)
		{
			if (grantAgreement == null)
				throw new ArgumentNullException(nameof(grantAgreement));

			_dbContext.Update<GrantAgreement>(grantAgreement);
			_dbContext.Commit();

			return grantAgreement;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ApplicationWorkflowStateMachine"/> object.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		private ApplicationWorkflowStateMachine CreateWorkflowStateMachine(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			return new ApplicationWorkflowStateMachine(grantApplication, _dbContext, _notificationService, this, _grantOpeningService, _prioritizationService, _noteService, _userService, _httpContext, _logger);
		}

		/// <summary>
		/// Get the first active template for the specified document template.
		/// </summary>
		/// <param name="documentType"></param>
		/// <returns></returns>
		public DocumentTemplate GetDocumentTemplate(DocumentTypes documentType)
		{
			return _dbContext.DocumentTemplates.OrderByDescending(dt => dt.DateAdded).FirstOrDefault(dt => dt.IsActive && dt.DocumentType == documentType);
		}

		/// <summary>
		/// Check if the agreement needs to be regenerated.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public bool AgreementUpdateRequired(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			var tracker = new EntityChanges(_dbContext.Context);
			return grantApplication.AgreementUpdateRequired(tracker);
		}

		/// <summary>
		/// Update the agreement details and documents.
		/// This does not update the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		public void UpdateAgreement(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (grantApplication.GrantAgreement == null)
				throw new ArgumentNullException(nameof(grantApplication), "The Grant Application must have a Grant Agreement before generating Schedule documents.");

			grantApplication.GrantAgreement.EndDate = grantApplication.EndDate.AddDays(60);
			var firstTrainingProgram = grantApplication.TrainingPrograms.OrderBy(tp => tp.StartDate).FirstOrDefault();
			grantApplication.GrantAgreement.ParticipantReportingDueDate = firstTrainingProgram.StartDate.AddDays(-5);
			grantApplication.GrantAgreement.ReimbursementClaimDueDate = firstTrainingProgram.StartDate.AddDays(30);

			// If the agreement has been accepted and the application is being updated, we must update GrantOpeningFinancials.
			if (grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.AgreementAccepted, ApplicationStateInternal.ChangeRequest, ApplicationStateInternal.ChangeForApproval, ApplicationStateInternal.ChangeForDenial, ApplicationStateInternal.ChangeReturned, ApplicationStateInternal.ChangeRequestDenied))
			{
				var originalCommitment = (decimal)(_dbContext.OriginalValue(grantApplication.TrainingCost, nameof(grantApplication.TrainingCost.AgreedCommitment)) ?? 0);
				var delta = grantApplication.TrainingCost.AgreedCommitment - originalCommitment;
				if (delta != 0)
				{
					grantApplication.GrantOpening.GrantOpeningFinancial.AssessedCommitments += delta;
					grantApplication.GrantOpening.GrantOpeningFinancial.OutstandingCommitments += delta;
					grantApplication.GrantOpening.GrantOpeningIntake.ReductionsAmt -= delta;
				}
			}

			GenerateDocuments(grantApplication);
		}
		
		/// <summary>
		/// Generate the grant agreement documents for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		public void GenerateDocuments(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (grantApplication.GrantAgreement == null)
				throw new ArgumentNullException(nameof(grantApplication), "The Grant Application must have a Grant Agreement before generating Schedule documents.");

			var coverLetter = _grantProgramService.GenerateApplicantCoverLetter(grantApplication);
			if (grantApplication.GrantAgreement.CoverLetter == null)
			{
				grantApplication.GrantAgreement.CoverLetter = coverLetter;
			}
			else
			{
				grantApplication.GrantAgreement.CoverLetter.CreateNewVersion(coverLetter);
			}

			var scheduleA = _grantProgramService.GenerateAgreementScheduleA(grantApplication);
			if (grantApplication.GrantAgreement.ScheduleA == null)
			{
				grantApplication.GrantAgreement.ScheduleA = scheduleA;
			}
			else
			{
				grantApplication.GrantAgreement.ScheduleA.CreateNewVersion(scheduleA);
			}

			var scheduleB = _grantProgramService.GenerateAgreementScheduleB(grantApplication);
			if (grantApplication.GrantAgreement.ScheduleB == null)
			{
				grantApplication.GrantAgreement.ScheduleB = scheduleB;
			}
			else
			{
				grantApplication.GrantAgreement.ScheduleB.CreateNewVersion(scheduleB);
			}

			if (grantApplication.ApplicationStateInternal != ApplicationStateInternal.OfferIssued)
			{
				_noteService.AddSystemNote(grantApplication, "Grant agreement was modified.");
			}
		}

		public void RemoveGrantAgreement(GrantApplication grantApplication)
		{
			var agreement = grantApplication.GrantAgreement;

			if (agreement == null)
				return;

			var coverLetter = agreement.CoverLetter;
			var scheduleA = agreement.ScheduleA;
			var scheduleB = agreement.ScheduleB;

			if (coverLetter != null)
			{
				agreement.CoverLetter = null;
				_dbContext.Documents.Remove(coverLetter);
			}

			if (scheduleA != null)
			{
				agreement.ScheduleA = null;
				_dbContext.Documents.Remove(scheduleA);
			}

			if (scheduleB != null)
			{
				agreement.ScheduleB = null;
				_dbContext.Documents.Remove(scheduleB);
			}

			grantApplication.GrantAgreement = null;
			_dbContext.GrantAgreements.Remove(agreement);
		}

		#region Workflow
		public void AcceptGrantAgreement(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			CreateWorkflowStateMachine(grantApplication).AcceptGrantAgreement();
		}

		public void CancelGrantAgreement(GrantApplication grantApplication, string reason)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			CreateWorkflowStateMachine(grantApplication).CancelAgreementHolder(reason);
		}

		public void RejectGrantAgreement(GrantApplication grantApplication, string reason)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			CreateWorkflowStateMachine(grantApplication).RejectGrantAgreement(reason);
		}
		#endregion
	}
}
