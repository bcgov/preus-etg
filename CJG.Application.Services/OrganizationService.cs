using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class OrganizationService : Service, IOrganizationService
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public OrganizationService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
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
		#endregion
	}
}
