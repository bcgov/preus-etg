using CJG.Infrastructure.Identity;
using CJG.Web.External.Models.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models
{
	public class UserViewModel : BaseViewModel
	{
		#region Properties
        [Required, MaxLength(250)]
        public string FirstName { get; set; }
        [Required, MaxLength(250)]
        public string LastName { get; set; }
        [Required, MaxLength(100)]
        public string IDIR { get; set; }
        [MaxLength(50)]
        public string Salutation { get; set; }
        [Required, MaxLength(500)]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }
        [MaxLength(20)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone Number.")]
        public string PhoneNumber { get; set; }
		public string ApplicationUserId { get; set; }
		[Required(ErrorMessage = "The Role field is required.")]
		public string RoleId { get; set; }
		public string Role { get; set; }
		public bool Active { get; set; }
		#endregion

		#region Constructors
		public UserViewModel() { }

		public UserViewModel(ApplicationUser user, List<ApplicationRole> roles) {
			ApplicationUserId = user.Id;
			FirstName = user.InternalUser.FirstName;
			LastName = user.InternalUser.LastName;
			Salutation = user.InternalUser.Salutation;
			IDIR = user.InternalUser.IDIR;
			RoleId = roles.Find(r => r.Id == user.Roles.FirstOrDefault()?.RoleId).Id;
			Role = roles.Find(r => r.Id == user.Roles.FirstOrDefault()?.RoleId).Name;
			Email = user.InternalUser.Email;
			PhoneNumber = user.InternalUser.PhoneNumber;
			Active = user.Active != null && user.Active.Value;
		}
		#endregion
	}
}