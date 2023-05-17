using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models
{
    public class ParticipantListViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }
		public IEnumerable<ParticipantViewModel> ParticipantInfo { get; set; }

		public ParticipantListViewModel() { }

		public ParticipantListViewModel(GrantApplication grantApplication, IParticipantService participantService)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (participantService == null)
				throw new ArgumentNullException(nameof(participantService));

			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);

			var ytdData = participantService.GetParticipantYTD(grantApplication);
			ParticipantInfo = participantService.GetParticipantFormsForGrantApplication(grantApplication.Id)
				.Select(pf => new ParticipantViewModel(pf))
				.ToList();

			ParticipantInfo.ForEach(p =>
				{
					if (ytdData != null && ytdData.ContainsKey(p.SIN))
						p.YTDFunded = ytdData[p.SIN];
				}
			);

			var claimWarnings = GetParticipantWarnings(grantApplication, participantService);
			if (!claimWarnings.Any())
				return;

			foreach (var participant in ParticipantInfo)
			{
				var warning = claimWarnings.FirstOrDefault(p => p.MappedParticipantFormId == participant.ParticipantId);
				if (warning == null)
					continue;

				participant.HasClaimWarnings = warning.CostsOnThisClaim + warning.CurrentClaims > warning.FiscalYearLimit;
			}
		}

		private List<ParticipantWarningModel> GetParticipantWarnings(GrantApplication grantApplication, IParticipantService participantService)
		{
			var currentClaim = grantApplication.GetCurrentClaim();
			var claimStatesToInclude = new List<ClaimState> { ClaimState.Complete, ClaimState.Unassessed, ClaimState.ClaimApproved };
			if (currentClaim == null)
				return new List<ParticipantWarningModel>();

			if (!claimStatesToInclude.Contains(currentClaim.ClaimState))
				return new List<ParticipantWarningModel>();

			var participantSinList = grantApplication.ParticipantForms.Select(pf => new { ParticipantFormId = pf.Id, pf.SIN }).ToList();

			var warnings = new List<ParticipantWarningModel>();

			var maxReimbursementAmount = grantApplication.MaxReimbursementAmt;
			var grantApplicationFiscal = grantApplication.GrantOpening.TrainingPeriod.FiscalYearId;

			var applicationClaimStatuses = new List<ApplicationStateInternal>
			{
				ApplicationStateInternal.Closed,
				ApplicationStateInternal.CompletionReporting
			};

			var claimCosts = currentClaim.EligibleCosts
				.SelectMany(ec => ec.ParticipantCosts)
				.ToList();

			foreach (var participant in grantApplication.ParticipantForms)
			{
				var otherParticipantForms = participantService.GetParticipantFormsBySIN(participant.SIN);

				var participantPayments = 0M;

				foreach (var form in otherParticipantForms.Where(opf => opf.GrantApplicationId != grantApplication.Id)
					.Where(opf => opf.GrantApplication.GrantOpening.TrainingPeriod.FiscalYearId == grantApplicationFiscal)
					.Where(opf => applicationClaimStatuses.Contains(opf.GrantApplication.ApplicationStateInternal)))
				{
					var totalPastCosts = form.ParticipantCosts.Sum(c => c.AssessedReimbursement);
					participantPayments += totalPastCosts;
				}

				var participantCosts = claimCosts
					.Where(cc => cc.ParticipantFormId == participant.Id)
					.Sum(pc => pc.ClaimParticipantCost);

				warnings.Add(new ParticipantWarningModel
				{
					MappedParticipantFormId = participantSinList.FirstOrDefault(p => p.SIN == participant.SIN)?.ParticipantFormId ?? 0,
					CostsOnThisClaim = participantCosts,
					CurrentClaims = participantPayments,
					FiscalYearLimit = maxReimbursementAmount
				});
			}

			return warnings;
		}

		private class ParticipantWarningModel
		{
			public int MappedParticipantFormId { get; set; }
			public decimal CurrentClaims { get; set; }
			public decimal FiscalYearLimit { get; set; }
			public decimal CostsOnThisClaim { get; set; }
		}
	}
}