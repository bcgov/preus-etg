using System;
using System.Linq;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.ParticipantReporting
{
	public class ParticipantViewModel
	{
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public string PhoneNumber1 { get; set; }
		public string PhoneExtension1 { get; set; }
		public string PrimaryCity { get; set; }
		public bool ClaimReported { get; set; }
		public bool IsIncludedInClaim { get; set; }
		public DateTime DateAdded { get; set; }
		public bool IsLate { get; set; }

		public bool? Approved { get; set; }
		public bool ShowEligibility { get; set; }

		public ParticipantViewModel()
		{

		}

		public ParticipantViewModel(ParticipantForm participantForm, bool showEligibility, Claim claim = null)
		{
			if (participantForm == null)
				throw new ArgumentNullException(nameof(participantForm));

			Id = participantForm.Id;
			RowVersion = Convert.ToBase64String(participantForm.RowVersion);
			FirstName = participantForm.FirstName;
			LastName = participantForm.LastName;
			EmailAddress = participantForm.EmailAddress;
			PhoneNumber1 = participantForm.PhoneNumber1;
			PhoneExtension1 = participantForm.PhoneExtension1;
			PrimaryCity = participantForm.PrimaryCity;

			// We are only looking to see if they were reported in another claim, not the current one.
			ClaimReported = participantForm.ClaimReported || participantForm.Claims.Any(c => c.Id != claim?.Id && c.ClaimVersion != claim?.ClaimVersion);
			IsIncludedInClaim = !participantForm.IsExcludedFromClaim;
			DateAdded = participantForm.DateAdded.ToLocalTime();
			IsLate = DateAdded > participantForm.GrantApplication.GetParticipantReportingDueDate();
			Approved = participantForm.Approved;
			ShowEligibility = showEligibility;
		}
	}
}