using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="EligibleExpenseTypeService"/> class, provides service methods related to <typeparamref name="EligibleExpenseType"/> objects.
	/// </summary>
	public class EligibleExpenseTypeService : Service, IEligibleExpenseTypeService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ClaimServer"/> and initializes the specified properties.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public EligibleExpenseTypeService(
			IDataContext context,
			HttpContextBase httpContext,
			ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public EligibleExpenseType Get(int id)
		{
			return Get<EligibleExpenseType>(id);
		}

		public EligibleExpenseType Add(EligibleExpenseType eligibleExpenseType)
		{
			if (eligibleExpenseType == null)
				throw new ArgumentNullException(nameof(eligibleExpenseType));

			_dbContext.EligibleExpenseTypes.Add(eligibleExpenseType);
			_dbContext.Commit();

			return eligibleExpenseType;
		}

		public EligibleExpenseType Update(EligibleExpenseType eligibleExpenseType)
		{
			if (eligibleExpenseType == null)
				throw new ArgumentNullException(nameof(eligibleExpenseType));

			Update(eligibleExpenseType);
			_dbContext.Commit();

			return eligibleExpenseType;
		}

		public void Delete(EligibleExpenseType eligibleExpenseType)
		{
			if (eligibleExpenseType == null)
				throw new ArgumentNullException(nameof(eligibleExpenseType));

			if (eligibleExpenseType.EligibleCosts.Any() || eligibleExpenseType.ClaimEligibleCosts.Any())
				throw new InvalidOperationException("This expense line is used in existing grant file and can’t be deleted");

			eligibleExpenseType.ProgramConfigurations.Clear();

			_dbContext.EligibleExpenseTypes.Remove(eligibleExpenseType);
			_dbContext.Commit();
		}
		#endregion
	}
}
