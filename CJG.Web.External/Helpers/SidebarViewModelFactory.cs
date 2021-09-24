using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models;

namespace CJG.Web.External.Helpers
{
	public static class SidebarViewModelFactory
	{
		private static readonly SidebarBuilder SidebarBuilder = new SidebarBuilder();

		public static SidebarViewModel Create(GrantApplication grantApplication, ControllerContext controllerContext)
		{
			return Create(grantApplication, GetPath(controllerContext));
		}

		public static SidebarViewModel Create(GrantApplication grantApplication, string path)
		{
			return SidebarBuilder.Build(grantApplication, path);
		}

		private static string GetPath(ControllerContext controllerContext)
		{
			return $"/{controllerContext.RouteData.Values["controller"]}/{controllerContext.RouteData.Values["action"]}";
		}
	}

	public class SidebarBuilder
	{
		private readonly List<SideBarLinkDefinition> _links;

		private class SideBarLinkDefinition
		{
			public SideBarLinkDefinition(SidebarLinkType linkType, SidebarLinkViewModel viewModel, string containerPath)
			{
				ViewModel = viewModel;
				ContainerPath = containerPath;
				LinkType = linkType;
			}

			public string ContainerPath { get; }
			public ApplicationStateExternal[] AllowedExternalStates { get; set; }
			public string[] AllowedPaths { get; set; }
			public SidebarLinkType LinkType { get; }
			public SidebarLinkViewModel ViewModel { get; }
			public Func<GrantApplication, bool> AndConditionFunc { get; set; }
			public Func<GrantApplication, bool> OrConditionFunc { get; set; }
		}

