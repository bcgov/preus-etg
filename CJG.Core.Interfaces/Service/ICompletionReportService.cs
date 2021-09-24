using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface ICompletionReportService : IService
	{
		CompletionReport GetCurrentCompletionReport(int completionReportId);
		IEnumerable<CompletionReportQuestion> GetCompletionReportQuestionsForStep(int completionReportId, int groupId);

		IEnumerable<ParticipantForm> GetAffirmativeCompletionReportParticipants(int[] participantIds, int affirmativeAnswerId);

		bool RecordCompletionReportAnswersForStep(int stepNo, IEnumerable<ParticipantCompletionReportAnswer> participantAnswers, IEnumerable<EmployerCompletionReportAnswer> employerAnswers, int completionReportId, int[] participantEnrollmentsForReport);

		int[] GetCompletionReportParticipantQuestionIds(int[] participantIds);

		CompletionReport GetCompletionReportForParticipants(int[] participantIds);

		IEnumerable<ParticipantCompletionReportAnswer> GetParticipantCompletionReportNonDefaultAnswers(int[] participantIds, int questionId);

		IEnumerable<ParticipantCompletionReportAnswer> GetParticipantCompletionReportAnswers(int[] participantIds, int questionId);

		IEnumerable<EmployerCompletionReportAnswer> GetEmployerCompletionReportAnswers(int grantApplicationId);

		bool AllParticipantsHaveCompletedReport(int[] participantsOnClaim, int completionReportId, int completionGroupId);

		string GetCompletionReportStatus(int grantApplicationId);

		IEnumerable<ParticipantForm> GetParticipantsForReport(int grantApplicationId);

		CompletionReportOption GetCompletionReportOption(int id);

		IEnumerable<CompletionReportOption> GetCompletionReportOptions();

		int DeleteAnswersFor(int[] participantFormIds, int[] questionIds = null);

		IEnumerable<CompletionReportGroup> GetCompletionReportGroups(int grantApplicationId);

		IEnumerable<T> GetCompletionReportGroups<T>(int grantApplicationId, Func<CompletionReportGroup, T> select);

		CompletionReportGroup GetCompletionReportGroup(int completionReportId, int completionReportGroupId);
	}
}
