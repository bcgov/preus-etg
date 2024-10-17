using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Validation;
using CJG.Web.External.Models.Shared;
using DataAnnotationsExtensions;

namespace CJG.Web.External.Areas.Int.Models.ProgramDescriptions
{
    public class ProgramDescriptionViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }

		[MaxLength(300, ErrorMessage = "Program Description cannot be longer than 300 characters.")]
		[Required(ErrorMessage = "Program Description is required.")]
		public string Description { get; set; }

		[Required(ErrorMessage = "Number of Supporting Employers is required.")]
		public int? SupportingEmployers { get; set; }

		[Required(ErrorMessage = "Number of Participants must be greater than or equal to 1.")]
		[Max(1500, ErrorMessage = "Number of Participants must be less than or equal to 1500.")]
		[Min(1,ErrorMessage = "Number of Participants must be greater than or equal to 1.")]
		public int? NumberOfParticipants { get; set; }

		public int? NumberOfParticipantsAgreed { get; set; }

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
		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateNOC")]
		public int? Noc5Id { get; set; }

		public string ExistingNOC { get; set; }

		public IEnumerable<int> SelectedVulnerableGroupIds { get; set; }

		public IEnumerable<int> SelectedUnderRepresentedPopulationIds { get; set; }

		[Required(ErrorMessage = "Community Names are required.")]
		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateCommunities")]
		public IEnumerable<int> SelectedCommunityIds { get; set; }

		[RequiredEnumerable(ErrorMessage = "You must select at least one participant employment status.")]
		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateParticipantEmploymentStatuses")]
		public IEnumerable<int> SelectedParticipantEmploymentStatusIds { get; set; }

		public ProgramDescriptionViewModel()
		{ }

		public ProgramDescriptionViewModel(ProgramDescription programDescription,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService,
			INationalOccupationalClassificationService nationalOccupationalClassificationService)
		{
			if (programDescription == null)
				throw new ArgumentNullException(nameof(programDescription));

			if (naIndustryClassificationSystemService == null)
				throw new ArgumentNullException(nameof(naIndustryClassificationSystemService));

			if (nationalOccupationalClassificationService == null)
				throw new ArgumentNullException(nameof(nationalOccupationalClassificationService));

			Utilities.MapProperties(programDescription, this);
			Id = programDescription.GrantApplicationId;
			NumberOfParticipants = programDescription.RowVersion == null ? (int?)null : programDescription.GrantApplication.TrainingCost.EstimatedParticipants;
			SupportingEmployers = programDescription.RowVersion == null ? (int?)null : programDescription.SupportingEmployers;
			SelectedUnderRepresentedPopulationIds = programDescription.UnderRepresentedPopulations.Select(o => o.Id).ToArray();
			SelectedVulnerableGroupIds = programDescription.VulnerableGroups.Select(o => o.Id).ToArray();
			SelectedCommunityIds = programDescription.Communities.Select(c => c.Id).ToArray();
			SelectedParticipantEmploymentStatusIds = programDescription.ParticipantEmploymentStatuses.Select(c => c.Id).ToArray();

			ExistingNOC = $"{programDescription.NationalOccupationalClassification.Code} | {programDescription.NationalOccupationalClassification.Description} ({programDescription.NationalOccupationalClassification.NOCVersion})";

			#region NAICS and NOC
			var naics = naIndustryClassificationSystemService.GetNaIndustryClassificationSystems(programDescription.TargetNAICS.Id);
			var nocs = nationalOccupationalClassificationService.GetNationalOccupationalClassifications(programDescription.NationalOccupationalClassification.Id);

			naics.ForEach(item =>
			{
				var property = GetType().GetProperty($"Naics{item.Level}Id");
				property?.SetValue(this, item.Id);
			});

			// If we're using a matching NOC Coding, populate the dropdowns with the information to select from
			var useNocCoding = nationalOccupationalClassificationService.UseNocVersion;
			if (useNocCoding == programDescription.NationalOccupationalClassification.NOCVersion)
			{
				nocs.ForEach(item =>
				{
					var property = GetType().GetProperty($"Noc{item.Level}Id");
					property?.SetValue(this, item.Id);
				});
			}
			#endregion
		}

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

			if (SelectedVulnerableGroupIds != null) {
				var vulnerableGroups = vulnerableGroupService.GetVulnerableGroups(SelectedVulnerableGroupIds.ToArray());
				foreach (var vulnerableGroup in vulnerableGroups)
					programDescription.VulnerableGroups.Add(vulnerableGroup);
			}

			if (SelectedUnderRepresentedPopulationIds != null) {
				var underRepresentedPopulations = underRepresentedPopulationService.GetUnderRepresentedPopulations(SelectedUnderRepresentedPopulationIds.ToArray());
				foreach (var underRepresentedPopulation in underRepresentedPopulations)
					programDescription.UnderRepresentedPopulations.Add(underRepresentedPopulation);
			}

			if (SelectedCommunityIds != null) {
				var communities = SelectedCommunityIds == null ? communityService.GetCommunities(new int[] { }) : communityService.GetCommunities(SelectedCommunityIds.ToArray());
				foreach (var community in communities)
					programDescription.Communities.Add(community);
			}

			programDescription.TargetNAICSId = Naics5Id ?? Naics4Id ?? Naics3Id ?? Naics2Id ?? Naics1Id;
			programDescription.TargetNOCId = Noc5Id ?? Noc4Id ?? Noc3Id ?? Noc2Id ?? Noc1Id;
		}
	}
}