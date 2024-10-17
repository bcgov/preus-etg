using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Web.External.Areas.Part.Models
{
    public abstract class StepCompletedViewModelBase
    {
        [NotMapped]
        public bool IsCompleted { get; set; }
    }
}