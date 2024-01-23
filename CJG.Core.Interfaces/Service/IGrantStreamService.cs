using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IGrantStreamService : IService
	{
		IEnumerable<GrantStream> GetAll();
		GrantStream Get(string streamName);
		GrantStream Get(int id);
		IEnumerable<GrantStream> GetGrantStreamsForProgram(int grantProgramId, bool onlyLive = true);
		GrantStream Add(GrantStream grantStream);
		GrantStream Update(GrantStream grantStream);
		void Delete(GrantStream grantStream);
		IEnumerable<ReportRate> GetReportRates(int grantStreamId);
		EligibleExpenseType AddEligibleExpenseType(int grantStreamId, EligibleExpenseType eligibleExpenseType);
		IEnumerable<EligibleExpenseType> GetAllActiveEligibleExpenseTypes(int grantStreamId);
		IEnumerable<EligibleExpenseType> GetAutoIncludeActiveEligibleExpenseTypes(int grantStreamId);
		IEnumerable<EligibleExpenseType> GetAllEligibleExpenseTypes(int grantStreamId);
		IEnumerable<GrantStream> GetGrantStreams(int? year, int? program);
		IEnumerable<GrantStreamEligibilityQuestion> GetGrantStreamQuestions(int grantStreamId);
		GrantStreamEligibilityQuestion GetOldGrowthGrantStreamQuestion();
		GrantStreamEligibilityQuestion GetGrantStreamQuestion(int id);
		GrantStreamEligibilityQuestion UpdateGrantStreamQuestion(GrantStreamEligibilityQuestion question);
		IEnumerable<GrantStreamEligibilityQuestion> UpdateGrantStreamQuestions(IEnumerable<GrantStreamEligibilityQuestion> questions);
		GrantStreamEligibilityQuestion AddGrantStreamQuestion(GrantStreamEligibilityQuestion question);
		void AddGrantStreamAnswers(IEnumerable<GrantStreamEligibilityAnswer> answers);
		IEnumerable<GrantStreamEligibilityAnswer> GetGrantStreamAnswers(int grantApplicationId);
		void RemoveGrantStreamAnswers(int grantApplicationId);
		bool HasApplications(int grantStreamId);
	}
}
