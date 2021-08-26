using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class EligibleExpenseBreakdownValidationTests
    {
        [TestMethod, TestCategory("EligibleExpenseBreakdown"), TestCategory("Validate")]
        public void Validate_When_EligibleExpenseBreakdown_Duplicate_Caption_Within_Expensetype()
        {
            var user = EntityHelper.CreateExternalUser();

            var helper = new ServiceHelper(typeof(EligibleExpenseBreakdownService), user);

            helper.MockContext();

            var eligibleExpenseTypes = new EligibleExpenseType() { Id = 1, Caption = "Eligible Expense Type", Breakdowns = { new EligibleExpenseBreakdown() { Id = 1, Description = "Expense Breakdown 1", Caption = "Expense Breakdown" } } };

            var expenseBreakDowns = new EligibleExpenseBreakdown() { Id = 3, EligibleExpenseTypeId = 1, Description = "Expense Breakdown 2", Caption = "Expense Breakdown", ExpenseType = eligibleExpenseTypes };

            helper.MockDbSet<EligibleExpenseBreakdown>(new[] { expenseBreakDowns });
            helper.MockDbSet<EligibleExpenseType>(new[] { eligibleExpenseTypes });

            var service = helper.Create<EligibleExpenseBreakdownService>();

            // Act
            var validationResults = service.Validate(expenseBreakDowns).ToArray();

            string validateMsg = "The caption must be unique within an expense type.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }
    }
}