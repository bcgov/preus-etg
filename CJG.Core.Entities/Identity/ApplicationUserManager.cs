using CJG.Core.Entities.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;

namespace CJG.Infrastructure.Identity
{
	public class ApplicationUserManager : UserManager<ApplicationUser>
    {
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store) { }

		/// <summary>
		/// Get all internal users in a paged list.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public PageList<ApplicationUser> GetUsers(int page, int quantity, UserFilter filter)
		{
			if (page <= 0) page = 1;
			if (quantity <= 0 || quantity > 100) quantity = 10;

			var query = Users.Where(x => true);
			var total = query.Count();

			// Search FirstName, LastName, and Email for search criteria
			if (!String.IsNullOrWhiteSpace(filter.SearchCriteria)) {
				query = query.Where(u => u.InternalUser.FirstName.Contains(filter.SearchCriteria) ||
										 u.InternalUser.LastName.Contains(filter.SearchCriteria) ||
										 u.InternalUser.Email.Contains(filter.SearchCriteria));
			}

			// Order columns by
			if (filter.OrderBy?.Any(f => f.Contains("Role")) ?? false) {
				if (filter.OrderBy.Any(x => x.Contains("desc"))) {
					query = query.OrderByDescending(u => u.Roles.FirstOrDefault().RoleId.ToString()).Skip((page - 1) * quantity).Take(quantity);
				} else {
					query = query.OrderBy(u => u.Roles.FirstOrDefault().RoleId.ToString()).Skip((page - 1) * quantity).Take(quantity);
				}
			} else {
				var orderBy = (filter.OrderBy != null && filter.OrderBy.Length > 0) ? filter.OrderBy : new[] { "InternalUser.FirstName" };
				query = query.OrderByProperty(orderBy).Skip((page - 1) * quantity).Take(quantity);
			}

			return new PageList<ApplicationUser>(page, quantity, total, query.ToArray());
		}
	}

	public class UserManagerAdapter : IUserManagerAdapter
    {
        private readonly ApplicationUserManager _applicationUserManager;

        public UserManagerAdapter(ApplicationUserManager applicationUserManager)
        {
            _applicationUserManager = applicationUserManager;
        }
        
        public ApplicationUser FindById(string userId)
        {
            return _applicationUserManager.FindById(userId);
        }
    }

    public interface IUserManagerAdapter
    {
        ApplicationUser FindById(string userId);
    }
}