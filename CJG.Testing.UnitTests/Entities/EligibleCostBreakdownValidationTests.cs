using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;
using System.Collections.Generic;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class EligibleCostBreakdownValidationTests
    {
        ServiceHelper helper { get; set; }

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            helper = new ServiceHelper(typeof(EligibleCostBreakdownService), user);
            helper.MockContext();
            helper.MockDbSet<EligibleCostBreakdown>();
        }

        [TestMethod, TestCategory("EligibleCostBreakdown"), TestCategory("Validate")]
        public void Validate_When_EligibleCostBreakdown_EstimatedCost_Exceeds_EligibleCost_EstimatedCost()
        {
            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                EstimatedCost = 100
            };

            var eligibleCostBreakdown = new EligibleCostBreakdown()
            {
                EstimatedCost = 1000,
                Id = 1,
                EligibleCostId = 1,
                EligibleCost = eligibleCost
            };

            helper.MockDbSet<EligibleCostBreakdown>(new[] { eligibleCostBreakdown });
            helper.MockDbSet<EligibleCost>(new[] { eligibleCost });

            var service = helper.Create<EligibleCostBreakdownService>();

            // Act
            var validationResults = service.Validate(eligibleCostBreakdown).ToArray();

            string validateMsg = "The estimated cost cannot exceed " + eligibleCostBreakdown.EligibleCost.EstimatedCost.ToString("c2") + ".";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("EligibleCostBreakdown"), TestCategory("Validate")]
        public void Validate_When_EligibleCostBreakdown_AssessedCost_Exceeds_EligibleCost_AgreedMaxCost()
        {

            var eligibleCost = new EligibleCost()
            {
                Id = 1,
                AgreedMaxCost = 100,
            };

            var eligibleCostBreakdown = new EligibleCostBreakdown()
            {
                AssessedCost = 1000,
                Id = 1,
                EligibleCostId = 1,
                EligibleCost = eligibleCost
            };

            helper.MockDbSet<EligibleCostBreakdown>(new[] { eligibleCostBreakdown });
            helper.MockDbSet<EligibleCost>(new[] { eligibleCost });

            var service = helper.Create<EligibleCostBreakdownService>();

            // Act
            var validationResults = service.Validate(eligibleCostBreakdown).ToArray();

            string validateMsg = "The agreed maximum cost cannot exceed " + eligibleCostBreakdown.EligibleCost.AgreedMaxCost.ToString("c2") + ".";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }
    }
}