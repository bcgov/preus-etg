using CJG.Core.Entities;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
    public class ApplicationTrainingCostReviewViewModel : BaseApplicationViewModel
    {
        #region Properties
        public string PreviousStepUrl { get; set; }
        public string NextStepUrl { get; set; }
        public int CurrentStep { get; set; }
        public int Steps { get; set; } = 2;
        #endregion

        #region Constructors
        public ApplicationTrainingCostReviewViewModel()
        {

        }

        public ApplicationTrainingCostReviewViewModel(GrantApplication grantApplication)
        {
            var hasOfferBeenIssued = grantApplication.HasOfferBeenIssued();
            this.Id = grantApplication.Id;
            this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
            this.ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;

            if (grantApplication.TrainingCost != null)
            {
                this.TrainingCost = new OverviewTrainingCost(grantApplication.TrainingCost);
                if (grantApplication.TrainingCost.EligibleCosts != null)
                {
                    this.TrainingCost.EstimatedCosts = grantApplication.TrainingCost.EligibleCosts.Where(ec => hasOfferBeenIssued || !ec.AddedByAssessor).OrderBy(ec => ec.EligibleExpenseType?.RowSequence).Select(x => new EstimatedCostViewModel(x, grantApplication)).ToArray();
                    this.TrainingCost.TotalCost = grantApplication.TrainingCost.TotalEstimatedCost;
                    this.TrainingCost.TotalRequest = grantApplication.TrainingCost.TotalEstimatedReimbursement;
                    this.TrainingCost.TotalEmployer = this.TrainingCost.TotalCost - this.TrainingCost.TotalRequest;
                    this.TrainingCost.ESSAveragePerParticipant = this.TrainingCost.EstimatedCosts.Where(x => x.ServiceType == (int?)ServiceTypes.EmploymentServicesAndSupports).Sum(x => x.EstimatedParticipantCost);
                }
            }

            if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
            {

                if (grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Any(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && x.IsActive))
                {
                    this.Steps += 1;
                    this.PreviousStepUrl = string.Format("/Ext/Application/Review/ESS/View/{0}", grantApplication.Id);
                }

                if (grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).Any())
                {
                    this.Steps = this.Steps + grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).Count();
                    if (string.IsNullOrEmpty(this.PreviousStepUrl))
                    {
                        var programConfiguration = grantApplication.GrantOpening.GrantStream.ProgramConfiguration;
                        this.PreviousStepUrl = string.Format("/Ext/Application/Review/Skills/Training/View/{0}/{1}", grantApplication.Id, programConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).OrderBy(x => x.RowSequence).ThenBy(x => x.Caption).Select(x => x.Id).Last());
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(this.PreviousStepUrl))
                    {
                        this.PreviousStepUrl = string.Format("/Ext/Application/Review/Program/View/{0}", grantApplication.Id);
                    }
                }

                this.Steps += 1;
                this.CurrentStep = this.Steps;

            }

            if (grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner)
            {
                this.Steps += 1;
                this.NextStepUrl = string.Format("/Ext/Application/Review/Delivery/Partner/View/{0}", grantApplication.Id);
            }
            else
            {
                this.NextStepUrl = string.Format("/Ext/Application/Review/Applicant/Declaration/View/{0}", grantApplication.Id);
            }

            this.Steps += 1;
        }
        #endregion
    }
}