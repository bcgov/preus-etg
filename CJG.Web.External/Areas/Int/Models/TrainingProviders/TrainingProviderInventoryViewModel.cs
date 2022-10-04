using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.TrainingProviders
{
	public class TrainingProviderInventoryViewModel : BaseViewModel
	{
		public int GrantApplicationId { get; set; }
		public string Name { get; set; }
		public string Acronym { get; set; }
		public string Notes { get; set; }
		public bool IsActive { get; set; }
		public bool IsEligible { get; set; }
		public bool RiskFlag { get; set; }
		public string RowVersion { get; set; }

		public TrainingProviderInventoryViewModel() { }

		public TrainingProviderInventoryViewModel(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null)
				throw new ArgumentNullException(nameof(trainingProvider));

			Utilities.MapProperties(trainingProvider, this);

			var grantApplication = trainingProvider.GetGrantApplication();
			GrantApplicationId = grantApplication.Id;
		}

		public TrainingProviderInventoryViewModel(TrainingProviderInventory trainingProviderInventory)
		{
			if (trainingProviderInventory == null)
				throw new ArgumentNullException(nameof(trainingProviderInventory));

			Utilities.MapProperties(trainingProviderInventory, this);
		}
	}
}
