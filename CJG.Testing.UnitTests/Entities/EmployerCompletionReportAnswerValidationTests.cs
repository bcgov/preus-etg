using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class EmployerCompletionReportAnswerValidationTests
    {
        [TestMethod, TestCategory("EmployerCompletionReportAnswer"), TestCategory("Validate")]
        public void Validate_When_EmployerCompletionReportAnswer_Missing_Answer()
        {
            var user = EntityHelper.CreateExternalUser();

            var helper = new ServiceHelper(typeof(EligibleExpenseBreakdownService), user);

            helper.MockContext();

            var employerCompletionReportAnswer = new EmployerCompletionReportAnswer();

            helper.MockDbSet<EmployerCompletionReportAnswer>(new[] { employerCompletionReportAnswer });

            var service = helper.Create<EligibleExpenseBreakdownService>();

            // Act
            var validationResults = service.Validate(employerCompletionReportAnswer).ToArray();

            string validateMsg = "An answer must be entered.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }
    }
}