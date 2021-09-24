using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models
{
    public class ProgramCostViewModel : BaseViewModel
    {
        public string RowVersion { get; set; }
        public TrainingCostModel TrainingCost { get; set; } = new TrainingCostModel();

        public ProgramCostViewModel()
        {

        }
        public ProgramCostViewModel(GrantApplication grantApplication)
        {
            this.Id = grantApplication.Id;
            this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
            this.TrainingCost = new TrainingCostModel(grantApplication);
        }
    }
}