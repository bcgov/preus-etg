using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
    public class ExpenseTypeServiceTests
    {
        [TestMethod]
        [TestCategory("Expense Types"), TestCategory("Service")]
        public void GetAllExpenses()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(ExpenseTypeService), identity);
            var expenseTypes = new List<ExpenseType>()
            {
                new ExpenseType(){ Id = ExpenseTypes.ParticipantAssigned, IsActive = true },
                new ExpenseType(){ Id = ExpenseTypes.ParticipantLimited, IsActive = false }
            };
            var service = helper.Create<ExpenseTypeService>();
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            helper.MockDbSet(expenseTypes);

            // Act
            var results = service.GetAll();

            // Assert
            results.Count().Should().Be(2);
        }

        [TestMethod]
        [TestCategory("Expense Types"), TestCategory("Service")]
        public void GetAllActiveExpenses()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(ExpenseTypeService), identity);
            var expenseTypes = new List<ExpenseType>()
            {
                new ExpenseType() { Id = ExpenseTypes.ParticipantAssigned, IsActive = true },
                new ExpenseType() { Id = ExpenseTypes.ParticipantLimited, IsActive = false }
            };
			var context = helper.GetMock<IDataContext>();
			var service = helper.Create<ExpenseTypeService>();
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
		    var dbSetMock = helper.MockDbSet(expenseTypes);
			context.Setup(c => c.ExpenseTypes.AsNoTracking()).Returns(dbSetMock.Object);

            // Act
            var results = service.GetAll(true);

            // Assert
            results.Count().Should().Be(1);
        }
        [TestMethod]
        [TestCategory("Expense Types"), TestCategory("Service")]
        public void GetExpenseType()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(ExpenseTypeService), identity);
            var expenseType = new ExpenseType(){
                Id = ExpenseTypes.ParticipantAssigned,
                Description = "Expense type 1"
            };
            var service = helper.Create<ExpenseTypeService>();
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            helper.MockDbSet(new[] { expenseType });

            // Act
            var results = service.Get(ExpenseTypes.ParticipantAssigned);

            // Assert
            results.Description.Should().Be("Expense type 1");
        }

        /// <summary>
        /// GetExpenseType with a non-existing Id should throw an exception.
        /// </summary>
        [TestMethod]
        [TestCategory("Expense Types"), TestCategory("Service")]
        public void GetExpenseType_WithInvalidId_ThrowsException()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(ExpenseTypeService), identity);
            var expenseTypes = new List<ExpenseType>()
            {
                new ExpenseType(){ Id = ExpenseTypes.ParticipantAssigned, Description = "Expense type 1"}
            };
            var service = helper.Create<ExpenseTypeService>();
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            helper.MockDbSet(expenseTypes);

            // Act
            Action actionToTest = () => service.Get(ExpenseTypes.NotParticipantLimited);

            // Assert
            actionToTest.Should().Throw<NoContentException>();
        }
    }
}
