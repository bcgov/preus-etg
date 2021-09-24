using Microsoft.VisualStudio.TestTools.UnitTesting;
using CJG.Core.Entities;
using System.Linq;
using CJG.Testing.Core;
using CJG.Application.Services;
using System;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass()]
	public class TrainingProviderValidationTests
	{
		ServiceHelper helper { get; set; }

		[TestInitialize]
		public void Setup()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			helper = new ServiceHelper(typeof(TrainingProviderService), user);
			helper.MockContext();
			helper.MockDbSet<TrainingProvider>();
			helper.MockDbSet<TrainingProgram>();
			helper.MockDbSet<GrantApplication>();
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Validate")]
		public void Validate_When_TrainingProvider_Is_Required_Properties_Error()
		{
			var trainingProvider = new TrainingProvider();

			var service = helper.Create<TrainingProviderService>();

			// validate grant stream
			var validationResults = service.Validate(trainingProvider).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "Provider name is required"));
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "Contact first name is required"));
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "Contact last name is required"));
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "Contact email is required"));
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "Contact phone number is required"));
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Validate")]
		public void Validate_When_Training_Provider_Is_Not_Associated_With_Training_Provider_Type()
		{
			var grantApplication = new GrantApplication()
			{
				Id = 1
			};

			var trainingProviderType = new TrainingProviderType("test", TrainingProviderPrivateSectorValidationTypes.Always)
			{
				Id = 1
			};

			var trainingProvider = new TrainingProvider(grantApplication)
			{
				Id = 0,
				Name = "Wireframes International Inc.",
				ContactFirstName = "John",
				ContactLastName = "Doe",
				ContactEmail = "john.doe@email.com",
				ContactPhoneNumber = "(604) 555-9876"
			};

			helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			helper.MockDbSet<Setting>();
			helper.MockDbSet<TrainingProviderType>(new[] { trainingProviderType });

			var service = helper.Create<TrainingProviderService>();

			// validate grant stream
			var validationResults = service.Validate(trainingProvider).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "Training Provider Type is required."));
		}

		[Ignore, TestMethod, TestCategory("Training Provider"), TestCategory("Validate")]
		public void Validate_When_Training_Is_Outside_BC_Is_True_And_Business_Case_And_Or_Document_Not_Provided()
		{
			var grantApplication = new GrantApplication()
			{
				Id = 1
			};

			var trainingProviderType = new TrainingProviderType("test", TrainingProviderPrivateSectorValidationTypes.Never)
			{
				Id = 1
			};

			var trainingProvider = new TrainingProvider(grantApplication)
			{
				Id = 1,
				Name = "Wireframes International Inc.",
				ContactFirstName = "John",
				ContactLastName = "Doe",
				ContactEmail = "john.doe@email.com",
				ContactPhoneNumber = "(604) 555-9876",
				TrainingProviderState = TrainingProviderStates.Requested,
				TrainingOutsideBC = true,
				TrainingProviderTypeId = trainingProviderType.Id,
				TrainingProviderType = trainingProviderType
			};

			helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			helper.MockDbSet<Setting>();
			helper.MockDbSet<TrainingProviderType>(new[] { trainingProviderType });

			var service = helper.Create<TrainingProviderService>();

			// validate grant stream
			var validationResults = service.Validate(trainingProvider).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "If training is outside of BC you must provide a business case and/or document."));
		}

		[Ignore, TestMethod, TestCategory("Training Provider"), TestCategory("Validate")]
		public void Validate_When_Training_Provider_Not_Provided_Proof_Of_Qualifications_And_Course_Outline()
		{
			var grantApplication = new GrantApplication()
			{
				Id = 1
			};

			var trainingProviderType = new TrainingProviderType("test", TrainingProviderPrivateSectorValidationTypes.Always)
			{
				Id = 1
			};

			var trainingProvider = new TrainingProvider(grantApplication)
			{
				Id = 1,
				Name = "Wireframes International Inc.",
				ContactFirstName = "John",
				ContactLastName = "Doe",
				ContactEmail = "john.doe@email.com",
				ContactPhoneNumber = "(604) 555-9876",
				TrainingProviderState = TrainingProviderStates.Requested,
				TrainingProviderTypeId = trainingProviderType.Id,
				TrainingProviderType = trainingProviderType
			};

			helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			helper.MockDbSet<Setting>();
			helper.MockDbSet<TrainingProviderType>(new[] { trainingProviderType });

			var service = helper.Create<TrainingProviderService>();

			// validate grant stream
			var validationResults = service.Validate(trainingProvider).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "You must provide proof of qualifications."));
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "You must provide a course outline document."));
		}

		[Ignore, TestMethod, TestCategory("Training Provider"), TestCategory("Validate")]
		public void Validate_When_Training_Provider_Address_Not_Entered()
		{
			var grantApplication = new GrantApplication()
			{
				Id = 1
			};

			var trainingProviderType = new TrainingProviderType("test", TrainingProviderPrivateSectorValidationTypes.Never)
			{
				Id = 1
			};

			var trainingProvider = new TrainingProvider(grantApplication)
			{
				Id = 1,
				Name = "Wireframes International Inc.",
				ContactFirstName = "John",
				ContactLastName = "Doe",
				ContactEmail = "john.doe@email.com",
				ContactPhoneNumber = "(604) 555-9876",
				TrainingProviderTypeId = trainingProviderType.Id,
				TrainingProviderType = trainingProviderType
			};

			helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			helper.MockDbSet<Setting>();
			helper.MockDbSet<TrainingProviderType>(new[] { trainingProviderType });

			var service = helper.Create<TrainingProviderService>();

			// validate grant stream
			var validationResults = service.Validate(trainingProvider).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "You must enter the training provider address information."));
		}
	}
}