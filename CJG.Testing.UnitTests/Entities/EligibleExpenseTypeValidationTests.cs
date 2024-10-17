using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;
using System.Collections.Generic;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class EligibleExpenseTypeValidationTests
    {
        [TestMethod, TestCategory("EligibleExpenseType"), TestCategory("Validate")]
        public void Validate_When_EligibleExpenseType_Duplicate_Caption_Within_Same_ProgramConfiguration()
        {
            var user = EntityHelper.CreateExternalUser();

            var helper = new ServiceHelper(typeof(EligibleExpenseTypeService), user);

            helper.MockContext();

            var expenseTypes = new List<EligibleExpenseType>()
            {
                new EligibleExpenseType(){ Id = 1, IsActive = true, Caption="Eligible Expense Type"},
                new EligibleExpenseType(){ Id =2, IsActive = false,Caption="Eligible Expense Type"}
            };
            var ClaimConfig = new ProgramConfiguration()
            {
                EligibleExpenseTypes = expenseTypes
            };

            var expenseTypesToValidate = new EligibleExpenseType()
            { Id = 3, ProgramConfigurations = { ClaimConfig }, Caption = "Eligible Expense Type" };


            helper.MockDbSet<EligibleExpenseType>(new[] { expenseTypesToValidate });

            var service = helper.Create<EligibleExpenseTypeService>();

            // Act
            var validationResults = service.Validate(expenseTypesToValidate).ToArray();

            string validateMsg = "The caption must be unique within a program configuration.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }
    }
}