using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// ProgramConfiguration class, provides a way to control what types of claims and eligible expense types are available within a grant program or grant stream.
	/// </summary>
	public class ProgramConfiguration : EntityBase
	{
		#region Properties
		/// <summary>
		/// get - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The unique caption to identify the claim configuration.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage = "The caption is required."), MaxLength(200), Index("IX_ClaimType_Caption", IsUnique = true)]
		public string Caption { get; set; }

		/// <summary>
		/// get/set - The description of this claim configuration.
		/// </summary>
		[MaxLength(1000)]
		public string Description { get; set; }

		/// <summary>
		/// get/set - The foreign key to identify which claim type to use.
		/// </summary>
		[Index("IX_ProgramConfiguration_ClaimTypeId")]
		public ClaimTypes ClaimTypeId { get; set; }

		/// <summary>
		/// get/set - The claim type this configuration uses.
		/// </summary>
		[ForeignKey(nameof(ClaimTypeId))]
		public virtual ClaimType ClaimType { get; set; }

		/// <summary>
		/// get/set - Controls whether this claim configuration is active for use.
		/// </summary>
		[Index("IX_ProgramConfiguration_IsActive")]
		public bool IsActive { get; set; }

		/// <summary>
		/// get/set - Controls the maximum estimated participant costs for eligible cost types which belong to a service category with a service type of 'SkillsTraining'
		/// </summary>
		[Min(0, ErrorMessage = "The maximum skills training estimated participant cost must be greater than or equal to 0.")]
		[Max(999999.99, ErrorMessage = "The maximum skills training estimated participant cost must be less than or equal to 999999.99.")]
		public decimal SkillsTrainingMaxEstimatedParticipantCosts { get; set; }

		/// <summary>
		/// get/set - Controls the maximum estimated participant costs for eligible cost types which belong to a service category with a service type of 'EmploymentServicesAndSupports'
		/// </summary>
		[Min(0, ErrorMessage = "The maximum employment services and supports estimated participant cost must be greater than or equal to 0.")]
		[Max(999999.99, ErrorMessage = "The maximum employment services and supports estimated participant cost must be less than or equal to 999999.99.")]
		public decimal ESSMaxEstimatedParticipantCost { get; set; }

		/// <summary>
		/// get - All the grant programs that use this program configuration.
		/// </summary>
		public virtual ICollection<GrantProgram> GrantPrograms { get; set; } = new List<GrantProgram>();

		/// <summary>
		/// get - All the grant streams that use this program configuration.
		/// </summary>
		public virtual ICollection<GrantStream> GrantStreams { get; set; } = new List<GrantStream>();

		/// <summary>
		/// get/set - All the eligible expense types allowed to be used with this program configuration.
		/// </summary>
		public virtual ICollection<EligibleExpenseType> EligibleExpenseTypes { get; set; } = new List<EligibleExpenseType>();

		/// <summary>
		/// get/set - The user guidance for cost estimate
		/// </summary>
		[MaxLength(1000)]
		public string UserGuidanceCostEstimates { get; set; }

		/// <summary>
		/// get/set - The user guidance for claims
		/// </summary>
		[MaxLength(1000)]
		public string UserGuidanceClaims { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ProgramConfiguration.
		/// </summary>
		public ProgramConfiguration()
		{

		}

		/// <summary>
		/// Creates a new instance of a ProgramConfiguration and initializes it with the specified values.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="type"></param>
		/// <param name="isActive"></param>
		public ProgramConfiguration(string caption, ClaimTypes type, bool isActive = true)
		{
			this.Caption = caption ?? throw new ArgumentNullException(nameof(caption));
			this.ClaimTypeId = type;
			this.IsActive = isActive;
		}

		/// <summary>
		/// Creates a new instance of a ProgramConfiguration and initializes it with the specified values.
		/// </summary>
		/// <param name="claimType"></param>
		/// <param name="isActive"></param>
		public ProgramConfiguration(ClaimType claimType, bool isActive = true)
		{
			this.ClaimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
			this.ClaimTypeId = claimType.Id;
			this.Caption = claimType.Caption;
			this.IsActive = isActive;
		}

		/// <summary>
		/// Creates a new instance of a ProgramConfiguration and initializes it with the specified values.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="claimType"></param>
		/// <param name="isActive"></param>
		public ProgramConfiguration(string caption, ClaimType claimType, bool isActive = true)
		{
			this.Caption = caption ?? throw new ArgumentNullException(nameof(caption));
			this.ClaimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
			this.ClaimTypeId = claimType.Id;
			this.IsActive = isActive;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates this claim configuration before updating the datasource.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var context = validationContext.GetDbContext();
			var entry = validationContext.GetDbEntityEntry();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null)
				yield break;

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}
