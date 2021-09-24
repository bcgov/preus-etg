using CJG.Core.Entities;
using System;
using CJG.Core.Entities.Attributes;
using CJG.Application.Services;

namespace CJG.Web.External.Models.Shared.Reports
{
	public class CompletionReportParticipantDetailsViewModel
	{
		#region Properties
		public int Id { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string ParticipantName { get { return $"{FirstName} {LastName}"; } }

		public string EmailAddress { get; set; }

		public string PhoneNumber1 { get; set; }

		public string PhoneExtension1 { get; set; }

		public string PrimaryCity { get; set; }

		public bool ClaimReported { get; set; }

		public bool IsExcludedFromClaim { get; set; }

		public DateTime DateAdded { get; set; }
		#endregion

		#region Constructors
		public CompletionReportParticipantDetailsViewModel()
		{
		}

		public CompletionReportParticipantDetailsViewModel(ParticipantForm participantForm)
		{
			if (participantForm == null)
				throw new ArgumentNullException(nameof(participantForm));

			Utilities.MapProperties(participantForm, this);
			this.DateAdded = participantForm.DateAdded.ToLocalTime();
		}
		#endregion
	}
}
