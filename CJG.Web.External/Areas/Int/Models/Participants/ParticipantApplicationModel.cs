using System;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Participants
{
    public class ParticipantApplicationModel : BaseViewModel
	{
		public string SIN { get; set; }
		public int ParticipantFormId { get; set; }
		public string ParticipantLastName { get; set; }
		public string ParticipantMiddleName { get; set; }
		public string ParticipantFirstName { get; set; }
		public string FileNumber { get; set; }
		public string CourseName { get; set; }
		public string EmployerName { get; set; }
		public decimal PaidToDate { get; set; }

		public DateTime LastApplicationDateTime { get; set; }
	}
}