		// TODO: Need to rewrite this to not have hardcoded URL paths.
		public SidebarBuilder()
		{
			var currentDate = AppDateTime.UtcNow;

			_links = new List<SideBarLinkDefinition>
			{
				new SideBarLinkDefinition(SidebarLinkType.GrantFiles, new SidebarLinkViewModel("Reporting"), "/Reporting/GrantFileView")
				{
					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.Approved,
						ApplicationStateExternal.ChangeRequestApproved,
						ApplicationStateExternal.ChangeRequestSubmitted,
						ApplicationStateExternal.ChangeRequestDenied,
						ApplicationStateExternal.ClaimSubmitted,
						ApplicationStateExternal.ClaimApproved,
						ApplicationStateExternal.ClaimDenied,
						ApplicationStateExternal.ClaimReturned,
						ApplicationStateExternal.CancelledByMinistry,
						ApplicationStateExternal.CancelledByAgreementHolder,
						ApplicationStateExternal.Closed
					},
					AllowedPaths = new[]
					{
						"/Reporting/ClaimReportView",
						"/ApplicationView/ApplicationDetailsView",
						"/GrantAgreement/AgreementOverviewView",
						"/Application/ApplicationOverviewView",
						"/Claim/DetailsView",
						"/CompletionReporting/CompletionReportView",
						"/AlternateContact/AlternateContactView"
					}
				},
				new SideBarLinkDefinition(SidebarLinkType.ApplicationView, new SidebarLinkViewModel("View Application"), "/Application/ApplicationOverviewView")
				{
					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.AcceptGrantAgreement,
						ApplicationStateExternal.Approved,
						ApplicationStateExternal.ChangeRequestApproved,
						ApplicationStateExternal.ChangeRequestSubmitted,
						ApplicationStateExternal.ChangeRequestDenied,
						ApplicationStateExternal.ClaimSubmitted,
						ApplicationStateExternal.ClaimApproved,
						ApplicationStateExternal.ClaimDenied,
						ApplicationStateExternal.ClaimReturned,
						ApplicationStateExternal.AmendClaim,
						ApplicationStateExternal.CancelledByMinistry,
						ApplicationStateExternal.CancelledByAgreementHolder,
						ApplicationStateExternal.Closed,
						ApplicationStateExternal.ReportCompletion
					},
					AllowedPaths = new[]
					{
						"/GrantAgreement/AgreementReviewView",
						"/GrantAgreement/AgreementOverviewView",
						"/Reporting/GrantFileView",
						"/Claim/ClaimReportView",
						"/GrantAgreement/CoverLetterView",
						"/GrantAgreement/ScheduleAView",
						"/GrantAgreement/ScheduleBView",
						"/Claim/DetailsView",
						"/Claim/AssessmentView",
						"/CompletionReporting/CompletionReportView",
						"/AlternateContact/AlternateContactView"
					}
				},
				new SideBarLinkDefinition(SidebarLinkType.ReviewAgreement, new SidebarLinkViewModel("Review Agreement"), "/GrantAgreement/AgreementReviewView")
				{
					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.AcceptGrantAgreement
					},
					AllowedPaths = new[]
					{
						"/ApplicationView/ApplicationDetailsView"
					}
				},
				new SideBarLinkDefinition(SidebarLinkType.AgreementOverview, new SidebarLinkViewModel("View Agreement"), "/GrantAgreement/AgreementOverviewView")
				{
					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.Approved,
						ApplicationStateExternal.ChangeRequestApproved,
						ApplicationStateExternal.ChangeRequestSubmitted,
						ApplicationStateExternal.ChangeRequestDenied,
						ApplicationStateExternal.ClaimSubmitted,
						ApplicationStateExternal.ClaimApproved,
						ApplicationStateExternal.ClaimDenied,
						ApplicationStateExternal.ClaimReturned,
						ApplicationStateExternal.CancelledByMinistry,
						ApplicationStateExternal.CancelledByAgreementHolder,
						ApplicationStateExternal.Closed,
						ApplicationStateExternal.AmendClaim,
						ApplicationStateExternal.ReportCompletion
					},
					AllowedPaths = new[]
					{
						"/Reporting/GrantFileView",
						"/Claim/ClaimReportView",
						"/Application/ApplicationOverviewView",
						"/ApplicationView/ApplicationDetailsView",
						"/Claim/Index",
						"/Claim/DetailsView",
						"/Claim/AssessmentView",
						"/CompletionReporting/CompletionReportView",
						"/AlternateContact/AlternateContactView"
					}
				},
				new SideBarLinkDefinition(SidebarLinkType.ViewParticipantList, new SidebarLinkViewModel("View Participant List"), "/ParticipantReporting/ParticipantReportingView")
				{
					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.ClaimSubmitted,
						ApplicationStateExternal.ClaimApproved,
						ApplicationStateExternal.ClaimDenied,
						ApplicationStateExternal.CancelledByMinistry,
						ApplicationStateExternal.CancelledByAgreementHolder,
						ApplicationStateExternal.Closed,
						ApplicationStateExternal.ReportCompletion
					},
					AllowedPaths = new[]
					{
						"/GrantAgreement/AgreementOverviewView",
						"/Reporting/GrantFileView",
						"/Claim/ClaimReportView",
						"/ApplicationView/ApplicationDetailsView",
						"/Claim/DetailsView",
						"/CompletionReporting/CompletionReportView"
					}
				},
				new SideBarLinkDefinition(SidebarLinkType.ViewClaim, new SidebarLinkViewModel("View Claim"), "/Claim/ClaimReportView")
				{
					// TODO: /ViewClaim ?? – View only the most recent version of a submitted claim.  
					// After assessment the claim information is shown in the assessment and this link is no longer available.

					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.ClaimSubmitted
					},
					AllowedPaths = new[]
					{
						"/GrantAgreement/AgreementOverviewView",
						"/Reporting/GrantFileView",
						"/ApplicationView/ApplicationDetailsView"
					}
				},
				new SideBarLinkDefinition(SidebarLinkType.WithdrawClaim, new SidebarLinkViewModel("Withdraw Claim"), "/Claim/DetailsView")
				{
					// TODO: /ViewClaim ?? – View only the most recent version of a submitted claim.  
					// After assessment the claim information is shown in the assessment and this link is no longer available.

					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.ClaimSubmitted
					},
					AllowedPaths = new[]
					{
						"/Claim/DetailsView"
					},
					AndConditionFunc = x=> x.ApplicationStateInternal == ApplicationStateInternal.NewClaim
				},
				new SideBarLinkDefinition(SidebarLinkType.CreateNewClaim, new SidebarLinkViewModel("Create New Claim"), "/Claim/AssessmentView")
				{
					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.ClaimDenied
					},
					AllowedPaths = new[]
					{
						"/Claim/AssessmentView"
					}
				},
				new SideBarLinkDefinition(SidebarLinkType.ViewCompletionReport, new SidebarLinkViewModel("View Completion Report"), "/CompletionReporting/CompletionReportView")
				{
					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.Closed
					},
					AllowedPaths = new[]
					{
						"/GrantAgreement/AgreementOverviewView",
						"/Reporting/GrantFileView",
						"/Claim/ClaimReportView",
						"/ApplicationView/ApplicationDetailsView"
					}
				},
				new SideBarLinkDefinition(SidebarLinkType.ChangeTrainingProvider, new SidebarLinkViewModel("Request Training Provider Change"), "/GrantAgreement/AgreementOverviewView")
				{
					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.Approved,
						ApplicationStateExternal.ChangeRequestApproved,
						ApplicationStateExternal.ChangeRequestDenied
					},
					AndConditionFunc = app =>  currentDate < app.StartDate
				},
				new SideBarLinkDefinition(SidebarLinkType.ChangeTrainingDates, new SidebarLinkViewModel("Change Training Start and End Date"), "/GrantAgreement/AgreementOverviewView")
				{
					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.Approved,
						ApplicationStateExternal.ChangeRequestApproved,
						ApplicationStateExternal.ChangeRequestSubmitted,
						ApplicationStateExternal.ChangeRequestDenied,
					},
					AndConditionFunc = app =>  currentDate < app.StartDate
				},
				new SideBarLinkDefinition(SidebarLinkType.AlternateContact, new SidebarLinkViewModel("Alternate Contact"), "/AlternateContact/AlternateContactView")
				{
					AllowedExternalStates = new[]
					{
						ApplicationStateExternal.AcceptGrantAgreement,
						ApplicationStateExternal.Approved,
						ApplicationStateExternal.ChangeRequestApproved,
						ApplicationStateExternal.ChangeRequestSubmitted,
						ApplicationStateExternal.ChangeRequestDenied,
						ApplicationStateExternal.ClaimSubmitted,
						ApplicationStateExternal.ClaimApproved,
						ApplicationStateExternal.ClaimDenied,
						ApplicationStateExternal.ClaimReturned,
						ApplicationStateExternal.AmendClaim,
						ApplicationStateExternal.Complete,
						ApplicationStateExternal.CancelledByMinistry,
						ApplicationStateExternal.CancelledByAgreementHolder,
						ApplicationStateExternal.Closed,
						ApplicationStateExternal.ReportCompletion
					},
					AllowedPaths = new[]
					{
						"/Application/ApplicationOverviewView",
						"/ApplicationView/ApplicationDetailsView",
						"/GrantAgreement/AgreementReviewView",
						"/GrantAgreement/AgreementOverviewView",
						"/Reporting/GrantFileView",
						"/Claim/ClaimReportView",
						"/GrantAgreement/CoverLetterView",
						"/GrantAgreement/ScheduleAView",
						"/GrantAgreement/ScheduleBView",
						"/Claim/DetailsView",
						"/Claim/AssessmentView",
						"/CompletionReporting/CompletionReportView"
					}
				}
			};
		}

		public SidebarViewModel Build(GrantApplication grantApplication, string path)
		{
			return new SidebarViewModel(grantApplication,
				_links.Where(x =>
					x.AllowedPaths != null && x.AllowedPaths.Contains(path, StringComparer.InvariantCultureIgnoreCase) &&
					(
						x.AllowedExternalStates.Contains(grantApplication.ApplicationStateExternal)
						? x.AndConditionFunc == null || x.AndConditionFunc(grantApplication)
						: x.OrConditionFunc != null && x.OrConditionFunc(grantApplication)
					)
				)
				.ToDictionary(x => x.LinkType, x => SetHighlighted(x.ViewModel, x.ContainerPath, path)));
		}

		private SidebarLinkViewModel SetHighlighted(SidebarLinkViewModel sidebarLinkViewModel, string containerPath, string currentPath)
		{
			sidebarLinkViewModel.IsHighlighted = string.Compare(currentPath, containerPath, StringComparison.InvariantCultureIgnoreCase) == 0;
			return sidebarLinkViewModel;
		}
	}
}