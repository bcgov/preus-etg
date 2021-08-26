using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Areas.Int.Models.Organizations
{
	public class OrganizationProfileUserViewModel : BaseViewModel
	{
		public string BCeID { get; set; }
		public Guid BCeIDGuid { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public string JobTitle { get; set; }
		public string PhoneNumber { get; set; }
		public string PhoneExtension { get; set; }
		public bool IsOrganizationProfileAdministrator { get; set; }
		public int OrganizationId { get; set; }
		public string RowVersion { get; set; }

		public OrganizationProfileUserViewModel()
		{
		}

		public OrganizationProfileUserViewModel(User user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));

			Utilities.MapProperties(user, this);
		}
	}
}
