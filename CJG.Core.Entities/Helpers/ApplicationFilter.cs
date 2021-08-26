using System.Collections.Generic;
using System.Linq;

namespace CJG.Core.Entities.Helpers
{
	public struct ApplicationFilter
	{
		#region Properties
		public int? AssessorId { get; }

		public int? FiscalYearId { get; }

		public int? TrainingPeriodId { get; }

		public int? GrantProgramId { get; }

		public int? GrantStreamId { get; }

		public StateFilter<ApplicationStateInternal>[] States { get; }

		public bool? IsAssigned { get; }

		public string FileNumber { get; }

		public string Applicant { get; }

		public string[] OrderBy { get; }
		#endregion

		#region Constructors
		public ApplicationFilter(IEnumerable<ApplicationStateInternal> states)
		{
			this.States = states.Select(s => new StateFilter<ApplicationStateInternal>(s)).ToArray();
			this.AssessorId = null;
			this.FiscalYearId = null;
			this.TrainingPeriodId = null;
			this.GrantProgramId = null;
			this.GrantStreamId = null;
			this.OrderBy = null;
			this.IsAssigned = null;
			this.FileNumber = null;
			this.Applicant = null;
		}

		public ApplicationFilter(StateFilter<ApplicationStateInternal>[] states, int? assessorId, int? fiscalYearId, int? trainingPeriodId, int? grantProgramId, int? grantStreamId, string[] orderBy = null)
		{
			this.States = states;
			this.AssessorId = assessorId;
			this.FiscalYearId = fiscalYearId;
			this.TrainingPeriodId = trainingPeriodId;
			this.GrantProgramId = grantProgramId;
			this.GrantStreamId = grantStreamId;
			this.OrderBy = orderBy;
			this.IsAssigned = null;
			this.FileNumber = null;
			this.Applicant = null;
		}

		public ApplicationFilter(StateFilter<ApplicationStateInternal>[] states, string fileNumber, string applicant, int? assessorId, int? fiscalYearId, int? trainingPeriodId, int? grantProgramId, int? grantStreamId, bool? isAssigned, string[] orderBy = null)
		{
			this.States = states;
			this.AssessorId = assessorId;
			this.FiscalYearId = fiscalYearId;
			this.TrainingPeriodId = trainingPeriodId;
			this.GrantProgramId = grantProgramId;
			this.GrantStreamId = grantStreamId;
			this.OrderBy = orderBy;
			this.IsAssigned = isAssigned;
			this.FileNumber = fileNumber;
			this.Applicant = applicant;
		}
		#endregion
	}
}
