using System;

namespace CJG.Application.Business.Models
{
	public class GroupedParticipantsModel
	{
		public string SIN { get; set; }
		public int ParticipantFormId { get; set; }
		public string ParticipantFirstName { get; set; }
		public string ParticipantMiddleName { get; set; }
		public string ParticipantLastName { get; set; }
		public string FileNumber { get; set; }
		public string Status { get; set; }
		public string Eligibility { get; set; }
		public string CourseName { get; set; }
		public string EmployerName { get; set; }

		public DateTime LastApplicationDateTime { get; set; }
	}
}