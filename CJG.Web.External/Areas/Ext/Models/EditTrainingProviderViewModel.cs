using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class EditTrainingProviderViewModel
    {
        public int GrantApplicationId { get; set; }

        public TrainingProviderViewModel TrainingProviderViewModel { get; set; }

        public virtual List<Note> Notes { get; set; }

        public NoteViewModel NoteViewModel { get; set; }

        public string ReasonForTrainingProviderChange { get; set; }

        public EditTrainingProviderViewModel()
        {

        }

        public EditTrainingProviderViewModel(TrainingProvider trainingProvider, string allowFileAttachmentExtensions)
        {
            this.GrantApplicationId = trainingProvider.GrantApplicationId ?? trainingProvider.TrainingPrograms.FirstOrDefault().GrantApplicationId;
            this.TrainingProviderViewModel = new TrainingProviderViewModel(trainingProvider, allowFileAttachmentExtensions);
        }
    }
}