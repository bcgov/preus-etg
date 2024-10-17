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
    public class EligibleExpenseTypeServiceTests
    {

        [TestMethod]
        [TestCategory("Eligible Expense Types"), TestCategory("Service")]
        public void GetAllExpensesForGrantProgram()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(GrantProgramService), identity);
            var expenseTypes = new List<EligibleExpenseType>()
            {
                new EligibleExpenseType(){ Id = 1, IsActive = true},
                new EligibleExpenseType(){Id =2, IsActive = false}
            };
            var claimConfig = new ProgramConfiguration()
            {
                EligibleExpenseTypes = expenseTypes
            };
            var programs = new List<GrantProgram>()
            {
                 new GrantProgram()
                 {
                     Id = 1,
                     Name = "Program",
                     ProgramConfiguration = claimConfig

                }
             };
            
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            helper.MockDbSet(programs);
            helper.MockDbSet(expenseTypes);

            var service = helper.Create<GrantProgramService>();
            // Act
            var results = service.GetAllEligibleExpenseTypes(1);

            // Assert
            results.Count().Should().Be(2);         
        }

        [TestMethod]
        [TestCategory("Eligible Expense Types"), TestCategory("Service")]
        public void GetAllActiveExpensesForGrantProgram()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(GrantProgramService), identity);
            var expenseTypes = new List<EligibleExpenseType>()
            {
                new EligibleExpenseType(){ Id = 1, IsActive = true},
                new EligibleExpenseType(){ Id =2, IsActive = false}
            };
            var ClaimConfig = new ProgramConfiguration()
            {
                EligibleExpenseTypes = expenseTypes
            };
            var programs = new List<GrantProgram>()
            {
                 new GrantProgram()
                 {
                     Id = 1,
                     Name = "Program",
                     ProgramConfiguration = ClaimConfig

                }
             };
            
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            helper.MockDbSet(programs);
            helper.MockDbSet(expenseTypes);
            var programToGet = new GrantProgram()
            {
                Id = 1
            };
            // Act
            var service = helper.Create<GrantProgramService>();
            var results = service.GetAllActiveEligibleExpenseTypes(programToGet.Id);

            // Assert
            results.Count().Should().Be(1);
        }

        [TestMethod]
        [TestCategory("Eligible Expense Types"), TestCategory("Service")]
        public void GetEligibleExpensesType_WithValidId_ReturnsEligibleExpense()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(EligibleExpenseTypeService), identity);
            var expenseTypes = new List<EligibleExpenseType>()
            {
                new EligibleExpenseType(){ Id = 1},
            };     
            var service = helper.Create<EligibleExpenseTypeService>();
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            helper.MockDbSet(expenseTypes);

            // Act
            var results = service.Get(1);

            // Assert
            results.Id.Should().Be(1);
        }

        /// <summary>
        /// GetEligibleExpenseType with non-existing Id should throw an exception. 
        /// </summary>
        [TestMethod]
        [TestCategory("Eligible Expense Types"), TestCategory("Service")]
        public void GetEligibleExpensesType_WithInValidId_ThrowsException()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(EligibleExpenseTypeService), identity);
            var expenseTypes = new List<EligibleExpenseType>()
            {
                new EligibleExpenseType(){ Id = 1},
            };
            var service = helper.Create<EligibleExpenseTypeService>();
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            helper.MockDbSet(expenseTypes);

            // Act
            Action actionToTest = () => service.Get(2);

            //Assert
            actionToTest.Should().Throw<NoContentException>();
        }

        [TestMethod]
        [TestCategory("Eligible Expense Types"), TestCategory("Service")]
        public void UpdateGrantProgram_WithNewEligibleExpenses()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(GrantProgramService), identity);
            var programs = new List<GrantProgram>()
            {
                 new GrantProgram()
                 {
                     Id = 1,
                     Name = "Program",
                }
             };
            var expenseTypes = new List<EligibleExpenseType>();           
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            helper.MockDbSet(programs);
            var dbSetMock = helper.MockDbSet(expenseTypes);
            var expenseToAdd = new EligibleExpenseType()
            {
                Id = 1
            };

            var service = helper.Create<GrantProgramService>();

            // Act
            service.AddEligibleExpenseType(1, expenseToAdd);

            // Assert
            dbSetMock.Verify(x => x.Add(It.Is<EligibleExpenseType>(et => et.DateAdded.Date == AppDateTime.UtcNow.Date)));
        }

        /// <summary>
        /// DeleteEligibleExpenseType with a non-existing Id should throw an exception.
        /// </summary>
        [TestMethod]
        [TestCategory("Eligible Expense Types"), TestCategory("Service")]
        public void DeleteEligibleExpenses_With_Invalid_ID_ThrowsException()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(EligibleExpenseTypeService), identity);
            var expenseTypes = new List<EligibleExpenseType>()
            {
                new EligibleExpenseType(){Id = 1}
            };
            var service = helper.Create<EligibleExpenseTypeService>();
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            var dbSetMock = helper.MockDbSet(expenseTypes);
            var expenseToRemove = new EligibleExpenseType()
            {
                Id = 2
            };
            var dbContextMock = helper.GetMock<IDataContext>();

            // Act
            service.Delete(expenseToRemove);

			// Assert
			dbSetMock.Verify(x => x.Remove(It.IsAny<EligibleExpenseType>()), Times.Exactly(1));
            dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
        }

        /// <summary>
        /// DeleteEligibleExpenseType with an affiliated EligibleCost should throw an exception.
        /// </summary>
        [TestMethod]
        [TestCategory("Eligible Expense Types"), TestCategory("Service")]
        public void DeleteEligibleExpenses_With_EligibleCosts_ThrowsException()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(EligibleExpenseTypeService), identity);
            var costs = new List<EligibleCost>()
            {
               new EligibleCost()
               {
                   Id=1,
                   EligibleExpenseTypeId=2,
               }
            };
            var expenseTypes = new List<EligibleExpenseType>()
            {
                new EligibleExpenseType(){ Id = 1,},
                new EligibleExpenseType(){
                    Id = 2,
                    EligibleCosts = costs
                }
            };
            var service = helper.Create<EligibleExpenseTypeService>();
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.MockDbSet(costs);
            var dbSetMock = helper.MockDbSet(expenseTypes);
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            var expenseToRemove = new EligibleExpenseType()
            {
                Id = 3
            };     
            var dbContextMock = helper.GetMock<IDataContext>();

            // Act
            service.Delete(expenseToRemove);

			// Assert
			dbSetMock.Verify(x => x.Remove(It.IsAny<EligibleExpenseType>()), Times.Exactly(1));
            dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
        }

        /// <summary>
        /// DeleteEligibleExpenseType with an affiliated ClaimEligibleCost should throw an exception.
        /// </summary>
        [TestMethod]
        [TestCategory("Eligible Expense Types"), TestCategory("Service")]
        public void DeleteEligibleExpenses_With_ClaimEligibleCosts_ThrowsException()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(EligibleExpenseTypeService), identity);
            var costs = new List<ClaimEligibleCost>()
            {
               new ClaimEligibleCost()
               {
                   Id=1,
                   EligibleExpenseTypeId=2,
               }
            };
            var expenseTypes = new List<EligibleExpenseType>()
            {
                new EligibleExpenseType(){ Id = 1,},
                new EligibleExpenseType(){
                    Id = 2,
                    ClaimEligibleCosts = costs
                }
            };
            var service = helper.Create<EligibleExpenseTypeService>();
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.MockDbSet(costs);
            var dbSetMock = helper.MockDbSet(expenseTypes);
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            var expenseToRemove = new EligibleExpenseType() {Id = 3};

            //Assert
            var dbContextMock = helper.GetMock<IDataContext>();

            // Act
            service.Delete(expenseToRemove);

			// Assert
			dbSetMock.Verify(x => x.Remove(It.IsAny<EligibleExpenseType>()), Times.Exactly(1));
            dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
        }

        [TestMethod]
        [TestCategory("Eligible Expense Types"), TestCategory("Service")]
        public void DeleteEligibleExpenses_With_Existing_ID_Remove()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(EligibleExpenseTypeService), identity);
            var expenseTypes = new List<EligibleExpenseType>()
            {
                new EligibleExpenseType(){Id = 1},
                new EligibleExpenseType(){Id = 2}
            };
            var service = helper.Create<EligibleExpenseTypeService>();
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
            var dbSetMock = helper.MockDbSet(expenseTypes);
            var expenseToRemove = new EligibleExpenseType()
            {
                Id = 1
            };
            var dbContextMock = helper.GetMock<IDataContext>();

            // Act
            service.Delete(expenseToRemove);

			// Assert
			dbSetMock.Verify(x => x.Remove(It.IsAny<EligibleExpenseType>()), Times.Exactly(1));
            dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
        }


    }
}
