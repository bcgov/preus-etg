using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;

namespace CJG.Web.External.Areas.Int.Models
{
    public class ParticipantInfoViewModel
	{
		public int ParticipantId { get; set; }
		public string FileNo { get; set; }
		public string OrganizationName { get; set; }
		public string ReportingDate { get; set; }
		public string ExpectedTrainingOutcome { get; set; }
		public string EmployerAdministrator { get; set; }
		public string TrainingStartDate { get; set; }
		public string EmployerAdministratorEmail { get; set; }
		public int TotalParticipants { get; set; }
		public string EmployerAdministratorPhone { get; set; }
		public ParticipantContactInfoViewModel ContactInfo { get; set; }
		public ParticipantEmploymentInfoViewModel EmployerInfo { get; set; }
		public List<ParticipantTrainingHistory> TrainingHistory { get; set; }

		public ParticipantInfoViewModel(ParticipantForm participant,
										INationalOccupationalClassificationService nationalOccupationalClassificationService,
										IUserService userService,
										IParticipantService participantService)
		{
			ParticipantId = participant.Id;
			FileNo = participant.GrantApplication.FileNumber;
			OrganizationName = participant.GrantApplication.OrganizationLegalName;

			ReportingDate = $"{participant.DateAdded.Date:yyyy-MM-dd}";
			ExpectedTrainingOutcome = participant.ExpectedParticipantOutcome.HasValue ? participant.ExpectedParticipantOutcome.Value.GetDescription() : string.Empty;
			participant.GrantApplication.Organization.Users.Any();

			var businessContact = participant.GrantApplication.BusinessContactRoles.FirstOrDefault(c => c.GrantApplicationId > 0);
			var employerAdministrator = userService.GetUser(businessContact.UserId);
			if (employerAdministrator != null)
			{
				EmployerAdministrator = $"{employerAdministrator.FirstName} {employerAdministrator.LastName}";
				EmployerAdministratorEmail = employerAdministrator.EmailAddress;
				EmployerAdministratorPhone = employerAdministrator.PhoneNumber + (string.IsNullOrWhiteSpace(employerAdministrator.PhoneNumber) ? "" : employerAdministrator.PhoneExtension);
			}
			TrainingStartDate = $"{participant.GrantApplication.StartDate.ToLocalMorning():yyyy-MM-dd}";
			TotalParticipants = participant.GrantApplication.ParticipantForms.Count;
			ContactInfo = new ParticipantContactInfoViewModel(participant);
			EmployerInfo = new ParticipantEmploymentInfoViewModel(participant, nationalOccupationalClassificationService);

			TrainingHistory = new List<ParticipantTrainingHistory>();

			var pifs = participantService.GetParticipantFormsBySIN(participant.SIN);

			// ETG grants only
			foreach (var p in pifs.Where(w => w.GrantApplication.IsWDAService() == false))
			{
				var reimbursement = 0.0m;
				var amtPaid = 0.0m;

				reimbursement = p.ParticipantCosts.Sum(s => s.AssessedReimbursement);
				amtPaid = p.ParticipantCosts
					.Where(w => w.ClaimEligibleCost.Claim.ClaimState == ClaimState.ClaimApproved ||
					            w.ClaimEligibleCost.Claim.ClaimState == ClaimState.PaymentRequested ||
								w.ClaimEligibleCost.Claim.ClaimState == ClaimState.ClaimPaid)
					.Sum(s => s.AssessedReimbursement);
				
				foreach (var t in p.GrantApplication.TrainingPrograms)
				{
					// Do not show grant apps that have not yet been submitted
					if (t.GrantApplication.FileNumber == null)
						continue;

					TrainingHistory.Add(new ParticipantTrainingHistory(t, reimbursement, amtPaid));
				}
			}
		}

		public ParticipantInfoViewModel()
		{
			ContactInfo = new ParticipantContactInfoViewModel();
			EmployerInfo = new ParticipantEmploymentInfoViewModel();
		}
	}
}