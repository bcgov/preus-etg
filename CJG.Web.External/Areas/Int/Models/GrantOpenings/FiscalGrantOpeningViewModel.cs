using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using CJG.Application.Services;
using CJG.Core.Interfaces.Service;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.GrantOpenings
{
	public class FiscalGrantOpeningViewModel : BaseViewModel
	{
		public List<FiscalGrantOpeningTrainingPeriod> TrainingPeriods { get; set; }
		public List<FiscalGrantOpeningGrantStream> GrantStreams { get; set; }
		public decimal GrantTotal { get; set; }
		public string DisplayGrantTotal
		{
			get
			{
				return GrantTotal.ToDollarCurrencyString(0);
			}
		}

		public FiscalGrantOpeningViewModel()
		{
		}

		public FiscalGrantOpeningViewModel(int fiscalId, int grantProgramId,
											IGrantOpeningService _grantOpeningService,
											IGrantProgramService _grantProgramService,
											IGrantStreamService _grantStreamService,
											IStaticDataService _staticDataService)
		{
			var _fiscalYear = _staticDataService.GetFiscalYear(fiscalId) ?? throw new NoContentException(); ;
			var _grantProgram = _grantProgramService.Get(grantProgramId);

			var trainingPeriods = _staticDataService.GetTrainingPeriodsForFiscalYear(fiscalId);
			this.TrainingPeriods = new List<FiscalGrantOpeningTrainingPeriod>();
			foreach (var t in trainingPeriods)
			{
				this.TrainingPeriods.Add(new FiscalGrantOpeningTrainingPeriod
				{
					Id = t.Id,
					Caption = t.Caption,
					StartDate = t.StartDate,
					EndDate = t.EndDate
				});
			}

			var grantStreams = _grantStreamService.GetGrantStreamsForProgram(grantProgramId);
			this.GrantStreams = new List<FiscalGrantOpeningGrantStream>();

			foreach (var s in grantStreams.ToList())
			{
				var stream = new FiscalGrantOpeningGrantStream
				{
					Id = s.Id,
					Name = s.Name,
					IsActive = s.IsActive,
					GrantStreamInformation = new List<GrantStreamInformation>()
				};
				foreach (var t in this.TrainingPeriods)
				{
					var info = new GrantStreamInformation
					{
						TrainingPeriodId = t.Id,
						DisplayMode = DisplayMode.GM1
					};
					var opening = _grantOpeningService.GetGrantOpening(s.Id, t.Id);
					if (opening != null)
					{
						info.OpenState = (GrantOpeningStates)Enum.Parse(typeof(GrantOpeningStates), opening.State.ToString());
						info.TargetAmount = opening.BudgetAllocationAmt;
						stream.TotalTargetAmount += opening.BudgetAllocationAmt;
						this.GrantTotal += opening.BudgetAllocationAmt;
						info.DisplayMode = DisplayMode.OpeningLocated;
						info.GrantOpeningId = opening.Id;
						t.TotalTrainingPeriodAmount += opening.BudgetAllocationAmt;
					}
					if (t.EndDate < AppDateTime.UtcNow && opening == null)
					{
						info.DisplayMode = DisplayMode.None;
					}
					stream.GrantStreamInformation.Add(info);
				}
				this.GrantStreams.Add(stream);
			}
		}
	}
}
