using System;
using CJG.Core.Entities;
using System.Collections.Generic;
using CJG.Core.Entities.Helpers;

namespace CJG.Core.Interfaces.Service
{
	public interface IOrganizationService : IService
	{
		Organization Get(int id);
		IEnumerable<Organization> Search(string keyword);

		void AddOrganizationProfile(Organization newOrganization);
		void UpdateOrganization(Organization organization);
		int GetOrganizationProfileAdminUserId(int orgId);
		OrganizationType GetOrganizationType(int id);
		OrganizationType GetDefaultOrganizationType();
		PrioritySector GetDefaultPrioritySector();
		LegalStructure GetDefaultLegalStructure();
		bool IsOrganizationNaicsStatusUpdated(int orgId);
		void ClearOrganizationOldNaicsCode(int orgId);
		int NotSubmittedGrantApplications(int orgId);
		int NotSubmittedGrantApplicationsForUser(int orgId, Guid userBCeID);
		int SubmittedGrantApplications(int orgId);
		bool RequiresBusinessLicenseDocuments(int orgId);
		PageList<Organization> GetOrganizationList(int page, int quantity, string search, bool? isActive = null, bool? isRisk = null);
		dynamic GetOrganizationYTD(int organizationId);
		bool ProfileSubjectToVerification(Organization organization);
	}
}
