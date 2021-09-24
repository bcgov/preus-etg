using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;
using CJG.Core.Interfaces.Service;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class GrantProgramValidationTests
    {
        ServiceHelper helper { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            helper = new ServiceHelper(typeof(GrantProgramService), identity);
            helper.MockContext();
            helper.MockDbSet<GrantProgram>();
        }

        [Ignore, TestMethod, TestCategory("Grant Program"), TestCategory("Validate")]
        public void Validate_When_GrantProgram_Is_Required_Properties_Error()
        {
			// Arrange
            var grantProgram = new GrantProgram();
            helper.MockDbSet<GrantProgram>(new[] { grantProgram });
            var service = helper.Create<GrantProgramService>();

            // Act
            var validationResults = service.Validate(grantProgram).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The grant agreement must be associated with a grant application."));
        }
    }
}