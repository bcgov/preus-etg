using CJG.Core.Entities.Helpers;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Participants
{
    public class ParticipantsFilterViewModel : BaseViewModel
	{
		public string FileNumber { get; set; }
		public string Participant { get; set; }
		public string[] OrderBy { get; set; }

		public ApplicationFilter GetFilter()
		{
			return new ApplicationFilter(FileNumber, Participant, orderBy: OrderBy);
		}
	}
}