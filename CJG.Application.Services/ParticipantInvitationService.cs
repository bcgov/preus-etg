using System;
using System.Linq;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Entities.Extensions;
using CJG.Core.Interfaces.Service;
using CJG.Core.Interfaces.Service.Settings;
using CJG.Infrastructure.Entities;
using NLog;

namespace CJG.Application.Services
{
	public class ParticipantInvitationService : Service, IParticipantInvitationService
	{
		private readonly INotificationService _notificationService;
		private readonly INotificationSettings _notificationSettings;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		/// <param name="notificationService"></param>
		/// <param name="notificationSettings"></param>
		public ParticipantInvitationService(IDataContext context, HttpContextBase httpContext, ILogger logger, INotificationService notificationService, INotificationSettings notificationSettings)
			: base(context, httpContext, logger)
		{
			_notificationService = notificationService;
			_notificationSettings = notificationSettings;
		}

		public ParticipantInvitation UpdateParticipantInvitation(ParticipantInvitation participantInvitation)
		{
				if (participantInvitation == null)
					throw new ArgumentNullException(nameof(participantInvitation));

				_dbContext.Update(participantInvitation);
				_dbContext.Commit();

				return participantInvitation;
		}

		public ParticipantInvitation RemoveParticipantInvitation(ParticipantInvitation participantInvitation)
		{
			if (participantInvitation == null)
				throw new ArgumentNullException(nameof(participantInvitation));

			if (participantInvitation.GrantApplication.HasBeenReturnedToDraft())
			{
				var pifInvitationNotificationType = _notificationService.GetPIFInvitationNotificationType();

				var invitationNotifications = _dbContext.NotificationQueue
					.Where(p => p.EmailRecipients == participantInvitation.EmailAddress)
					.Where(p => p.GrantApplicationId == participantInvitation.GrantApplicationId)
					.Where(p => p.NotificationType.Id == pifInvitationNotificationType.Id)
					.ToList();

				_dbContext.NotificationQueue.RemoveRange(invitationNotifications);
			}

			participantInvitation.IndividualKey = Guid.NewGuid();
			participantInvitation.FirstName = null;
			participantInvitation.LastName = null;
			participantInvitation.EmailAddress = null;
			participantInvitation.ExpectedParticipantOutcome = 0;
			participantInvitation.ParticipantInvitationStatus = ParticipantInvitationStatus.Empty;

			if (participantInvitation.ParticipantForm != null)
			{
				participantInvitation.GrantApplication.ParticipantForms.Remove(participantInvitation.ParticipantForm);
				_dbContext.ParticipantForms.Remove(participantInvitation.ParticipantForm);
			}

			_dbContext.Update(participantInvitation);
			_dbContext.Commit();

			return participantInvitation;
		}

		public ParticipantInvitation RemoveParticipantNotInvitation(ParticipantInvitation participantInvitation)
		{
			if (participantInvitation == null)
				throw new ArgumentNullException(nameof(participantInvitation));

			if (participantInvitation.GrantApplication.ApplicationStateInternal == ApplicationStateInternal.Draft
			     || participantInvitation.GrantApplication.HasBeenReturnedToDraft())
			{
				var pifInvitationNotificationType = _notificationService.GetPIFInvitationNotificationType();

				var invitationNotifications = _dbContext.NotificationQueue
					.Where(p => p.EmailRecipients == participantInvitation.EmailAddress)
					.Where(p => p.GrantApplicationId == participantInvitation.GrantApplicationId)
					.Where(p => p.NotificationType.Id == pifInvitationNotificationType.Id)
					.ToList();

				_dbContext.NotificationQueue.RemoveRange(invitationNotifications);
			}

			participantInvitation.ParticipantInvitationStatus = ParticipantInvitationStatus.NotSent;

			if (participantInvitation.ParticipantForm != null)
			{
				participantInvitation.GrantApplication.ParticipantForms.Remove(participantInvitation.ParticipantForm);
				_dbContext.ParticipantForms.Remove(participantInvitation.ParticipantForm);
			}

			_dbContext.Update(participantInvitation);
			_dbContext.Commit();

			return participantInvitation;
		}

		public ParticipantInvitation SendParticipantInvitation(ParticipantInvitation participantInvitation)
		{
			if (participantInvitation == null)
				throw new ArgumentNullException(nameof(participantInvitation));

			var grantApplication = participantInvitation.GrantApplication;

//			var GrantProgramName = grantApplication?.GrantOpening?.GrantStream?.GrantProgram?.Name ?? throw new ArgumentNullException(nameof(grantApplication), "The argument 'grantApplication' must provide the grant program name.");
			var startDate = grantApplication.StartDate.ToLocalTime();
			var invitationBrowserLink = $"{_httpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Part/Information/{HttpUtility.UrlEncode(grantApplication.InvitationKey.ToString())}/{HttpUtility.UrlEncode(participantInvitation.IndividualKey.ToString())}";

			var inviteSubject = $"Invitation to Participate in {grantApplication.GetProgramDescription()} - Training Funded by the B.C. Employer Training Grant";
			var inviteBody = $@"<p>Dear {participantInvitation.FirstName} {participantInvitation.LastName},</p>
<p>You have been chosen by your employer to participate in {grantApplication.GetProgramDescription()} training starting on {startDate.ToLocalMorning():yyyy-MM-dd}. This training will be funded by your employer as well as through the B.C. Employer Training Grant program. If approved, your training expenses will be fully covered, and you will not be required to pay anything.</p>
<p>To make sure you are eligible, please complete your Participant Information Form (PIF) as soon as possible. Click the following link using the Chrome or Firefox browser: <a href='{invitationBrowserLink}'>{invitationBrowserLink}</a></p>
<p>For questions, please contact your employer. We are also available to answer any questions at <a href='mailto: ETG@gov.bc.ca'>ETG@gov.bc.ca</a>.</p>
<p>Thank you,</p>
<p>The B.C. Employer Training Grant</p>";

			// This service doesn't use the template subject or body for content - just hooks it up for Ref. Integrity
			var pifInvitationNotificationType = _notificationService.GetPIFInvitationNotificationType();

			var sender = $"{_notificationSettings.DefaultSenderName} <{_notificationSettings.DefaultSenderAddress}>";
			var email = new NotificationQueue(participantInvitation.GrantApplication, participantInvitation, sender, inviteBody, inviteSubject, pifInvitationNotificationType);
			_notificationService.SendNotification(email);

			participantInvitation.ParticipantInvitationStatus = ParticipantInvitationStatus.Sent;

			_dbContext.Update(participantInvitation);
			_dbContext.Commit();

			return participantInvitation;
		}

		public ParticipantInvitation GetInvitation(int grantApplicationId, int invitationId)
		{
			var invitation = Get<ParticipantInvitation>(invitationId);

			if (!_httpContext.User.CanPerformAction(invitation.GrantApplication, ApplicationWorkflowTrigger.ViewParticipants))
				throw new NotAuthorizedException($"User does not have permission to view participants from grant application '{invitation.GrantApplication}'.");

			return invitation;
		}

		public ParticipantInvitation CompleteIndividualInvitation(ParticipantForm participantForm, ParticipantInvitation participantInvitation)
		{
			if (participantForm == null)
				throw new ArgumentNullException(nameof(participantForm));

			if (participantInvitation == null)
				throw new ArgumentNullException(nameof(participantInvitation));

			participantInvitation.ParticipantForm = participantForm;
			participantInvitation.ParticipantInvitationStatus = ParticipantInvitationStatus.Completed;

			_dbContext.Update(participantInvitation);
			_dbContext.Commit();

			return participantInvitation;
		}
	}
}
