using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Ext.Models.ChangeRequest
{
    public class ChangeRequestSkillTrainingViewModel
    {
        public int TrainingProgramId { get; set; }

        public string CourseTitle { get; set; }
        public string TrainingProviderName { get; set; }
        public int TrainingProviderId { get; set; }

        public string RequestTrainingProviderName { get; set; }
        public int RequestTrainingProviderId { get; set; }

        public ChangeRequestProgramTrainingDateViewModel ProgramTrainingDate { get; set; }

        public ChangeRequestSkillTrainingViewModel()
        {

        }
        public ChangeRequestSkillTrainingViewModel(TrainingProgram trainingProgram)
        {
            if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));
            this.TrainingProgramId = trainingProgram.Id;
            this.CourseTitle = trainingProgram.CourseTitle;

            this.TrainingProviderName = trainingProgram.TrainingProvider.Name;
            this.TrainingProviderId = trainingProgram.TrainingProvider.Id;

            var requestTrainingProvider = trainingProgram.RequestedTrainingProvider;
            if (requestTrainingProvider != null)
            {
                this.RequestTrainingProviderName = requestTrainingProvider.Name;
                this.RequestTrainingProviderId = requestTrainingProvider.Id;
            }

            this.ProgramTrainingDate = new ChangeRequestProgramTrainingDateViewModel(trainingProgram);
        }
    }
}