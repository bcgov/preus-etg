using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="ExpenseTypeService"/> class, provides a way to manage access to training related content.
	/// </summary>
	public class ExpenseTypeService : Service, IExpenseTypeService
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ExpenseTypeService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public ExpenseType Get(ExpenseTypes id)
		{
			return Get<ExpenseType>(id);
		}

		public IEnumerable<ExpenseType> GetAll()
		{
			return _dbContext.ExpenseTypes.ToList();
		}

		public IEnumerable<ExpenseType> GetAll(bool isActive)
		{
			return _dbContext.ExpenseTypes.AsNoTracking().Where(t => t.IsActive == isActive);
		}
		#endregion
	}
}
