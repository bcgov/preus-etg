using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.ChangeRequest
{
	public class ApplicationAddressViewModel : AddressSharedViewModel
	{
		#region Properties
		#endregion

		#region Constructors
		public ApplicationAddressViewModel() { }

		public ApplicationAddressViewModel(ApplicationAddress address) : base(address)
		{

		}
		#endregion
	}
}