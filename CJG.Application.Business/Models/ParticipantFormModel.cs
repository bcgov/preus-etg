using System;
using CJG.Core.Entities;

namespace CJG.Application.Business.Models
{
	public class ParticipantFormModel
    {
		public int Id { get; set; }
		public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WorkLocation { get; set; }
		public bool? Attended { get; set; }

		public ParticipantFormModel()
        {
        }

        public ParticipantFormModel(ParticipantForm participantForm)
        {
            if (participantForm == null)
                throw new ArgumentNullException(nameof(participantForm));

			Id = participantForm.Id;
            Name = $"{participantForm.LastName}, {participantForm.FirstName}";
            Email = participantForm.EmailAddress;
            Phone = $"{participantForm.PhoneNumber1 + (string.IsNullOrWhiteSpace(participantForm.PhoneExtension1) ? null : $" ext. {participantForm.PhoneExtension1}")}";
            WorkLocation = participantForm.PrimaryCity;
			Attended = participantForm.Attended;
		}
    }
}
