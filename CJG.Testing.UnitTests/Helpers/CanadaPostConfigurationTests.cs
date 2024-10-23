using System.Configuration;
using CJG.Web.External.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.Helpers
{
	[TestClass]
	public class CanadaPostConfigurationTests
	{
		private string _testKey;

		[TestInitialize]
		public void Setup()
		{
			_testKey = "1234-abcd-5678-efgh";
		}

		[TestMethod, TestCategory("Unit")]
		public void GetJSPath_WithoutKey_ReturnsPathButDoesNotExplode()
		{
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = null;

			var postConfig = new CanadaPostConfiguration();
			var path = postConfig.GetJsPath();
			path.Should().Be($"https://ws1.postescanada-canadapost.ca/js/addresscomplete-2.30.min.js?key=");
		}

		[TestMethod, TestCategory("Unit")]
		public void GetJSPath_WithKey_ReturnsPath()
		{
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = _testKey;

			var postConfig = new CanadaPostConfiguration();
			var path = postConfig.GetJsPath();
			path.Should().Be($"https://ws1.postescanada-canadapost.ca/js/addresscomplete-2.30.min.js?key={_testKey}");

			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = null;
		}

		[TestMethod, TestCategory("Unit")]
		public void GetCSSPath_WithKey_ReturnsPath()
		{
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = _testKey;

			var postConfig = new CanadaPostConfiguration();
			var path = postConfig.GetCssPath();
			path.Should().Be($"https://ws1.postescanada-canadapost.ca/css/addresscomplete-2.30.min.css?key={_testKey}");

			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = null;
		}

		[TestMethod, TestCategory("Unit")]
		public void GetCSSPath_WithHttps_IgnoresSetting()
		{
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = _testKey;
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiHttps"] = "false";

			var postConfig = new CanadaPostConfiguration();
			var path = postConfig.GetCssPath();
			path.Should().Be($"https://ws1.postescanada-canadapost.ca/css/addresscomplete-2.30.min.css?key={_testKey}");

			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = null;
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiHttps"] = null;
		}

		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithKeySetting_ReturnsKey()
		{
			// Can't mock out ConfigurationManager directly - no interface
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = _testKey;
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiHttps"] = "true";

			var postConfig = new CanadaPostConfiguration();
			var canadaPostKey = postConfig.Key;

			canadaPostKey.Should().Be(_testKey);

			// ConfigurationManager is persistent across tests, so we have to reset it
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = null;
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiHttps"] = null;
		}

		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithoutKeySetting_ReturnsBlank()
		{
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = null;
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiHttps"] = null;

			var postConfig = new CanadaPostConfiguration();
			var canadaPostKey = postConfig.Key;

			canadaPostKey.Should().Be(string.Empty);
		}
	}
}