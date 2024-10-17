using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
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
	public class TrainingServiceTest : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void AddTrainingProvider_PassInTrainingProvider_ShouldAddToDataStore()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProviderService), applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplication(applicationAdministrator, ApplicationStateInternal.Draft);
			var newTrainingProvider = new TrainingProvider()
			{
				Id = 1,
				ContactFirstName = "First Name",
				ContactLastName = "Last Name",
				GrantApplication = grantApplication,
				GrantApplicationId = 1
			};
			var service = helper.Create<TrainingProviderService>();
			var dbSetMock = helper.MockDbSet(newTrainingProvider);

			// Act
			service.Add(newTrainingProvider);

			// Assert
			dbSetMock.Verify(x => x.Add(It.IsAny<TrainingProvider>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void AddTrainingProgram_PassInTrainingProgram_ShouldAddToDataStore()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProgramService), applicationAdministrator);

			var grantApplication = EntityHelper.CreateGrantApplication(applicationAdministrator, ApplicationStateInternal.Draft);
			var newTrainingProgram = new TrainingProgram()
			{
				Id = 1,
				CourseTitle = "A test program",
				GrantApplicationId = 1,
				GrantApplication = grantApplication
			};

			var dbSetMock = helper.MockDbSet<TrainingProgram>(newTrainingProgram);
			helper.MockDbSet<GrantApplication>();

			var service = helper.Create<TrainingProgramService>();

			// Act
			service.Add(newTrainingProgram);

			// Assert
			dbSetMock.Verify(x => x.Add(It.IsAny<TrainingProgram>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void AddEligibleCost_PassInEligibleCost_ShouldAddToDataStore()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var grantApplication = EntityHelper.CreateGrantApplication(applicationAdministrator, ApplicationStateInternal.Draft);
			var eligibleCost = new EligibleCost(new TrainingCost(grantApplication, 1), new EligibleExpenseType(), 50, 1);
			var helper = new ServiceHelper(typeof(EligibleCostService), applicationAdministrator);

			var mockDbSet = helper.MockDbSet<EligibleCost>();
			var service = helper.Create<EligibleCostService>();

			// Act
			service.Add(eligibleCost);

			// Assert
			mockDbSet.Verify(x => x.Add(It.IsAny<EligibleCost>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void GetEligibleCosts_PassInTrainingProgramId_ShouldReturnAllCostsForTheProgram()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(GrantApplicationService), applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplication(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.TrainingCost = new TrainingCost()
			{
				GrantApplicationId = 1,
				EligibleCosts = new List<EligibleCost>()
				{
					new EligibleCost() { Id = 1, EstimatedCost = 10, EligibleExpenseTypeId = 1 },
					new EligibleCost() { Id = 2, EstimatedCost = 20, EligibleExpenseTypeId = 2}
				}
			};
			var trainingProgram = new TrainingProgram()
			{
				Id = 1,
				CourseTitle = "Training program",
				GrantApplication = grantApplication
			};
			
			helper.MockDbSet(trainingProgram);
			helper.MockDbSet(grantApplication);
			var service = helper.Create<GrantApplicationService>();

			// Act
			var results = service.Get(trainingProgram.Id).TrainingCost.EligibleCosts;

			// Assert
			results.Count().Should().Be(2);
			results.ToList()[0].Id.Should().Be(1);
			results.ToList()[1].Id.Should().Be(2);
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void GetEligibleCosts_PassInTrainingProgramId_ShouldReturnNull()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(GrantApplicationService), applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplication(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.TrainingCost = new TrainingCost()
			{
				GrantApplicationId = 1
			};
			var trainingProgram = new TrainingProgram(grantApplication)
			{
				Id = 1,
				CourseTitle = "Training program"
			};
			helper.MockDbSet(trainingProgram);
			helper.MockDbSet(grantApplication);
			var service = helper.Create<GrantApplicationService>();

			// Act
			var results = service.Get(grantApplication.Id).TrainingCost.EligibleCosts;

			// Assert
			results.Should().BeNullOrEmpty();
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void GetEligibleCost_PassInCostId_ShouldReturnEligibleCost()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(EligibleCostService), applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplication(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var trainingCost = new TrainingCost()
			{
				GrantApplication = grantApplication
			};
			var searchEligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = 2,
				TrainingCost = trainingCost
			};
			helper.MockDbSet(searchEligibleCost);
			var service = helper.Create<EligibleCostService>();

			// Act
			var results = service.Get(searchEligibleCost.Id);

			// Assert
			results.Id.Should().Be(searchEligibleCost.Id);
			results.EligibleExpenseTypeId.Should().Be(searchEligibleCost.EligibleExpenseTypeId);
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void UpdateEligibleCost_PassInEligibleCost_ShouldUpdateEligibleCostAndProgramInDataStore()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(EligibleCostService), applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplication(applicationAdministrator, ApplicationStateInternal.Draft);
			grantApplication.TrainingPrograms = new List<TrainingProgram>()
				{
					new TrainingProgram() {
						Id = 1,
						CourseTitle = "Training Program"
					}
				};
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = 2,
				TrainingCost = new TrainingCost()
				{
					GrantApplicationId = 1,
					GrantApplication = grantApplication
				}
			};
			var dbMockSet = helper.MockDbSet(eligibleCost);
			var dbMockSetTraining = helper.MockDbSet<TrainingProgram>();

			var service = helper.Create<EligibleCostService>();

			// Act
			service.Update(eligibleCost);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<EligibleCost>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void DeleteEligibleCost_PassInEligibleCost_ShouldRemoveEligibleCostFromDataStore()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(EligibleCostService), applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplication(applicationAdministrator, ApplicationStateInternal.Draft);
			grantApplication.TrainingPrograms = new List<TrainingProgram>()
				{
					new TrainingProgram() {
						Id = 1,
						CourseTitle = "Training Program"
					}
				};
			var eligibleCost = new EligibleCost()
			{
				Id = 1,
				EligibleExpenseTypeId = 2,
				TrainingCost = new TrainingCost()
				{
					GrantApplicationId = 1,
					GrantApplication = grantApplication
				}
			};
			var dbSetMock = helper.MockDbSet(eligibleCost);
			var service = helper.Create<EligibleCostService>();

			// Act
			service.Delete(eligibleCost);

			// Assert
			dbSetMock.Verify(x => x.Remove(It.IsAny<EligibleCost>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void AddTrainingProviderInventory_PassInTrainingProviderInventory_ShouldAddToDataStore()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProviderInventoryService), applicationAdministrator);
			var mockDbSet = helper.MockDbSet<TrainingProviderInventory>();
			var service = helper.Create<TrainingProviderInventoryService>();

			// Act
			service.Add(new TrainingProviderInventory());

			// Assert
			mockDbSet.Verify(x => x.Add(It.IsAny<TrainingProviderInventory>()));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void GetAllTrainingProviderInventory_WithPageQuantity()
		{
			// TODO
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void SearchTrainingProviderInventory_WithNamePageQuantity()
		{
			// TODO
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void UpdateTrainingProviderInventory_PassInTrainingProviderInventory_ShouldUpdateDataInDataStore()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProviderInventoryService), applicationAdministrator);
			var dbSetMock = helper.MockDbSet<TrainingProviderInventory>();

			var service = helper.Create<TrainingProviderInventoryService>();

			// Act
			service.Add(new TrainingProviderInventory());

			// Assert
			dbSetMock.Verify(x => x.Add(It.IsAny<TrainingProviderInventory>()));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void DeleteTrainingProviderInventory_PassInParameter_ShouldRemoveRecordAndReturnSuccessMessage()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProviderInventoryService), applicationAdministrator);
			string retMessageType = "", retMessage = "";
			var trainingProviderInventory = new TrainingProviderInventory()
			{
				Id = 1,
				Name = "Training Provider will be deleted"
			};
			var trainingProviders = new List<TrainingProvider>();
			var dbSetMock = helper.MockDbSet(trainingProviders);
			helper.MockDbSet(trainingProviderInventory);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.TrainingProviders.AsNoTracking()).Returns(dbSetMock.Object);

			var service = helper.Create<TrainingProviderInventoryService>();

			// Act
			service.Delete(trainingProviderInventory.Id, ref retMessageType, ref retMessage);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Exactly(1));
			retMessageType.Should().Be("");
			retMessage.Should().Contain("deleted successfully");
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void DeleteTrainingProviderInventory_PassInParameter_ShouldReturnErrorMessage()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProviderInventoryService), applicationAdministrator);
			string retMessageType = "", retMessage = "";
			var trainingProviderInventory = new TrainingProviderInventory()
			{
				Id = 1,
				Name = "Training Provider will be deleted"
			};
			var trainingProviders = new List<TrainingProvider>()
			{
				new TrainingProvider() { Id = 1, Name = "Provider Name", TrainingProviderInventoryId = 1 }
			};
			helper.MockDbSet(trainingProviderInventory);
			var dbSetMock = helper.MockDbSet(trainingProviders);

			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.TrainingProviders.AsNoTracking()).Returns(dbSetMock.Object);

			var service = helper.Create<TrainingProviderInventoryService>();

			// Act
			service.Delete(trainingProviderInventory.Id, ref retMessageType, ref retMessage);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Never);
			retMessageType.Should().Be("W");
			retMessage.Should().Contain("has been used and cannot be deleted");
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void GetTrainingProviderInventory_PassInId_ShouldReturnTrainingProviderInventory()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProviderInventoryService), applicationAdministrator);
			var trainingProviderInventory = new TrainingProviderInventory()
			{
				Id = 1,
				Name = "Training Provider"
			};
			helper.MockDbSet(trainingProviderInventory);
			var service = helper.Create<TrainingProviderInventoryService>();

			// Act
			var result = service.Get(trainingProviderInventory.Id);

			// Assert
			result.Id.Should().Be(trainingProviderInventory.Id);
			result.Name.Should().Be(trainingProviderInventory.Name);
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void GetTrainingProvidersInventory_ShouldReturnTrainingProviderInventorys()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProviderInventoryService), applicationAdministrator);
			var trainingProviderInventorys = new List<TrainingProviderInventory>()
			{
				new TrainingProviderInventory() { Id = 1, Name = "Training Provider2"},
				new TrainingProviderInventory() { Id = 2, Name = "Training Provider1"}
			};
			var dbSetMock = helper.MockDbSet(trainingProviderInventorys);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.TrainingProviderInventory).Returns(dbSetMock.Object);

			var service = helper.Create<TrainingProviderInventoryService>();

			// Act
			var results = service.GetInventory(1, int.MaxValue, null);

			// Assert
			results.Items.Count().Should().Be(2);
			results.Items.ToList()[0].Name.Should().Be("Training Provider1");
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void GetActiveTrainingProvidersInventory_ShouldReturnTrainingProviderInventorys()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProviderInventoryService), applicationAdministrator);
			var trainingProviderInventorys = new List<TrainingProviderInventory>()
			{
				new TrainingProviderInventory() { Id = 2, Name = "Training Provider1", IsActive = true},
				new TrainingProviderInventory() { Id = 3, Name = "Training Provider3", IsActive = true},
			};
			var dbSetMock = helper.MockDbSet(trainingProviderInventorys); 

			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.TrainingProviderInventory.AsNoTracking()).Returns(dbSetMock.Object);

			var service = helper.Create<TrainingProviderInventoryService>();

			// Act
			var results = service.GetActiveTrainingProvidersFromInventory();

			// Assert
			results.Count().Should().Be(2);
			results.ToList()[0].Name.Should().Be("Training Provider1");
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void IsTrainingProviderInventoryUsedInApplications_PassInTrainingProviderInventoryID_ShouldReturnTrue()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProviderInventoryService), applicationAdministrator);
			var trainingProviders = new List<TrainingProvider>()
			{
				new TrainingProvider() { Id = 2, Name = "Training Provider1", TrainingProviderInventoryId = 2},
			};
			var dbSetMock = helper.MockDbSet(trainingProviders);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.TrainingProviders.AsNoTracking()).Returns(dbSetMock.Object);

			var service = helper.Create<TrainingProviderInventoryService>();

			// Act
			var result = service.IsTrainingProviderInventoryUsedInApplications(2);

			// Assert
			result.Should().Be(true);
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void IsTrainingProviderInventoryUsedInApplications_PassInTrainingProviderInventoryID_ShouldReturnfalse()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProviderInventoryService), applicationAdministrator);
			var trainingProviders = new List<TrainingProvider>();
			var service = helper.Create<TrainingProviderInventoryService>();
			var dbSetMock = helper.MockDbSet(trainingProviders);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.TrainingProviders.AsNoTracking()).Returns(dbSetMock.Object);

			// Act
			var result = service.IsTrainingProviderInventoryUsedInApplications(2);

			// Assert
			result.Should().Be(false);
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void RemoveParticipant_PassInParameters_ShouldRemoveParticipantFromDataStore()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(ParticipantService), applicationAdministrator);
			var participantForm = new ParticipantForm() { Id = 1, GrantApplicationId = 1 };
			var grantApplication = EntityHelper.CreateGrantApplication(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var trainingProgram = new TrainingProgram(grantApplication);

			var participantForms = new List<ParticipantForm>()
			{
				new ParticipantForm(grantApplication, Guid.NewGuid()) { Id = 1 }
			};
			var claim = new Claim(1, 1, grantApplication);
			var claimEligibleCost = new ClaimEligibleCost(claim);
			var participantCosts = new List<ParticipantCost>()
			{
				new ParticipantCost(claimEligibleCost, 1, 0)
			};
			grantApplication.Claims.Add(claim);

			helper.MockDbSet(grantApplication);
			var dbSetMock = helper.MockDbSet(participantForm);
			helper.MockDbSet<ParticipantCost>();
			helper.MockDbSet<Claim>();
			helper.MockDbSet(trainingProgram);
			helper.MockDbSet<ParticipantCompletionReportAnswer>();

			var service = helper.Create<ParticipantService>();

			// Act
			service.RemoveParticipant(participantForms.First());

			// Assert
			dbSetMock.Verify(x => x.Remove(It.IsAny<ParticipantForm>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void RemoveParticipant_PassInParameters_ShouldNotRemoveParticipantFromDataStore()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(ParticipantService), applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplication(
				applicationAdministrator, 
				ApplicationStateInternal.AgreementAccepted);
			var participantForm = new ParticipantForm(grantApplication, Guid.NewGuid()) { ClaimReported = true };
			var trainingProgram = new TrainingProgram()
			{
				Id = 2,
				GrantApplication = grantApplication                
			};
			var claim = new Claim(1, 1, grantApplication);
			var claimEligibleCost = new ClaimEligibleCost(claim);
			var participantCost = new ParticipantCost(claimEligibleCost, 1, 0);
			var participantCompletionReportAnswer = new ParticipantCompletionReportAnswer() { ParticipantFormId = 1 };
			grantApplication.Claims.Add(claim);
			helper.MockDbSet(participantForm);
			helper.MockDbSet(participantCost);
			helper.MockDbSet(trainingProgram);
			helper.MockDbSet(grantApplication);
			helper.MockDbSet(claim);
			helper.MockDbSet(claimEligibleCost);
			helper.MockDbSet(participantCompletionReportAnswer);
			var service = helper.Create<ParticipantService>();

			// Act
			Action action = () => service.RemoveParticipant(participantForm);

			// Assert
			CoreAssert.Throws<NotAuthorizedException>(action);
			helper.GetMock<IDataContext>().Verify(x => x.ParticipantForms.Remove(It.IsAny<ParticipantForm>()), Times.Never);
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Never);
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void GetDefaultTrainingProviderType_ShouldReturnDefaultTrainingProviderType()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(TrainingProviderService), applicationAdministrator);
			var trainingProviderType = new TrainingProviderType()
			{
				Id = 1,
				IsActive = true,
				Caption = "Provider Type"
			};
			helper.MockDbSet(trainingProviderType);
			var service = helper.Create<TrainingProviderService>();

			// Act
			var result = service.GetDefaultTrainingProviderType();

			// Assert
			result.IsActive.Should().Be(true);
			result.Caption.Should().Be(trainingProviderType.Caption);
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void CanRequestChangeToTrainingProviderWhenNoClaimsOrClaimReturned()
		{
			// Arrange
			var newTrainingProgram = new TrainingProgram()
			{
				Id = 1,
				CourseTitle = "A test program",
				GrantApplication = new GrantApplication()
			};

			// Act & Assert
			newTrainingProgram.GrantApplication.HasSubmittedAClaim().Should().BeFalse();
		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void CannotRequestChangeToTrainingProviderWhenClaimsSubmitted()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.NewClaim);
			var newClaim = new Claim()
			{
				Id = 1,
				ClaimState = ClaimState.Unassessed
			};
			grantApplication.Claims.Add(newClaim);

			// Act & Assert
			grantApplication.HasSubmittedAClaim().Should().BeTrue();
		}



		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void GetTrainingProgram_When_InvitationKey_Does_Not_Exist_Return_Null_TrainingProgram()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(EligibleCostService), applicationAdministrator);
			var service = helper.Create<GrantApplicationService>();

			var invitationKey = Guid.Empty;

			//Act
			var result = service.Get(invitationKey);

			//Assert
			result.Should().BeNull();

		}

		[TestMethod, TestCategory("Training"), TestCategory("Service")]
		public void GetTrainingProgram_When_GetParticipantForms_Fails_And_Throws_Exception_GetTrainingProgram_Throws_Exception()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(EligibleCostService), applicationAdministrator);
			var service = helper.Create<GrantApplicationService>();
			var guid = Guid.NewGuid();

			// Act
			Action action = () => service.Get(guid);

			// Assert
			action.Should().Throw<Exception>();
		}
	}
}
