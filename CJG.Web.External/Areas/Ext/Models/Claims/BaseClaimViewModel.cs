using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
	public class BaseClaimViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public ServiceTypes ServiceType { get; set; }
		public string FileNumber { get; set; }
		public string FileName { get; set; }
		public string GrantProgramName { get; set; }
		public string GrantStreamName { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public ApplicationStateExternal ApplicationStateExternal { get; set; }
		public GrantOpeningStates GrantOpeningState { get; set; }
		public DateTime? GrantOpeningOpeningDate { get; set; }
		public DateTime? DateSubmitted { get; set; }
		public ProgramTypes ProgramType { get; set; }
		public DateTime GrantOpeningTrainingPeriodStartDate { get; set; }
		public DateTime GrantOpeningTrainingPeriodEndDate { get; set; }
		public double ReimbursementRate { get; set; }
		public string ApplicationInternalStatus { get; set; }
		public string ApplicantName { get; set; }
		public DateTime? GrantAgreementStartDate { get; set; }
		public string TrainingProviderName { get; set; }
		public string DeliveryParterName { get; set; }
		public string LegalName { get; set; }
		public string UserGuidanceClaims { get; set; }
		public Dictionary<string, dynamic> Variables = new Dictionary<string, dynamic>();
		public bool RequireAllParticipantsBeforeSubmission { get; set; }		
		#endregion

		#region Constructors
		public BaseClaimViewModel()
		{

		}
		public BaseClaimViewModel(Claim claim)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));

			var grantApplication = claim.GrantApplication;
			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.FileNumber = claim.ClaimNumber;
			this.FileName = grantApplication.GetFileName();
			this.LegalName = grantApplication.Organization.LegalName;
			this.GrantProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name;
			this.ProgramType = grantApplication.GetProgramType();
			this.GrantStreamName = grantApplication.GrantOpening.GrantStream.Name;
			this.GrantOpeningOpeningDate = grantApplication.GrantOpening.OpeningDate.ToLocalMorning();
			this.StartDate = grantApplication.StartDate.ToLocalTime();
			this.EndDate = grantApplication.EndDate.ToLocalTime();
			this.ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			this.GrantOpeningState = grantApplication.GrantOpening.State;
			this.DateSubmitted = grantApplication.DateSubmitted?.ToLocalMorning();
			this.GrantOpeningTrainingPeriodStartDate = grantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime();
			this.GrantOpeningTrainingPeriodEndDate = grantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime();
			this.GrantAgreementStartDate = grantApplication.GrantAgreement?.StartDate.AddDays(5).ToLocalTime();
			this.ReimbursementRate = grantApplication.ReimbursementRate;
			this.ApplicationInternalStatus = grantApplication.ApplicationStateInternal.GetDescription();
			this.ApplicantName = grantApplication.Organization.LegalName;
			this.UserGuidanceClaims = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.UserGuidanceClaims;
			this.RequireAllParticipantsBeforeSubmission = grantApplication.RequireAllParticipantsBeforeSubmission;

			if (grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
			{
				this.TrainingProviderName = grantApplication.TrainingPrograms.First().TrainingProvider.Name;
				this.DeliveryParterName = grantApplication.DeliveryPartner?.Caption;
			}
		}
		#endregion
	}
}