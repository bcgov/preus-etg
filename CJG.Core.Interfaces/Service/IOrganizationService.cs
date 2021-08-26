using CJG.Core.Entities;
using System.Collections.Generic;

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
	}
}
