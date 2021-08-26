using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class ClaimEligibleCostServiceTest : ServiceUnitTestBase
	{
		#region Initialize
		[TestInitialize]
		public void Setup()
		{
		}
		#endregion

		#region Tests
		/// <summary>
		/// Getting a <typeparamref name="ClaimEligibleCost"/> with an unauthorized user should throw a <typeparamref name="NotAuthorizedException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void GetClaimEligibleCost_WithUnauthorizedUserShouldThrowNotAuthorizedException()
		{
			// Arrange
			const int id = 1;

			var internalUser = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(internalUser, Privilege.IA2);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(internalUser, ApplicationStateInternal.AgreementAccepted);
			var claim = EntityHelper.CreateClaim(grantApplication);
			grantApplication.Claims.Add(claim);
			var claimEligibleCost = new ClaimEligibleCost(claim)
			{
				Id = id
			};

			var helper = new ServiceHelper(typeof(ClaimEligibleCostService), identity);
			helper.MockDbSet(claimEligibleCost);
			helper.GetMock<IDataContext>().Setup(m => m.ClaimEligibleCosts.Add(It.IsAny<ClaimEligibleCost>()));
			var service = helper.Create<ClaimEligibleCostService>();

			// Act & Assert (handled by decorator)
			service.Get(id);
		}

		/// <summary>
		/// Getting a <typeparamref name="ClaimEligibleCost"/> should return a <typeparamref name="ClaimEligibleCost"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetClaimEligibleCost_ShouldReturnClaimEligibleCost()
		{
			// Arrange
			const int id = 1;

			var internalUser = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(internalUser, "Financial Clerk");
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(internalUser, ApplicationStateInternal.AgreementAccepted);
			var claim = EntityHelper.CreateClaim(grantApplication);
			grantApplication.Claims.Add(claim);
			var claimEligibleCost = new ClaimEligibleCost(claim)
			{
				Id = id
			};

			var helper = new ServiceHelper(typeof(ClaimEligibleCostService), identity);
			helper.MockDbSet(claimEligibleCost);
			helper.GetMock<IDataContext>().Setup(m => m.ClaimEligibleCosts.Add(It.IsAny<ClaimEligibleCost>()));
			var service = helper.Create<ClaimEligibleCostService>();

			// Act
			var result = service.Get(id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ClaimEligibleCost>();
			result.Id.Should().Be(id);
		}

		/// <summary>
		/// Updating a <typeparamref name="ClaimEligibleCost"/> with an unauthorized user should throw a <typeparamref name="NotAuthorizedException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void UpdateClaimEligibleCost_WithUnauthorizedUserShouldThrowNotAuthorizedException()
		{
			// Arrange
			const int id = 1;

			var internalUser = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(internalUser, Privilege.IA1);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(internalUser, ApplicationStateInternal.AgreementAccepted);
			var claim = EntityHelper.CreateClaim(grantApplication);
			grantApplication.Claims.Add(claim);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var claimEligibleCost = new ClaimEligibleCost(claim, eligibleCost)
			{
				Id = id,
				EligibleExpenseType = new EligibleExpenseType("caption", "description")
				{
					Id = id,
					ExpenseType = EntityHelper.CreateExpenseType()
				}
			};
			var claimEligibleCostModel = new ClaimEligibleCostModel(claimEligibleCost);

			var helper = new ServiceHelper(typeof(ClaimEligibleCostService), identity);
			helper.MockDbSet(claimEligibleCost);
			helper.GetMock<IDataContext>().Setup(m => m.ClaimEligibleCosts.Add(It.IsAny<ClaimEligibleCost>()));
			var service = helper.Create<ClaimEligibleCostService>();

			// Act & Assert (handled by decorator)
			service.Update(new List<ClaimEligibleCostModel>(new[] { claimEligibleCostModel }));
		}

		/// <summary>
		/// Updating a <typeparamref name="ClaimEligibleCost"/> as an external user with a service type should update the <typeparamref name="Claim"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void UpdateClaimEligibleCost_AsExternalUser_WithServiceType_ShouldUpdateClaim()
		{
			// Arrange
			const int id = 1;
			const decimal claimParticipantCost = 5;

			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var claim = EntityHelper.CreateClaim(grantApplication);
			grantApplication.Claims.Add(claim);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var claimEligibleCost = new ClaimEligibleCost(claim, eligibleCost)
			{
				Id = id,
				EligibleExpenseType = new EligibleExpenseType("caption", "description")
				{
					Id = id,
					ExpenseType = EntityHelper.CreateExpenseType()
				}
			};
			claimEligibleCost.ParticipantCosts.First().ClaimParticipantCost = claimParticipantCost;
			var claimEligibleCostModel = new ClaimEligibleCostModel(claimEligibleCost);

			var helper = new ServiceHelper(typeof(ClaimEligibleCostService), identity);
			helper.MockDbSet(claim);
			helper.MockDbSet(claimEligibleCost);
			helper.GetMock<IDataContext>().Setup(m => m.ClaimEligibleCosts.Add(It.IsAny<ClaimEligibleCost>()));
			var service = helper.Create<ClaimEligibleCostService>();

			// Act
			service.Update(new List<ClaimEligibleCostModel>(new[] { claimEligibleCostModel }));

			// Assert
			claimEligibleCost.ParticipantCosts.Should().NotBeEmpty().And.HaveCount(1);
			claimEligibleCost.ParticipantCosts.First().ClaimParticipantCost.Should().Be(claimParticipantCost);
			helper.GetMock<IClaimService>().Verify(x => x.Update(It.IsAny<Claim>(), false), Times.Once);
		}

		/// <summary>
		/// Updating a <typeparamr name="ClaimEligibleCost"/> as an internal user with participant costs should update the <typeparamref name="Claim"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void UpdateClaimEligibleCost_AsInternalUser_WithParticipantCosts_ShouldUpdateClaim()
		{
			// Arrange
			const int id = 1;
			const decimal assessedCost = 10;
			const decimal assessedParticipantCost = 10;
			const decimal assessedReimbursement = 8;
			const decimal assessedEmployerContribution = 2;

			var applicant = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicant);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicant, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var claim = EntityHelper.CreateClaim(grantApplication);
			grantApplication.Claims.Add(claim);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var eligibleExpenseType = new EligibleExpenseType("caption", "description")
			{
				Id = id,
				ExpenseType = EntityHelper.CreateExpenseType()
			};
			var claimEligibleCost = new ClaimEligibleCost(claim, eligibleCost)
			{
				Id = id,
				EligibleExpenseType = eligibleExpenseType
			};
			var eligibleCostBreakdown = new EligibleCostBreakdown {
				TrainingPrograms = { new TrainingProgram { CourseTitle = "course title" }}
			};
			var claimBreakDownCost = new ClaimBreakdownCost()
			{
				AssessedCost = 10,
				ClaimEligibleCost = claimEligibleCost,
				EligibleExpenseBreakdown = new EligibleExpenseBreakdown("caption", eligibleExpenseType),
				EligibleCostBreakdown = eligibleCostBreakdown,
				RowVersion = new byte[9999]
			};
			claimEligibleCost.Breakdowns.Add(claimBreakDownCost);
			var participantCost = claimEligibleCost.ParticipantCosts.First();
			participantCost.AssessedParticipantCost = assessedParticipantCost;
			participantCost.AssessedReimbursement = assessedReimbursement;
			participantCost.AssessedEmployerContribution = assessedEmployerContribution;

			var claimEligibleCostModel = new ClaimEligibleCostModel(claimEligibleCost)
			{
				ServiceType = ServiceTypes.SkillsTraining
			};
			claimEligibleCostModel.Breakdowns.Add(new ClaimEligibleCostBreakdownModel(claimBreakDownCost));

			var helper = new ServiceHelper(typeof(ClaimEligibleCostService), identity);
			helper.MockDbSet(claim);
			helper.MockDbSet(claimEligibleCost);
			helper.GetMock<IDataContext>().Setup(m => m.ClaimEligibleCosts.Add(It.IsAny<ClaimEligibleCost>()));
			var service = helper.Create<ClaimEligibleCostService>();

			// Act
			service.Update(new List<ClaimEligibleCostModel>(new[] { claimEligibleCostModel }));

			// Assert
			claimEligibleCost.Breakdowns.Should().NotBeEmpty().And.HaveCount(1);
			claimEligibleCost.Breakdowns.First().AssessedCost.Should().Be(assessedCost);
			claimEligibleCost.ParticipantCosts.Should().NotBeEmpty().And.HaveCount(1);
			claimEligibleCost.ParticipantCosts.First().AssessedParticipantCost.Should().Be(assessedParticipantCost);
			claimEligibleCost.ParticipantCosts.First().AssessedReimbursement.Should().Be(assessedReimbursement);
			claimEligibleCost.ParticipantCosts.First().AssessedEmployerContribution.Should().Be(assessedEmployerContribution);
			helper.GetMock<IClaimService>().Verify(x => x.Update(It.IsAny<Claim>(), false), Times.Once);
		}
		#endregion
	}
}
