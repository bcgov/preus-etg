using CJG.Core.Entities;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Entity.Validation;
using System.Linq;

namespace CJG.Testing.IntegrationTests.Entities
{
    [TestClass]
    public class GrantOpeningTests : TransactionalTestBase
    {
        #region Variables
        #endregion

        #region Tests
        [TestMethod, TestCategory("Grant Opening"), TestCategory("Validation"), TestCategory("Integration")]
        public void GrantOpening_Validate_Invalid_TrainingPeriod()
        {
            // Arrange
            var grantOpening = new GrantOpening();

            try
            {
                // Act
                this.Context.GrantOpenings.Add(grantOpening);
                var result = this.Context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                // Assert
                Assert.IsTrue(e.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage == "The grant opening must be associated with a valid training period.");
            }
        }

        [TestMethod, TestCategory("Grant Opening"), TestCategory("Validation")]
        public void GrantOpening_Validate_MustBeing_Unscheduled()
        {
            // Arrange
            var grantOpening = this.Context.CreateGrantOpening(new DateTime(2017, 04, 01));
            grantOpening.State = GrantOpeningStates.Unscheduled;

            try
            {
                // Act
                AppDateTime.SetNow(new DateTime(2017, 01, 01));
                this.Context.GrantOpenings.Add(grantOpening);
                var result = this.Context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                // Assert
                Assert.IsTrue(e.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage == "A new Grant Opening must begin as unscheduled.");
            }
        }

        [TestMethod, TestCategory("Grant Opening"), TestCategory("Validation")]
        public void GrantOpening_Validate_Remain_Unscheduled()
        {
            // Arrange
            var grantOpening = this.Context.CreateGrantOpening(new DateTime(2017, 04, 01));
            grantOpening.State = GrantOpeningStates.Unscheduled;

            // Act
            AppDateTime.SetNow(new DateTime(2017, 01, 01));
            this.Context.GrantOpenings.Add(grantOpening);
            var result = this.Context.SaveChanges();

            // Assert
            Assert.AreEqual(GrantOpeningStates.Unscheduled, grantOpening.State);
        }

        [TestMethod, TestCategory("Grant Opening"), TestCategory("Validation")]
        public void GrantOpening_Validate_ChangeTo_Published()
        {
            // Arrange
            var grantOpening = this.Context.CreateGrantOpening(new DateTime(2017, 04, 01));
            grantOpening.State = GrantOpeningStates.Unscheduled;

            AppDateTime.SetNow(new DateTime(2017, 01, 01));
            this.Context.GrantOpenings.Add(grantOpening);
            var result = this.Context.SaveChanges();

            // Act
            AppDateTime.SetNow(new DateTime(2017, 01, 01));
            grantOpening.State = GrantOpeningStates.Scheduled;
            result += this.Context.SaveChanges();

            // Assert
            Assert.AreEqual(GrantOpeningStates.Published, grantOpening.State);
        }

        [TestMethod, TestCategory("Grant Opening"), TestCategory("Validation")]
        public void GrantOpening_Validate_ChangeTo_Open()
        {
            // Arrange
            var grantOpening = this.Context.CreateGrantOpening(new DateTime(2017, 04, 01));
            grantOpening.State = GrantOpeningStates.Unscheduled;

            AppDateTime.SetNow(new DateTime(2017, 01, 01));
            this.Context.GrantOpenings.Add(grantOpening);
            var result = this.Context.SaveChanges();

            // Act
            AppDateTime.SetNow(new DateTime(2017, 02, 01));
            grantOpening.State = GrantOpeningStates.Scheduled;
            result += this.Context.SaveChanges();

            // Assert
            Assert.AreEqual(GrantOpeningStates.Open, grantOpening.State);
        }

        [TestMethod, TestCategory("Grant Opening"), TestCategory("Validation")]
        public void GrantOpening_Validate_ChangeTo_Closed()
        {
            // Arrange
            var grantOpening = this.Context.CreateGrantOpening(new DateTime(2017, 04, 01));
            grantOpening.State = GrantOpeningStates.Unscheduled;

            AppDateTime.SetNow(new DateTime(2017, 01, 01));
            this.Context.GrantOpenings.Add(grantOpening);
            var result = this.Context.SaveChanges();

            // Act
            AppDateTime.SetNow(new DateTime(2017, 09, 01));
            grantOpening.State = GrantOpeningStates.Scheduled;
            result += this.Context.SaveChanges();

            // Assert
            Assert.AreEqual(GrantOpeningStates.Closed, grantOpening.State);
        }
        #endregion
    }
}
