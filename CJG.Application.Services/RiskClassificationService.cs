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
	/// RiskClassificationService class, provides a way to manage risk classifications.
	/// </summary>
	public class RiskClassificationService : Service, IRiskClassificationService
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public RiskClassificationService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public RiskClassification Get(int id)
		{
			return Get<RiskClassification>(id);
		}

		public IEnumerable<RiskClassification> GetAll()
		{
			return _dbContext.RiskClassifications.ToList();
		}

		public IEnumerable<RiskClassification> GetAll(bool isActive)
		{
			return _dbContext.RiskClassifications.AsNoTracking().Where(x => x.IsActive == isActive).ToList();
		}

		public RiskClassification Add(RiskClassification riskClassification)
		{
			_dbContext.RiskClassifications.Add(riskClassification);
			_dbContext.Commit();

			return riskClassification;
		}

		public RiskClassification Update(RiskClassification riskClassification)
		{
			_dbContext.Update(riskClassification);
			_dbContext.Commit();

			return riskClassification;
		}

		public void Delete(RiskClassification riskClassification)
		{
			Remove(riskClassification);
			_dbContext.Commit();
		}
		#endregion
	}
}
