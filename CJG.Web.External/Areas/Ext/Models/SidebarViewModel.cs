using System.Collections.Generic;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.Claims;

namespace CJG.Web.External.Areas.Ext.Models
{
	public enum SidebarLinkType
	{
		GrantFiles,
		ReviewAgreement,
		ChangeTrainingProvider,
		ChangeTrainingDates,
		ApplicationView,
		AgreementOverview,
		ViewParticipantList,
		ViewClaim,
		ViewCompletionReport,
		WithdrawClaim,
		CreateNewClaim,
		AlternateContact
	}

	public class SidebarLinkViewModel
	{
		public string Title { get; }
		public bool IsHighlighted { get; set; }

		public SidebarLinkViewModel(string title)
		{
			Title = title;
		}
	}

	public class SidebarViewModel
	{
		public Dictionary<SidebarLinkType, SidebarLinkViewModel> SideBarLinks { get; }

		public GrantApplication GrantApplication { get; }
		public SidebarClaimModel CurrentClaim { get; set; }
		public List<ParticipantForm> ApprovedParticipants { get; }
		public string CurrentPath { get; set; }
		public bool ShowClaimInformation { get; set; }

		public SidebarViewModel(GrantApplication grantApplication, Dictionary<SidebarLinkType, SidebarLinkViewModel> sideBarLinks)
		{
			GrantApplication = grantApplication;
			CurrentClaim = new SidebarClaimModel(grantApplication);
			ApprovedParticipants = grantApplication.ApprovedParticipants();
			SideBarLinks = sideBarLinks;
		}
	}
}