using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models
{
    public class IntakeManagementViewModel
    {
        public enum NavigationCommand
        {
            None,
            Previous,
            Next
        }

        public List<TrainingPeriodViewModel> TrainingPeriods { get; set; }
        public int? TrainingPeriodId { get; set; }

        public int? GrantProgramId { get; set; }
        public IEnumerable<KeyValuePair<int, string>> GrantPrograms { get; set; }

        public int? GrantStreamId { get; set; }
        public IEnumerable<KeyValuePair<int, string>> GrantStreams { get; set; }

        public IEnumerable<KeyValuePair<int, string>> StateNames
        {
            get
            {
                return TrainingPeriods
                    .SelectMany(x => x.GrantOpeningIntakes)
                    .Select(x => new {x.Key, x.Value.StateName})
                    .Distinct()
                    .ToDictionary(x => x.Key, x => x.StateName);
            }
        }

        public class TrainingPeriodViewModel
        {
            public int Id { get; set; }
            public string FiscalYearName { get; set; }
            public string TrainingPeriodName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Status { get; set; }
            
            public Dictionary<int, GrantOpeningIntakeViewModel> GrantOpeningIntakes { get; set; }

            public int TotalApplicationsIntake { get; set; }
            public decimal TotalApplicationsIntakeAmt { get; set; }
            public decimal IntakeTargetAmt { get; set; }
            public decimal OverUnderAmt { get; set; }
            public decimal? OverUnderPerc { get; set; }
            public decimal? CurrentReservations { get; set; }
        }

        public class GrantOpeningIntakeViewModel
        {
            public GrantOpeningIntakeViewModel(string stateName, int number, decimal value)
            {
                StateName = stateName;
                Number = number;
                Value = value;
            }

            public string StateName { get; }
            public int Number { get; }
            public decimal Value { get; }
        }
    }
}