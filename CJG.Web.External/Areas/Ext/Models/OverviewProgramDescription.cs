using CJG.Application.Services;
using CJG.Core.Entities;
using System.Collections.Generic;

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
            ProgramDescription programDescription = grantApplication.ProgramDescription ?? new ProgramDescription(grantApplication);

            Utilities.MapProperties(programDescription, this);
            this.ParticipantEmploymentStatus = programDescription.ParticipantEmploymentStatus;
            this.VulnerableGroups = programDescription.VulnerableGroups;
            this.UnderRepresentedPopulations = programDescription.UnderRepresentedPopulations;

            this.Communities = programDescription.Communities;
            this.ApplicantType = programDescription.ApplicantOrganizationType?.Caption == "Other" ? programDescription.ApplicantOrganizationTypeInfo :programDescription.ApplicantOrganizationType?.Caption;
            this.EstimatedParticipants = programDescription.GrantApplication.TrainingCost.EstimatedParticipants;

            this.NAICS = programDescription.TargetNAICS;
            this.NOC = programDescription.NationalOccupationalClassification;
        }
    }
}