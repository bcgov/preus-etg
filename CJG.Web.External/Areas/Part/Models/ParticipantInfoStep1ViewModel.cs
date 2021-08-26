using System.ComponentModel.DataAnnotations;
using System;
using CJG.Application.Business.Models;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Part.Models
{
    public class ParticipantInfoStep1ViewModel : StepCompletedViewModelBase
    {
        public string GrantProgramName { get; set; }

        public string ProgramEmployerName { get; set; }

        public string ProgramSponsorName { get; set; }

        public string ProgramDescription { get; set; }

        [UIHint("HiddenDate")]
        public DateTime ProgramStartDate { get; set; }

        public string TimeoutPeriod { get; set; }

        public ProgramTypes ProgramType { get; set; }

    }
}
