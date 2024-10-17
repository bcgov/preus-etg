using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.Entity;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class AccountCodeServiceTests : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod, TestCategory("Account Codes")]
		public void GetAll_When_Multiple_AccountCodes_Exist_Return_All()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(AccountCodeService), user);
			var service = helper.Create<AccountCodeService>();
			helper.MockDbSet(new[]
			{
				new AccountCode(),
				new AccountCode()
			});

			// Act
			var result = service.GetAll();

			// Assert
			result.Should().HaveCount(2);
		}

		[TestMethod, TestCategory("Account Codes")]
		public void Get_By_Id_Returns_AccountCode()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(AccountCodeService), user);
			var service = helper.Create<AccountCodeService>();
			var accountCode = new AccountCode { Id = 1 };
			helper.MockDbSet(accountCode);

			// Act
			var result = service.Get(1);

			// Assert
			result.Id.Should().Be(1);
		}

		[TestMethod, TestCategory("Account Codes")]
		public void Get_When_Find_AccountCode_By_Id_Returns_Null_Throw_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(AccountCodeService), user);
			var service = helper.Create<AccountCodeService>();
			helper.MockDbSet((AccountCode)null);

			// Act
			Action action = () => service.Get(1);

			// Assert
			action.Should().Throw<NoContentException>();
		}

		[TestMethod, TestCategory("Account Codes")]
		public void Add_AccoundCode_Added_To_AccountCodes_And_Commited()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(AccountCodeService), user);
			var accountCode = new AccountCode { Id = 1 };
			var service = helper.Create<AccountCodeService>();
			helper.MockDbSet(accountCode);

			// Act
			service.Add(accountCode);

			// Assert
			helper.GetMock<DbSet<AccountCode>>().Verify(x => x.Add(It.IsAny<AccountCode>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Account Codes")]
		public void Update()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(AccountCodeService), user);
			var accountCode = new AccountCode { Id = 1 };
			var service = helper.Create<AccountCodeService>();
			helper.MockDbSet(accountCode);

			// Act
			service.Update(accountCode);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<AccountCode>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Account Codes")]
		public void Delete()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(AccountCodeService), user);
			var accountCode = new AccountCode { Id = 1 };
			var service = helper.Create<AccountCodeService>();
			helper.MockDbSet(accountCode);

			// Act
			service.Delete(accountCode);

			// Assert
			helper.GetMock<DbSet<AccountCode>>().Verify(x => x.Remove(It.IsAny<AccountCode>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Exactly(1));
		}
	}
}