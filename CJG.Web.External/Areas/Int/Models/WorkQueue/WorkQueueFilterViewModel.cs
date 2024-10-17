using System;
using System.Linq;
using System.Security.Principal;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.WorkQueue
{
	public class WorkQueueFilterViewModel : BaseViewModel
	{
		public bool MyFiles { get; set; }
		public bool? IsAssigned { get; set; }
		public int? AssessorId { get; set; }
		public int? FiscalYearId { get; set; }
		public int? GrantProgramId { get; set; }
		public int? GrantStreamId { get; set; }
		public int? TrainingPeriodId { get; set; }
		public string TrainingPeriodCaption { get; set; }
		public string FileNumber { get; set; }
		public string Applicant { get; set; }
		public string[] OrderBy { get; set; }
		public ApplicationStateInternal[] States { get; set; }

		public ApplicationFilter GetFilter(IPrincipal user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (MyFiles)
				AssessorId = user.GetUserId().Value;

			var states = States?.Select(s => new StateFilter<ApplicationStateInternal>(s)).ToArray() ?? new[] { new StateFilter<ApplicationStateInternal>(ApplicationStateInternal.Draft, false) };

			return new ApplicationFilter(states, FileNumber, Applicant, AssessorId, FiscalYearId, TrainingPeriodCaption, GrantProgramId, GrantStreamId, IsAssigned, OrderBy);
		}
	}
}