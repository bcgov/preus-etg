using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class ParticipantViewModel
	{
		public int ParticipantFormId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }

		public ParticipantViewModel() { }

		public ParticipantViewModel(ParticipantForm participant)
		{
			if (participant == null)
				throw new ArgumentNullException(nameof(participant));

			ParticipantFormId = participant.Id;
			Name = $"{participant.LastName}, {participant.FirstName}";
			Email = participant.EmailAddress;
			PhoneNumber = $"{participant.PhoneNumber1} {participant.PhoneExtension1}";
		}
	}
}