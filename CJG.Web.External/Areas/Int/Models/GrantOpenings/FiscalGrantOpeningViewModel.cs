using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using CJG.Core.Interfaces.Service;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.GrantOpenings
{
	public class FiscalGrantOpeningViewModel : BaseViewModel
	{
		// This is a template of all the periods for the fiscal year/program. Variable number of periods.
		public List<FiscalGrantOpeningTrainingPeriod> TemplateTrainingPeriods { get; set; }

		public List<FiscalGrantOpeningGrantStream> GrantStreams { get; set; }

		public decimal GrantTotal { get; set; }
		public string DisplayGrantTotal => GrantTotal.ToDollarCurrencyString(0);

		public FiscalGrantOpeningViewModel()
		{
		}

		public FiscalGrantOpeningViewModel(int fiscalId, int grantProgramId,
											IGrantOpeningService grantOpeningService,
											IGrantStreamService grantStreamService,
											ITrainingPeriodService trainingPeriodService)
		{
			var trainingPeriodsForFiscalYear = trainingPeriodService.GetAllFor(fiscalId, grantProgramId)
				.ToList();

			var distinctTrainingPeriods = trainingPeriodsForFiscalYear
				.Select(t => t.Caption)
				.Distinct();

			TemplateTrainingPeriods = new List<FiscalGrantOpeningTrainingPeriod>();
			foreach (var t in distinctTrainingPeriods)
			{
				TemplateTrainingPeriods.Add(new FiscalGrantOpeningTrainingPeriod
				{
					Caption = t
				});
			}

			var grantStreams = grantStreamService.GetGrantStreamsForProgram(grantProgramId);
			GrantStreams = new List<FiscalGrantOpeningGrantStream>();

			foreach (var grantStream in grantStreams.ToList())
			{
				var stream = new FiscalGrantOpeningGrantStream
				{
					Id = grantStream.Id,
					Name = grantStream.Name,
					IsActive = grantStream.IsActive,
					GrantStreamInformation = new List<GrantStreamInformation>()
				};

				foreach (var trainingPeriod in trainingPeriodsForFiscalYear.Where(tp => tp.GrantStreamId == grantStream.Id))
				{
					var info = new GrantStreamInformation
					{
						TrainingPeriodId = trainingPeriod.Id,
						DisplayMode = DisplayMode.GM1,
						StreamTrainingPeriod = new FiscalGrantOpeningTrainingPeriod
						{
							Id = trainingPeriod.Id,
							StartDate = trainingPeriod.StartDate,
							EndDate = trainingPeriod.EndDate,
							Caption = trainingPeriod.Caption,
							GrantStreamId = grantStream.Id, // Not sure we need this,
							IsActive = trainingPeriod.IsActive
						}
					};

					var opening = grantOpeningService.GetGrantOpening(grantStream.Id, trainingPeriod.Id);
					if (opening != null)
					{
						info.OpenState = (GrantOpeningStates)Enum.Parse(typeof(GrantOpeningStates), opening.State.ToString());
						info.TargetAmount = opening.BudgetAllocationAmt;
						stream.TotalTargetAmount += opening.BudgetAllocationAmt;
						GrantTotal += opening.BudgetAllocationAmt;
						info.DisplayMode = DisplayMode.OpeningLocated;
						info.GrantOpeningId = opening.Id;
						info.StreamTrainingPeriod.TotalTrainingPeriodAmount += opening.BudgetAllocationAmt;
					}

					if (trainingPeriod.EndDate < AppDateTime.UtcNow && opening == null)
						info.DisplayMode = DisplayMode.None;

					stream.GrantStreamInformation.Add(info);
				}

				GrantStreams.Add(stream);
			}
		}
	}
}
