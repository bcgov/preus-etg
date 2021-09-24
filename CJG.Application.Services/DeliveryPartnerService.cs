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
	/// <typeparamref name="DeliveryPartnerService"/> class, provides a way to manage access to Delivery Partner related content.
	/// </summary>
	public class DeliveryPartnerService : Service, IDeliveryPartnerService
	{
		#region Constructors
		public DeliveryPartnerService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public DeliveryPartner GetDeliveryPartner(int id)
		{
			return Get<DeliveryPartner>(id);
		}

		public Core.Entities.DeliveryPartnerService GetDeliveryPartnerService(int id)
		{
			return Get<Core.Entities.DeliveryPartnerService>(id);
		}

		public IEnumerable<DeliveryPartner> GetDeliveryPartners(int grantProgramId)
		{
			return _dbContext.DeliveryPartners.AsNoTracking().Where(o => o.GrantProgramId == grantProgramId).ToList();
		}

		public IEnumerable<Core.Entities.DeliveryPartnerService> GetDeliveryPartnerServices(int grantProgramId)
		{
			return _dbContext.DeliveryPartnerServices.AsNoTracking().Where(o => o.GrantProgramId == grantProgramId).ToList();
		}

		/// <summary>
		/// Check if the delivery partner is selected by any grant applications.  If it has, it can't be deleted.
		/// </summary>
		/// <param name="deliveryPartner"></param>
		/// <returns></returns>
		public bool IsSelectedInApplication(DeliveryPartner deliveryPartner)
		{
			return _dbContext.GrantApplications.Any(ga => ga.DeliveryPartnerId == deliveryPartner.Id);
		}

		/// <summary>
		/// Check if the delivery partner service is selected by any grant applications.  If it has, it can't be deleted.
		/// </summary>
		/// <param name="deliveryPartnerService"></param>
		/// <returns></returns>
		public bool IsSelectedInApplication(Core.Entities.DeliveryPartnerService deliveryPartnerService)
		{
			return _dbContext.GrantApplications.Any(ga => ga.DeliveryPartnerServices.Any(dps => dps.Id == deliveryPartnerService.Id));
		}
		#endregion
	}
}
