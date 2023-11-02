using CJG.Application.Business.Models;
using CJG.Core.Entities.Helpers;

namespace CJG.Core.Interfaces.Service
{
	public interface IParticipantsService : IService
	{
		PageList<GroupedParticipantsModel> GetParticipants(int page, int quantity, ApplicationFilter filter);
	}
}