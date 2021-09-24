using CJG.Core.Entities;
using System;
using CJG.Web.External.Models.Shared;
using CJG.Core.Interfaces.Service;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Interfaces;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ParticipantListViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public IEnumerable<ParticipantViewModel> ParticipantInfo { get; set; }
		#endregion

		#region Constructors
		public ParticipantListViewModel() { }

		public ParticipantListViewModel(GrantApplication grantApplication, IParticipantService participantService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (participantService == null) throw new ArgumentNullException(nameof(participantService));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			var ytdData = participantService.GetParticipantYTD(grantApplication);
			this.ParticipantInfo = participantService.GetParticipantFormsForGrantApplication(grantApplication.Id).Select(pf => new ParticipantViewModel(pf)).ToArray();
			this.ParticipantInfo.AsQueryable().ForEach(p =>
			{
				if (ytdData != null && ytdData.ContainsKey(p.SIN))
					p.YTDFunded = ytdData[p.SIN];
			}
);

		}
		#endregion
	}
}