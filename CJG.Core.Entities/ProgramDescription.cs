using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CJG.Core.Entities
{
	/// <summary>
	/// ProgramDescription class, provides a way to capture information about the grant application program.
	/// This is a one-to-one relationship with the grant application.
	/// </summary>
	public class ProgramDescription : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key and foriegn key to the grant application.
		/// </summary>

		[Required, Key, ForeignKey(nameof(GrantApplication))]
		public int GrantApplicationId { get; set; }

		/// <summary>
		/// get/set - The parent grant application.
		/// </summary>
		public virtual GrantApplication GrantApplication { get; set; }

	    /// <summary>
		/// get/set - A description of the grant application training program.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage = "Program Description is required."), MaxLength(2000)]
		public string Description { get; set; }

		/// <summary>
		/// get/set - The current state of the program description.
		/// </summary>
		[DefaultValue(ProgramDescriptionStates.Incomplete)]
		public ProgramDescriptionStates DescriptionState { get; set; } = ProgramDescriptionStates.Incomplete;

		/// <summary>
		/// get/set - The number of supporting employers.
		/// </summary>
		[Range(0, int.MaxValue, ErrorMessage = "Supporting Employers must be greater than or equal to 0.")]
		public int SupportingEmployers { get; set; }

		/// <summary>
		/// get/set - The foreign key to the participant employment status.
		/// </summary>
		public int? ParticipantEmploymentStatusId { get; set; }

		/// <summary>
		/// get/set - The participant employment status.
		/// </summary>
		[ForeignKey(nameof(ParticipantEmploymentStatusId))]
		public virtual ParticipantEmploymentStatus ParticipantEmploymentStatus { get; set; }

		/// <summary>
		/// get/set - The foreign key to the national industry classification system.
		/// </summary>
		public int? TargetNAICSId { get; set; }

		/// <summary>
		/// get/set - The national industry classification system.
		/// </summary>
		[ForeignKey(nameof(TargetNAICSId))]
		public virtual NaIndustryClassificationSystem TargetNAICS { get; set; }

		/// <summary>
		/// get/set - The foreign key to the national occupational classification.
		/// </summary>
		public int? TargetNOCId { get; set; }

		/// <summary>
		/// get/set - The national occupational classification.
		/// </summary>
		[ForeignKey(nameof(TargetNOCId))]
		public virtual NationalOccupationalClassification NationalOccupationalClassification { get; set; }

		/// <summary>
		/// get/set - The foreign key to the applicant organization type.
		/// </summary>
		public int? ApplicantOrganizationTypeId { get; set; }

		/// <summary>
		/// get/set - The applicant organization type.
		/// </summary>
		[ForeignKey(nameof(ApplicantOrganizationTypeId))]
		public virtual ApplicantOrganizationType ApplicantOrganizationType { get; set; }

		/// <summary>
		/// get/set - The application organization type when the above is "Other".
		/// </summary>
		[MaxLength(200)]
		public string ApplicantOrganizationTypeInfo { get; set; }

		/// <summary>
		/// get - All of the selected communities for this program description.
		/// </summary>
		public virtual ICollection<Community> Communities { get; set; } = new List<Community>();

		/// <summary>
		/// get - All of the vulnerable groups for this program description.
		/// </summary>
		public virtual ICollection<VulnerableGroup> VulnerableGroups { get; set; } = new List<VulnerableGroup>();

		/// <summary>
		/// get - All of the under represented population for this program description.
		/// </summary>
		public virtual ICollection<UnderRepresentedPopulation> UnderRepresentedPopulations { get; set; } = new List<UnderRepresentedPopulation>();

		/// <summary>
		/// get/set - A collection of participant employment status associated with this program description.
		/// </summary>
		public virtual ICollection<ParticipantEmploymentStatus> ParticipantEmploymentStatuses { get; set; } = new List<ParticipantEmploymentStatus>();


		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ProgramDescription object.
		/// </summary>
		public ProgramDescription()
		{

		}

		/// <summary>
		/// Creates a new instance of a ProgramDescription object and initializes it.
		/// </summary>
		/// <param name="grantApplication"></param>
		public ProgramDescription(GrantApplication grantApplication)
		{
			this.GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
			this.GrantApplicationId = grantApplication.Id;
			grantApplication.ProgramDescription = this;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validate the ProgramDescription properties.
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

			// This is done to stop random errors from being thrown when updating NAICS codes systematically.
			if (DescriptionState == ProgramDescriptionStates.Incomplete && TargetNAICSId == 0)
				yield break;

			if (this.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.EmployerGrant)
				yield break;

			context.Set<ProgramDescription>().Include(m => m.Communities).FirstOrDefault(pd => pd.GrantApplicationId == this.GrantApplicationId);

			// Can't change the state to complete unless all required values are entered.
			if (this.DescriptionState == ProgramDescriptionStates.Complete)
			{
				// WDA Service program types must fill out the following program description information.
				if (this.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
				{
					// Target NAICS required.
					if (this.TargetNAICSId == null)
						yield return new ValidationResult($"The target NAICS must be selected.", new[] { nameof(this.TargetNAICSId) });

					/// Target NOC required.
					if (this.TargetNOCId == null)
						yield return new ValidationResult($"The target NOC must be selected.", new[] { nameof(this.TargetNOCId) });
				}
			}

			if (this.ApplicantOrganizationTypeId == 21 && string.IsNullOrEmpty(this.ApplicantOrganizationTypeInfo))
			{
				yield return new ValidationResult($"Applicant Other Type is required.", new[] { nameof(this.ApplicantOrganizationTypeInfo) });
			}

			// Community is required field
			if (Communities == null || Communities.Count < 1)
				yield return new ValidationResult("Communities must be selected.", new[] { nameof(Communities) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}

		public void Clone(ProgramDescription pd)
		{
			Description = pd.Description;
			DescriptionState = pd.DescriptionState;
			SupportingEmployers = pd.SupportingEmployers;
			ParticipantEmploymentStatusId = pd.ParticipantEmploymentStatusId;
			TargetNAICSId = pd.TargetNAICSId;
			TargetNOCId = pd.TargetNOCId;
			ApplicantOrganizationTypeId = pd.ApplicantOrganizationTypeId;
			ApplicantOrganizationTypeInfo = pd.ApplicantOrganizationTypeInfo;
			
			foreach (var community in pd.Communities)
			{
				Communities.Add(community);
			}

			foreach (var vulnerableGroup in pd.VulnerableGroups)
			{
				VulnerableGroups.Add(vulnerableGroup);
			}

			foreach (var underRepresentedPopulation in pd.UnderRepresentedPopulations)
			{
				UnderRepresentedPopulations.Add(underRepresentedPopulation);
			}

			foreach(var participantEmploymentStatus in pd.ParticipantEmploymentStatuses)
			{
				ParticipantEmploymentStatuses.Add(participantEmploymentStatus);
			}
		}
		#endregion
	}
}
