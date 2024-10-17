using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IProgramNotificationService : IService
	{
		ProgramNotification Get(int id);
		ProgramNotification Add(ProgramNotification programNotification);
		ProgramNotification Update(ProgramNotification programNotification);
		void Delete(ProgramNotification programNotification);
		void DeleteRecipient(ProgramNotificationRecipient programNotificationRecipient);

		PageList<ProgramNotification> GetProgramNotifications(int page, int quantity, string search);
		bool Exists(int id, string caption);
		int GetNumberOfApplicants();
		IEnumerable<GrantProgramApplicantModel> GetNumberOfApplicantsPerGrantProgram();
		void Send(int id);
	}
}
