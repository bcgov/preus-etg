using CJG.Core.Entities;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CJG.Testing.IntegrationTests.Entities
{
    [TestClass]
    public class GrantApplicationTests : TransactionalTestBase
    {
        #region Constructors
        public GrantApplicationTests() : base(true)
        {

        }
        #endregion

        #region Tests
        [TestMethod, TestCategory("GrantApplication"), TestCategory("Validation"), TestCategory("Integration")]
        public void GrantApplication_AcceptAgreement()
        {
            // Arrange
            var applicationAdministrator = this.Context.AddExternalUser();
            var assessor = this.Context.CreateInternalUser();

            var grantOpening = this.Context.AddGrantOpening(new DateTime(2017, 04, 01), new DateTime(2017, 01, 01));
            AppDateTime.SetNow(new DateTime(2017, 04, 01));
            grantOpening.Schedule(this.Context);

            var grantApplication = this.Context.AddGrantApplicationWithCosts(grantOpening, applicationAdministrator);
            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            var trainingProvider = trainingProgram.TrainingProvider;

            grantApplication.Complete(this.Context);
            grantApplication.Submit(this.Context);
            grantApplication.SelectForAssessment(this.Context);
            grantApplication.BeginAssessment(this.Context, assessor.InternalUser);
            grantApplication.RecommendForApproval(this.Context);
            grantApplication.IssueOffer(this.Context);
            grantApplication.AcceptAgreement(this.Context);

            // Act
            var result = this.Context.SaveChanges();

            // Assert
            Assert.AreEqual(GrantOpeningStates.Open, grantOpening.State);
            Assert.AreEqual(ApplicationStateInternal.AgreementAccepted, grantApplication.ApplicationStateInternal);
            Assert.AreEqual(ApplicationStateExternal.Approved, grantApplication.ApplicationStateExternal);
            Assert.AreEqual(TrainingProgramStates.Complete, trainingProgram.TrainingProgramState);
            Assert.AreEqual(TrainingCostStates.Complete, grantApplication.TrainingCost.TrainingCostState);
            Assert.AreEqual(TrainingProviderStates.Complete, trainingProvider.TrainingProviderState);
        }
        #endregion  
    }
}
