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

			var claimWarnings = new ParticipantWarnings(participantService).GetParticipantWarnings(grantApplication);
			if (!claimWarnings.Any())
				return;

			foreach (var participant in ParticipantInfo)
			{
				var warning = claimWarnings.FirstOrDefault(p => p.MappedParticipantFormId == participant.ParticipantId);
				if (warning == null)
					continue;

				participant.HasClaimWarnings = warning.HasWarning();
			}
		}
	}
}