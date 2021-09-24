using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Xml.Serialization;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="Organization"/> class, provides the ORM a way to manage organization information.
	/// </summary>
	public class Organization : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key and unique identifier for the Organization.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The unique identifier from BCeID for the Organization.
		/// </summary>
		[Required, Index("IX_Organization_BCeID", IsUnique = true)]
		public Guid BCeIDGuid { get; set; }

		/// <summary>
		/// get/set - The organization legal name.
		/// </summary>
		[Required, MaxLength(250), Index("IX_Organization", 1)]
		public string LegalName { get; set; }

		/// <summary>
		/// get/set - Foreign key to the head office address.
		/// </summary>
		public int? HeadOfficeAddressId { get; set; }

		/// <summary>
		/// get/set - Head office address.
		/// </summary>
		[ForeignKey(nameof(HeadOfficeAddressId))]
		public virtual Address HeadOfficeAddress { get; set; }

		/// <summary>
		/// get/set - Foreign key to the organization type.
		/// </summary>
		[Index("IX_Organization", 2)]
		public int? OrganizationTypeId { get; set; }

		/// <summary>
		/// get/set - The organization type.
		/// </summary>
		[ForeignKey(nameof(OrganizationTypeId))]
		public virtual OrganizationType OrganizationType { get; set; }

		/// <summary>
		/// get/set - Foreign key to the legal structure.
		/// </summary>
		[Index("IX_Organization", 3)]
		public int? LegalStructureId { get; set; }

		/// <summary>
		/// get/set - The organization legal structure.
		/// </summary>
		[ForeignKey(nameof(LegalStructureId))]
		public virtual LegalStructure LegalStructure { get; set; }

		/// <summary>
		/// get/set - If the organization structure is not identified by the standard legal structures.
		/// </summary>
		[MaxLength(250)]
		public string OtherLegalStructure { get; set; }

		/// <summary>
		/// get/set - The year the organization was established.
		/// </summary>
		[Min(1800)]
		public int? YearEstablished { get; set; }

		/// <summary>
		/// get/set - The total number of employees in the organization worldwide.
		/// </summary>
		[Range(0, 999999, ErrorMessage = "The Number of Worldwide Employees must be between 0 and 999,999")]
		public int? NumberOfEmployeesWorldwide { get; set; }

		/// <summary>
		/// get/set - The total number of employees only within BC.
		/// </summary>
		[Range(0, 999999, ErrorMessage = "The Number of employees in BC must be between 0 and 999,999, and not less than the number worldwide")]
		public int? NumberOfEmployeesInBC { get; set; }

		/// <summary>
		/// get/set - The annual budgeted training allocation.
		/// </summary>
		[Range(0, 9999999, ErrorMessage = "The Annual Trained budget must be between 0 and $9,999,999")]
		public decimal? AnnualTrainingBudget { get; set; }

		/// <summary>
		/// get/set - The number of employees that receive training.
		/// </summary>
		[Range(0, 999999, ErrorMessage = "The Number of employees trained must be between 0 and 999,999")]
		public int? AnnualEmployeesTrained { get; set; }

		/// <summary>
		/// get/set - A description of what the business does.
		/// </summary>
		[MaxLength(500)]
		public string DoingBusinessAs { get; set; }

		/// <summary>
		/// get/set - An additional property for the ministry to identify the applicant organization more quickly.
		/// </summary>
		[MaxLength(500)]
		public string DoingBusinessAsMinistry { get; set; }

		/// <summary>
		/// get/set - The business number.
		/// </summary>
		[MaxLength(250)]
		public string BusinessNumber { get; set; }

		/// <summary>
		/// get/set - Whether the business number has been verified.
		/// </summary>
		public bool? BusinessNumberVerified { get; set; }

		/// <summary>
		/// get/set - Short identifier of the business type for this organization.
		/// </summary>
		[MaxLength(250)]
		public string BusinessType { get; set; }

		/// <summary>
		/// get/set - The statement of registration number.
		/// </summary>
		[MaxLength(250)]
		public string StatementOfRegistrationNumber { get; set; }

		/// <summary>
		/// get/set - The corporation number.
		/// </summary>
		[MaxLength(250)]
		public string IncorporationNumber { get; set; }

		/// <summary>
		/// get/set - The juristriction the organization was incorporated under.
		/// </summary>
		[MaxLength(250)]
		public string JurisdictionOfIncorporation { get; set; }

		/// <summary>
		/// get/set - The foriegn key for the NAICS
		/// </summary>
		public int? NaicsId { get; set; }

		/// <summary>
		/// get/set - The National Industry Classification System for this Organization.
		/// </summary>
		[ForeignKey(nameof(NaicsId))]
		public virtual NaIndustryClassificationSystem Naics { get; set; }

		/// <summary>
		/// get/set - The business license number.
		/// </summary>
		[MaxLength(20)]
		public string BusinessLicenseNumber { get; set; }

		/// <summary>
		/// get/set - Whether the NAICS code has been updated from 2012 to 2017 version
		/// </summary>
		public bool IsNaicsUpdated { get; set; }

		/// <summary>
		/// get/set - whether an organization is classified as risky or not
		/// </summary>
		public bool RiskFlag { get; set; }

		/// <summary>
		/// get/set - The notest associated with this organization.
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// get/set - The users associated with this Organization.
		/// </summary>
		[XmlIgnore]
		public ICollection<User> Users { get; set; } = new List<User>();
		[NotMapped]
		public bool RequiredProfileUpdate { get; set; } = false;

		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of <typeparamref name="Organization"/> object.
		/// </summary>
		public Organization()
		{ }

		/// <summary>
		/// Creates a new instance of <typeparamref name="Organization"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="organizationType"></param>
		/// <param name="bceId"></param>
		/// <param name="name"></param>
		/// <param name="legalStructure"></param>
		/// <param name="yearEstablished"></param>
		/// <param name="numberOfEmployeesWorldwide"></param>
		/// <param name="numberOfEmployeesInBc"></param>
		/// <param name="annualTrainingBudget"></param>
		/// <param name="annualEmployeesTrained"></param>
		public Organization(OrganizationType organizationType, Guid bceId, string name, LegalStructure legalStructure,
			int yearEstablished, int numberOfEmployeesWorldwide, int numberOfEmployeesInBc,
			decimal annualTrainingBudget, int annualEmployeesTrained)
		{
			if (organizationType == null)
				throw new ArgumentNullException(nameof(organizationType));

			if (bceId == Guid.NewGuid())
				throw new ArgumentException("BCeID field is required.", nameof(bceId));

			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if (legalStructure == null)
				throw new ArgumentNullException(nameof(legalStructure));

			if (yearEstablished < AppDateTime.UtcNow.AddYears(-250).Year || yearEstablished > AppDateTime.UtcNow.AddYears(1).Year)
				throw new ArgumentException($"The year established must be between '{AppDateTime.UtcNow.AddYears(-250).Year}' and '{AppDateTime.UtcNow.AddYears(1).Year}'.", nameof(yearEstablished));

			if (numberOfEmployeesWorldwide < 0 || numberOfEmployeesWorldwide < numberOfEmployeesInBc)
				throw new ArgumentException($"The number of employees worldwide must be equal to or more than the number of employees in BC which is {numberOfEmployeesInBc}.", nameof(numberOfEmployeesWorldwide));

			if (numberOfEmployeesInBc < 0)
				throw new ArgumentException("The number of employees in BC must be greater than or equal to 0.", nameof(numberOfEmployeesInBc));

			if (annualTrainingBudget < 0)
				throw new ArgumentException("The annual training budget must be greater than or equal to 0.", nameof(annualTrainingBudget));

			if (annualEmployeesTrained < 0)
				throw new ArgumentException("The number of employees trained annually must be greater than or equal to 0.", nameof(annualEmployeesTrained));

			this.OrganizationType = organizationType;
			this.OrganizationTypeId = organizationType.Id;
			this.BCeIDGuid = bceId;
			this.LegalName = name;
			this.LegalStructure = legalStructure;
			this.LegalStructureId = legalStructure.Id;
			this.YearEstablished = yearEstablished;
			this.NumberOfEmployeesWorldwide = numberOfEmployeesWorldwide;
			this.NumberOfEmployeesInBC = numberOfEmployeesInBc;
			this.AnnualTrainingBudget = annualTrainingBudget;
			this.AnnualEmployeesTrained = annualEmployeesTrained;
		}
		#endregion

		#region Methods
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var entry = validationContext.GetDbEntityEntry();
			var context = validationContext.GetDbContext();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null || context == null)
				yield break;

			if (entry.State == EntityState.Modified)
			{
				entry.GetDatabaseValues();
			}

			// Must be associated with an OrganizationType.
			if (this.OrganizationType == null && ((this.OrganizationTypeId == null && !this.RequiredProfileUpdate) || this.OrganizationTypeId == 0))
				yield return new ValidationResult("The organization must be associated to an organization type.", new[] { nameof(this.OrganizationType) });

			// Must be associated with an NAICS code.
			if (!this.RequiredProfileUpdate && this.Naics == null && (this.NaicsId == null))// || this.NaicsId==0))
				yield return new ValidationResult("The organization must be associated to an NAICS code.", new[] { nameof(this.Naics) });

			// Must be associated with an legal structure.
			if (this.LegalStructure == null && ((this.LegalStructureId == null && !this.RequiredProfileUpdate) || this.LegalStructureId == 0))
				yield return new ValidationResult("The organization must be associated to an legal structure.", new[] { nameof(this.LegalStructure) });

			// Must have a BCeID.
			if (this.BCeIDGuid == new Guid("00000000-0000-0000-0000-000000000000"))
				yield return new ValidationResult("BCeID field is required.", new[] { nameof(this.BCeIDGuid) });

			// Must have a YearEstablished no older than 250 years and no more than one year in the future.
			if ((!this.RequiredProfileUpdate && this.YearEstablished == null) || this.YearEstablished < AppDateTime.UtcNow.AddYears(-250).Year || this.YearEstablished > AppDateTime.UtcNow.AddYears(1).Year)
				yield return new ValidationResult($"The year established must be between '{AppDateTime.UtcNow.AddYears(-250).Year}' and '{AppDateTime.UtcNow.AddYears(1).Year}'.", new[] { nameof(this.YearEstablished) });

			// Must be greater than or equal to NumberOfEmployeesInBC.
			if ((!this.RequiredProfileUpdate && this.NumberOfEmployeesWorldwide == null) || this.NumberOfEmployeesWorldwide < this.NumberOfEmployeesInBC)
				yield return new ValidationResult($"The number of employees worldwide must be equal to or more than the number of employees in BC which is {this.NumberOfEmployeesInBC}.", new[] { nameof(this.NumberOfEmployeesWorldwide) });

			// Must be less than or equal to NumberOfEmployeesWorldwide.
			if ((!this.RequiredProfileUpdate && this.AnnualEmployeesTrained == null) || this.AnnualEmployeesTrained < 0 || this.AnnualEmployeesTrained > this.NumberOfEmployeesWorldwide)
				yield return new ValidationResult("The number of employees trained annually must be greater than or equal to 0, and not greater than the number of employees worldwide.", new[] { nameof(this.AnnualEmployeesTrained) });

			if (!this.RequiredProfileUpdate && this.AnnualTrainingBudget == null)
				yield return new ValidationResult("The Annual Trained budget must be between 0 and $9,999,999.", new[] { nameof(this.AnnualTrainingBudget) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}
