using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IGrantProgramService : IService
	{
		GrantProgram Get(int id);
		IEnumerable<GrantProgram> GetAll(GrantProgramStates? state = null);
		IEnumerable<GrantProgram> GetWithActiveStreams();
		IEnumerable<GrantProgram> GetForFiscalYear(int? year);
		IEnumerable<GrantProgram> GetImplementedGrantPrograms();

		GrantProgram Add(GrantProgram grantProgram);
		GrantProgram Update(GrantProgram grantProgram);
		void Delete(GrantProgram grantProgram);
		void Implement(GrantProgram grantProgram);
		void Terminate(GrantProgram grantProgram);

		IEnumerable<InternalUser>  GetExpenseAuthorities();
		IEnumerable<ReportRate> GetReportRates(int grantProgramId);

		string GenerateApplicantDeclarationBody(GrantApplication grantApplication);
		Document GenerateApplicantDeclaration(GrantApplication grantApplication);
		string GenerateApplicantCoverLetterBody(GrantApplication grantApplication);
		Document GenerateApplicantCoverLetter(GrantApplication grantApplication);
		string GenerateAgreementScheduleABody(GrantApplication grantApplication);
		Document GenerateAgreementScheduleA(GrantApplication grantApplication);
		string GenerateAgreementScheduleBBody(GrantApplication grantApplication);
		Document GenerateAgreementScheduleB(GrantApplication grantApplication);
		string GenerateParticipantConsentBody(GrantApplication grantApplication);
		Document GenerateParticipantConsent(GrantApplication grantApplication);

		void ValidateTemplates(GrantProgram grantProgram);

		IEnumerable<EligibleExpenseType> GetAllActiveEligibleExpenseTypes(int grantProgramId);
		IEnumerable<EligibleExpenseType> GetAutoIncludeActiveEligibleExpenseTypes(int grantProgramId);
		IEnumerable<EligibleExpenseType> GetAllEligibleExpenseTypes(int grantProgramId);

		int CountApplicantsWithApplications(int grantProgramId);
		int CountSubscribedApplicants(int grantProgramId);

		IEnumerable<string> GetApplicantEmailsFor(int[] grantProgramIds);
		IEnumerable<string> GetSubscriberEmailsFor(int[] grantProgramIds);
	}
}
