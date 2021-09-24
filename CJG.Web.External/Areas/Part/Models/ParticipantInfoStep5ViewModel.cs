using System.ComponentModel.DataAnnotations;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Part.Models
{
    public class ParticipantInfoStep5ViewModel : StepCompletedViewModelBase
    {
        public string ParticipantConsentBody { get; set; }

        [StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
        public string ConsentNameEntered { get; set; }
    }
}