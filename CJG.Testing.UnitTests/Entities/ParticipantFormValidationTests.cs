using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Entity;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;
using CJG.Infrastructure.Entities;
using Moq;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass]
    public class ParticipantFormValidationTests
    {
	    private Mock<IDataContext> _mockContext;
	    private ServiceHelper Helper { get; set; }

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            Helper = new ServiceHelper(typeof(ParticipantService), user);
            _mockContext = Helper.MockContext();
            Helper.MockDbSet<ParticipantForm>();
        }

        [TestMethod, TestCategory("Participant Form"), TestCategory("Validate")]
        public void Validate_When_ParticipantForm_Is_Required_Properties_Error()
        {
            var invitationKey = Guid.NewGuid();
            var participantForm = new ParticipantForm()
            {
                InvitationKey = invitationKey,
                BirthDate = AppDateTime.UtcNow,
                GrantApplication = new GrantApplication()
            };

            var service = Helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participantForm).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The ProgramSponsorName field is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The ProgramDescription field is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The FirstName field is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The LastName field is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The SIN field is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The PhoneNumber1 field is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The EmailAddress field is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The AddressLine1 field is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The City field is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The PostalCode field is required.")));
        }

        [TestMethod, TestCategory("Participant Form"), TestCategory("Validate")]
        public void Validate_When_ParticipantForm_BirthDate_Age_IsNot_Between_16_And_150()
        {
            var invitationKey = Guid.NewGuid();
            var participantForm = new ParticipantForm()
            {
                InvitationKey = invitationKey,
                BirthDate = AppDateTime.UtcNow,
                GrantApplication = new GrantApplication(),
                ProgramSponsorName = "Bennet, Tony",
                ProgramDescription = "Music Program",
                FirstName = "Randy",
                LastName = "Rivers",
                SIN = "123-456-789",
                PhoneNumber1 = "(604) 555-1212",
                EmailAddress = "rr@x.com",
                AddressLine1 = "123 Main Street",
                City = "Sometown",
                PostalCode = "V1V 2V3"
            };

            var service = Helper.Create<ParticipantService>();

            _mockContext.Object.Entry(participantForm).State = EntityState.Added;

			// Act
			var validationResults = service.Validate(participantForm).ToArray();

            var oldest = AppDateTime.UtcNow.AddYears(-1 * Participant.OldestAge).Date;
            var youngest = AppDateTime.UtcNow.AddYears(-1 * Participant.YoungestAge).Date;

            string validationMsg = $"The birthdate must be between '{oldest:yyyy}' and '{youngest:yyyy}'.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
        }        
    }
}