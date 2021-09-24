using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class ProgramDescriptionValidationTests
    {
		private ServiceHelper helper;
		private User user;

        [TestInitialize]
        public void Setup()
        {
            user = EntityHelper.CreateExternalUser();
            helper = new ServiceHelper(typeof(ProgramDescriptionService), user);
            helper.MockContext();
            helper.MockDbSet<GrantApplication>();
            helper.MockDbSet<ProgramDescription>();
        }

        [TestMethod, TestCategory("Program Description"), TestCategory("Validate")]
        public void Validate_When_Program_Description_Is_Required_Properties_Error()
        {
			// Arrange
			ProgramDescription programDescription = new ProgramDescription();

            var service = helper.Create<ProgramDescriptionService>();

            // Act
            var validationResults = service.Validate(programDescription).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "Program Description is required."));
        }

        [Ignore, TestMethod, TestCategory("Program Description"), TestCategory("Validate")]
        public void Validate_When_Program_Description_Participant_Employment_Status_Not_Selected()
        {
			// Arrange
            var programDescription = new ProgramDescription()
            {
                DescriptionState = ProgramDescriptionStates.Complete,
                GrantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.RecommendedForApproval),
                Description = "Music Program"
            };

            programDescription.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId = ProgramTypes.WDAService;

            var service = helper.Create<ProgramDescriptionService>();

            // Act
            var validationResults = service.Validate(programDescription).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "Participant Employment Status must be selected."));
        }

        [TestMethod, TestCategory("Program Description"), TestCategory("Validate")]
        public void Validate_When_Program_Description_Target_NAICS_Not_Selected()
        {
			// Arrange
			var programDescription = new ProgramDescription()
            {
                DescriptionState = ProgramDescriptionStates.Complete,
                GrantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.RecommendedForApproval),
                Description = "Music Program"
            };

            programDescription.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId = ProgramTypes.WDAService;

            var service = helper.Create<ProgramDescriptionService>();

            // Act
            var validationResults = service.Validate(programDescription).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The target NAICS must be selected."));
        }

		[TestMethod, TestCategory("Program Description"), TestCategory("Validate")]
		public void Validate_When_Program_Description_Target_NAICS_Needs_To_Be_Updated()
		{
			// Arrange
			var programDescription = new ProgramDescription()
			{
				DescriptionState = ProgramDescriptionStates.Incomplete,
				GrantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.Draft),
				Description = "Music Program",
				TargetNAICSId = 0
			};

			programDescription.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId = ProgramTypes.EmployerGrant;

			var service = helper.Create<ProgramDescriptionService>();

			// Act
			var validationResults = service.Validate(programDescription).ToArray();

			// Assert
			Assert.AreEqual(false, validationResults.Any());
		}

		[TestMethod, TestCategory("Program Description"), TestCategory("Validate")]
        public void Validate_When_Program_Description_Target_NOC_Not_Selected()
        {
			// Arrange
			var programDescription = new ProgramDescription()
            {
                DescriptionState = ProgramDescriptionStates.Complete,
                GrantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.RecommendedForApproval),
                Description = "Music Program"
            };

            programDescription.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId = ProgramTypes.WDAService;

            var service = helper.Create<ProgramDescriptionService>();

            // validate entity -
            var validationResults = service.Validate(programDescription).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The target NOC must be selected."));
        }
    }
}