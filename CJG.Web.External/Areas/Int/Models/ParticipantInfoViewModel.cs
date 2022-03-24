using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;


namespace CJG.Web.External.Areas.Int.Models
{
	public class ParticipantInfoViewModel
	{
		#region Properties
		public string FileNo { get; set; }
		public string OrganizationName { get; set; }
		public string ReportingDate { get; set; }
		public string EmployerAdministrator { get; set; }
		public string TrainingStartDate { get; set; }
		public string EmployerAdministratorEmail { get; set; }
		public int TotalParticipants { get; set; }
		public string EmployerAdministratorPhone { get; set; }
		public ParticipantContactInfoViewModel ContactInfo { get; set; }
		public ParticipantEmploymentInfoViewModel EmployerInfo { get; set; }
		public List<ParticipantTrainingHistory> TrainingHistory { get; set; }
		#endregion

		#region Constructors
		public ParticipantInfoViewModel(ParticipantForm participant,
										CJG.Core.Interfaces.Service.INationalOccupationalClassificationService nationalOccupationalClassificationService,
										CJG.Core.Interfaces.Service.IUserService userService,
										CJG.Core.Interfaces.Service.IParticipantService participantService)
		{
			this.FileNo = participant.GrantApplication.FileNumber;
			this.OrganizationName = participant.GrantApplication.OrganizationLegalName;
			var daysLate = (int)(participant.GrantApplication.StartDate - participant.DateAdded).TotalDays;
			this.ReportingDate = String.Format("{0:yyyy-MM-dd} {1}", participant.DateAdded.Date, (daysLate > 5) ? "" : string.Format("(Late {0} days)", daysLate));
			participant.GrantApplication.Organization.Users.Any();
			var businessContact = participant.GrantApplication.BusinessContactRoles.FirstOrDefault(c => c.GrantApplicationId > 0);
			var employerAdministrator = userService.GetUser(businessContact.UserId);
			if (employerAdministrator != null)
			{
				this.EmployerAdministrator = string.Format("{0} {1}", employerAdministrator.FirstName, employerAdministrator.LastName);
				this.EmployerAdministratorEmail = employerAdministrator.EmailAddress;
				this.EmployerAdministratorPhone = employerAdministrator.PhoneNumber + (string.IsNullOrWhiteSpace(employerAdministrator.PhoneNumber) ? "" : employerAdministrator.PhoneExtension);
			}
			this.TrainingStartDate = String.Format("{0:yyyy-MM-dd}", participant.GrantApplication.StartDate.ToLocalMorning());
			this.TotalParticipants = participant.GrantApplication.ParticipantForms.Count;
			this.ContactInfo = new ParticipantContactInfoViewModel(participant);
			this.EmployerInfo = new ParticipantEmploymentInfoViewModel(participant, nationalOccupationalClassificationService);

			this.TrainingHistory = new List<ParticipantTrainingHistory>();

			var pifs = participantService.GetParticipantFormsBySIN(participant.SIN);
			foreach (var p in pifs)
			{
				decimal reimbursement = 0.0m;
				decimal amtPaid = 0.0m;
				foreach(var c in p.ParticipantCosts)
                {
					reimbursement += c.AssessedReimbursement;
					if(c.ClaimEligibleCost.Claim.ClaimState == ClaimState.ClaimApproved || c.ClaimEligibleCost.Claim.ClaimState == ClaimState.PaymentRequested)
                    {
						amtPaid += c.AssessedReimbursement;
					}
				}

				foreach (var t in p.GrantApplication.TrainingPrograms)
				{
					//do not show grant apps that have not yet been submitted
					//grant apps get a filenumber when they are submitted
					if (t.GrantApplication.FileNumber != null){
						this.TrainingHistory.Add(new ParticipantTrainingHistory(t, reimbursement, amtPaid));
					}					
				}
			}
		}

		public ParticipantInfoViewModel()
		{
			this.ContactInfo = new ParticipantContactInfoViewModel();
			this.EmployerInfo = new ParticipantEmploymentInfoViewModel();
		}
		#endregion
	}

}