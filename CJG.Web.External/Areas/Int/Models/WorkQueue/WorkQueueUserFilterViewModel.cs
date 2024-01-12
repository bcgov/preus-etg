using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.WorkQueue
{
    public class WorkQueueUserFilterViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public int RowSequence { get; set; }
		public IEnumerable<WorkQueueFilterAttributeViewModel> Attributes { get; set; }

		public WorkQueueUserFilterViewModel() { }

		public WorkQueueUserFilterViewModel(InternalUserFilter filter)
		{
			Utilities.MapProperties(filter, this);
			Attributes = filter?.Attributes.Select(a => new WorkQueueFilterAttributeViewModel(a));
		}

		public InternalUserFilter GetFilter(IPrincipal user)
		{
			var filter = new InternalUserFilter();
			Utilities.MapProperties(this, filter);
			filter.InternalUserId = user.GetUserId() ?? throw new ArgumentNullException(nameof(user));
			filter.Attributes = this.Attributes.Select(a => new InternalUserFilterAttribute(filter, a.Key, a.Value, a.Operator)).ToList();
			return filter;
		}
	}
}