using CJG.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="InternalUser"/> class, provides the ORM a way to manage internal users.
	/// </summary>
	public class InternalUser : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The user's first name.
		/// </summary>
		[Required, MaxLength(250)]
		[Index("IX_InternalUser_Name", 2)]
		public string FirstName { get; set; }

		/// <summary>
		/// get/set - The user's last name.
		/// </summary>
		[Required, MaxLength(250)]
		[Index("IX_InternalUser_Name", 1)]
		public string LastName { get; set; }

		/// <summary>
		/// get/set - The user's IDIR account.
		/// </summary>
		[Required, MaxLength(100)]
		[Index("IX_InternalUser", 1, IsUnique = true)]
		public string IDIR { get; set; }

		/// <summary>
		/// get/set - The user's salutation.
		/// </summary>
		[MaxLength(50)]
		public string Salutation { get; set; }

		/// <summary>
		/// get/set - The user's email address.
		/// </summary>
		[Required, MaxLength(500)]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		/// <summary>
		/// get/set - The user's primary phone number.
		/// </summary>
		[MaxLength(20)]
		[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone Number")]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// get/set - The user's primary phone number extension.
		/// </summary>
		[MaxLength(20)]
		[RegularExpression(@"[0-9]*")]
		public string PhoneNumberExt { get; set; }

		/// <summary>
		/// get - All the identity users assocated with this internal user (there should only be one).
		/// </summary>
		public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

		/// <summary>
		/// get - All the grant applications this user is the current assessor.
		/// </summary>
		public virtual ICollection<GrantApplication> AssessorApplications { get; set; }

		/// <summary>
		/// get - All of the claims this user is the current assessor.
		/// </summary>
		public virtual ICollection<Claim> AssessorClaims { get; set; }

		/// <summary>
		/// get - All the filters for this user.
		/// </summary>
		public virtual ICollection<InternalUserFilter> Filters { get; set; }

		/// <summary>
		/// get - All the grant programs this user is the expense authority.
		/// </summary>
		public virtual ICollection<GrantProgram> GrantPrograms { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="InternalUser"/> object.
		/// </summary>
		public InternalUser()
		{ }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="InternalUser"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="iDir"></param>
		/// <param name="firstName"></param>
		/// <param name="lastName"></param>
		/// <param name="email"></param>
		public InternalUser(string iDir, string firstName, string lastName, string email)
		{
			if (string.IsNullOrEmpty(iDir))
				throw new ArgumentNullException(nameof(iDir));

			if (String.IsNullOrEmpty(firstName))
				throw new ArgumentNullException(nameof(firstName));

			if (String.IsNullOrEmpty(lastName))
				throw new ArgumentNullException(nameof(lastName));

			if (String.IsNullOrEmpty(email))
				throw new ArgumentNullException(nameof(email));

			this.IDIR = iDir;
			this.FirstName = firstName;
			this.LastName = lastName;
			this.Email = email;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Returns the users identification in the following format $"{IDIR} - {LastName}, {FirstName} ({Email})".
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{IDIR} - {LastName}, {FirstName} ({Email})";
		}
		#endregion
	}
}
