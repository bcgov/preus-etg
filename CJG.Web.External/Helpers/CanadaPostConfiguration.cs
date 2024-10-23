using System.Configuration;

namespace CJG.Web.External.Helpers
{
	/// <summary>
	/// Get CanadaPost Autocomplete Api Configuration settings.
	///  - The configuration value "CanadaPostAutoCompleteApiKey" is required to be supplied in web.config AppSettings
	///  - The configuration value "CanadaPostAutoCompleteApiHttps" is now deprecated and won't be used
	/// </summary>
    public class CanadaPostConfiguration : ICanadaPostConfiguration
	{
		public string Key { get; set; }

		public CanadaPostConfiguration()
		{
			Key = ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] ?? string.Empty;
		}

		public string GetJsPath()
		{
			return $"https://ws1.postescanada-canadapost.ca/js/addresscomplete-2.30.min.js?key={Key}";
		}
	
		public string GetCssPath()
		{
			return $"https://ws1.postescanada-canadapost.ca/css/addresscomplete-2.30.min.css?key={Key}";
		}
	}

	public interface ICanadaPostConfiguration
	{
		string Key { get; set; }
		string GetJsPath();
		string GetCssPath();
	}
}