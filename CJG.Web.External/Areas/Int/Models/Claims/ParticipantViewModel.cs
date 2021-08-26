using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class ParticipantViewModel
	{
		#region Properties
		public int ParticipantFormId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		#endregion

		#region Constructors
		public ParticipantViewModel() { }

		public ParticipantViewModel(ParticipantForm participant)
		{
			if (participant == null) throw new ArgumentNullException(nameof(participant));

			this.ParticipantFormId = participant.Id;
			this.Name = $"{participant.FirstName} {participant.LastName}";
			this.Email = participant.EmailAddress;
			this.PhoneNumber = $"{participant.PhoneNumber1} {participant.PhoneExtension1}";
		}
		#endregion
	}
}