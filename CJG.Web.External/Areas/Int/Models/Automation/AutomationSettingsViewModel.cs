using System.Collections.Generic;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Automation
{
	public class AutomationSettingsViewModel : BaseViewModel
	{
		public bool ReturnedToUnassessedServiceState { get; set; }
		public List<KeyValuePair<string, bool>> StateOptions { get; set; }

		public AutomationSettingsViewModel()
		{
			var options = new List<KeyValuePair<string, bool>>
			{
				new KeyValuePair<string, bool>("On", true),
				new KeyValuePair<string, bool>("Off", false)
			};

			StateOptions = options;
		}
	}
}