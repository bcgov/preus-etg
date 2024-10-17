using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass]
	public class TrainingCostValidationTests
	{
		private ServiceHelper Helper { get; set; }
		private User User { get; set; }

		[TestInitialize]
		public void Setup()
		{
			// Arrange
			User = EntityHelper.CreateExternalUser();
			Helper = new ServiceHelper(typeof(EligibleCostService), User);
			Helper.MockContext();
			Helper.MockDbSet<TrainingCost>();
			Helper.MockDbSet<EligibleCost>();
			Helper.MockDbSet<GrantApplication>();
			Helper.MockDbSet<GrantOpening>();
			Helper.MockDbSet<GrantStream>();
			Helper.MockDbSet<EligibleExpenseType>();
		}

		[TestMethod, TestCategory("Training Cost"), TestCategory("Validate")]
		public void Validate_When_TrainingCost_TotalEstimatedCost_Shouldbe_EligibleCost_EstimatedCost()
		{
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(User, ApplicationStateInternal.AgreementAccepted);

			var trainingCost = new TrainingCost(grantApplication, 0)
			{
				TotalEstimatedCost = 50
			};

			var eligibleExpenseType = new EligibleExpenseType()
			{
				Id = 2,
				Caption = "Mandatory student fees"
			};

			var eligibleCost = new EligibleCost(trainingCost, eligibleExpenseType, 100, 1)
			{
				Id = 1,
				EstimatedCost = 100
			};
			trainingCost.EligibleCosts.Add(eligibleCost);

			Helper.MockDbSet<TrainingCost>(new[] { trainingCost });
			Helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			Helper.MockDbSet<EligibleCost>(new[] { eligibleCost });
			Helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseType });
			Helper.MockDbSet<ServiceCategory>(new[] { new ServiceCategory("category", ServiceTypes.SkillsTraining, true, false, 0, 0, 1, 10) });

			var dbContextMock = Helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), nameof(GrantApplication.ApplicationStateInternal))).Returns(grantApplication.ApplicationStateInternal);

			var service = Helper.Create<EligibleCostService>();

			decimal totalEstimatedCost = eligibleCost.EstimatedCost;

			// Act
			var validationResults = service.Validate(trainingCost).ToArray();

			string validationMsg = "The total estimated cost is incorrect and should be " + totalEstimatedCost.ToString("c2");

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}

		[TestMethod, TestCategory("Training Cost"), TestCategory("Validate")]
		[Ignore("Test was originally meant for CWRG. Need to update logic for ETG.")]
		public void Validate_When_TrainingCost_TotalEstimatedReimbursement_Shouldbe_EligibleCost_EstimatedReimbursement()
		{
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(User, ApplicationStateInternal.AgreementAccepted);
			grantApplication.GrantOpening.GrantStream.GrantProgram = new GrantProgram()
			{
				ProgramTypeId = ProgramTypes.EmployerGrant
			};

			var trainingCost = new TrainingCost(grantApplication, 1)
			{
				TotalEstimatedReimbursement = 50
			};

			var eligibleExpenseType = new EligibleExpenseType()
			{
				Id = 2,
				Caption = "Mandatory student fees"
			};

			var eligibleCost = new EligibleCost(trainingCost, eligibleExpenseType, 0, 1)
			{
				Id = 1,
				EstimatedReimbursement = 100
			};
			trainingCost.EligibleCosts.Add(eligibleCost);

			Helper.MockDbSet<TrainingCost>(new[] { trainingCost });
			Helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			Helper.MockDbSet<EligibleCost>(new[] { eligibleCost });
			Helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseType });
			Helper.MockDbSet<ServiceCategory>(new[] { new ServiceCategory("category", ServiceTypes.SkillsTraining, true, false, 0, 0, 1, 10) });

			var dbContextMock = Helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), nameof(GrantApplication.ApplicationStateInternal))).Returns(grantApplication.ApplicationStateInternal);

			var service = Helper.Create<EligibleCostService>();

			decimal totalEstimatedReimbursement = eligibleCost.EstimatedReimbursement;

			// Act
			var validationResults = service.Validate(trainingCost).ToArray();

			string validationMsg = "The total estimated reimbursement is incorrect and should be " + totalEstimatedReimbursement.ToString("c2");

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg), message: validationMsg);
		}

		[TestMethod, TestCategory("Training Cost"), TestCategory("Validate")]
		public void Validate_When_TrainingCost_TotalAgreedMaxCost_Shouldbe_EligibleCost_AgreedMaxCost()
		{
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(User, ApplicationStateInternal.AgreementAccepted);

			var trainingCost = new TrainingCost(grantApplication, 0)
			{
				TotalAgreedMaxCost = 50
			};

			var eligibleExpenseType = new EligibleExpenseType()
			{
				Id = 2,
				Caption = "Mandatory student fees"
			};

			var eligibleCost = new EligibleCost(trainingCost, eligibleExpenseType, 0, 0)
			{
				Id = 1,
				AgreedMaxCost = 100
			};
			trainingCost.EligibleCosts.Add(eligibleCost);

			Helper.MockDbSet<TrainingCost>(new[] { trainingCost });
			Helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			Helper.MockDbSet<EligibleCost>(new[] { eligibleCost });
			Helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseType });
			Helper.MockDbSet<ServiceCategory>(new[] { new ServiceCategory("category", ServiceTypes.SkillsTraining, true, false, 0, 0, 1, 10) });

			var dbContextMock = Helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), nameof(GrantApplication.ApplicationStateInternal))).Returns(grantApplication.ApplicationStateInternal);

			var service = Helper.Create<EligibleCostService>();

			decimal totalAgreedMaxCost = eligibleCost.AgreedMaxCost;

			// Act
			var validationResults = service.Validate(trainingCost).ToArray();

			string validationMsg = "The total agreed maximum cost is incorrect and should be " + totalAgreedMaxCost.ToString("c2");

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}

		[TestMethod, TestCategory("Training Cost"), TestCategory("Validate")]
		public void Validate_When_TrainingCost_Required_Participants()
		{
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(User, ApplicationStateInternal.AgreementAccepted);

			var trainingCost = new TrainingCost()
			{
				GrantApplication = grantApplication,
				TotalAgreedMaxCost = 50,
				TrainingCostState = TrainingCostStates.Complete
			};

			var eligibleExpenseType = new EligibleExpenseType()
			{
				Id = 2,
				Caption = "Mandatory student fees"
			};

			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = 2,
				EligibleExpenseType = eligibleExpenseType,
				TrainingCost = trainingCost,
				AgreedMaxCost = 100
			};

			Helper.MockDbSet<TrainingCost>(new[] { trainingCost });
			Helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			Helper.MockDbSet<EligibleCost>(new[] { eligibleCost });
			Helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseType });
			Helper.MockDbSet<ServiceCategory>(new[] { new ServiceCategory("category", ServiceTypes.SkillsTraining, true, false, 0, 0, 1, 10) });

			var service = Helper.Create<EligibleCostService>();

			// Act
			var validationResults = service.Validate(trainingCost).ToArray();

			string validationMsg = "You must enter the number of participants.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}

		[TestMethod, TestCategory("Training Cost"), TestCategory("Validate")]
		public void Validate_When_TrainingCost_EstimatedParticipants_Equal_Greaterthan_EligibleCosts_EstimatedParticipants()
		{
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(User, ApplicationStateInternal.AgreementAccepted);

			var trainingCost = new TrainingCost(grantApplication, 50)
			{
				TotalAgreedMaxCost = 50,
				TrainingCostState = TrainingCostStates.Complete
			};

			var eligibleExpenseType = new EligibleExpenseType()
			{
				Id = 2,
				Caption = "Mandatory student fees"
			};

			var eligibleCost = new EligibleCost(trainingCost, eligibleExpenseType, 0, 0)
			{
				Id = 1,
				AgreedMaxCost = 100,
				EstimatedParticipants = 100
			};
			trainingCost.EligibleCosts.Add(eligibleCost);

			Helper.MockDbSet<TrainingCost>(new[] { trainingCost });
			Helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			Helper.MockDbSet<EligibleCost>(new[] { eligibleCost });
			Helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseType });
			Helper.MockDbSet<ServiceCategory>(new[] { new ServiceCategory("category", ServiceTypes.SkillsTraining, true, false, 0, 0, 1, 10) });

			var dbContextMock = Helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), nameof(GrantApplication.ApplicationStateInternal))).Returns(grantApplication.ApplicationStateInternal);

			var service = Helper.Create<EligibleCostService>();

			// Act
			var validationResults = service.Validate(trainingCost).ToArray();

			string validationMsg = "The number of participants for one expense type cannot exceed the number of participants you entered above, which was " + trainingCost.EstimatedParticipants + ".";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}

		[TestMethod, TestCategory("Training Cost"), TestCategory("Validate")]
		public void Validate_When_TrainingCost_AgreedParticipants_Not_Exceed_Any_EligibleCost_AgreedMaxParticipants()
		{
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(User, ApplicationStateInternal.AgreementAccepted);
			var trainingCost = new TrainingCost(grantApplication, 0)
			{
				AgreedParticipants = 50
			};
			var eligibleExpenseType = new EligibleExpenseType()
			{
				Id = 2,
				Caption = "Mandatory student fees"
			};
			var eligibleCost = new EligibleCost(trainingCost, eligibleExpenseType, 0, 0)
			{
				Id = 1,
				AgreedMaxParticipants = 100
			};
			string validationMsg = "The number of participants for an eligible cost cannot be greater than the application maximum number of participants, which was " + trainingCost.AgreedParticipants + ".";

			trainingCost.EligibleCosts.Add(eligibleCost);

			Helper.MockDbSet<TrainingCost>(new[] { trainingCost });
			Helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			Helper.MockDbSet<EligibleCost>(new[] { eligibleCost });
			Helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseType });
			Helper.MockDbSet<ServiceCategory>(new[] { new ServiceCategory("category", ServiceTypes.SkillsTraining, true, false, 0, 0, 1, 10) });

			var dbContextMock = Helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), nameof(GrantApplication.ApplicationStateInternal))).Returns(grantApplication.ApplicationStateInternal);

			var service = Helper.Create<EligibleCostService>();

			// Act
			var validationResults = service.Validate(trainingCost).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}

		[TestMethod, TestCategory("Training Cost"), TestCategory("Validate")]
		public void Validate_When_TrainingCost_SumAvgAgreedMaxReimbursement_Cost_Per_Participant_Not_Exceed_MaxESSParticipantCost_EmploymentServicesAndSupports()
		{
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(User, ApplicationStateInternal.AgreementAccepted);
			grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost = 60;
			var trainingCost = new TrainingCost(grantApplication, 1)
			{
				TotalEstimatedCost = 100,
				TotalEstimatedReimbursement = 66.67m,
				AgreedParticipants = 1,
				TotalAgreedMaxCost = 100,
				AgreedCommitment = 70
			};
			var serviceCategory = new ServiceCategory("test", ServiceTypes.EmploymentServicesAndSupports)
			{
				Id = 1
			};
			var eligibleExpenseType = new EligibleExpenseType(serviceCategory)
			{
				Id = 2,
				Caption = "Mandatory student fees"
			};
			var eligibleCost = new EligibleCost(trainingCost, eligibleExpenseType, 100, 1)
			{
				Id = 1,
				AgreedMaxCost = 100,
				AgreedMaxReimbursement = 70
			};
			trainingCost.EligibleCosts.Add(eligibleCost);

			var maxESSParticipantCost = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost;

			Helper.MockDbSet<TrainingCost>(new[] { trainingCost });
			Helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			Helper.MockDbSet<EligibleCost>(new[] { eligibleCost });
			Helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseType });
			Helper.MockDbSet<ServiceCategory>(new[] { serviceCategory });

			var dbContextMock = Helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), nameof(GrantApplication.ApplicationStateInternal))).Returns(grantApplication.ApplicationStateInternal);

			var service = Helper.Create<EligibleCostService>();

			// Act
			var validationResults = service.Validate(trainingCost).ToArray();

			string validationMsg = $"The total average cost per participant for {serviceCategory.Caption} combined may not exceed {maxESSParticipantCost.ToString("c2")}.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}

		[TestMethod, TestCategory("Training Cost"), TestCategory("Validate")]
		public void Validate_When_TrainingCost_SumAvgEstimatedReimbursement_Cost_Per_Participant_Not_Exceed_MaxESSParticipantCost_EmploymentServicesAndSupports()
		{
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(User, ApplicationStateInternal.AgreementAccepted);

			grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost = 60;

			var trainingCost = new TrainingCost(grantApplication, 1)
			{
				TotalEstimatedCost = 100,
				TotalEstimatedReimbursement = 66.67m,
				AgreedParticipants = 1,
				TotalAgreedMaxCost = 100,
				AgreedCommitment = 70
			};

			var serviceCategory = new ServiceCategory("test", ServiceTypes.EmploymentServicesAndSupports)
			{
				Id = 1
			};

			var eligibleExpenseType = new EligibleExpenseType(serviceCategory)
			{
				Id = 2,
				Caption = "Mandatory student fees"
			};

			var eligibleCost = new EligibleCost(trainingCost, eligibleExpenseType, 100, 1)
			{
				Id = 1,
				AgreedMaxCost = 100,
				AgreedMaxReimbursement = 70
			};
			trainingCost.EligibleCosts.Add(eligibleCost);

			var maxESSParticipantCost = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost;

			Helper.MockDbSet<TrainingCost>(new[] { trainingCost });
			Helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			Helper.MockDbSet<EligibleCost>(new[] { eligibleCost });
			Helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseType });
			Helper.MockDbSet<ServiceCategory>(new[] { serviceCategory });

			var dbContextMock = Helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), nameof(GrantApplication.ApplicationStateInternal))).Returns(grantApplication.ApplicationStateInternal);

			var service = Helper.Create<EligibleCostService>();

			// Act
			var validationResults = service.Validate(trainingCost).ToArray();

			string validationMsg = $"The total average cost per participant for {serviceCategory.Caption} combined may not exceed {maxESSParticipantCost.ToString("c2")}.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}

		[TestMethod, TestCategory("Training Cost"), TestCategory("Validate")]
		public void Validate_When_TrainingCost_SumAvgEstimatedReimbursement_Cost_Per_Participant_Not_Exceed_SkillsTrainingMaxEstimatedParticipantCosts()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(User, ApplicationStateInternal.AgreementAccepted);

			grantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts = 100;

			var trainingCost = new TrainingCost(grantApplication, 10)
			{
				TotalEstimatedReimbursement = 50
			};

			var serviceCategory = new ServiceCategory("test", ServiceTypes.SkillsTraining)
			{
				Id = 1
			};

			var eligibleExpenseType = new EligibleExpenseType(serviceCategory)
			{
				Id = 2,
				Caption = "Mandatory student fees",
				ServiceCategoryId = 1,
				ServiceCategory = serviceCategory
			};

			var eligibleCost = new EligibleCost(trainingCost, eligibleExpenseType, 0, 0)
			{
				Id = 1,
				EstimatedReimbursement = 5000
			};
			trainingCost.EligibleCosts.Add(eligibleCost);

			var maxSkillsTrainingParticipantCost = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts;

			Helper.MockDbSet<TrainingCost>(new[] { trainingCost });
			Helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			Helper.MockDbSet<EligibleCost>(new[] { eligibleCost });
			Helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseType });
			Helper.MockDbSet<ServiceCategory>(new[] { serviceCategory });

			var dbContextMock = Helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), nameof(GrantApplication.ApplicationStateInternal))).Returns(grantApplication.ApplicationStateInternal);

			var service = Helper.Create<EligibleCostService>();

			// Act
			var validationResults = service.Validate(trainingCost).ToArray();

			string validationMsg = $"The total average cost per participant for skills training components may not exceed {maxSkillsTrainingParticipantCost.ToString("c2")}.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}

		[TestMethod, TestCategory("Training Cost"), TestCategory("Validate")]
		public void Validate_When_TrainingCost_SumAvgAgreedMaxReimbursement_Cost_Per_Participant_Not_Exceed_SkillsTrainingMaxEstimatedParticipantCosts()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(User, ApplicationStateInternal.AgreementAccepted);

			grantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts = 60;

			var trainingCost = new TrainingCost(grantApplication, 1)
			{
				TotalEstimatedCost = 100,
				TotalEstimatedReimbursement = 66.67m,
				AgreedParticipants = 1,
				TotalAgreedMaxCost = 100,
				AgreedCommitment = 70
			};

			var serviceCategory = new ServiceCategory("test", ServiceTypes.SkillsTraining)
			{
				Id = 1
			};

			var eligibleExpenseType = new EligibleExpenseType(serviceCategory)
			{
				Id = 2,
				Caption = "Mandatory student fees"
			};

			var eligibleCost = new EligibleCost(trainingCost, eligibleExpenseType, 100, 1)
			{
				Id = 1,
				AgreedMaxCost = 100,
				AgreedMaxReimbursement = 70
			};
			trainingCost.EligibleCosts.Add(eligibleCost);

			var maxSkillsTrainingParticipantCost = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts;

			Helper.MockDbSet<TrainingCost>(new[] { trainingCost });
			Helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			Helper.MockDbSet<EligibleCost>(new[] { eligibleCost });
			Helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseType });
			Helper.MockDbSet<ServiceCategory>(new[] { serviceCategory });

			var dbContextMock = Helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), nameof(GrantApplication.ApplicationStateInternal))).Returns(grantApplication.ApplicationStateInternal);

			var service = Helper.Create<EligibleCostService>();

			// Act
			var validationResults = service.Validate(trainingCost).ToArray();

			string validationMsg = $"The total average cost per participant for skills training components may not exceed {maxSkillsTrainingParticipantCost.ToString("c2")}.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}

		[TestMethod, TestCategory("Training Cost"), TestCategory("Validate")]
		public void Validate_When_TrainingCost_TotalEstimatedReimbursement_Exceeds_MaxReimbursementAmount_Per_EstimatedParticipants()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(User, ApplicationStateInternal.AgreementAccepted);

			grantApplication.MaxReimbursementAmt = 50;
			grantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts = 2000;

			var trainingCost = new TrainingCost(grantApplication, 1)
			{
				TotalEstimatedCost = 100,
				TotalEstimatedReimbursement = 66.67m
			};

			var serviceCategory = new ServiceCategory("Skills Training", ServiceTypes.SkillsTraining)
			{
				Id = 1
			};

			var eligibleExpenseType = new EligibleExpenseType(serviceCategory)
			{
				Id = 2,
				Caption = "Mandatory student fees"
			};

			var eligibleCost = new EligibleCost(trainingCost, eligibleExpenseType, 100, 1)
			{
				Id = 1
			};
			trainingCost.EligibleCosts.Add(eligibleCost);

			Helper.MockDbSet<TrainingCost>(new[] { trainingCost });
			Helper.MockDbSet<GrantApplication>(new[] { grantApplication });
			Helper.MockDbSet<EligibleCost>(new[] { eligibleCost });
			Helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseType });
			Helper.MockDbSet<ServiceCategory>(new[] { new ServiceCategory("category", ServiceTypes.SkillsTraining, true, false, 0, 0, 1, 10) });

			var dbContextMock = Helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), nameof(GrantApplication.ApplicationStateInternal))).Returns(grantApplication.ApplicationStateInternal);

			var service = Helper.Create<EligibleCostService>();

			// Act
			var validationResults = service.Validate(trainingCost).ToArray();

			var maxReimbursementAmount = grantApplication.MaxReimbursementAmt;

			string validationMsg = "The total requested government contribution exceeds the per participant fiscal year maximum reimbursement allowed of '$" + maxReimbursementAmount.ToString("#,##0.00") + "'.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}
	}
}