using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass]
	public class ClaimEligibleCostValidationTests
	{
		private ServiceHelper helper;
		private User user;

		[TestInitialize]
		public void Setup()
		{
			// Arrange
			user = EntityHelper.CreateExternalUser();
			helper = new ServiceHelper(typeof(ClaimService), user);
			helper.MockContext();
			helper.MockDbSet<ParticipantCost>();
			helper.MockDbSet<TrainingProgram>();
			helper.MockDbSet<GrantApplication>();
			helper.MockDbSet<Claim>();
			helper.MockDbSet<ClaimEligibleCost>();
		}

		[TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_Not_Associated_With_EligibleExpenseType()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 25;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var eligibleCost = new EligibleCost() { Id = 1, EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited, TrainingCost = trainingCost };
			var claimEligibleCost = new ClaimEligibleCost() { Id = 1, Claim = claim, EligibleCostId = 1, EligibleCost = eligibleCost };
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimEligibleCost = claimEligibleCost };

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "The claim eligible cost must be associated with a eligible expense type.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_Not_Associated_With_EligibleExpense()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 25;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1, ClaimVersion = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var eligibleCost = new EligibleCost() { Id = 1, EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited, TrainingCost = trainingCost };
			var claimEligibleCost = new ClaimEligibleCost() { Id = 1, Claim = claim};
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimEligibleCost = claimEligibleCost };

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "The claim eligible cost must be associated with an eligible expense.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_ClaimParticipants_AgreedMaxParticipants_Or_AssessedParticipants()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				EligibleExpenseType = eligibleExpenseType,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10
			};
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimEligibleCost = claimEligibleCost };

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "Number of participants claimed cannot exceed the agreed maximum number of participants.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_ClaimCost_Exceeds_AgreedMaxCost_Or_AssessedCost()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				EligibleExpenseType = eligibleExpenseType,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10
			};
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimEligibleCost = claimEligibleCost };

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "The new claim Total Cost cannot exceed the remaining to be claimed amount";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains(validateMsg)));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_ClaimMaxParticipantCost_Not_Equal_MaxParticipantCost()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantAssigned));
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantAssigned,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantAssigned,
				EligibleExpenseType = eligibleExpenseType,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10
			};
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimEligibleCost = claimEligibleCost };

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			var rate = claim.GrantApplication.ReimbursementRate;
			var maxParticipantCost = eligibleCost?.AgreedMaxParticipantCost ?? claimEligibleCost.AssessedMaxParticipantCost;
			var validClaimParticipantCost = claimEligibleCost.CalculateClaimParticipantCost();
			var validClaimParticipantReimbursement = claimEligibleCost.CalculateClaimMaxParticipantReimbursement();

			if (validClaimParticipantCost > maxParticipantCost)
			{
				validClaimParticipantCost = maxParticipantCost;
				validClaimParticipantReimbursement = Math.Truncate(validClaimParticipantCost * (decimal)rate * 100) / 100;
			}

			string validateMsg = $"The claim max participant cost is invalid and should be {validClaimParticipantCost:C}.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_ClaimMaxParticipantReimbursementCost_Not_Equal_MaxParticipantReimbursement()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantAssigned));
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantAssigned,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantAssigned,
				EligibleExpenseType = eligibleExpenseType,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10
			};
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimEligibleCost = claimEligibleCost };

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			var rate = claim.GrantApplication.ReimbursementRate;
			var maxParticipantCost = eligibleCost?.AgreedMaxParticipantCost ?? claimEligibleCost.AssessedMaxParticipantCost;
			var validClaimParticipantCost = claimEligibleCost.CalculateClaimParticipantCost();
			var validClaimParticipantReimbursement = claimEligibleCost.CalculateClaimMaxParticipantReimbursement();

			if (validClaimParticipantCost > maxParticipantCost)
			{
				validClaimParticipantCost = maxParticipantCost;
				validClaimParticipantReimbursement = Math.Truncate(validClaimParticipantCost * (decimal)rate * 100) / 100;
			}

			string validateMsg = $"The claim max participant reimbursement cost is invalid and should be {validClaimParticipantReimbursement:C}.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_ClaimMaxParticipantCost_Not_Equal_Reimbursement_Plus_EmployerContribution()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				EligibleExpenseType = eligibleExpenseType,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10
			};
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimEligibleCost = claimEligibleCost };

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "The claim max participant cost must equal the reimbursement + employer contribution.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_Each_ClaimParticipantCost_Exceeds_MaxParticipantCost()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType() { Id = 2, Caption = "Mandatory student fees" };
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimParticipantCost = 800, AssessedParticipantCost = 50};
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost },
				EligibleExpenseType = new EligibleExpenseType() { Caption = "Expense Type Caption" }
			};            

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "You may not exceed the maximum cost or reimbursement per participant for the expense type '" + eligibleExpenseType?.Caption + "'.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_Each_ClaimReimbursement_Exceeds_MaxParticipantCost_MaxParticipantReimbursement()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType() { Id = 2, Caption = "Mandatory student fees" };
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimParticipantCost = 1, AssessedParticipantCost = 50 , ClaimReimbursement = 800};
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost },
				EligibleExpenseType = new EligibleExpenseType() { Caption = "Expense Type Caption" }
			};

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "You may not exceed the maximum cost or reimbursement per participant for the expense type '" + eligibleExpenseType?.Caption + "'.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_AssessedParticipants_Exceeds_MaxAssessedParticipants()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimParticipantCost = 800, AssessedParticipantCost = 50 };
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				EligibleExpenseType = eligibleExpenseType,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 50,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost }
			};

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "Number of Participants cannot exceed Agreement Number of Participants.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_AssessedGovernmentContribution_Exceeds_RemainingToBeClaimedFor()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimParticipantCost = 800, AssessedParticipantCost = 50 };
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				EligibleExpenseType = eligibleExpenseType,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 50,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost }
			};

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "The assessed Total Cost cannot exceed the remaining to be claimed amount of";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains(validateMsg)));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_AssessedMaxParticipantCost_Not_Equal_AssessedCost_DividedBy_AssessedParticipants()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantAssigned));
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimParticipantCost = 800, AssessedParticipantCost = 50 };
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantAssigned,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantAssigned,
				EligibleExpenseType = eligibleExpenseType,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 50,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost }
			};

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			var maxParticipantCost = eligibleCost?.AgreedMaxParticipantCost ?? claimEligibleCost.AssessedMaxParticipantCost;
			var validAssessedParticipantCost = claimEligibleCost.CalculateAssessedParticipantCost();
			var maxAssessedParticipantCost = eligibleCost?.AgreedMaxParticipantCost ?? validAssessedParticipantCost;

			if (validAssessedParticipantCost > maxParticipantCost)
			{
				validAssessedParticipantCost = maxParticipantCost;
			}

			string validateMsg = $"The assessed max participant cost is invalid and should be {validAssessedParticipantCost:C}.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_AssessedMaxParticipantReimbursementCost_Exceed_AssessedMaxParticipantCost()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimParticipantCost = 800, AssessedParticipantCost = 50 };
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				EligibleExpenseType = eligibleExpenseType,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 55,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost }
			};

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "The assessed max participant reimbursement must be less than or equal to the assessed max participant cost.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_AssessedMaxParticipantCost_Exceeds_SumOf_AssessedReimbursement_And_EmployerContribution()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimParticipantCost = 800, AssessedParticipantCost = 50 };
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				EligibleExpenseType = eligibleExpenseType,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost }
			};

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "The assessed max participant cost does not equal the sum of the assessed reimbursement and employer contribution.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_AssessedMaxParticipantReimbursementCost_Exceeds_MaxAssessedParticipantReimbursement()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantAssigned));
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimParticipantCost = 800, AssessedParticipantCost = 50 };
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantAssigned,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantAssigned,
				EligibleExpenseType = eligibleExpenseType,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 50,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost }
			};

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "The assessed max participant reimbursement is greater than the agreed max participant reimbursement.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_AssessedParticipantCost_Exceeds_MaxAssessedParticipantCost()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType() { Id = 2, Caption = "Mandatory student fees" };
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimParticipantCost = 800, AssessedParticipantCost = 50 };
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost },
				EligibleExpenseType = eligibleExpenseType
			};

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "You may not exceed the maximum cost or reimbursement per participant for the expense type '" + eligibleExpenseType?.Caption + "'.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_AssessedReimbursement_Exceeds_MaxAssessedParticipantReimbursement()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType() { Id = 2, Caption = "Mandatory student fees" };
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimParticipantCost = 800, AssessedParticipantCost = 1, AssessedReimbursement = 50 };
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 100
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 5,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost },
				EligibleExpenseType = new EligibleExpenseType() { Caption = "Expense Type Caption" }
			};

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "You may not exceed the maximum cost or reimbursement per participant for the expense type '" + eligibleExpenseType.Caption + "'.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[Ignore, TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_AssessedReimbursement_Exceeds_AssessedMaxParticipantReimbursementCost()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType() { Id = 2, Caption = "Mandatory student fees" };
			var participantCost = new ParticipantCost() { Id = 1, ParticipantFormId = 1, ClaimParticipantCost = 800, AssessedParticipantCost = 1, AssessedReimbursement = 50 };
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				AssessedParticipants = 12,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 15,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost },
				EligibleExpenseType = new EligibleExpenseType() { Caption = "Expense Type Caption" }
			};

			helper.MockDbSet<ParticipantCost>(participantCost);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "You may not exceed the maximum cost or reimbursement per participant for the expense type '" + eligibleExpenseType.Caption + "'.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_NumberClaimParticipants_Exceeds_NumberClaimParticipantCost()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var participantCost_1 = new ParticipantCost()
			{
				Id = 1,
				ParticipantFormId = 1,
				ClaimParticipantCost = 800,
				AssessedParticipantCost = 50
			};
			var participantCost_2 = new ParticipantCost()
			{
				Id = 2,
				ParticipantFormId = 1,
				ClaimParticipantCost = 500,
				AssessedParticipantCost = 20
			};
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = 2,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = 2,
				AssessedParticipants = 1,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 1,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost_1, participantCost_2 },
				EligibleExpenseType = new EligibleExpenseType() { Caption = "Expense Type Caption"}
			};

			helper.MockDbSet<ParticipantCost>(participantCost_1);
			helper.MockDbSet<ParticipantCost>(participantCost_2);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			string validateMsg = "Number of participants with assigned cost exceeds Maximum Number of Participants.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}

		[TestMethod, TestCategory("Claim Eligible Cost"), TestCategory("Validate")]
		public void Validate_When_ClaimEligibleCost_Not_Have_Unique_ParticipantCost_Per_EligibleCost()
		{
			var grantApplication = EntityHelper.CreateGrantApplication(
				user,
				ApplicationStateInternal.AgreementAccepted);

			grantApplication.ReimbursementRate = 2;

			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };
			var claim = new Claim() { Id = 1, GrantApplication = grantApplication, GrantApplicationId = 1 };
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication, GrantApplicationId = 1, AgreedParticipants = 10 };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var participantCost_1 = new ParticipantCost()
			{
				Id = 1,
				ParticipantFormId = 1,
				ClaimParticipantCost = 800,
				AssessedParticipantCost = 50
			};
			var participantCost_2 = new ParticipantCost()
			{
				Id = 2,
				ParticipantFormId = 1,
				ClaimParticipantCost = 500,
				AssessedParticipantCost = 20
			};
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				TrainingCost = trainingCost,
				AgreedMaxCost = 800,
				AgreedMaxParticipants = 10,
				AgreedMaxParticipantCost = 25
			};
			var claimEligibleCost = new ClaimEligibleCost()
			{
				Id = 1,
				Claim = claim,
				EligibleCostId = 1,
				EligibleCost = eligibleCost,
				EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited,
				AssessedParticipants = 1,
				AssessedCost = 1000,
				AssessedMaxParticipantCost = 50,
				AssessedMaxParticipantReimbursementCost = 25,
				ClaimCost = 1000,
				ClaimParticipants = 1,
				ClaimMaxParticipantCost = 10,
				ParticipantCosts = { participantCost_1, participantCost_2 },
				EligibleExpenseType = new EligibleExpenseType() { Caption = "Expense Type Caption" }
			};
			string validateMsg = "A single claim eligible cost must not have more than one participant cost associated to a single participant.";

			helper.MockDbSet<ParticipantCost>(participantCost_1);
			helper.MockDbSet<ParticipantCost>(participantCost_2);
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<GrantApplication>(grantApplication);
			helper.MockDbSet<Claim>(claim);
			helper.MockDbSet<ClaimEligibleCost>(claimEligibleCost);
			helper.MockDbSet<EligibleCost>(eligibleCost);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet<TrainingCost>(trainingCost);
			helper.MockDbSet<ClaimBreakdownCost>(new List<ClaimBreakdownCost>());

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(claimEligibleCost).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
		}
	}
}