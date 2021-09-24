using System.Collections.Generic;
using CJG.Core.Entities;

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
        public SidebarLinkViewModel(string title)
        {
            Title = title;
        }

        public string Title { get; }

        public bool IsHighlighted { get; set; }
    }

    public class SidebarViewModel
    {
        public Dictionary<SidebarLinkType, SidebarLinkViewModel> SideBarLinks { get; }

        public GrantApplication GrantApplication { get; }
		public List<ParticipantForm> ApprovedParticipants { get; }

		public SidebarViewModel(GrantApplication grantApplication, Dictionary<SidebarLinkType, SidebarLinkViewModel> sideBarLinks)
        {
            GrantApplication = grantApplication;
			ApprovedParticipants = grantApplication.ApprovedParticipants();
			SideBarLinks = sideBarLinks;
        }
    }
}