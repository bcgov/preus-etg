using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using CJG.Core.Entities.Helpers;

namespace CJG.Application.Services
{
	public class OrganizationService : Service, IOrganizationService
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public OrganizationService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		/// <summary>
		/// Get the organization for the specified 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Organization Get(int id)
		{
			return Get<Organization>(id);
		}

		/// <summary>
		/// Get a list of all the organizations.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Organization> Search(string keyword)
		{
			if (string.IsNullOrEmpty(keyword))
				return new Organization[] { };
			return _dbContext.Organizations.Where(o => o.LegalName.Contains(keyword)).OrderBy(o => o.LegalName).Take(100).ToArray();
		}

		public void AddOrganizationProfile(Organization newOrganization)
		{
			_dbContext.Organizations.Add(newOrganization);
			_dbContext.Commit();
		}

		public void UpdateOrganization(Organization organization)
		{
			_dbContext.Update(organization);
			_dbContext.Commit();
		}

		public int GetOrganizationProfileAdminUserId(int orgId)
		{
			return _dbContext.Users.FirstOrDefault(x => x.OrganizationId == orgId && x.IsOrganizationProfileAdministrator)?.Id ?? 0;
		}

		public bool IsOrganizationNaicsStatusUpdated(int orgId)
		{
			return _dbContext.Organizations.FirstOrDefault(x => x.Id == orgId).IsNaicsUpdated;
		}

		public OrganizationType GetOrganizationType(int id)
		{
			return _dbContext.OrganizationTypes.Find(id);
		}

		public OrganizationType GetDefaultOrganizationType()
		{
			return _dbContext.OrganizationTypes.Find(1);
		}

		public PrioritySector GetDefaultPrioritySector()
		{
			return _dbContext.PrioritySectors.Find(1);
		}

		public LegalStructure GetDefaultLegalStructure()
		{
			return _dbContext.LegalStructures.Find(1);
		}

		public void ClearOrganizationOldNaicsCode(int orgId)
		{
			_logger.Debug($"Clearing Organization Old Naics Code: [orgId: {orgId}]");
			var organization = _dbContext.Organizations.FirstOrDefault(x => x.Id == orgId);
			organization.NaicsId = 0;
			organization.RequiredProfileUpdate = true;

			_dbContext.Update(organization);
			_dbContext.Commit();
		}

		public int NotSubmittedGrantApplicationsForUser(int orgId, Guid userBCeID)
		{
			var defaultGrantProgramId = GetDefaultGrantProgramId();

			var applicationCount = (from p in _dbContext.ProgramDescriptions
									join g in _dbContext.GrantApplications
									on p.GrantApplicationId equals g.Id
									join n in _dbContext.NaIndustryClassificationSystems
									on p.TargetNAICSId equals n.Id
									where n.NAICSVersion == 2012 && g.ApplicantBCeID == userBCeID && g.OrganizationId == orgId &&
									(g.ApplicationStateExternal == ApplicationStateExternal.Complete || g.ApplicationStateExternal == ApplicationStateExternal.Incomplete)
									&& g.GrantOpening.GrantStream.GrantProgram.Id == defaultGrantProgramId
									select p).Count();

			return applicationCount;
		}

		public int NotSubmittedGrantApplications(int orgId)
		{
			var defaultGrantProgramId = GetDefaultGrantProgramId();

			var applicationCount = _dbContext.GrantApplications
				.Where(x => x.OrganizationId == orgId && (x.ApplicationStateExternal == ApplicationStateExternal.Complete || x.ApplicationStateExternal == ApplicationStateExternal.Incomplete))
				.Where(x => x.GrantOpening.GrantStream.GrantProgram.Id == defaultGrantProgramId)
				.Count();

			return applicationCount;
		}

		public PageList<Organization> GetOrganizationList(int page, int quantity, string search, bool? isActive = null)
		{
			var filtered = _dbContext.Organizations
				.Where(x => (string.IsNullOrEmpty(search) || x.LegalName.Contains(search) || x.DoingBusinessAs != null && x.DoingBusinessAs.Contains(search))
					&& (isActive == null)
				).OrderBy(x => x.LegalName);

			var total = filtered.Count();
			var result = filtered.Skip((page - 1) * quantity).Take(quantity);

			return new PageList<Organization>(page, quantity, total, result.ToArray());
		}

