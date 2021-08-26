using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class AccountCodeService : Service, IAccountCodeService
	{
		public AccountCodeService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		public IEnumerable<AccountCode> GetAll()
		{
			return _dbContext.AccountCodes.ToList();
		}
		
		public AccountCode Get(int id)
		{
			return this.Get<AccountCode>(id) ?? throw new NoContentException();
		}
		
		public void Add(AccountCode accountCode)
		{
			_dbContext.AccountCodes.Add(accountCode);
			_dbContext.Commit();
		}

		public void Update(AccountCode accountCode)
		{
			_dbContext.Update<AccountCode>(accountCode);
			_dbContext.Commit();
		}

		public void Delete(AccountCode accountCode)
		{
			_dbContext.AccountCodes.Remove(accountCode);
			_dbContext.Commit();
		}
	}
}
