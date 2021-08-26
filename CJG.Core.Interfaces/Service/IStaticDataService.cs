using System.Collections.Generic;
using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface IStaticDataService : IService
	{
		void CopyTo<TEntity>(TEntity source, TEntity destination) where TEntity : class;

		IEnumerable<DeliveryMethod> GetDeliveryMethods();
		DeliveryMethod GetDeliveryMethod(int id);

		IEnumerable<UnderRepresentedGroup> GetUnderRepresentedGroups();
		UnderRepresentedGroup GetUnderRepresentedGroup(int id);

		IEnumerable<ExpectedQualification> GetExpectedQualifications();
		ExpectedQualification GetExpectedQualification(int id);

		IEnumerable<InDemandOccupation> GetInDemandOccupations();
		InDemandOccupation GetInDemandOccupation(int id);

		IEnumerable<SkillsFocus> GetSkillsFocuses();
		SkillsFocus GetSkillsFocus(int id);

		IEnumerable<SkillLevel> GetSkillLevels();
		SkillLevel GetSkillLevel(int id);

		IEnumerable<TrainingLevel> GetTrainingLevels();
		TrainingLevel GetTrainingLevel(int id);

		IEnumerable<TrainingProviderType> GetTrainingProviderTypes();
		TrainingProviderType GetTrainingProviderType(int id);
		
		IEnumerable<OrganizationType> GetOrganizationTypes();
		OrganizationType GetOrganizationType(int id);

		IEnumerable<Country> GetImmigrationCountries();
		IEnumerable<Country> GetTrainingProviderCountries();
		IEnumerable<Country> GetCountries();
		Country GetCountry(string id);

		IEnumerable<Region> GetRegions();
		Region GetRegion(string countryId, string id);

		IEnumerable<Region> GetProvinces();
		Region GetProvince(string id);

		IEnumerable<LegalStructure> GetLegalStructures();
		LegalStructure GetLegalStructure(int id);

		IEnumerable<CanadianStatus> GetCanadianStatuses();
		CanadianStatus GetCanadianStatus(int id);

		IEnumerable<AboriginalBand> GetAboriginalBands();
		AboriginalBand GetAboriginalBand(int id);

		IEnumerable<ApplicationType> GetApplicationTypes();
		ApplicationType GetApplicationType(int id);

		IEnumerable<MartialStatus> GetMartialStatuses();
		MartialStatus GetMartialStatus(int id);

		IEnumerable<FederalOfficialLanguage> GetFederalOfficialLanguages();
		FederalOfficialLanguage GetFederalOfficialLanguage(int id);

		IEnumerable<EducationLevel> GetEducationLevels();
		EducationLevel GetEducationLevel(int id);

		IEnumerable<EmploymentType> GetEmploymentTypes();
		EmploymentType GetEmploymentType(int id);

		IEnumerable<EmploymentStatus> GetEmploymentStatuses();
		EmploymentStatus GetEmploymentStatus(int id);

		IEnumerable<TrainingResult> GetTrainingResults();
		TrainingResult GetTrainingResult(int id);

		IEnumerable<PrioritySector> GetPrioritySectors();
		PrioritySector GetPrioritySector(int id);

		IEnumerable<FiscalYear> GetFiscalYears();
		FiscalYear GetFiscalYear(int id);

		IEnumerable<TrainingPeriod> GetTrainingPeriods();
		IEnumerable<TrainingPeriod> GetTrainingPeriodsForFiscalYear(int fiscalYearId);
		TrainingPeriod GetTrainingPeriod(int id);
		
		IEnumerable<NoteType> GetNoteTypes();
		NoteType GetNoteType(NoteTypes type);

		IEnumerable<RateFormat> GetRateFormats();
		RateFormat GetRateFormat(double rate);

		IEnumerable<ClaimType> GetClaimTypes();
		ClaimType GetClaimType(ClaimTypes type);

		IEnumerable<ExpenseType> GetExpenseTypes();
		ExpenseType GetExpenseType(ExpenseTypes type);

		IEnumerable<ParticipantEmploymentStatus> GetParticipantEmploymentStatuses();
		IEnumerable<UnderRepresentedPopulation> GetUnderRepresentedPopulations();
		IEnumerable<ApplicantOrganizationType> GetApplicantOrganizationTypes();
		IEnumerable<VulnerableGroup> GetVulnerableGroups();

		IEnumerable<ProgramType> GetProgramTypes();

		IEnumerable<ServiceType> GetServiceTypes();

		IEnumerable<RiskClassification> GetRiskClassifications();
		RiskClassification GetRiskClassification(int id);

		IEnumerable<NotificationType> GetNotificationTypes();
		NotificationType GetNotificationType(int id);
	}
}
