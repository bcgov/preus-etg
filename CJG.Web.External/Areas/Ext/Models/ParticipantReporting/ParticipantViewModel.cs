using CJG.Core.Entities;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.ParticipantReporting
{
	public class ParticipantViewModel
	{
		#region Properties
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
		#endregion

		#region Constructors
		public ParticipantViewModel()
		{

		}

		public ParticipantViewModel(ParticipantForm participantForm, Claim claim = null)
		{
			if (participantForm == null)
				throw new ArgumentNullException(nameof(participantForm));

			this.Id = participantForm.Id;
			this.RowVersion = Convert.ToBase64String(participantForm.RowVersion);
			this.FirstName = participantForm.FirstName;
			this.LastName = participantForm.LastName;
			this.EmailAddress = participantForm.EmailAddress;
			this.PhoneNumber1 = participantForm.PhoneNumber1;
			this.PhoneExtension1 = participantForm.PhoneExtension1;
			this.PrimaryCity = participantForm.PrimaryCity;

			// We are only looking to see if they were reported in another claim, not the current one.
			this.ClaimReported = participantForm.ClaimReported || participantForm.Claims.Where(c => c.Id != claim?.Id && c.ClaimVersion != claim?.ClaimVersion).Any();
			this.IsIncludedInClaim = !participantForm.IsExcludedFromClaim;
			this.DateAdded = participantForm.DateAdded.ToLocalTime();
			this.IsLate = (participantForm.GrantApplication.StartDate - this.DateAdded).TotalDays < 5;
		}
		#endregion
	}
}