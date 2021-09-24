using CJG.Core.Entities;
using System;

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

			this.Id = participantForm.Id;
            this.Name = $"{participantForm.LastName}, {participantForm.FirstName}";
            this.Email = participantForm.EmailAddress;
            this.Phone = string.Format("{0}", participantForm.PhoneNumber1 + (string.IsNullOrWhiteSpace(participantForm.PhoneExtension1) ? null : $" ext. {participantForm.PhoneExtension1}"));
            this.WorkLocation = participantForm.PrimaryCity;
			this.Attended = participantForm.Attended;
		}
    }
}
