using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Application.Business.Models
{
	public class RequestOnHoldModel
	{
		public List<RequestOnHoldClaimModel> FilesWithClaim { get; set; }
		public List<RequestOnHoldItemModel> FilesWithoutClaim { get; set; }
		public int Count => (FilesWithClaim?.Count ?? 0) + (FilesWithoutClaim?.Count ?? 0);
	}

	public class RequestOnHoldItemModel
	{
		public int FileId { get; set; }
		public string FileNumber { get; set; }
		public string Applicant { get; set; }

		public RequestOnHoldItemModel()
		{
		}

		public RequestOnHoldItemModel(GrantApplication grantApplication)
		{
			this.FileId = grantApplication.Id;
			this.FileNumber = grantApplication.FileNumber;
			this.Applicant = grantApplication.OrganizationLegalName;
		}
	}

	public class RequestOnHoldClaimModel : RequestOnHoldItemModel
	{
		public string ClaimVersion { get; set; }
		public decimal Assessed { get; set; }
		public string ClaimState { get; set; }
		public PaymentTypes Type { get; set; }
		public decimal Amount { get; set; }

		public RequestOnHoldClaimModel(Claim claim, DateTime fiscalYearStartDate)
		{
			this.FileId = claim.Id;
			this.FileNumber = claim.ClaimNumber;
			this.ClaimVersion = claim.ClaimVersion.ToString();
			this.Applicant = claim.GrantApplication.OrganizationLegalName;
			this.Assessed = claim.TotalAssessedReimbursement;
			this.ClaimState = claim.ClaimState.GetDescription();
			this.Amount = claim.AmountPaidOrOwing();
			this.Type = this.Amount == 0 ? PaymentTypes.None : claim.GrantApplication.StartDate < fiscalYearStartDate ? PaymentTypes.Accrual : PaymentTypes.Normal;
		}
	}
}
