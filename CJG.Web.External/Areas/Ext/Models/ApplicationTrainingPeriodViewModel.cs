using CJG.Core.Entities;
using CJG.Application.Services;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class ApplicationTrainingPeriodViewModel
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string ShortName { get; set; }
        public int FiscalYearId { get; set; }

        public ApplicationTrainingPeriodViewModel()
        {
        }

        public ApplicationTrainingPeriodViewModel(TrainingPeriod entity)
        {
            Utilities.MapProperties(entity, this);
        }
    }
}
