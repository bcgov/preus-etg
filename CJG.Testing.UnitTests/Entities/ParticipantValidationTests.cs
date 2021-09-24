using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class ParticipantValidationTests
    {
        [TestMethod, TestCategory("Participant"), TestCategory("Validate")]
        public void Validate_When_Participant_IsRequired_Properties()
        {
            var user = EntityHelper.CreateExternalUser();

            var helper = new ServiceHelper(typeof(ParticipantService), user);

            helper.MockContext();

            var participant = new Participant();

            helper.MockDbSet<Participant>(new[] { participant });

            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participant).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The FirstName field is required."));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The LastName field is required."));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The Email field is required."));
        }

        [TestMethod, TestCategory("Participant"), TestCategory("Validate")]
        public void Validate_When_Participant_Age_OlderThan_150()
        {
            var user = EntityHelper.CreateExternalUser();

            var helper = new ServiceHelper(typeof(ParticipantService), user);

            helper.MockContext();

            var oldest = AppDateTime.UtcNow.AddYears(-1 * Participant.OldestAge).Date;
            var youngest = AppDateTime.UtcNow.AddYears(-1 * Participant.YoungestAge).Date;

            var participant = new Participant()
            {
                FirstName = "Randy",
                LastName = "Rivers",
                Email = "rr@x.com",
                BirthDate = oldest.AddYears(-2)
            };

            helper.MockDbSet<Participant>(new[] { participant });

            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participant).ToArray();

            string validateMsg = $"The birthdate must be between '{oldest:yyyy}' and '{youngest:yyyy}'.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("Participant"), TestCategory("Validate")]
        public void Validate_When_Participant_Age_YoungerThan_16()
        {
            var user = EntityHelper.CreateExternalUser();

            var helper = new ServiceHelper(typeof(ParticipantService), user);

            helper.MockContext();

            var oldest = AppDateTime.UtcNow.AddYears(-1 * Participant.OldestAge).Date;
            var youngest = AppDateTime.UtcNow.AddYears(-1 * Participant.YoungestAge).Date;

            var participant = new Participant()
            {
                FirstName = "Randy",
                LastName = "Rivers",
                Email = "rr@x.com",
                BirthDate = youngest.AddYears(2)
            };

            helper.MockDbSet<Participant>(new[] { participant });

            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participant).ToArray();

            string validateMsg = $"The birthdate must be between '{oldest:yyyy}' and '{youngest:yyyy}'.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }
    }
}