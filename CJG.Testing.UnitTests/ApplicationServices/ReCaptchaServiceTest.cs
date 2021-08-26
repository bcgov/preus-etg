using CJG.Application.Services;
using CJG.Infrastructure.Entities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System.Configuration;
using System.Net;
using System.Web;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
    public class ReCaptchaServiceTest
    {

        [TestMethod, TestCategory("ReCaptcha"), TestCategory("Service")]
        public void Validate_withNullAsFirstParameter_ReturnFalse()
        {
            // Arrange
            ConfigurationManager.AppSettings["EnableReCaptcha"] = "true";
            ConfigurationManager.AppSettings["AcceptAllCertifications"] = "true";
            ConfigurationManager.AppSettings["ReCaptchaUrl"] = "https://www.google.com/recaptcha/api/siteverify";
            ConfigurationManager.AppSettings["ReCaptchaSecret"] = "";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpContextMock = new Mock<HttpContextBase>();
            var loggerMock = new Mock<ILogger>();
			var unitOfWork = new Mock<IDataContext>();
            var service = new ReCaptchaService(unitOfWork.Object, httpContextMock.Object, loggerMock.Object);
            string errorCode = "";

            // Act
            var result = service.Validate(null, ref errorCode);

            // Assert
            result.Should().Be(false);
        }

        [Ignore, TestMethod, TestCategory("ReCaptcha"), TestCategory("Service")]
        public void Validate_withNonNullValueAsFirstParameter_ShouldReturnFalseWhenSecretKeyIsEmpty()
        {
            // Arrange
            ConfigurationManager.AppSettings["EnableReCaptcha"] = "true";
            ConfigurationManager.AppSettings["AcceptAllCertifications"] = "true";
            ConfigurationManager.AppSettings["ReCaptchaUrl"] = "https://www.google.com/recaptcha/api/siteverify";
            ConfigurationManager.AppSettings["ReCaptchaSecret"] = "";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpContextMock = new Mock<HttpContextBase>();
            var loggerMock = new Mock<ILogger>();
			var unitOfWork = new Mock<IDataContext>();
            var service = new ReCaptchaService(unitOfWork.Object, httpContextMock.Object, loggerMock.Object);
            string errorCode = "";

            // Act
            var result = service.Validate("Response", ref errorCode);

            // Assert
            result.Should().Be(false);
        }
    }
}
