using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass]
	public class GrantApplicationValidationTests
	{
		ServiceHelper helper { get; set; }

		[TestInitialize]
		public void Setup()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			helper = new ServiceHelper(typeof(GrantApplicationService), user);
			helper.MockContext();
			helper.MockDbSet<GrantApplication>();
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<TrainingProgram>();
			helper.MockDbSet<TrainingProvider>();
			helper.MockDbSet<TrainingPeriod>();
			helper.MockDbSet<GrantAgreement>();
		}

		[TestMethod, TestCategory("Grant Application"), TestCategory("Validate")]
		public void Validate_When_GrantApplication_Is_Required_Properties_Error()
		{
			var grantApplication = new GrantApplication();

			helper.MockDbSet<GrantApplication>(grantApplication);

			var service = helper.Create<GrantApplicationService>();

			// Act
			var validationResults = service.Validate(grantApplication).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The ApplicantFirstName field is required."));
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The ApplicantLastName field is required."));
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The ApplicantPhoneNumber field is required."));
		}

		[TestMethod, TestCategory("Grant Application"), TestCategory("Validate")]
		public void Validate_When_GrantApplication_Not_Associated_With_Grant_Opening()
		{
			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpeningId = 0;

			helper.MockDbSet<GrantApplication>(grantApplication);

			var service = helper.Create<GrantApplicationService>();

			// Act
			//var validationResults = service.Validate(grantApplication).ToArray();

			// Assert
			//Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The grant application must be associated with a grant opening."));
		}

		[TestMethod, TestCategory("Grant Application"), TestCategory("Validate")]
		public void Validate_When_Grant_Application_Not_Select_Delivery_Partner_Services()
		{
			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.DeliveryPartner = new DeliveryPartner();

			helper.MockDbSet<GrantApplication>(grantApplication);

			var service = helper.Create<GrantApplicationService>();

			// Act
			//var validationResults = service.Validate(grantApplication).ToArray();

			// Assert
			//Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "You must select delivery partner services"));
		}

	}
}