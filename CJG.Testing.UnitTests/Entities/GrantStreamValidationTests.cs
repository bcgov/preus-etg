using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CJG.Testing.Core;

namespace CJG.Testing.UnitTests.Entities
{

	[TestClass]
	public class GrantStreamValidationTests
	{
		ServiceHelper helper { get; set; }

		[TestInitialize]
		public void Setup()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			helper = new ServiceHelper(typeof(GrantStreamService), user);
			helper.MockContext();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<GrantProgram>();
		}
		[TestMethod, TestCategory("Grant Stream"), TestCategory("Validate")]
		public void Initial_GrantStream()
		{
			var grantProgram = new GrantProgram()
			{
				Id = 1
			};

			var grantStream = new GrantStream()
			{
				GrantProgramId = 1
			};

			Assert.AreEqual(true, grantStream.IncludeDeliveryPartner);
			Assert.AreEqual(false, grantStream.CanApplicantReportParticipants);
			Assert.AreEqual(false, grantStream.HasParticipantOutcomeReporting);
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Validate")]
		public void Validate_When_GrantStream_Is_Required_Properties_Error()
		{
			// Arrange
			var grantProgram = new GrantProgram()
			{
				Id = 1
			};

			var grantStream = new GrantStream()
			{
				GrantProgramId = 1
			};

			helper.MockDbSet<GrantProgram>(new[] { grantProgram });
			var service = helper.Create<GrantStreamService>();

			// Act
			var validationResults = service.Validate(grantStream).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The Name field is required."));
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The reimbursement rate must be greater than or equal to 0.05."));
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Validate")]
		public void Validate_When_GrantStream_EligibilityEnabled_Is_True_And_EligibilityQuestion_Is_Empty_Add_Validation_Error()
		{
			// Arrange
			var grantProgram = new GrantProgram()
			{
				Id = 1
			};

			var grantStream = new GrantStream()
			{
				Name = "Grant Stream",
				EligibilityQuestion = null,
				EligibilityEnabled = true,
				ReimbursementRate = 1,
				GrantProgramId = 1
			};

			helper.MockDbSet<GrantProgram>(new[] { grantProgram });
			var service = helper.Create<GrantStreamService>();

			// Act
			var validationResults = service.Validate(grantStream).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The grant stream must have an eligibility question defined."));
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Validate")]
		public void Validate_When_GrantStream_Is_Active_And_Objective_Is_Null_Add_Validation_Error()
		{
			// Arrange
			var grantProgram = new GrantProgram()
			{
				Id = 1
			};

			var grantStream = new GrantStream()
			{
				Name = "Grant Stream",
				Objective = null,
				ReimbursementRate = 1,
				GrantProgramId = 1
			};

			helper.MockDbSet<GrantProgram>(new[] { grantProgram });

			var service = helper.Create<GrantStreamService>();

			// Act
			var validationResults = service.Validate(grantStream).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The grant stream must have an objective defined."));
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Validate")]
		public void Validate_When_GrantStream_Is_Active_And_AccountCode_Is_Null_Add_Validation_Error()
		{
			// Arrange
			var grantProgram = new GrantProgram()
			{
				Id = 1
			};

			var grantStream = new GrantStream()
			{
				Name = "Grant Stream",
				IsActive = true,
				AccountCode = null,
				ReimbursementRate = 1,
				GrantProgramId = 1
			};

			helper.MockDbSet<GrantProgram>(new[] { grantProgram });
			var service = helper.Create<GrantStreamService>();

			// Act
			var validationResults = service.Validate(grantStream).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "A grant stream cannot be made active until it has an objective, maximum reimbursement amount, reimbursement rate and the Grant Program with which it is associated is implemented."));
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Validate")]
		public void Validate_When_GrantStream_AttachmentsIsEnabled_Is_True_And_AttachmentsHeader_Is_Null_Add_Validation_Error()
		{
			// Arrange
			var grantProgram = new GrantProgram()
			{
				Id = 1
			};
			var grantStream = new GrantStream()
			{
				Name = "Grant Stream",                
				ReimbursementRate = 1,
				GrantProgramId = 1,
				AttachmentsIsEnabled = true,
				AttachmentsHeader = null
			};

			helper.MockDbSet<GrantProgram>(new[] { grantProgram });
			var service = helper.Create<GrantStreamService>();

			// Act
			var validationResults = service.Validate(grantStream).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The grant stream must have an attachements header defined."));
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Validate")]
		public void Validate_When_GrantStream_AttachmentsIsEnabled_Is_True_And_AttachmentsUserGuidance_Is_Null_Add_Validation_Error()
		{
			// Arrange
			var grantProgram = new GrantProgram()
			{
				Id = 1
			};
			var grantStream = new GrantStream()
			{
				Name = "Grant Stream",
				ReimbursementRate = 1,
				GrantProgramId = 1,
				AttachmentsIsEnabled = true,
				AttachmentsUserGuidance = null
			};

			helper.MockDbSet<GrantProgram>(new[] { grantProgram });
			var service = helper.Create<GrantStreamService>();

			// Act
			var validationResults = service.Validate(grantStream).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The grant stream must have an attachements user guidance defined."));
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Validate")]
		public void Validate_When_GrantStream_Maximum_Number_Of_Attachments_Allowed_Greater_Than_50_Validation_Error()
		{
			// Arrange
			var grantProgram = new GrantProgram()
			{
				Id = 1
			};
			var grantStream = new GrantStream()
			{
				Name = "Grant Stream",
				ReimbursementRate = 1,
				GrantProgramId = 1,
				AttachmentsMaximum = 55
			};

			helper.MockDbSet<GrantProgram>(new[] { grantProgram });
			var service = helper.Create<GrantStreamService>();

			// Act
			var validationResults = service.Validate(grantStream).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "Maximum Number of Attachment Permitted is 50."));
		}
	}
}

