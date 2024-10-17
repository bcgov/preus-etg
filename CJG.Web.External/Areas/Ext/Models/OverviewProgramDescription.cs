using System.Collections.Generic;
using CJG.Application.Services;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class OverviewProgramDescription
	{
		public int GrantApplicationId { get; set; }
		public string ApplicantType { get; set; }
		public string Description { get; set; }
		public ProgramDescriptionStates DescriptionState { get; set; }
		public int SupportingEmployers { get; set; }
		public ParticipantEmploymentStatus ParticipantEmploymentStatus { get; set; }
		public IEnumerable<ParticipantEmploymentStatus> ParticipantEmploymentStatuses { get; set; }
		public IEnumerable<VulnerableGroup> VulnerableGroups { get; set; }
		public IEnumerable<UnderRepresentedPopulation> UnderRepresentedPopulations { get; set; }
		public NaIndustryClassificationSystem NAICS { get; set; }
		public NationalOccupationalClassification NOC { get; set; }
		public IEnumerable<Community> Communities { get; set; }
		public int EstimatedParticipants { get; set; }

		public OverviewProgramDescription()
		{
		}

		public OverviewProgramDescription(GrantApplication grantApplication)
		{
			var programDescription = grantApplication.ProgramDescription ?? new ProgramDescription(grantApplication);

			Utilities.MapProperties(programDescription, this);

			ParticipantEmploymentStatus = programDescription.ParticipantEmploymentStatus;
			VulnerableGroups = programDescription.VulnerableGroups;
			UnderRepresentedPopulations = programDescription.UnderRepresentedPopulations;
			ParticipantEmploymentStatuses = programDescription.ParticipantEmploymentStatuses;

			Communities = programDescription.Communities;
			ApplicantType = programDescription.ApplicantOrganizationType?.Caption == "Other" ? programDescription.ApplicantOrganizationTypeInfo : programDescription.ApplicantOrganizationType?.Caption;
			EstimatedParticipants = programDescription.GrantApplication.TrainingCost.EstimatedParticipants;

			NAICS = programDescription.TargetNAICS;
			NOC = programDescription.NationalOccupationalClassification;
		}
	}
}