using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Repository;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Infrastructure.Repositories;
using CJG.Testing.Core;
using CJG.Web.External;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.IntegrationTests.Services
{
    [Ignore]
    [TestClass]
    public class GrantApplicationServiceTests : TransactionalTestBase
    {
        #region Variables
        private static int _applicationAdministratorId;
        private static int _assessorId;
        private static int _financialClerkId;
        private static int _directorId;
        private static int _grantOpeningId;
        #endregion

        #region Constructors
        public GrantApplicationServiceTests() : base(true)
        {

        }
        #endregion

        #region Setup
        private ServiceHelper CreateHelper(User applicationAdministrator)
        {
            var helper = new ServiceHelper(typeof(GrantApplicationService), applicationAdministrator);
            return CreateHelperBase(helper);
        }
        private ServiceHelper CreateHelper(InternalUser user, Roles role)
        {
            var helper = new ServiceHelper(typeof(GrantApplicationService), user, role);
            return CreateHelperBase(helper);
        }

        private ServiceHelper CreateHelperBase(ServiceHelper helper)
        {
            helper.SetMockAs<UnitOfWork, IUnitOfWork>(this.Context);
            helper.SetMockAs<Repository<GrantApplication>, IRepository<GrantApplication>>(this.Context);
            helper.SetMockAs<Repository<GrantOpening>, IRepository<GrantOpening>>(this.Context);
            helper.SetMockAs<Repository<User>, IRepository<User>>(this.Context);
            helper.SetMockAs<Repository<InternalUser>, IRepository<InternalUser>>(this.Context);
            helper.SetMockAs<Repository<TrainingProviderInventory>, IRepository<TrainingProviderInventory>>(this.Context);
            helper.SetMockAs<Repository<DocumentTemplate>, IRepository<DocumentTemplate>>(this.Context);
            helper.SetMockAs<Repository<RateFormat>, IRepository<RateFormat>>(this.Context);
            helper.SetMockAs<Repository<NotificationType>, IRepository<NotificationType>>(this.Context);
            helper.SetMockAs<Repository<NotificationScheduleQueue>, IRepository<NotificationScheduleQueue>>(this.Context);

            var userStoreMock = helper.SetMockAs<ApplicationUserStore, IUserStore<ApplicationUser>>(this.Context);
            var applicationUserManager = new ApplicationUserManager(userStoreMock.Object);
            helper.SetMockAs<UserManagerAdapter, IUserManagerAdapter>(applicationUserManager);
            helper.SetMockAs<NotificationQueueService, INotificationQueueService>();
            helper.SetMockAs<UserService, IUserService>();
            helper.SetMockAs<NoteService, INoteService>();
            helper.SetMockAs<StaticDataService, IStaticDataService>();
            helper.SetMockAs<GrantOpeningService, IGrantOpeningService>();
            helper.SetMockAs<EligibleCostService, IEligibleCostService>();
            helper.SetMockAs<NotificationService, INotificationService>();

            helper.GetMock<IUnitOfWork>().Setup(m => m.GrantApplications).Returns(helper.GetMock<IRepository<GrantApplication>>().Object);
            helper.GetMock<IUnitOfWork>().Setup(m => m.GrantOpenings).Returns(helper.GetMock<IRepository<GrantOpening>>().Object);
            helper.GetMock<IUnitOfWork>().Setup(m => m.Users).Returns(helper.GetMock<IRepository<User>>().Object);
            helper.GetMock<IUnitOfWork>().Setup(m => m.InternalUsers).Returns(helper.GetMock<IRepository<InternalUser>>().Object);
            helper.GetMock<IUnitOfWork>().Setup(m => m.TrainingProviderInventory).Returns(helper.GetMock<IRepository<TrainingProviderInventory>>().Object);
            helper.GetMock<IUnitOfWork>().Setup(m => m.Notes).Returns(helper.GetMock<IRepository<Note>>().Object);
            helper.GetMock<IUnitOfWork>().Setup(m => m.DocumentTemplates).Returns(helper.GetMock<IRepository<DocumentTemplate>>().Object);
            helper.GetMock<IUnitOfWork>().Setup(m => m.RateFormats).Returns(helper.GetMock<IRepository<RateFormat>>().Object);
            helper.GetMock<IUnitOfWork>().Setup(m => m.NotificationTypes).Returns(helper.GetMock<IRepository<NotificationType>>().Object);
            helper.GetMock<IUnitOfWork>().Setup(m => m.NotificationScheduleQueue).Returns(helper.GetMock<IRepository<NotificationScheduleQueue>>().Object);

            return helper;
        }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            var dbContext = TransactionalTestBase.CreateContext();

            _applicationAdministratorId = dbContext.AddExternalUser().Id;

            var grantOpening = dbContext.AddGrantOpening(new DateTime(2017, 04, 01), new DateTime(2017, 01, 01));
            grantOpening.Schedule(dbContext);
            grantOpening.Publish(dbContext);
            _grantOpeningId = grantOpening.Id;

            var userManager = new ApplicationUserManager(new ApplicationUserStore(dbContext));

            var assessor = dbContext.CreateInternalUser();
            userManager.Create(assessor);
            userManager.AddToRole(assessor.Id, Roles.Assessor.GetDescription());
            _assessorId = assessor.InternalUser.Id;

            var financialClerk = dbContext.CreateInternalUser();
            userManager.Create(financialClerk);
            userManager.AddToRole(financialClerk.Id, Roles.FinancialClerk.GetDescription());
            _financialClerkId = financialClerk.InternalUser.Id;

            var director = dbContext.CreateInternalUser();
            userManager.Create(director);
            userManager.AddToRole(director.Id, Roles.Director.GetDescription());
            _directorId = director.InternalUser.Id;

            dbContext.Dispose();
        }
        #endregion

        #region Tests
        [TestMethod, TestCategory("GrantApplication"), TestCategory("Service"), TestCategory("Integration")]
        public void GrantApplication_Add()
        {
            // Arrange
            AppDateTime.SetNow(2017, 01, 01);
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var grantOpening = this.Context.GrantOpenings.Find(_grantOpeningId);

            var helper = new ServiceHelper(typeof(GrantApplicationService), applicationAdministrator);
            helper.SetMockAs<UnitOfWork, IUnitOfWork>(this.Context);
            helper.SetMockAs<Repository<GrantApplication>, IRepository<GrantApplication>>(this.Context);
            helper.GetMock<IUnitOfWork>().Setup(m => m.GrantApplications.Add(It.IsAny<GrantApplication>())).Callback<GrantApplication>(ga => helper.GetMock<IRepository<GrantApplication>>().Object.Add(ga));
            var service = helper.Create<GrantApplicationService>();

            var grantApplication = this.Context.CreateGrantApplication(grantOpening, applicationAdministrator);

            // Act
            service.Add(grantApplication);

            // Assert
            grantApplication.Id.Should().NotBe(0);
            grantApplication.RowVersion.Should().NotBeNull();
            grantApplication.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.Draft);
            grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
            helper.GetMock<IUnitOfWork>().Verify(m => m.CommitTransaction(), Times.Exactly(1));
            helper.GetMock<IUnitOfWork>().Verify(m => m.GrantApplications.Add(It.IsAny<GrantApplication>()), Times.Exactly(1));
        }

        [TestMethod, TestCategory("GrantApplication"), TestCategory("Service"), TestCategory("Integration")]
        public void GrantApplication_Get()
        {
            // Arrange
            AppDateTime.SetNow(2017, 01, 01);
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var grantOpening = this.Context.GrantOpenings.Find(_grantOpeningId);

            var helper = CreateHelper(applicationAdministrator);
            var service = helper.Create<GrantApplicationService>();

            // Act
            var grantApplication = this.Context.CreateGrantApplication(grantOpening, applicationAdministrator);
            service.Add(grantApplication);
            grantApplication = service.Get(grantApplication.Id);

            // Assert
            grantApplication.Id.Should().NotBe(0);
            grantApplication.RowVersion.Should().NotBeNull();
            grantApplication.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.Draft);
            grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
        }

        [TestMethod, TestCategory("GrantApplication"), TestCategory("Service"), TestCategory("Integration")]
        public void GrantApplication_AddTrainingProvider()
        {
            // Arrange
            AppDateTime.SetNow(2017, 01, 01);
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var grantOpening = this.Context.GrantOpenings.Find(_grantOpeningId);

            var helper = CreateHelper(applicationAdministrator);
            var service = helper.Create<GrantApplicationService>();

            // Act
            var grantApplication = this.Context.CreateGrantApplication(grantOpening, applicationAdministrator);
            service.Add(grantApplication);
            var original_rowversion = grantApplication.RowVersion;
            this.Context.CreateTrainingProvider(grantApplication);
            service.Update(grantApplication);

            // Assert
            grantApplication.Id.Should().NotBe(0);
            grantApplication.RowVersion.Should().NotBeNull();
            grantApplication.RowVersion.Should().NotEqual(original_rowversion);
            grantApplication.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.DateUpdated.Should().Be(AppDateTime.UtcNow);
            grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.Draft);
            grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
            grantApplication.TrainingProviders.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.TrainingProviderState.Should().Be(TrainingProviderStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.DateAdded.Should().Be(AppDateTime.UtcNow);
            helper.GetMock<IUnitOfWork>().Verify(m => m.CommitTransaction(), Times.Exactly(2));
        }

        [TestMethod, TestCategory("GrantApplication"), TestCategory("Service"), TestCategory("Integration")]
        public void GrantApplication_AddTrainingProgram()
        {
            // Arrange
            AppDateTime.SetNow(2017, 01, 01);
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var grantOpening = this.Context.GrantOpenings.Find(_grantOpeningId);

            var helper = CreateHelper(applicationAdministrator);
            var service = helper.Create<GrantApplicationService>();

            // Act
            var grantApplication = this.Context.CreateGrantApplication(grantOpening, applicationAdministrator);
            service.Add(grantApplication);
            var original_rowversion = grantApplication.RowVersion;
            var trainingProvider = this.Context.CreateTrainingProvider(grantApplication);
            this.Context.CreateTrainingProgram(grantApplication, trainingProvider);
            service.Update(grantApplication);

            // Assert
            grantApplication.Id.Should().NotBe(0);
            grantApplication.RowVersion.Should().NotBeNull();
            grantApplication.RowVersion.Should().NotEqual(original_rowversion);
            grantApplication.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.DateUpdated.Should().Be(AppDateTime.UtcNow);
            grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.Draft);
            grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
            grantApplication.TrainingProviders.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.TrainingProviderState.Should().Be(TrainingProviderStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingPrograms.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault().Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().TrainingProgramState.Should().Be(TrainingProgramStates.Complete);
            grantApplication.TrainingCost.TrainingCostState.Should().Be((int)TrainingCostStates.Incomplete);
            grantApplication.TrainingPrograms.FirstOrDefault().Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault().RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.EligibleCosts.Count().Should().Be(0);
            helper.GetMock<IUnitOfWork>().Verify(m => m.CommitTransaction(), Times.Exactly(2));
        }

        [TestMethod, TestCategory("GrantApplication"), TestCategory("Service"), TestCategory("Integration")]
        public void GrantApplication_AddTrainingCosts()
        {
            // Arrange
            AppDateTime.SetNow(2017, 01, 01);
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var grantOpening = this.Context.GrantOpenings.Find(_grantOpeningId);

            var helper = CreateHelper(applicationAdministrator);
            var service = helper.Create<GrantApplicationService>();

            // Act
            var grantApplication = this.Context.CreateGrantApplication(grantOpening, applicationAdministrator);
            service.Add(grantApplication);
            var original_rowversion = grantApplication.RowVersion;
            var trainingProvider = this.Context.CreateTrainingProvider(grantApplication);
            this.Context.CreateTrainingProgramWithCosts(grantApplication, trainingProvider);
            service.Update(grantApplication);

            // Assert
            grantApplication.Id.Should().NotBe(0);
            grantApplication.RowVersion.Should().NotBeNull();
            grantApplication.RowVersion.Should().NotEqual(original_rowversion);
            grantApplication.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.DateUpdated.Should().Be(AppDateTime.UtcNow);
            grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.Draft);
            grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Complete);
            grantApplication.TrainingProviders.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.TrainingProviderState.Should().Be(TrainingProviderStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingPrograms.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault().Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().TrainingProgramState.Should().Be(TrainingProgramStates.Complete);
            grantApplication.TrainingCost.TrainingCostState.Should().Be((int)TrainingCostStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault().Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault().RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.EligibleCosts.Count().Should().Be(1);
            grantApplication.TrainingCost.EligibleCosts.First().Id.Should().NotBe(0);
            grantApplication.TrainingCost.EligibleCosts.First().RowVersion.Should().NotBeNull();
            grantApplication.TrainingCost.EligibleCosts.First().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.TotalEstimatedCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedCost));
            grantApplication.TrainingCost.TotalEstimatedReimbursement.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedReimbursement));
            helper.GetMock<IUnitOfWork>().Verify(m => m.CommitTransaction(), Times.Exactly(2));
        }

        [TestMethod, TestCategory("GrantApplication"), TestCategory("Service"), TestCategory("Integration")]
        public void GrantApplication_Submit()
        {
            // Arrange
            AppDateTime.SetNow(2017, 01, 01);
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var grantOpening = this.Context.GrantOpenings.Find(_grantOpeningId);

            var helper = CreateHelper(applicationAdministrator);
            var service = helper.Create<GrantApplicationService>();

            // Act
            var grantApplication = this.Context.CreateGrantApplication(grantOpening, applicationAdministrator);
            service.Add(grantApplication);
            var trainingProvider = this.Context.CreateTrainingProvider(grantApplication);
            this.Context.CreateTrainingProgramWithCosts(grantApplication, trainingProvider);
            service.Update(grantApplication);
            service.Submit(grantApplication);

            // Assert
            grantApplication.Id.Should().NotBe(0);
            grantApplication.RowVersion.Should().NotBeNull();
            grantApplication.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.DateUpdated.Should().Be(AppDateTime.UtcNow);
            grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.New);
            grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
            grantApplication.TrainingProviders.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.TrainingProviderState.Should().Be(TrainingProviderStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingPrograms.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault().Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().TrainingProgramState.Should().Be(TrainingProgramStates.Complete);
            grantApplication.TrainingCost.TrainingCostState.Should().Be((int)TrainingCostStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault().Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault().RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.EligibleCosts.Count().Should().Be(1);
            grantApplication.TrainingCost.EligibleCosts.First().Id.Should().NotBe(0);
            grantApplication.TrainingCost.EligibleCosts.First().RowVersion.Should().NotBeNull();
            grantApplication.TrainingCost.EligibleCosts.First().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.TotalEstimatedCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedCost));
            grantApplication.TrainingCost.TotalEstimatedReimbursement.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedReimbursement));
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxParticipants.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedParticipants);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedCost);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxParticipantCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedParticipantCost);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxReimbursement.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedReimbursement);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedEmployerContribution.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedEmployerContribution);
            grantApplication.TrainingCost.TotalAgreedMaxCost.Should().Be(grantApplication.TrainingCost.TotalEstimatedCost);
            grantApplication.DateSubmitted.Should().Be(AppDateTime.UtcNow);
            helper.GetMock<IUnitOfWork>().Verify(m => m.CommitTransaction(), Times.Exactly(3));
        }

        [TestMethod, TestCategory("GrantApplication"), TestCategory("Service"), TestCategory("Integration")]
        public void GrantApplication_SelectForAssessment()
        {
            // Arrange
            AppDateTime.SetNow(2017, 01, 01);
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var assessor = this.Context.InternalUsers.Find(_assessorId);
            var grantOpening = this.Context.GrantOpenings.Find(_grantOpeningId);

            var helper = CreateHelper(applicationAdministrator);
            var service = helper.Create<GrantApplicationService>();

            // Act
            var grantApplication = this.Context.CreateGrantApplication(grantOpening, applicationAdministrator);
            service.Add(grantApplication);
            var trainingProvider = this.Context.CreateTrainingProvider(grantApplication);
            this.Context.CreateTrainingProgramWithCosts(grantApplication, trainingProvider);
            service.Update(grantApplication);
            service.Submit(grantApplication);

            service = helper.CreateFor<GrantApplicationService>(assessor, ServiceHelper.Roles.Assessor);
            service.SelectForAssessment(grantApplication);

            // Assert
            grantApplication.Id.Should().NotBe(0);
            grantApplication.RowVersion.Should().NotBeNull();
            grantApplication.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.DateUpdated.Should().Be(AppDateTime.UtcNow);
            grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.PendingAssessment);
            grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
            grantApplication.TrainingProviders.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.TrainingProviderState.Should().Be(TrainingProviderStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingPrograms.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault().Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().TrainingProgramState.Should().Be(TrainingProgramStates.Complete);
            grantApplication.TrainingCost.TrainingCostState.Should().Be((int)TrainingCostStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault().Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault().RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.EligibleCosts.Count().Should().Be(1);
            grantApplication.TrainingCost.EligibleCosts.First().Id.Should().NotBe(0);
            grantApplication.TrainingCost.EligibleCosts.First().RowVersion.Should().NotBeNull();
            grantApplication.TrainingCost.EligibleCosts.First().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.TotalEstimatedCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedCost));
            grantApplication.TrainingCost.TotalEstimatedReimbursement.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedReimbursement));
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxParticipants.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedParticipants);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedCost);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxParticipantCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedParticipantCost);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxReimbursement.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedReimbursement);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedEmployerContribution.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedEmployerContribution);
            grantApplication.TrainingCost.TotalAgreedMaxCost.Should().Be(grantApplication.TrainingCost.TotalEstimatedCost);
            grantApplication.DateSubmitted.Should().Be(AppDateTime.UtcNow);
            helper.GetMock<IUnitOfWork>().Verify(m => m.CommitTransaction(), Times.Exactly(4));
        }

        [TestMethod, TestCategory("GrantApplication"), TestCategory("Service"), TestCategory("Integration")]
        public void GrantApplication_BeginAssessment()
        {
            // Arrange
            AppDateTime.SetNow(2017, 01, 01);
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var assessor = this.Context.InternalUsers.Find(_assessorId);
            var grantOpening = this.Context.GrantOpenings.Find(_grantOpeningId);

            var helper = CreateHelper(applicationAdministrator);
            var service = helper.Create<GrantApplicationService>();

            // Act
            var grantApplication = this.Context.CreateGrantApplication(grantOpening, applicationAdministrator);
            service.Add(grantApplication);
            var trainingProvider = this.Context.CreateTrainingProvider(grantApplication);
            this.Context.CreateTrainingProgramWithCosts(grantApplication, trainingProvider);
            service.Update(grantApplication);
            service.Submit(grantApplication);

            service = helper.CreateFor<GrantApplicationService>(assessor, ServiceHelper.Roles.Assessor);
            service.SelectForAssessment(grantApplication);
            service.BeginAssessment(grantApplication, assessor.Id);

            // Assert
            grantApplication.Id.Should().NotBe(0);
            grantApplication.RowVersion.Should().NotBeNull();
            grantApplication.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.DateUpdated.Should().Be(AppDateTime.UtcNow);
            grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.UnderAssessment);
            grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
            grantApplication.TrainingProviders.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.TrainingProviderState.Should().Be(TrainingProviderStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingPrograms.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault().Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().TrainingProgramState.Should().Be(TrainingProgramStates.Complete);
            grantApplication.TrainingCost.TrainingCostState.Should().Be((int)TrainingCostStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault().Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault().RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.EligibleCosts.Count().Should().Be(1);
            grantApplication.TrainingCost.EligibleCosts.First().Id.Should().NotBe(0);
            grantApplication.TrainingCost.EligibleCosts.First().RowVersion.Should().NotBeNull();
            grantApplication.TrainingCost.EligibleCosts.First().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.TotalEstimatedCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedCost));
            grantApplication.TrainingCost.TotalEstimatedReimbursement.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedReimbursement));
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxParticipants.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedParticipants);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedCost);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxParticipantCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedParticipantCost);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxReimbursement.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedReimbursement);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedEmployerContribution.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedEmployerContribution);
            grantApplication.TrainingCost.TotalAgreedMaxCost.Should().Be(grantApplication.TrainingCost.TotalEstimatedCost);
            grantApplication.DateSubmitted.Should().Be(AppDateTime.UtcNow);
            helper.GetMock<IUnitOfWork>().Verify(m => m.CommitTransaction(), Times.Exactly(5));
        }

        [TestMethod, TestCategory("GrantApplication"), TestCategory("Service"), TestCategory("Integration")]
        public void GrantApplication_RecommendForApproval()
        {
            // Arrange
            AppDateTime.SetNow(2017, 01, 01);
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var assessor = this.Context.InternalUsers.Find(_assessorId);
            var grantOpening = this.Context.GrantOpenings.Find(_grantOpeningId);
            var trainingProviderInventory = this.Context.TrainingProviderInventory.First();

            var helper = CreateHelper(applicationAdministrator);
            var service = helper.Create<GrantApplicationService>();
            var trainingProviderServie = helper.Create<TrainingProviderService>();

            // Act
            var grantApplication = this.Context.CreateGrantApplication(grantOpening, applicationAdministrator);
            service.Add(grantApplication);
            var trainingProvider = this.Context.CreateTrainingProvider(grantApplication);
            this.Context.CreateTrainingProgramWithCosts(grantApplication, trainingProvider);
            service.Update(grantApplication);
            service.Submit(grantApplication);

            service = helper.CreateFor<GrantApplicationService>(assessor, ServiceHelper.Roles.Assessor);
            service.SelectForAssessment(grantApplication);
            service.BeginAssessment(grantApplication, assessor.Id);
            trainingProviderServie.ValidateTrainingProvider(grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider, trainingProviderInventory.Id);
            service.RecommendForApproval(grantApplication);

            // Assert
            grantApplication.Id.Should().NotBe(0);
            grantApplication.RowVersion.Should().NotBeNull();
            grantApplication.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.DateUpdated.Should().Be(AppDateTime.UtcNow);
            grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.RecommendedForApproval);
            grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
            grantApplication.TrainingProviders.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.TrainingProviderState.Should().Be(TrainingProviderStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingPrograms.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault().Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().TrainingProgramState.Should().Be(TrainingProgramStates.Complete);
            grantApplication.TrainingCost.TrainingCostState.Should().Be((int)TrainingCostStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault().Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault().RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.EligibleCosts.Count().Should().Be(1);
            grantApplication.TrainingCost.EligibleCosts.First().Id.Should().NotBe(0);
            grantApplication.TrainingCost.EligibleCosts.First().RowVersion.Should().NotBeNull();
            grantApplication.TrainingCost.EligibleCosts.First().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.TotalEstimatedCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedCost));
            grantApplication.TrainingCost.TotalEstimatedReimbursement.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedReimbursement));
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxParticipants.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedParticipants);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedCost);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxParticipantCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedParticipantCost);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxReimbursement.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedReimbursement);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedEmployerContribution.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedEmployerContribution);
            grantApplication.TrainingCost.TotalAgreedMaxCost.Should().Be(grantApplication.TrainingCost.TotalEstimatedCost);
            grantApplication.DateSubmitted.Should().Be(AppDateTime.UtcNow);
            helper.GetMock<IUnitOfWork>().Verify(m => m.CommitTransaction(), Times.Exactly(7));
        }

        [TestMethod, TestCategory("GrantApplication"), TestCategory("Service"), TestCategory("Integration")]
        public void GrantApplication_IssueOffer()
        {
            // Arrange
            AppDateTime.SetNow(2017, 01, 01);
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var assessor = this.Context.InternalUsers.Find(_assessorId);
            var director = this.Context.InternalUsers.Find(_directorId);
            var grantOpening = this.Context.GrantOpenings.Find(_grantOpeningId);
            var trainingProviderInventory = this.Context.TrainingProviderInventory.First();

            var helper = CreateHelper(applicationAdministrator);
            var service = helper.Create<GrantApplicationService>();
            var trainingProviderServie = helper.Create<TrainingProviderService>();

            // Act
            var grantApplication = this.Context.CreateGrantApplication(grantOpening, applicationAdministrator);
            service.Add(grantApplication);
            var trainingProvider = this.Context.CreateTrainingProvider(grantApplication);
            this.Context.CreateTrainingProgramWithCosts(grantApplication, trainingProvider);
            service.Update(grantApplication);
            service.Submit(grantApplication);

            service = helper.CreateFor<GrantApplicationService>(assessor, ServiceHelper.Roles.Assessor);
            service.SelectForAssessment(grantApplication);
            service.BeginAssessment(grantApplication, assessor.Id);
            trainingProviderServie.ValidateTrainingProvider(grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider, trainingProviderInventory.Id);
            service.RecommendForApproval(grantApplication);

            service = helper.CreateFor<GrantApplicationService>(director, ServiceHelper.Roles.Director);
            service.IssueOffer(grantApplication);

            // Assert
            grantApplication.Id.Should().NotBe(0);
            grantApplication.RowVersion.Should().NotBeNull();
            grantApplication.DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.DateUpdated.Should().Be(AppDateTime.UtcNow);
            grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.OfferIssued);
            grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.AcceptGrantAgreement);
            grantApplication.TrainingProviders.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault().Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().TrainingProvider.TrainingProviderState.Should().Be(TrainingProviderStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault().Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault().RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingPrograms.Count().Should().Be(1);
            grantApplication.TrainingPrograms.FirstOrDefault().Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().TrainingProgramState.Should().Be(TrainingProgramStates.Complete);
            grantApplication.TrainingCost.TrainingCostState.Should().Be((int)TrainingCostStates.Complete);
            grantApplication.TrainingPrograms.FirstOrDefault().Id.Should().NotBe(0);
            grantApplication.TrainingPrograms.FirstOrDefault().RowVersion.Should().NotBeNull();
            grantApplication.TrainingPrograms.FirstOrDefault().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.EligibleCosts.Count().Should().Be(1);
            grantApplication.TrainingCost.EligibleCosts.First().Id.Should().NotBe(0);
            grantApplication.TrainingCost.EligibleCosts.First().RowVersion.Should().NotBeNull();
            grantApplication.TrainingCost.EligibleCosts.First().DateAdded.Should().Be(AppDateTime.UtcNow);
            grantApplication.TrainingCost.TotalEstimatedCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedCost));
            grantApplication.TrainingCost.TotalEstimatedReimbursement.Should().Be(grantApplication.TrainingCost.EligibleCosts.Sum(ec => ec.EstimatedReimbursement));
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxParticipants.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedParticipants);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedCost);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxParticipantCost.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedParticipantCost);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedMaxReimbursement.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedReimbursement);
            grantApplication.TrainingCost.EligibleCosts.First().AgreedEmployerContribution.Should().Be(grantApplication.TrainingCost.EligibleCosts.First().EstimatedEmployerContribution);
            grantApplication.TrainingCost.TotalAgreedMaxCost.Should().Be(grantApplication.TrainingCost.TotalEstimatedCost);
            grantApplication.DateSubmitted.Should().Be(AppDateTime.UtcNow);
            helper.GetMock<IUnitOfWork>().Verify(m => m.CommitTransaction(), Times.Exactly(8));
        }
        #endregion
    }
}
