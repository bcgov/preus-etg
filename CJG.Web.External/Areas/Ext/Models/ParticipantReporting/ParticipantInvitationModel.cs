using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.ParticipantReporting
{
	public class ParticipantInvitationModel : BaseViewModel
	{
		public string RowVersion { get; set; }

		public int InvitationId { get; set; }

		public Guid IndividualKey { get; set; }

		[Required(ErrorMessage = "First name is required")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Last name is required")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Email address is required")]
		public string EmailAddress { get; set; }

		public ExpectedParticipantOutcome ExpectedParticipantOutcome { get; set; }
		public ParticipantInvitationStatus ParticipantInvitationStatus { get; set; }
		public string Outcome { get; set; }
		public string Status { get; set; }

		public List<KeyValuePair<int, string>> ExpectedOutcomes { get; set; }

		public bool CanSend { get; set; }
		public bool CanResend { get; set; }
		public bool CanRemove { get; set; }

		public ParticipantInvitationModel()
		{
		}

		public ParticipantInvitationModel(ParticipantInvitation invitation, bool applicationHasBeenReturnedToDraft)
		{
			Id = invitation.GrantApplication.Id;
			RowVersion = Convert.ToBase64String(invitation.GrantApplication.RowVersion);

			InvitationId = invitation.Id;
			IndividualKey = invitation.IndividualKey;
			LastName = invitation.LastName ?? string.Empty;
			FirstName = invitation.FirstName ?? string.Empty;
			EmailAddress = invitation.EmailAddress ?? string.Empty;

			ExpectedParticipantOutcome = invitation.ExpectedParticipantOutcome;
			ParticipantInvitationStatus = invitation.ParticipantInvitationStatus;

			Outcome = invitation.ExpectedParticipantOutcome.GetDescription();
			Status = invitation.ParticipantInvitationStatus.GetDescription();

			var expectedOutcomes = new List<KeyValuePair<int, string>>
			{
				new KeyValuePair<int, string>(0, "Please select expected training outcome"),
				GetExpectedItem(ExpectedParticipantOutcome.IncreasedJobSecurity),
				GetExpectedItem(ExpectedParticipantOutcome.IncreasedPay),
				GetExpectedItem(ExpectedParticipantOutcome.Promotion),
				GetExpectedItem(ExpectedParticipantOutcome.MoveFromPartTimeToFullTime),
				GetExpectedItem(ExpectedParticipantOutcome.MoveFromTransitionalToPermanent),
				GetExpectedItem(ExpectedParticipantOutcome.NoOutcome)
			};
					
			ExpectedOutcomes = expectedOutcomes;

			CanSend = invitation.ParticipantInvitationStatus == ParticipantInvitationStatus.NotSent;
			CanResend = invitation.ParticipantInvitationStatus == ParticipantInvitationStatus.Sent;
			CanRemove = (applicationHasBeenReturnedToDraft && invitation.ParticipantInvitationStatus == ParticipantInvitationStatus.Completed)
			            || invitation.ParticipantInvitationStatus == ParticipantInvitationStatus.NotSent
			            || invitation.ParticipantInvitationStatus == ParticipantInvitationStatus.Sent;

			var participantForm = invitation.ParticipantForm;
			if (invitation.ParticipantInvitationStatus == ParticipantInvitationStatus.Completed && participantForm != null)
			{
				EmailAddress = participantForm.EmailAddress;
				LastName = participantForm.LastName;
				FirstName = participantForm.FirstName;
			}
		}

		private static KeyValuePair<int, string> GetExpectedItem(ExpectedParticipantOutcome item)
		{
			return new KeyValuePair<int, string>((int)item, item.GetDescription());
		}


	}

	public class ParticipantInvitationPostModel
	{
		public int Id { get; set; }
		public int InvitationId { get; set; }

		public Guid IndividualKey { get; }

		[Required(ErrorMessage = "First name is required")]
		public string LastName { get; }

		[Required(ErrorMessage = "Last name is required")]
		public string FirstName { get; }

		[Required(ErrorMessage = "Email address is required")]
		public string EmailAddress { get; }

		public ExpectedParticipantOutcome ExpectedParticipantOutcome { get; }
		public ParticipantInvitationStatus ParticipantInvitationStatus { get; }
	}
}