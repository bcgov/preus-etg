using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Areas.Ext.Models.ChangeRequest
{
    public class ChangeRequestESSServiceProviderViewModel
    {
        public int GrantApplicationId { get; set; }
        public int EligibleExpenseTypeId { get; set; }
        public string TrainingProviderName { get; set; }
        public int TrainingProviderId { get; set; }

        public string RequestTrainingProviderName { get; set; }
        public int RequestTrainingProviderId { get; set; }
        public ChangeRequestESSServiceProviderViewModel()
        {

        }
        public ChangeRequestESSServiceProviderViewModel(TrainingProvider trainingProvider, int grantApplicationId)
        {
            if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
            this.GrantApplicationId = grantApplicationId;
            this.TrainingProviderId = trainingProvider.Id;
            this.EligibleExpenseTypeId = trainingProvider.EligibleCost.EligibleExpenseTypeId;
            this.TrainingProviderName = trainingProvider.Name;

            var baseTrainingProvider = trainingProvider.OriginalTrainingProvider ?? trainingProvider;
            var requestTrainingProvider = baseTrainingProvider.RequestedTrainingProvider;
            if (requestTrainingProvider != null)
            {
                this.RequestTrainingProviderName = requestTrainingProvider.Name;
                this.RequestTrainingProviderId = requestTrainingProvider.Id;
            }
        }
    }
}