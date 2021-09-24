using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass()]
	public class TrainingProgramValidationTests
	{
		ServiceHelper helper { get; set; }

		[TestInitialize]
		public void Setup()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			helper = new ServiceHelper(typeof(TrainingProgramService), user);
			helper.MockContext();
			helper.MockDbSet<GrantApplication>();
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		public void Validate_When_Training_Program_Is_Required_Properties_Error()
		{
			// Arrange
			TrainingProgram trainingProgram = new TrainingProgram();

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The CourseTitle field is required."));
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "Total training hours must be greater than or equal to 1."));
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Validate_When_Training_Program_Is_Not_Associated_With_Grant_Application()
		{
			// Arrange
			TrainingProgram trainingProgram = new TrainingProgram()
			{
				Id = 1,
				CourseTitle = "Seemless Wireframe Design",
				TotalTrainingHours = 1
			};

			helper.MockDbSet<TrainingProgram>();

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToArray();

			// Assert
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		public void Validate_When_Training_Program_Atleast_One_Delivery_Method_Not_Selected()
		{
			// Arrange
			TrainingProgram trainingProgram = new TrainingProgram()
			{
				Id = 1,
				CourseTitle = "Seemless Wireframe Design",
				TotalTrainingHours = 1,
				GrantApplication = new GrantApplication()
			};

			helper.MockDbSet<TrainingProgram>();

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "You must select at least one primary delivery method."));
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		public void Validate_When_Training_Not_Start_Program_Date_Must_Be_Within_GrantApplication_Start_End_Date()
		{
			// Arrange
			GrantApplication grantApplication = new GrantApplication()
			{
				Id = 1,
				ApplicationStateInternal = ApplicationStateInternal.Draft,
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddDays(7)
			};

			TrainingProgram trainingProgram = new TrainingProgram()
			{
				Id = 1,
				GrantApplication = grantApplication,
				GrantApplicationId = 1,
				StartDate = DateTime.Now.AddDays(-7),
				EndDate = DateTime.Now.AddDays(14),
				CourseTitle = "Seemless Wireframe Design",
				TotalTrainingHours = 1,
				TrainingProgramState = TrainingProgramStates.Complete
			};

			helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			helper.MockDbSet<TrainingProgram>(new[] { trainingProgram });

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The training start date must fall within the program delivery dates")));
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		public void Validate_When_Training_Not_End_Program_Date_Must_Be_Within_GrantApplication_Start_End_Date()
		{
			// Arrange
			GrantApplication grantApplication = new GrantApplication()
			{
				Id = 1,
				ApplicationStateInternal = ApplicationStateInternal.Draft,
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddDays(7)
			};

			TrainingProgram trainingProgram = new TrainingProgram()
			{
				Id = 1,
				GrantApplication = grantApplication,
				GrantApplicationId = 1,
				StartDate = DateTime.Now.AddDays(-7),
				EndDate = DateTime.Now.AddDays(14),
				CourseTitle = "Seemless Wireframe Design",
				TotalTrainingHours = 1,
				TrainingProgramState = TrainingProgramStates.Complete
			};

			helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			helper.MockDbSet<TrainingProgram>(new[] { trainingProgram });

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The training end date must fall within the program delivery dates")));
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		public void Validate_When_Training_Not_End_Date_Must_Occur_After_Or_On_Start_Date()
		{
			// Arrange
			TrainingProgram trainingProgram = new TrainingProgram()
			{
				Id = 1,
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddDays(-1),
				CourseTitle = "Seemless Wireframe Design",
				TotalTrainingHours = 1,
				GrantApplication = new GrantApplication()
			};

			helper.MockDbSet<TrainingProgram>();

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The end date must occur on or after the start date")));
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		public void Validate_Training_Program_When_You_Not_Select_A_Member_Of_Under_Represented_Group()
		{
			// Arrange
			GrantApplication grantApplication = new GrantApplication()
			{
				Id = 1,
				ApplicationStateInternal = ApplicationStateInternal.Draft,
                DeliveryPartner = new DeliveryPartner()
            };

			TrainingProgram trainingProgram = new TrainingProgram()
			{
				Id = 1,
				GrantApplication = grantApplication,
				GrantApplicationId = 1,
				TrainingProgramState = TrainingProgramStates.Complete,
				MemberOfUnderRepresentedGroup = null,
				SkillFocus = new SkillsFocus(),
				SkillFocusId = 5,
				CourseTitle = "Seemless Wireframe Design",
				TotalTrainingHours = 1
			};

			helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			helper.MockDbSet<TrainingProgram>(new[] { trainingProgram });

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "You must select whether you are a member of a under represented group."));
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		public void Validate_Training_Program_When_You_Not_Select_In_Demand_Occupation()
		{
			// Arrange
			GrantApplication grantApplication = new GrantApplication()
			{
				Id = 1,
				ApplicationStateInternal = ApplicationStateInternal.Draft,
                DeliveryPartner = new DeliveryPartner()
            };
			TrainingProgram trainingProgram = new TrainingProgram()
			{
				Id = 1,
				GrantApplication = grantApplication,
				GrantApplicationId = 1,
				TrainingProgramState = TrainingProgramStates.Complete,
				MemberOfUnderRepresentedGroup = true,
				SkillFocus = new SkillsFocus(),
				SkillFocusId = 5,
				CourseTitle = "Seemless Wireframe Design",
				TotalTrainingHours = 1
			};

			helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			helper.MockDbSet<TrainingProgram>(new[] { trainingProgram });

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToList();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "You must select an in demand occupation."));
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		public void Validate_Training_Program_When_You_Not_Select_Training_Level()
		{
			// Arrange
			GrantApplication grantApplication = new GrantApplication()
			{
				Id = 1,
				ApplicationStateInternal = ApplicationStateInternal.Draft,
                DeliveryPartner = new DeliveryPartner()
            };
			TrainingProgram trainingProgram = new TrainingProgram()
			{
				Id = 1,
				GrantApplication = grantApplication,
				GrantApplicationId = 1,
				TrainingProgramState = TrainingProgramStates.Complete,
				MemberOfUnderRepresentedGroup = true,
				SkillFocus = new SkillsFocus(),
				SkillFocusId = 5,
				CourseTitle = "Seemless Wireframe Design",
				TotalTrainingHours = 1
			};

			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<TrainingProgram>(trainingProgram);

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToList();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "You must select a training level."));
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		public void Validate_Training_Program_When_You_Not_Select_Under_Represented_Groups_Apply()
		{
			// Arrange
			GrantApplication grantApplication = new GrantApplication()
			{
				Id = 1,
				ApplicationStateInternal = ApplicationStateInternal.Draft
			};

			TrainingProgram trainingProgram = new TrainingProgram()
			{
				Id = 1,
				GrantApplication = grantApplication,
				GrantApplicationId = 1,
				TrainingProgramState = TrainingProgramStates.Complete,
				MemberOfUnderRepresentedGroup = true,
				SkillFocusId = 5,
				CourseTitle = "Seemless Wireframe Design",
				TotalTrainingHours = 1
			};

			helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			helper.MockDbSet<TrainingProgram>(new[] { trainingProgram });

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToList();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "You must select under represented groups that apply."));
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		public void Validate_Training_Program_When_You_Have_A_Qualifications_And_Not_Include_Title_Qualification()
		{
			// Arrange
			GrantApplication grantApplication = new GrantApplication()
			{
				Id = 1,
				ApplicationStateInternal = ApplicationStateInternal.Draft
			};

			TrainingProgram trainingProgram = new TrainingProgram()
			{
				Id = 1,
				GrantApplication = grantApplication,
				GrantApplicationId = 1,
				TrainingProgramState = TrainingProgramStates.Complete,
				ExpectedQualificationId = 7,
				CourseTitle = "Seemless Wireframe Design",
				TotalTrainingHours = 1
			};

			helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			helper.MockDbSet<TrainingProgram>(new[] { trainingProgram });

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToList();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "If you a have expected qualifications you must include the title of the qualification."));
		}

		[TestMethod, TestCategory("Training Program"), TestCategory("Validate")]
		public void Validate_Training_Program_When_You_Requested_Additional_Funding_Not_Include_Funding_Request_Description()
		{
			// Arrange
			GrantApplication grantApplication = new GrantApplication()
			{
				Id = 1,
				ApplicationStateInternal = ApplicationStateInternal.Draft
			};

			TrainingProgram trainingProgram = new TrainingProgram()
			{
				Id = 1,
				GrantApplication = grantApplication,
				GrantApplicationId = 1,
				TrainingProgramState = TrainingProgramStates.Complete,
				HasRequestedAdditionalFunding = true,
				CourseTitle = "Seemless Wireframe Design",
				TotalTrainingHours = 1
			};

			helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			helper.MockDbSet<TrainingProgram>(new[] { trainingProgram });

			var service = helper.Create<TrainingProgramService>();

			// Act
			var validationResults = service.Validate(trainingProgram).ToList();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "If you have received or requested additional funding you must include a description of the funding request."));
		}
	}
}