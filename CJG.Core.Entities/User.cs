using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="User"/> class, provides the ORM a way to manage external user information.
	/// </summary>
	public class User : EntityBase
	{
		#region Properties
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[NotMapped]
		public string BCeID { get; set; }

		[Required, Index(IsUnique = true)]
		public Guid BCeIDGuid { get; set; }

		[DefaultValue(AccountTypes.External)]
		public AccountTypes AccountType { get; set; } = AccountTypes.External;

		[MaxLength(250)]
		public string Salutation { get; set; }

		[Required, MaxLength(250)]
		public string FirstName { get; set; }

		[Required, MaxLength(250)]
		public string LastName { get; set; }

		[Required, MaxLength(500)]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string EmailAddress { get; set; }

		[MaxLength(500)]
		public string JobTitle { get; set; }

		[MaxLength(20)]
		[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone Number")]
		public string PhoneNumber { get; set; }

		[MaxLength(20)]
		[RegularExpression(@"[0-9]*")]
		public string PhoneExtension { get; set; }

		public int? PhysicalAddressId { get; set; }

		[ForeignKey(nameof(PhysicalAddressId))]
		public virtual Address PhysicalAddress { get; set; }

		public int? MailingAddressId { get; set; }

		[ForeignKey(nameof(MailingAddressId))]
		public virtual Address MailingAddress { get; set; }

		public bool IsOrganizationProfileAdministrator { get; set; }

		public bool IsSubscriberToEmail { get; set; }

		public int OrganizationId { get; set; }

		[ForeignKey(nameof(OrganizationId))]
		public virtual Organization Organization { get; set; }

		[XmlIgnore]
		public ICollection<BusinessContactRole> BusinessContactRoles { get; set; } = new List<BusinessContactRole>();

		public virtual UserPreference UserPreference { get; set; }

		[XmlIgnore]
		[Obsolete("This is no longer used. Potentially remove collection from database.")]
		public virtual ICollection<UserGrantProgramPreference> UserGrantProgramPreferences { get; set; } = new List<UserGrantProgramPreference>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="User"/> object.
		/// </summary>
		public User()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="User"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="bceId"></param>
		/// <param name="firstName"></param>
		/// <param name="lastName"></param>
		/// <param name="email"></param>
		/// <param name="organization"></param>
		/// <param name="physicalAddress"></param>
		/// <param name="mailingAddress"></param>
		public User(Guid bceId, string firstName, string lastName, string email, Organization organization, Address physicalAddress, Address mailingAddress = null)
		{
			if (bceId == Guid.NewGuid())
				throw new ArgumentNullException(nameof(bceId));

			if (string.IsNullOrEmpty(firstName))
				throw new ArgumentNullException(nameof(firstName));

			if (string.IsNullOrEmpty(lastName))
				throw new ArgumentNullException(nameof(lastName));

			if (string.IsNullOrEmpty(email))
				throw new ArgumentNullException(nameof(email));

			if (physicalAddress == null)
				throw new ArgumentNullException(nameof(physicalAddress));

			AccountType = AccountTypes.External;
			BCeIDGuid = bceId;
			FirstName = firstName;
			LastName = lastName;
			EmailAddress = email;
			PhysicalAddress = physicalAddress;
			PhysicalAddressId = physicalAddress.Id;
			MailingAddress = mailingAddress;
			MailingAddressId = mailingAddress?.Id;
			Organization = organization;
			OrganizationId = organization.Id;
		}
		#endregion
	}
}