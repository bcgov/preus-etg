using System;
using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Int.Models.Attachments;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models
{
    public class ProgramCostViewModel : BaseViewModel
    {
        public string RowVersion { get; set; }
        public TrainingCostModel TrainingCost { get; set; } = new TrainingCostModel();
        public AttachmentViewModel TravelExpenseDocument { get; set; } = new AttachmentViewModel();

		public ProgramCostViewModel()
        {
        }

        public ProgramCostViewModel(GrantApplication grantApplication)
        {
            Id = grantApplication.Id;
            RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
            TrainingCost = new TrainingCostModel(grantApplication);

			if (grantApplication.TrainingCost.TravelExpenseDocument != null)
				TravelExpenseDocument = new AttachmentViewModel(grantApplication.TrainingCost?.TravelExpenseDocument);
		}
    }
}