using CJG.Core.Entities;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class InternalUserViewModel
	{
		#region Properties
		public int Id { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string IDIR { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public bool IsActive { get; set; }
		#endregion

		#region Constructors
		public InternalUserViewModel() { }

		public InternalUserViewModel(InternalUser user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			this.Id = user.Id;
			this.FirstName = user.FirstName;
			this.LastName = user.LastName;
			this.IDIR = user.IDIR;
			this.Email = user.Email;
			this.PhoneNumber = user.PhoneNumber;
			this.IsActive = user.ApplicationUsers.FirstOrDefault()?.Active ?? true;
		}
		#endregion
	}
}