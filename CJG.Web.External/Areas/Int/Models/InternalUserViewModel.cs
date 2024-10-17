using CJG.Core.Entities;
using CJG.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Int.Models
{
	public class InternalUserViewModel
	{
		#region Properties
		public int Id { get; set; }

		[Required]
		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		public string FirstName { get; set; }

		[Required]
		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		public string LastName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The field must be a string with a maximum length of 100")]
		public string IDIR { get; set; }

		[StringLength(50, ErrorMessage = "The field must be a string with a maximum length of 50")]
		public string Salutation { get; set; }

		[Required]
		[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string Email { get; set; }

		[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone Number")]
		[StringLength(20, ErrorMessage = "The field must be a string with a maximum length of 20")]
		public string PhoneNumber { get; set; }

		public string ApplicationUserId { get; set; }

		[Required]
		public string UserRole { get; set; }

		public bool Active { get; set; }

		public IEnumerable<ApplicationRole> Roles { get; set; }
		public IEnumerable<string> Salutations { get; set; }
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
			this.Salutation = user.Salutation;
			this.Email = user.Email;
			this.PhoneNumber = user.PhoneNumber;
		}
		#endregion
	}
}