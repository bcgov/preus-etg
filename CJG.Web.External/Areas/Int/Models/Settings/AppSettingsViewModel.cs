using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.Settings
{
    /// <summary>
    /// <typeparamref name="AppSettingsViewModel"/> class, provides a ViewModel for application settings.
    /// </summary>
    public class AppSettingsViewModel
	{
        #region Properties
        public IList<Setting> Settings { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
        #endregion

        #region Constructors
        public AppSettingsViewModel()
        {

        }

        public AppSettingsViewModel(ISettingService service)
        {
            this.Settings = service.GetAll().ToList();
        }
        #endregion
    }
}