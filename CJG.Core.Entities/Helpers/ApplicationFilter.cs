using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Core.Entities.Helpers
{
	public struct ApplicationFilter
	{
		public int? AssessorId { get; }
		public int? FiscalYearId { get; }
		public string TrainingPeriodCaption { get; }
		public int? GrantProgramId { get; set; }
		public int? GrantStreamId { get; }
		public bool? IsAssigned { get; }
		public string FileNumber { get; }
		public string Applicant { get; }
		public string[] OrderBy { get; }

		public StateFilter<ApplicationStateInternal>[] States { get; }

		public ApplicationFilter(IEnumerable<ApplicationStateInternal> states)
		{
			States = states.Select(s => new StateFilter<ApplicationStateInternal>(s)).ToArray();
			AssessorId = null;
			FiscalYearId = null;
			TrainingPeriodCaption = null;
			GrantProgramId = null;
			GrantStreamId = null;
			OrderBy = null;
			IsAssigned = null;
			FileNumber = null;
			Applicant = null;
		}

		public ApplicationFilter(StateFilter<ApplicationStateInternal>[] states, int? assessorId, int? fiscalYearId, string trainingPeriodCaption, int? grantProgramId, int? grantStreamId, string[] orderBy = null)
		{
			States = states;
			AssessorId = assessorId;
			FiscalYearId = fiscalYearId;
			TrainingPeriodCaption = trainingPeriodCaption;
			GrantProgramId = grantProgramId;
			GrantStreamId = grantStreamId;
			OrderBy = orderBy;
			IsAssigned = null;
			FileNumber = null;
			Applicant = null;
		}

		public ApplicationFilter(StateFilter<ApplicationStateInternal>[] states, string fileNumber, string applicant, int? assessorId, int? fiscalYearId, string trainingPeriodCaption, int? grantProgramId, int? grantStreamId, bool? isAssigned, string[] orderBy = null)
		{
			States = states;
			AssessorId = assessorId;
			FiscalYearId = fiscalYearId;
			TrainingPeriodCaption = trainingPeriodCaption;
			GrantProgramId = grantProgramId;
			GrantStreamId = grantStreamId;
			OrderBy = orderBy;
			IsAssigned = isAssigned;
			FileNumber = fileNumber;
			Applicant = applicant;
		}
	}
}
