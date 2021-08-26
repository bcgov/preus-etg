using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Int.Models.Settings
{
	public class UserSettingViewModel : BaseViewModel
	{
		#region Properties
		[Required(ErrorMessage = "Setting 'key' value is required.")]
		public string Key { get; set; }

		public string Value { get; set; }

		public string ValueType { get; set; }
		#endregion

		#region Constructors
		public UserSettingViewModel() { }

		public UserSettingViewModel(Setting setting)
		{
			this.Key = setting.Key;
			this.Value = setting.Value;
			this.ValueType = setting.ValueType;
		}
		#endregion
	}
}