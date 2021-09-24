using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class DeliveryPartnerViewModel : LookupTableViewModel
	{
		#region Properties
		public bool CanDelete { get; set; }
		public bool Delete { get; set; }
		#endregion

		#region Constructors
		public DeliveryPartnerViewModel()
		{
		}

		public DeliveryPartnerViewModel(DeliveryPartner entity, IDeliveryPartnerService deliveryPartnerService)
		{
			Utilities.MapProperties(entity, this);
			this.CanDelete = !deliveryPartnerService.IsSelectedInApplication(entity);
		}

		public DeliveryPartnerViewModel(Core.Entities.DeliveryPartnerService entity, IDeliveryPartnerService deliveryPartnerService)
		{
			Utilities.MapProperties(entity, this);
			this.CanDelete = !deliveryPartnerService.IsSelectedInApplication(entity);
		}
		#endregion
	}
}
