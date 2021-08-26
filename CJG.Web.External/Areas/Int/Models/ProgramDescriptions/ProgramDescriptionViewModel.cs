using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.ProgramDescriptions
{
	public class ProgramDescriptionViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }

		[MaxLength(300, ErrorMessage = "Program Description cannot be longer than 300 characters.")]
		[Required(ErrorMessage = "Program Description is required.")]
		public string Description { get; set; }

		[Required(ErrorMessage = "Number of Supporting Employers is required.")]
		public int? SupportingEmployers { get; set; }

		[Required(ErrorMessage = "Number of Participants must be greater than or equal to 1.")]
		[Max(1500, ErrorMessage = "Number of Participants must be less than or equal to 1500.")]
		public int? NumberOfParticipants { get; set; }

		public int? NumberOfParticipantsAgreed { get; set; }

		[Required(ErrorMessage = "Participant Employment Status is required.")]
		public int? ParticipantEmploymentStatusId { get; set; }

		[Required(ErrorMessage = "Applicant Type is required.")]
		public int? ApplicantOrganizationTypeId { get; set; }

		public string ApplicantOrganizationTypeInfo { get; set; }

		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateNAICS")]
		public int? Naics1Id { get; set; }

		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateNAICS")]
		public int? Naics2Id { get; set; }

		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateNAICS")]
		public int? Naics3Id { get; set; }

		public int? Naics4Id { get; set; }

		public int? Naics5Id { get; set; }

		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateNOC")]
		public int? Noc1Id { get; set; }

		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateNOC")]
		public int? Noc2Id { get; set; }

		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateNOC")]
		public int? Noc3Id { get; set; }

		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateNOC")]
		public int? Noc4Id { get; set; }

		public IEnumerable<int> SelectedVulnerableGroupIds { get; set; }

		public IEnumerable<int> SelectedUnderRepresentedPopulationIds { get; set; }

		[Required(ErrorMessage = "Community Names are required.")]
		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateCommunities")]
		public IEnumerable<int> SelectedCommunityIds { get; set; }
		#endregion

		#region Constructors
		public ProgramDescriptionViewModel()
		{ }

		public ProgramDescriptionViewModel(ProgramDescription programDescription,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService,
			INationalOccupationalClassificationService nationalOccupationalClassificationService)
		{
			if (programDescription == null) throw new ArgumentNullException(nameof(programDescription));
			if (naIndustryClassificationSystemService == null) throw new ArgumentNullException(nameof(naIndustryClassificationSystemService));
			if (nationalOccupationalClassificationService == null) throw new ArgumentNullException(nameof(nationalOccupationalClassificationService));

			Utilities.MapProperties(programDescription, this);
			this.Id = programDescription.GrantApplicationId;
			this.NumberOfParticipants = programDescription.RowVersion == null ? (int?)null : programDescription.GrantApplication.TrainingCost.EstimatedParticipants;
			this.SupportingEmployers = programDescription.RowVersion == null ? (int?)null : programDescription.SupportingEmployers;
			this.SelectedUnderRepresentedPopulationIds = programDescription.UnderRepresentedPopulations.Select(o => o.Id).ToArray();
			this.SelectedVulnerableGroupIds = programDescription.VulnerableGroups.Select(o => o.Id).ToArray();
			this.SelectedCommunityIds = programDescription.Communities.Select(c => c.Id).ToArray();

			#region NAICS and NOC
			var naics = naIndustryClassificationSystemService.GetNaIndustryClassificationSystems(programDescription.TargetNAICS.Id);
			var nocs = nationalOccupationalClassificationService.GetNationalOccupationalClassifications(programDescription.NationalOccupationalClassification.Id);

			naics.ForEach(item =>
			{
				var property = GetType().GetProperty($"Naics{item.Level}Id");
				property?.SetValue(this, item.Id);
			});

			nocs.ForEach(item =>
			{
				var property = GetType().GetProperty($"Noc{item.Level}Id");
				property?.SetValue(this, item.Id);
			});
			#endregion
		}
		#endregion

		#region Methods
		/// <summary>
		/// Copy the view model information into the specified program description.
		/// </summary>
		/// <param name="programDescription"></param>
		/// <param name="vulnerableGroupService"></param>
		/// <param name="underRepresentedPopulationService"></param>
		/// <param name="communityService"></param>
		public void MapToEntity(
			ProgramDescription programDescription,
			IVulnerableGroupService vulnerableGroupService,
			IUnderRepresentedPopulationService underRepresentedPopulationService,
			ICommunityService communityService)
		{
			if (programDescription == null) throw new ArgumentException(nameof(programDescription));
			if (vulnerableGroupService == null) throw new ArgumentNullException(nameof(vulnerableGroupService));
			if (underRepresentedPopulationService == null) throw new ArgumentNullException(nameof(underRepresentedPopulationService));
			if (communityService == null) throw new ArgumentNullException(nameof(communityService));

			Utilities.MapProperties(this, programDescription);

			programDescription.VulnerableGroups.Clear();
			programDescription.UnderRepresentedPopulations.Clear();
			programDescription.Communities.Clear();

			if (this.SelectedVulnerableGroupIds != null) {
				var vulnerableGroups = vulnerableGroupService.GetVulnerableGroups(this.SelectedVulnerableGroupIds.ToArray());
				foreach (var vulnerableGroup in vulnerableGroups)
					programDescription.VulnerableGroups.Add(vulnerableGroup);
			}

			if (this.SelectedUnderRepresentedPopulationIds != null) {
				var underRepresentedPopulations = underRepresentedPopulationService.GetUnderRepresentedPopulations(this.SelectedUnderRepresentedPopulationIds.ToArray());
				foreach (var underRepresentedPopulation in underRepresentedPopulations)
					programDescription.UnderRepresentedPopulations.Add(underRepresentedPopulation);
			}

			if (this.SelectedCommunityIds != null) { 
				var communities = this.SelectedCommunityIds == null ? communityService.GetCommunities(new int[] { }) : communityService.GetCommunities(this.SelectedCommunityIds.ToArray());
				foreach (var community in communities)
					programDescription.Communities.Add(community);
			}

			programDescription.TargetNAICSId = this.Naics5Id ?? this.Naics4Id ?? this.Naics3Id ?? this.Naics2Id ?? this.Naics1Id;
			programDescription.TargetNOCId = this.Noc4Id ?? this.Noc3Id ?? this.Noc2Id ?? this.Noc1Id;
		}
		#endregion
	}
}