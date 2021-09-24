using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass]
    public class EligibleCostValidationTests
    {
		private ServiceHelper helper;
		private User user;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            user = EntityHelper.CreateExternalUser();
            helper = new ServiceHelper(typeof(EligibleCostService), user);
            helper.MockContext();
		}

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_IsRequired_Properties()
        {
			// Arrange
			var eligibleCost = new EligibleCost();

            helper.MockDbSet<EligibleCost>(eligibleCost);

			var service = helper.Create<EligibleCostService>();
			helper.MockDbSet<ServiceCategory>();
			helper.MockDbSet<EligibleCost>();
			helper.MockDbSet<GrantApplication>();
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();
			// Act
			var validationResults = service.Validate(eligibleCost).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The estimated participants must be greater than or equal to 1."));
        }

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_AgreedParticipants_Exceeds_AgreedMaxParticipants()
        {
			// Arrange
            var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                AgreedParticipants = 10
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 0,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 10,
                AgreedMaxParticipants = 15
            };

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(new EligibleExpenseType("test", "", ExpenseTypes.ParticipantAssigned) { Id = 2 });
			helper.MockDbSet<GrantApplication>();
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The number of participants for an eligible cost cannot be greater than the application maximum number of participants."));
        }

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_EstimatedReimbursement_Plus_EstimatedEmployerContribution_Exceeds_EstimatedCost()
        {
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                AgreedParticipants = 15
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedReimbursement = 100,
                EstimatedEmployerContribution = 100,
                EstimatedCost = 50,
                EstimatedParticipants = 10
            };

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(new EligibleExpenseType("test", "", ExpenseTypes.ParticipantAssigned) { Id = 2 });
			helper.MockDbSet<GrantApplication>();
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The estimated reimbursement and employer contribution cannot exceed the estimated cost."));
        }

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_AgreedMaxReimbursement_Plus_AgreedEmployerContribution_Exceeds_AgreedMaxCost()
        {
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                AgreedParticipants = 15
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                AgreedMaxReimbursement = 100,
                AgreedEmployerContribution = 100,
                AgreedMaxCost = 50,
                EstimatedParticipants = 10
            };

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(new EligibleExpenseType("test", "", ExpenseTypes.ParticipantAssigned) { Id = 2 });
			helper.MockDbSet<GrantApplication>();
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The agreed reimbursement and employer contribution cannot exceed the agreed cost."));
        }

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_EstimatedParticipants_Exceeds_TrainingProgram_EstimatedParticipants()
        {
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                EstimatedParticipants = 15
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
                TrainingCost = trainingCost,
                EstimatedParticipants = 20
            };

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<GrantApplication>();
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // message
            string validateMsg = "The number of participants for expense type '" + eligibleExpenseType.Caption + "' cannot exceed the number of participants you entered in part 1, which was '" + trainingCost.EstimatedParticipants + "'";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_CanNot_Add_MultipleExpenses_SameType_If_NotAllowed()
        {
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                EstimatedParticipants = 15
            };
            var eligibleExpenseType = new EligibleExpenseType()
            {
                Id = 2,
                AllowMultiple = false,
                Caption = "Mandatory student fees"
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 20,
                GrantApplicationId = 1
            };
            var eligibleCost_duplicate = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 20,
                GrantApplicationId = 1
            };
            string validateMsg = "The number of participants for expense type '" + eligibleExpenseType.Caption + "' cannot exceed the number of participants you entered in part 1, which was '" + trainingCost.EstimatedParticipants + "'";

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleCost>(eligibleCost_duplicate);
            helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
            helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_Exceeds_MaxEstimatedCost()
        {
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                EstimatedParticipants = 15
            };
            var eligibleExpenseType = new EligibleExpenseType()
            {
                Id = 2,
                ExpenseTypeId = ExpenseTypes.AutoLimitEstimatedCosts,
                Rate = 50,
                Caption = "Mandatory student fees"
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 20,
                GrantApplicationId = 1,
                EstimatedCost = 100,
            };

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
            helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();
            var maxEstimatedCost = trainingCost.CalculateEstimatedCostLimit(eligibleCost);

            // message
            string validateMsg = "Estimated cost exceeded maximum allowed '" + maxEstimatedCost.ToString("c2") + "' for expense type '" + eligibleExpenseType?.Caption + "'";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }


        [Ignore, TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_Exceeds_Per_Participant_Fiscal_Year_Maximum_Reimbursement_Allowed()
        {
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            grantApplication.MaxReimbursementAmt = 400;
            grantApplication.ReimbursementRate = 50;
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                EstimatedParticipants = 15
            };
            var eligibleExpenseType = new EligibleExpenseType()
            {
                Id = 2,
                ExpenseTypeId = ExpenseTypes.ParticipantLimited,
                Rate = 50,
                Caption = "Mandatory student fees"
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 20,
                GrantApplicationId = 1,
                EstimatedParticipantCost = 10
            };

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
            helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            var maxReimbursementAmount = trainingCost.GrantApplication.MaxReimbursementAmt;

			// message
			string validateMsg = "The total requested government contribution exceeds the per participant fiscal year maximum reimbursement allowed of '"
								 + maxReimbursementAmount.ToString("c2") + "'";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_TotalCost_Exceeds_Limit_Of_Program_Total_cost()
        {
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                EstimatedParticipants = 15,
            };
            var eligibleExpenseType = new EligibleExpenseType()
            {
                Id = 2,
                ExpenseTypeId = ExpenseTypes.AutoLimitEstimatedCosts,
                Rate = 50,
                Caption = "Mandatory student fees"
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 20,
                GrantApplicationId = 1,
                EstimatedCost = 100,
            };
            string validateMsg = eligibleExpenseType.Caption + " Total Cost exceeds the limit for your program total cost.";

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
            helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_SumBreakDowns_Not_Exceed_EstimatedCost()
        {
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                EstimatedParticipants = 15,
            };
            var eligibleExpenseType = new EligibleExpenseType()
            {
                Id = 2,
                ExpenseTypeId = ExpenseTypes.AutoLimitEstimatedCosts,
                Rate = 50,
                Caption = "Mandatory student fees"
            };
            var eligibleCostBreakdown = new EligibleCostBreakdown()
            {
                EstimatedCost = 1000,
                Id = 1,
                EligibleCostId = 1
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 20,
                GrantApplicationId = 1,
                EstimatedCost = 100,
                Breakdowns = new List<EligibleCostBreakdown>()
                {
                    eligibleCostBreakdown
                }
            };
            string validateMsg = "The sum of the breakdown costs must not exceed the estimated cost of " + eligibleCost.EstimatedCost.ToString("c2") + ".";

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
            helper.MockDbSet<GrantApplication>(grantApplication);
            helper.MockDbSet<EligibleCostBreakdown>(eligibleCostBreakdown);
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_SumBreakdownCosts_NotExceed_AgreedMaxCost()
        {
			// Arrange
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                EstimatedParticipants = 15,
            };
            var eligibleExpenseType = new EligibleExpenseType()
            {
                Id = 2,
                ExpenseTypeId = ExpenseTypes.AutoLimitEstimatedCosts,
                Rate = 50,
                Caption = "Mandatory student fees"
            };
            var eligibleCostBreakdown = new EligibleCostBreakdown()
            {
                AssessedCost = 1000,
                Id = 1,
                EligibleCostId = 1
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 20,
                GrantApplicationId = 1,
                AgreedMaxCost = 100,
                Breakdowns = new List<EligibleCostBreakdown>()
                {
                    eligibleCostBreakdown
                }
            };
            string validateMsg = "The sum of the breakdown costs must not exceed the agreed maximum cost of " + eligibleCost.AgreedMaxCost.ToString("c2") + ".";

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
            helper.MockDbSet<GrantApplication>(grantApplication);
            helper.MockDbSet<EligibleCostBreakdown>(eligibleCostBreakdown);
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_EstimatedReimbursement_Not_Exceed_AvgReimbursement_Cost_Per_Participant_SkillsTraining()
        {
            var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                EstimatedParticipants = 15,
            };
            trainingCost.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId = ProgramTypes.WDAService;
            var serviceCategory = new ServiceCategory()
            {
                Id = 1,
                ServiceTypeId = ServiceTypes.SkillsTraining
            };
            var eligibleExpenseType = new EligibleExpenseType()
            {
                Id = 2,
                ExpenseTypeId = ExpenseTypes.AutoLimitEstimatedCosts,
                Rate = 50,
                Caption = "Mandatory student fees",
                ServiceCategoryId = 1,
                ServiceCategory = serviceCategory
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 20,
                GrantApplicationId = 1,
                EstimatedReimbursement = 100,
            };
            var skillTrainingMax = trainingCost.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts;
            string validateMsg = eligibleExpenseType.Caption + " average reimbursement cost per participant may not exceed '" + skillTrainingMax.ToString("c2") + "'.";

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
            helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_AgreedMaxReimbursement_Not_Exceed_AvgReimbursement_Cost_Per_Participant_SkillsTraining()
        {
            var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                EstimatedParticipants = 15,
            };
            trainingCost.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId = ProgramTypes.WDAService;
            var serviceCategory = new ServiceCategory()
            {
                Id = 1,
                ServiceTypeId = ServiceTypes.SkillsTraining
            };
            var eligibleExpenseType = new EligibleExpenseType()
            {
                Id = 2,
                ExpenseTypeId = ExpenseTypes.AutoLimitEstimatedCosts,
                Rate = 50,
                Caption = "Mandatory student fees",
                ServiceCategoryId = 1,
                ServiceCategory = serviceCategory
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 20,
                GrantApplicationId = 1,
                AgreedMaxReimbursement = 100,
                AgreedMaxParticipants = 10
            };
            var skillTrainingMax = trainingCost.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts;
            string validateMsg = eligibleExpenseType.Caption + " average reimbursement cost per participant may not exceed '" + skillTrainingMax.ToString("c2") + "'.";

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
            helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }


        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_EstimatedParticipantCost_Not_Exceed_AvgReimbursement_Cost_Per_Participant_EmploymentServicesAndSupports()
        {
            var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                EstimatedParticipants = 15,
            };
            trainingCost.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost = 2;
            trainingCost.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId = ProgramTypes.WDAService;
            var serviceCategory = new ServiceCategory()
            {
                Id = 1,
                ServiceTypeId = ServiceTypes.EmploymentServicesAndSupports
            };
            var eligibleExpenseType = new EligibleExpenseType()
            {
                Id = 2,
                ExpenseTypeId = ExpenseTypes.AutoLimitEstimatedCosts,
                Rate = 50,
                Caption = "Mandatory student fees",
                ServiceCategoryId = 1,
                ServiceCategory = serviceCategory
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 20,
                GrantApplicationId = 1,
                EstimatedParticipantCost = 100,
                EstimatedReimbursement = 50
            };
            var essMax = trainingCost.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost;
            string validateMsg = eligibleExpenseType.Caption + " average reimbursement cost per participant may not exceed '" + essMax.ToString("c2") + "'.";

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
            helper.MockDbSet<GrantApplication>(grantApplication);

            var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("Eligible Cost"), TestCategory("Validate")]
        public void Validate_When_EligibleCost_AgreedMaxReimbursement_Not_Exceed_AvgReimbursement_Cost_Per_Participant_EmploymentServicesAndSupports()
        {
            var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);
            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication,
                EstimatedParticipants = 15
            };
            trainingCost.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost = 2;
            trainingCost.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId = ProgramTypes.WDAService;
            var serviceCategory = new ServiceCategory()
            {
                Id = 1,
                ServiceTypeId = ServiceTypes.EmploymentServicesAndSupports
            };
            var eligibleExpenseType = new EligibleExpenseType()
            {
                Id = 2,
                ExpenseTypeId = ExpenseTypes.AutoLimitEstimatedCosts,
                Rate = 50,
                Caption = "Mandatory student fees",
                ServiceCategoryId = 1,
                ServiceCategory = serviceCategory
            };
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EligibleExpenseTypeId = 2,
                TrainingCost = trainingCost,
                EstimatedParticipants = 20,
                GrantApplicationId = 1,
                AgreedMaxParticipants = 10,
                AgreedMaxReimbursement = 50
            };
            var essMax = trainingCost.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost;
            string validateMsg = eligibleExpenseType.Caption + " average reimbursement cost per participant may not exceed '" + essMax.ToString("c2") + "'.";

            helper.MockDbSet<EligibleCost>(eligibleCost);
            helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
            helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<GrantStream>();
			helper.MockDbSet<TrainingCost>();
			helper.MockDbSet<ServiceCategory>();

			var service = helper.Create<EligibleCostService>();

            // Act
            var validationResults = service.Validate(eligibleCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }
    }
}