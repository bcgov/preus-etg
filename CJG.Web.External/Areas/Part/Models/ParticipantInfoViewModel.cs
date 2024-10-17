using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Part.Models
{
    public class ParticipantInfoViewModel : BaseViewModel
    {
		public string CanadaPostKey { get; set; }

	    public ParticipantInfoStep0ViewModel ParticipantInfoStep0ViewModel { get; set; }

        public ParticipantInfoStep1ViewModel ParticipantInfoStep1ViewModel { get; set; }

        public ParticipantInfoStep2ViewModel ParticipantInfoStep2ViewModel { get; set; }

        public ParticipantInfoStep3ViewModel ParticipantInfoStep3ViewModel { get; set; }

        public ParticipantInfoStep4ViewModel ParticipantInfoStep4ViewModel { get; set; }

        public ParticipantInfoStep5ViewModel ParticipantInfoStep5ViewModel { get; set; }

        public ParticipantInfoStep6ViewModel ParticipantInfoStep6ViewModel { get; set; }
    }
}
