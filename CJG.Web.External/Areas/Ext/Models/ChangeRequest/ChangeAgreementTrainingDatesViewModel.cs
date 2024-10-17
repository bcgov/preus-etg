using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Ext.Models.ChangeRequest
{
    public class ChangeAgreementTrainingDatesViewModel
    {
        public int GrantApplicationId { get; set; }

        public TrainingProvider TrainingProvider { get; set; }

        public DateTime? TermStartDate { get; set; }

        public DateTime? TermEndDate { get; set; }

        public int StartDay { get; set; }
        public int StartMonth { get; set; }
        public int StartYear { get; set; }
        public int EndDay { get; set; }
        public int EndMonth { get; set; }
        public int EndYear { get; set; }

        public DateTime TrainingPeriodStartDate { get; set; }

        public DateTime TrainingPeriodEndDate { get; set; }
    }
}