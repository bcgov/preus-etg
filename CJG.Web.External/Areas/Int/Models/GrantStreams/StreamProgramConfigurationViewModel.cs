using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.GrantStreams
{
	public class StreamProgramConfigurationViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }

		public string Caption { get; set; }

		public string Description { get; set; }

		public ClaimTypes ClaimTypeId { get; set; }

		public bool IsActive { get; set; }

		public decimal SkillsTrainingMaxEstimatedParticipantCosts { get; set; }

		public decimal ESSMaxEstimatedParticipantCost { get; set; }

		public string UserGuidanceCostEstimates { get; set; }

		public string UserGuidanceClaims { get; set; }

		public IEnumerable<EligibleExpenseTypeViewModel> EligibleExpenseTypes { get; set; }

		#endregion

		#region Constructors
		public StreamProgramConfigurationViewModel() { }

		public StreamProgramConfigurationViewModel(ProgramConfiguration programConfiguration)
		{
			Utilities.MapProperties(programConfiguration, this);

			this.EligibleExpenseTypes = programConfiguration.EligibleExpenseTypes.OrderBy(eet => eet.RowSequence).ThenBy(eet => eet.Caption).Select(eet => new EligibleExpenseTypeViewModel(eet)).ToArray();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Copy the values from this model into the specified grant stream configuration.
		/// </summary>
		/// <param name="grantStream"></param>
		/// <param name="_serviceCategoryService"></param>
		/// <param name="_serviceLineService"></param>
		public void MapTo(GrantStream grantStream, IServiceCategoryService serviceCategoryService, IServiceLineService serviceLineService)
		{
			if (grantStream == null) throw new ArgumentNullException(nameof(grantStream));

			var programConfiguration = grantStream.ProgramConfiguration ?? new ProgramConfiguration()
			{
				Caption = $"{grantStream.Name} - {Guid.NewGuid().ToString()}",
				IsActive = true,
				EligibleExpenseTypes = new List<EligibleExpenseType>()
			};

			Utilities.MapProperties(this, programConfiguration);

			foreach (var expenseType in EligibleExpenseTypes)
			{
				var serviceCategory = serviceCategoryService.Get(expenseType.ServiceCategoryId.Value);

				// Find the existing EligibleExpenseType and update it, or add a new one.
				var eligibleExpenseType = programConfiguration.EligibleExpenseTypes.SingleOrDefault(eet => eet.Id == expenseType.Id);
				if (eligibleExpenseType == null)
				{
					eligibleExpenseType = new EligibleExpenseType(serviceCategory, expenseType.ExpenseTypeId)
					{
						IsActive = expenseType.IsActive,
						RowSequence = expenseType.RowSequence
					};

					foreach (var breakdown in expenseType.Breakdowns)
					{
						var serviceLine = serviceLineService.Get(breakdown.ServiceLineId.Value);
						var eligibleExpenseBreakdown = new EligibleExpenseBreakdown(serviceLine, eligibleExpenseType)
						{
							IsActive = breakdown.IsActive,
							RowSequence = breakdown.RowSequence
						};
						eligibleExpenseType.Breakdowns.Add(eligibleExpenseBreakdown);

					}

					programConfiguration.EligibleExpenseTypes.Add(eligibleExpenseType);
				}
				else
				{
					eligibleExpenseType.Caption = serviceCategory.Caption;
					eligibleExpenseType.Description = serviceCategory.Description;
					eligibleExpenseType.IsActive = expenseType.IsActive;
					eligibleExpenseType.RowSequence = expenseType.RowSequence;
					eligibleExpenseType.ExpenseTypeId = expenseType.ExpenseTypeId;

					// Add/Update the ServiceLines as EligibleExpenseBreakdown.
					foreach (var breakdown in expenseType.Breakdowns)
					{
						var serviceLine = serviceLineService.Get(breakdown.ServiceLineId.Value);
						var eligibleExpenseBreakdown = eligibleExpenseType.Breakdowns.SingleOrDefault(b => b.ServiceLineId == serviceLine.Id);
						if (eligibleExpenseBreakdown == null)
						{
							eligibleExpenseBreakdown = new EligibleExpenseBreakdown(serviceLine, eligibleExpenseType)
							{
								IsActive = expenseType.IsActive,
								RowSequence = breakdown.RowSequence
							};
							eligibleExpenseType.Breakdowns.Add(eligibleExpenseBreakdown);
						}
						else
						{
							eligibleExpenseBreakdown.Caption = serviceLine.Caption;
							eligibleExpenseBreakdown.Description = serviceLine.Description;
							eligibleExpenseBreakdown.IsActive = breakdown.IsActive;
							eligibleExpenseBreakdown.RowSequence = serviceLine.RowSequence;
						}
					}
				}
			}

			grantStream.ProgramConfiguration = programConfiguration;
		}
		#endregion
	}
}
