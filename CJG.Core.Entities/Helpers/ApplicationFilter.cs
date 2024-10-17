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
		public string Participant { get; }
		public string[] OrderBy { get; }
		public PriorityExceptionsMode OnlyShowPriorityRegionExceptions { get; }

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
			Participant = null;
			OnlyShowPriorityRegionExceptions = PriorityExceptionsMode.IgnoreExceptionFiltering;
		}

		public ApplicationFilter(StateFilter<ApplicationStateInternal>[] states, int? assessorId, int? fiscalYearId, string trainingPeriodCaption, int? grantProgramId, int? grantStreamId, PriorityExceptionsMode onlyShowPriorityRegionExceptions = PriorityExceptionsMode.IgnoreExceptionFiltering, string[] orderBy = null)
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
			Participant = null;
			OnlyShowPriorityRegionExceptions = onlyShowPriorityRegionExceptions;
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
			Participant = null;
			OnlyShowPriorityRegionExceptions = PriorityExceptionsMode.IgnoreExceptionFiltering;
		}

		public ApplicationFilter(string fileNumber, string participant, string[] orderBy = null)
		{
			States = new StateFilter<ApplicationStateInternal>[0];

			FileNumber = fileNumber;
			Participant = participant;
			OrderBy = orderBy;

			AssessorId = null;
			FiscalYearId = null;
			TrainingPeriodCaption = null;
			GrantProgramId = null;
			GrantStreamId = null;
			IsAssigned = null;
			Applicant = null;
			OnlyShowPriorityRegionExceptions = PriorityExceptionsMode.IgnoreExceptionFiltering;
		}

		public (string PropertyName, bool IsAscending) GetOrderByParts(string defaultProperty, bool defaultAscending = true)
		{
			if (OrderBy == null)
				return (defaultProperty, defaultAscending);

			if (!OrderBy.Any())
				return (defaultProperty, defaultAscending);

			// Should be "Name" or "Name asc" or "Name desc"
			var orderByString = OrderBy.First();
			if (string.IsNullOrWhiteSpace(orderByString))
				return (defaultProperty, defaultAscending);

			var parts = orderByString.Split(' ');
			var property = parts[0];
			var direction = parts.Length > 1 ? parts[1] : "asc";
			var isAscending = direction != "desc";
			
			return (property, isAscending);
		}
	}

	public enum PriorityExceptionsMode
	{
		IgnoreExceptionFiltering = 0,

		OnlyShowExceptions = 1,

		OnlyShowNonExceptions = 2
	}
}
