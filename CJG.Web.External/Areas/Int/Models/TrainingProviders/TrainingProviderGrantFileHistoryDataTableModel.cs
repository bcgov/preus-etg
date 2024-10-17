using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.TrainingProviders
{
    public class TrainingProviderGrantFileHistoryDataTableModel
    {
        public int Id { get; set; }
        public string FileNumber { get; set; }
        public string CurrentStatus { get; set; }
        public string ApplicantName { get; set; }
        public string TrainingProgramTitle { get; set; }
        public string StartDate { get; set; }
        public string NumberOfParticipants { get; set; }
        public string TotalGovernmentContribution { get; set; }
        public string AverageCostPerParticipant { get; set; }
        public string RowVersionString { get; set; }
        Func<decimal, decimal, decimal> CalculateAverage = (decimal x, decimal y) => x / y;

        public TrainingProviderGrantFileHistoryDataTableModel(GrantApplication grantApplication)
        {
            Id = grantApplication.Id;

            FileNumber = grantApplication.FileNumber;

            CurrentStatus = grantApplication.ApplicationStateInternal.GetDescription();

            ApplicantName = grantApplication.OrganizationLegalName;

            TrainingProgramTitle = grantApplication.TrainingPrograms.FirstOrDefault().CourseTitle;

            StartDate = grantApplication.StartDate.ToLocalMorning().ToString("yyyy-MM-dd");

            NumberOfParticipants = $"<span class='align-left'>R</span> {grantApplication.TrainingCost.EstimatedParticipants}"
                + $"<br /><span class='align-left'>A</span> "
                + $"{ grantApplication.TrainingCost.AgreedParticipants}";

            TotalGovernmentContribution = $"{grantApplication.TrainingCost.TotalEstimatedReimbursement.ToDollarCurrencyString(0)}" 
                + $"<br />"
                + $"{grantApplication.TrainingCost.AgreedCommitment.ToDollarCurrencyString(0)}";

            AverageCostPerParticipant = string.Format("{0}<br />{1}",
                CalculateAverage(grantApplication.EstimatedNonTravelGovernmentReimbursement(), (grantApplication.TrainingCost.EstimatedParticipants != 0 ? grantApplication.TrainingCost.EstimatedParticipants : 1)).ToDollarCurrencyString(0),
                CalculateAverage(grantApplication.AgreedNonTravelGovernmentReimbursement(), (grantApplication.TrainingCost.AgreedParticipants != 0 ? grantApplication.TrainingCost.AgreedParticipants : 1)).ToDollarCurrencyString(0)
            );

            RowVersionString = Convert.ToBase64String(grantApplication.RowVersion);
        }
    }
}