using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ParticipantWarnings
	{
		private readonly IParticipantService _participantService;

		public ParticipantWarnings(IParticipantService participantService)
		{
			_participantService = participantService;
		}

		public List<ParticipantWarningModel> GetParticipantWarnings(GrantApplication grantApplication)
		{
			var currentClaim = grantApplication.GetCurrentClaim();
			var claimStatesToInclude = new List<ClaimState> { ClaimState.Complete, ClaimState.Unassessed, ClaimState.ClaimApproved };
			if (currentClaim == null)
				return new List<ParticipantWarningModel>();

			if (!claimStatesToInclude.Contains(currentClaim.ClaimState))
				return new List<ParticipantWarningModel>();

			var participantSinList = grantApplication.ParticipantForms
				.Select(pf => new
				{
					ParticipantFormId = pf.Id,
					pf.SIN
				})
				.ToList();

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
				var otherParticipantForms = _participantService.GetParticipantFormsBySIN(participant.SIN);

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
	}
}