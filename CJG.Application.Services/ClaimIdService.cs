using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Data.Entity.Validation;
using System.Web;

namespace CJG.Application.Services
{
	public class ClaimIdService : Service, IClaimIdService
	{
		public ClaimIdService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		public int AddClaimId(ClaimId newClaimId)
		{
			try
			{
				_dbContext.ClaimIds.Add(newClaimId);
				_dbContext.Commit();

				return newClaimId.Id;
			}
			catch (DbEntityValidationException)
			{
				throw;
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't add claim ID");
				throw;
			}
		}
	}
}
