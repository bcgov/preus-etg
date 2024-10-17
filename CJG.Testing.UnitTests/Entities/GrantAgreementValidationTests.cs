using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class GrantAgreementValidationTests
    {
        ServiceHelper helper { get; set; }

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            helper = new ServiceHelper(typeof(GrantAgreementService), user);
            helper.MockContext();
            helper.MockDbSet<GrantAgreement>();
        }

        [TestMethod, TestCategory("Grant Agreement"), TestCategory("Validate")]
        public void Validate_When_GrantAgreement_Not_Associated_With_Grant_Application()
        {
            var grantAgreement = new GrantAgreement()
            {
                GrantApplicationId = 0
            };

            helper.MockDbSet<GrantAgreement>(new[] { grantAgreement });

            var service = helper.Create<GrantAgreementService>();

            // Act
            var validationResults = service.Validate(grantAgreement).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The grant agreement must be associated with a grant application."));
        }

        [TestMethod, TestCategory("Grant Agreement"), TestCategory("Validate")]
        public void Validate_When_GrantAgreement_Missing_CoverLetter_Schedule_A_B_Confirmed()
        {
            var grantAgreement = new GrantAgreement()
            {
                GrantApplicationId = 0,
                DateAccepted = AppDateTime.UtcNow.AddDays(-5),
                CoverLetterConfirmed = false,
                ScheduleAConfirmed = false,
                ScheduleBConfirmed = false
            };

            helper.MockDbSet<GrantAgreement>(new[] { grantAgreement });

            var service = helper.Create<GrantAgreementService>();

            // Act
            var validationResults = service.Validate(grantAgreement).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "A grant agreement cannot be accepted before the cover letter, schedule A and schedule B are confirmed."));
        }
    }
}