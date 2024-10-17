using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IDeliveryPartnerService : IService
	{
		DeliveryPartner GetDeliveryPartner(int id);
		DeliveryPartnerService GetDeliveryPartnerService(int id);

		IEnumerable<DeliveryPartner> GetDeliveryPartners(int grantProgramId);
		IEnumerable<DeliveryPartnerService> GetDeliveryPartnerServices(int grantProgramId);

		bool IsSelectedInApplication(DeliveryPartner deliveryPartner);
		bool IsSelectedInApplication(Core.Entities.DeliveryPartnerService deliveryPartnerService);
	}
}
