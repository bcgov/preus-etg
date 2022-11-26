using System.Configuration;
using System.Security.Principal;
using CJG.Testing.Core;
using CJG.Web.External.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CJG.Testing.UnitTests.Helpers
{
	[TestClass]
	public class CanadaPostConfigurationTests
	{
		private IPrincipal _identity;

		[TestInitialize]
		public void Setup()
		{
			_identity = EntityHelper.CreateExternalUser().CreateIdentity();
		}

		[TestMethod, TestCategory("Unit")]
		public void GetJSPath_WithDevelop_ReturnsPath()
		{
			var httpContextMock = _identity.MockHttpContext("https://localhost:1234");
			var postConfig = new CanadaPostConfiguration(httpContextMock.Object);

			var path = postConfig.GetJsPath();
			path.Should().Be($"https://ws1.postescanada-canadapost.ca/js/addresscomplete-2.30.min.js?key={CanadaPostConfiguration.DevelopmentKey}");

			httpContextMock = _identity.MockHttpContext("https://test.cjg.com");
			postConfig = new CanadaPostConfiguration(httpContextMock.Object);

			path = postConfig.GetJsPath();
			path.Should().Be("https://ws1.postescanada-canadapost.ca/js/addresscomplete-2.30.min.js?key=ng11-gy73-rr71-ta98");
		}

		[TestMethod, TestCategory("Unit")]
		public void GetCSSPath_WithDevelop_ReturnsPath()
		{
			var httpContextMock = _identity.MockHttpContext("https://localhost:1234");
			var postConfig = new CanadaPostConfiguration(httpContextMock.Object);

			var path = postConfig.GetCssPath();
			path.Should().Be($"https://ws1.postescanada-canadapost.ca/css/addresscomplete-2.30.min.css?key={CanadaPostConfiguration.DevelopmentKey}");

			httpContextMock = _identity.MockHttpContext("https://test.cjg.com");
			postConfig = new CanadaPostConfiguration(httpContextMock.Object);

			path = postConfig.GetCssPath();
			path.Should().Be("https://ws1.postescanada-canadapost.ca/css/addresscomplete-2.30.min.css?key=ng11-gy73-rr71-ta98");
		}

		[TestMethod, TestCategory("Unit")]
		public void Test_HttpContext_MockedCorrectly()
		{
			var httpContextMock = _identity.MockHttpContext("https://www.domain.com:808/Page/Default.aspx");

			var requestUrl = httpContextMock.Object.Request.Url;
			requestUrl.Host.Should().Be("www.domain.com");
			requestUrl.Port.Should().Be(808);
		}

		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithConfigurationSettings_ReturnsKey()
		{
			// Can't mock out ConfigurationManager directly - no interface
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = "1234-abcd-5678-efgh";
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiHttps"] = "true";

			AssertConfiguration("https://localhost:1234/Page/Default.aspx", "1234-abcd-5678-efgh", true);

			// ConfigurationManager is persistent across tests, so we have to reset it
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = null;
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiHttps"] = null;
		}

		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithDevelop_And_BlankConfig_ReturnsDevelopKey()
		{
			ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"] = "   ";

			AssertConfiguration("https://localhost:1234/Page/Default.aspx", CanadaPostConfiguration.DevelopmentKey, false);
		}

		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithDevelop_ReturnsKey()
		{
			AssertConfiguration("https://localhost:1234/Page/Default.aspx", CanadaPostConfiguration.DevelopmentKey, false);
		}

		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithTest_ReturnsKey()
		{
			AssertConfiguration("https://test.cjg.com/Page/Default.aspx", "ng11-gy73-rr71-ta98", true);
		}

		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithSupport_ReturnsKey()
		{
			AssertConfiguration("https://support.cjg.com/Page/Default.aspx", "cz58-uj19-et92-zc83", true);
		}
		
		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithQA1_ReturnsKey()
		{
			AssertConfiguration("https://qa.url.com/Page/Default.aspx", "jr49-rt91-fm46-dp87", false);
		}

		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithQA2_ReturnsKey()
		{
			AssertConfiguration("https://qa.url.com:8080/Page/Default.aspx", "uj89-nx14-fn99-ne69", false);
		}

		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithQaUrl_ReturnsKey()
		{
			AssertConfiguration("https://qa.skillstraininggrants.gov.bc.ca/Page/Default.aspx", "jr49-rt91-fm46-dp87", false);
		}

		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithProductionUrl_ReturnsKey()
		{
			AssertConfiguration("https://skillstraininggrants.gov.bc.ca/Page/Default.aspx", "tf99-cf94-jh76-xd79", true);
		}

		[TestMethod, TestCategory("Unit")]
		public void GetKey_WithPartialName_ReturnsDevelopmentKey_NotTrainingKey()
		{
			AssertConfiguration("https://practicaltraining.skillstraininggrants.gov.bc.ca/Default.aspx", CanadaPostConfiguration.DevelopmentKey, false);
		}

		private void AssertConfiguration(string givenUrl, string expectKey, bool expectHttps)
		{
			var httpContextMock = _identity.MockHttpContext(givenUrl);
			var postConfig = new CanadaPostConfiguration(httpContextMock.Object);

			var canadaPostKey = postConfig.GetCanadaPostKey();

			canadaPostKey.Key.Should().Be(expectKey);
			canadaPostKey.IsHttps.Should().Be(expectHttps);
		}
	}
}