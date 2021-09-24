using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Areas.Int.Models.Organizations
{
	public class OrganizationProfileViewModel : BaseViewModel
	{
		public int SelectedUserId { get; set; }
		public string RowVersion { get; set; }
	}
}