		public dynamic GetOrganizationYTD(int organizationId, int grantProgramId = 0)
		{
			var utcFiscalYearStart = AppDateTime.CurrentFYStartDateMorning.ToUniversalTime();
			var utcFiscalYearEnd = AppDateTime.CurrentFYEndDateMidnight.ToUniversalTime();

			var query = (from tc in _dbContext.TrainingCosts
						join ga in _dbContext.GrantApplications
						on tc.GrantApplicationId equals ga.Id
						where ga.OrganizationId == organizationId && ga.ApplicationStateInternal != ApplicationStateInternal.Draft
						&& ga.StartDate >= utcFiscalYearStart && ga.StartDate <= utcFiscalYearEnd
						&& (grantProgramId == 0 || ga.GrantOpening.GrantStream.GrantProgramId == grantProgramId)
						select tc).ToList();

			// Getting the Total "Paid".
			// This code is probably more complex than required. The value to be used is the value from ClaimAssessmentViewModel,
			// AmountPaidOrOwing. It is ("Paid or Owing to Date:") that displays in the Claim Assessment Summary page.
			// This value derives from the claims on a grant, sometimes minus the PaymentRequests if the claim is a SingleAmendableClaim. 

			// Get all the claims for all the relevant grant applications.
			var queryClaims = (from ga1 in _dbContext.GrantApplications
							 join cl in _dbContext.Claims
							 on ga1.Id equals cl.GrantApplicationId
							 where ga1.OrganizationId == organizationId && ga1.ApplicationStateInternal != ApplicationStateInternal.Draft
							 && ga1.StartDate >= utcFiscalYearStart && ga1.StartDate <= utcFiscalYearEnd
							 && (grantProgramId == 0 || ga1.GrantOpening.GrantStream.GrantProgramId == grantProgramId)
							 select cl).ToList();

			// Select the claims in the correct state.
			// (Do not select ClaimState.AmountOwing (which is used in ClaimExtensions.AmountPaidOrOwing()), b/c it should not include owing).
			var result = queryClaims.Where(q => q.ClaimState.In(ClaimState.ClaimApproved , ClaimState.PaymentRequested, ClaimState.ClaimPaid, ClaimState.AmountReceived));

			var singleAmenable = result.Where(c => c.ClaimTypeId == ClaimTypes.SingleAmendableClaim);
			var notSingleAmenable = result.Where(c => c.ClaimTypeId != ClaimTypes.SingleAmendableClaim);

			// Sum the two claim types, SingleAmendableClaim and the rest.
			decimal totalPaid =  singleAmenable.Count() == 0 ? 0 : singleAmenable.Sum(q => q.TotalAssessedReimbursement
				- (q.GrantApplication.PaymentRequests.Where(o => o.ClaimVersion != q.ClaimVersion).Sum(o => o.PaymentAmount)));

			totalPaid += notSingleAmenable.Count() == 0 ? 0 : notSingleAmenable.Sum(q => q.TotalAssessedReimbursement);

			dynamic dynObj = new System.Dynamic.ExpandoObject();
			dynObj.TotalRequested = query.Count() == 0 ? 0 : query.Sum(q => q.TotalEstimatedReimbursement);
			dynObj.TotalApproved = query.Count() == 0 ? 0 : query.Sum(q => q.CalculateApprovedAmount());
			dynObj.TotalPaid = totalPaid;

			return dynObj;
		}

		/// <summary>
		/// Should this profile be verified by a User
		/// </summary>
		/// <param name="organization"></param>
		/// <returns></returns>
		public bool ProfileSubjectToVerification(Organization organization)
		{
			if (organization?.DateUpdated == null)
				return false;

			if (organization.DateUpdated.Value.AddMonths(12).Date < AppDateTime.UtcNow)
				return true;

			return false;
		}
	}
}
