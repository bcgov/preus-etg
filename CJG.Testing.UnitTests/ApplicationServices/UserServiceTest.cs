using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class UserServiceTest : ServiceUnitTestBase
	{		
		[TestInitialize]
		public void Setup()
		{

		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void GetUser_WhenPassInUserGuidID_ShouldFindTheUser()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			user.FirstName = "CJG01";
			user.LastName = "Test01";
			var helper = new ServiceHelper(typeof(UserService), user);
			helper.MockDbSet<User>(user);
			var service = helper.Create<UserService>();

			// Act
			var result = service.GetUser(user.BCeIDGuid);

			// Assert
			result.FirstName.Should().Be("CJG01");
			result.LastName.Should().Be("Test01");
			result.BCeIDGuid.Should().Be(user.BCeIDGuid);
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void GetUser_WhenPassInUserID_ShouldFindTheUser()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			user.FirstName = "FirstName1";
			user.LastName = "LastName1";
			var helper = new ServiceHelper(typeof(UserService), user);
			helper.MockDbSet<User>(new[] { user });
			var service = helper.Create<UserService>();

			// Act
			var result = service.GetUser(user.Id);

			// Assert
			result.FirstName.Should().Be("FirstName1");
			result.LastName.Should().Be("LastName1");
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void GetInternalUser_WhenPassInInternalUserID_ShouldFindTheInternalUser()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			user.FirstName = "FirstName1";
			user.LastName = "LastName1";
			var helper = new ServiceHelper(typeof(UserService), user, "Assessor");
			helper.MockDbSet<InternalUser>(new[] { user });
			var service = helper.Create<UserService>();

			// Act
			var result = service.GetInternalUser(user.Id);

			// Assert
			result.FirstName.Should().Be("FirstName1");
			result.LastName.Should().Be("LastName1");
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void GetInternalUser_WhenPassInInternalUserIDIR_ShouldFindTheInternalUser()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			user.FirstName = "FirstName1";
			user.LastName = "LastName1";
			user.IDIR = "IDIRNAME";
			var helper = new ServiceHelper(typeof(UserService), user, "Assessor");
			helper.MockDbSet<InternalUser>(new[] { user });
			var service = helper.Create<UserService>();

			// Act
			var result = service.GetInternalUser(user.IDIR);

			// Assert
			result.IDIR.Should().Be("IDIRNAME");
			result.FirstName.Should().Be(user.FirstName);
			result.LastName.Should().Be(user.LastName);
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void RegisterUser_WhenPassInUserWithNotMatchBCeIDGuid_ShouldAddUserToDataStoreAndReturnUserWithoutOrganizationAssigned()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var organization = new Organization()
			{
				BCeIDGuid = Guid.NewGuid(),
				LegalName = "Organization Legal Name"
			};
			user.Organization = organization;
			user.FirstName = "FirstName1";
			user.LastName = "LastName1";
			var helper = new ServiceHelper(typeof(UserService), user);
			var dbSetMock = helper.MockDbSet<User>();
			helper.MockDbSet<Organization>(new[] { organization });
			var service = helper.Create<UserService>();

			// Act
			var result = service.RegisterUser(user);

			// Assert
			result.FirstName.Should().Be("FirstName1");
			result.LastName.Should().Be("LastName1");
			dbSetMock.Verify(x => x.Add(It.IsAny<User>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void RegisterUser_WhenPassInUserWithMatchBCeIDGuid_ShouldAddUserToDataStoreAndReturnUserWithOrganizationAssigned()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var organization = new Organization()
			{
				BCeIDGuid = Guid.NewGuid(),
				LegalName = "Organization Legal Name"
			};
			user.Organization = organization;
			user.FirstName = "FirstName1";
			user.LastName = "LastName1";
			var helper = new ServiceHelper(typeof(UserService), user);
			helper.MockDbSet<User>();
			helper.MockDbSet<Organization>(new[] { organization });
			helper.GetMock<IDataContext>().Setup(m => m.Users.Add(It.IsAny<User>()));
			var service = helper.Create<UserService>();

			// Act
			var result = service.RegisterUser(user);

			// Assert
			result.FirstName.Should().Be("FirstName1");
			result.LastName.Should().Be("LastName1");
			result.Organization.LegalName.Should().Be("Organization Legal Name");
			helper.GetMock<IDataContext>().Verify(x => x.Users.Add(It.IsAny<User>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void UpdateUse_WhenPassInUserInfo_ShouldUpdateUserInfoInDataStore()
		{
			// Arrange
			var organizationBCEIDGuid = Guid.NewGuid();
			var updateUser = new User()
			{
				FirstName = "FirstName1",
				LastName = "LastName1",
				Organization = new Organization() { BCeIDGuid = organizationBCEIDGuid, LegalName = "Legal name" }
			};
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(UserService), identity);
			var dbContextMock = helper.GetMock<IDataContext>();
			var dbSetMock = helper.MockDbSet(updateUser);
			var service = helper.Create<UserService>();

			// Act
			service.Update(updateUser);

			// Assert
			dbContextMock.Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void GetBCeIDUser_WhenUserGuid_ShouldReturnUserInfo()
		{
			// Arrange
			var userBCEIDGuid = Guid.NewGuid();
			var searchUser = new User()
			{
				FirstName = "FirstName1",
				LastName = "LastName1",
				BCeIDGuid = userBCEIDGuid,
			};
			var dbContextMock = new Mock<IDataContext>();
			var externalUser = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(externalUser);
			var helper = new ServiceHelper(typeof(UserService), identity);
			var service = helper.Create<UserService>();
			var mockIBCeIDService = helper.GetMock<IBCeIDService>();

			mockIBCeIDService.Setup(x => x.GetBusinessUserInfo(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BCeIDAccountTypeCodes>())).Returns(searchUser);

			// Act
			var result = service.GetBCeIDUser(searchUser.BCeIDGuid);

			// Assert
			result.FirstName = searchUser.FirstName;
			result.LastName = searchUser.LastName;
			result.BCeIDGuid = searchUser.BCeIDGuid;
			mockIBCeIDService.Verify(x => x.GetBusinessUserInfo(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BCeIDAccountTypeCodes>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void GetIDIRUser_WhenUserID_ShouldReturnUserInfo()
		{
			// Arrange
			var userBCEIDGuid = Guid.NewGuid();
			var searchUser = new User()
			{
				FirstName = "FirstName1",
				LastName = "LastName1",
				BCeIDGuid = userBCEIDGuid,
			};
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(UserService), identity);
			var service = helper.Create<UserService>();
			var mockIBCeIDService = helper.GetMock<IBCeIDService>();

			mockIBCeIDService.Setup(x => x.GetInternalUserInfo(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BCeIDAccountTypeCodes>())).Returns(searchUser);

			// Act
			var result = service.GetIDIRUser(searchUser.BCeIDGuid);

			// Assert
			result.FirstName = searchUser.FirstName;
			result.LastName = searchUser.LastName;
			result.BCeIDGuid = searchUser.BCeIDGuid;
			mockIBCeIDService.Verify(x => x.GetInternalUserInfo(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BCeIDAccountTypeCodes>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void GetAccountType__ShouldReturnAccountTypeInternal()
		{
			// Arrange
			var loggerMock = new Mock<ILogger>();
			var dbContextMock = new Mock<IDataContext>();
			var httpContextMock = new Mock<HttpContextBase>();
			var siteMinderServiceMock = new Mock<ISiteMinderService>();
			siteMinderServiceMock.SetupGet(x => x.CurrentUserType).Returns(BCeIDAccountTypeCodes.Internal);
			var BCeIDServiceMock = new Mock<IBCeIDService>();
			var organizationServiceMock = new Mock<IOrganizationService>();
			var staticDataService = new Mock<IStaticDataService>();
			var service = new UserService(BCeIDServiceMock.Object, siteMinderServiceMock.Object, staticDataService.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			var result = service.GetAccountType();

			// Assert
			result.Should().Be(AccountTypes.Internal);
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void GetAccountType__ShouldReturnAccountTypeExternal()
		{
			// Arrange
			var loggerMock = new Mock<ILogger>();
			var dbContextMock = new Mock<IDataContext>();
			var siteMinderServiceMock = new Mock<ISiteMinderService>();
			siteMinderServiceMock.SetupGet(x => x.CurrentUserType).Returns(BCeIDAccountTypeCodes.Business);
			var organizationServiceMock = new Mock<IOrganizationService>(); var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(UserService), identity);
			var service = helper.Create<UserService>();

			// Act
			var result = service.GetAccountType();

			// Assert
			result.Should().Be(AccountTypes.External);
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void GetIDIRUser_WhenPassInIDIRString_ShouldReturnUserInfo()
		{
			//Arrange
			var searchUser = new User()
			{
				FirstName = "FirstName1",
				LastName = "LastName1",
			};
			string IDirString = "Idir";
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(UserService), identity);
			var service = helper.Create<UserService>();
			var mockIBCeIDService = helper.GetMock<IBCeIDService>();

			mockIBCeIDService.Setup(x => x.GetInternalUserInfo(It.IsAny<string>(), It.IsAny<Guid>())).Returns(searchUser);

			// Act
			var result = service.GetIDIRUser(IDirString);

			// Assert
			result.FirstName = searchUser.FirstName;
			result.LastName = searchUser.LastName;
			mockIBCeIDService.Verify(x => x.GetInternalUserInfo(It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void CopyBceidUserValues_WithValidBceIDUserAndEmptyAppUser_ShouldCopyAllvaluesAndReturnTrue()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(UserService), identity);
			var service = helper.Create<UserService>();
			var targetUser = new User() { FirstName = "F2", LastName = "L2", EmailAddress = "E2", Organization = new Organization() { Id = 1 } };

			var searchUser = new User()
			{
				FirstName = "FirstName1",
				LastName = "LastName1",
			};
			var mockIBCeIDService = helper.GetMock<IBCeIDService>();
			mockIBCeIDService.Setup(x => x.GetBusinessUserInfo(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BCeIDAccountTypeCodes>())).Returns(searchUser);

			// Act
			var result =
				service.CopyBceIDUserOrganizationValues(
					new User { FirstName = "F1", LastName = "L1", EmailAddress = "E1" }, targetUser);

			// Assert
			result.Should().BeTrue();
			targetUser.FirstName.Should().Be("F1");
			targetUser.LastName.Should().Be("L1");
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void CreateUserProfile_Should_Add_User_And_Commit()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			user.BCeIDGuid = Guid.NewGuid();
			user.FirstName = "FirstName1";
			user.LastName = "LastName1";
			user.PhysicalAddress = new Address { AddressLine1 = "422 sixth st" };
			var userProfileDetails = new UserProfileDetailModel
			{
				MailingAddressLine1 = "422 sixth st",
				Phone = "604-295-6655",
				PhysicalAddressLine1 = "422",
				MailingCity = "surrey",
				MailingPostalCode = "V4R4J8",
				MailingRegionId = "1",
				MailingCountryId = "2",
				PhysicalAddressLine2 = "422",
				PhysicalCity = "Surrey",
				PhysicalPostalCode = "V4R4J8",
				PhysicalRegionId = "12",
				PhysicalCountryId = "7"
			};
			var helper = new ServiceHelper(typeof(UserService), user);
			var dbSetMock = helper.MockDbSet<User>();
			helper.MockDbSet<Organization>();
			helper.MockDbSet<UserPreference>();
			helper.GetMock<IDataContext>().Setup(m => m.Users.Add(It.IsAny<User>()));
			var mockIBCeIDService = helper.GetMock<IBCeIDService>();
			var service = helper.Create<UserService>();

			mockIBCeIDService.Setup(x => x.GetBusinessUserInfo(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BCeIDAccountTypeCodes>())).Returns(user);

			// Act
			service.CreateUserProfile(user.BCeIDGuid, userProfileDetails);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void UpdateUserProfile_Should_Update_User_Twice_And_Commit_Zero_Times()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var userProfileDetails = new UserProfileDetailModel
			{
				MailingAddressLine1 = "422 sixth st",
				Phone = "604-295-6655",
				PhysicalAddressLine1 = "422",
				MailingCity = "surrey",
				MailingPostalCode = "V4R4J8",
				MailingRegionId = "1",
				MailingCountryId = "2",
				PhysicalAddressLine2 = "422",
				PhysicalCity = "Surrey",
				PhysicalPostalCode = "V4R4J8",
				PhysicalRegionId = "12",
				PhysicalCountryId = "7",
				Title = "Software"
			};

			var helper = new ServiceHelper(typeof(UserService), user);
			var mockUsers = helper.MockDbSet<User>(user);
			var service = helper.Create<UserService>();
			var mockIBCeIDService = helper.GetMock<IBCeIDService>();

			mockIBCeIDService.Setup(x => x.GetBusinessUserInfo(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BCeIDAccountTypeCodes>())).Returns(user);

			// Act
			service.UpdateUserProfile(user.Id, userProfileDetails);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Exactly(0));
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void UpdateUserDetails_When_standAloneTransaction_Should_Commit_Once()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var userProfileDetails = new UserProfileDetailModel
			{
				MailingAddressLine1 = "422 sixth st",
				Phone = "604-295-6655",
				PhysicalAddressLine1 = "422",
				MailingCity = "surrey",
				MailingPostalCode = "V4R4J8",
				MailingRegionId = "1",
				MailingCountryId = "2",
				PhysicalAddressLine2 = "422",
				PhysicalCity = "Surrey",
				PhysicalPostalCode = "V4R4J8",
				PhysicalRegionId = "12",
				PhysicalCountryId = "7",
				Title = "Software"
			};
			var preferencePrograms = new List<UserGrantProgramPreferenceModel>();
			var helper = new ServiceHelper(typeof(UserService), user);
			var mockUsers = helper.MockDbSet<User>( user );
			var service = helper.Create<UserService>();
			var mockIBCeIDService = helper.GetMock<IBCeIDService>();

			mockIBCeIDService.Setup(x => x.GetBusinessUserInfo(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BCeIDAccountTypeCodes>())).Returns(user);

			// Act
			service.UpdateUserDetails(user.Id, userProfileDetails, true);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void BindBusinessUserToEntity_Should_Bind_User_To_Entity()
		{
			// Arrange
			var dbContextMock = new Mock<IDataContext>();
			var externalUser = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(externalUser);
			var helper = new ServiceHelper(typeof(UserService), identity);
			var service = helper.Create<UserService>();

			var user = new User()
			{
				Id = 1,
				FirstName = "FirstName1",
				LastName = "LastName1",
				PhysicalAddress = new Address { AddressLine1 = "422 sixth st" },
				JobTitle = "Software Developer"
			};
			var userProfileDetails = new UserProfileDetailModel
			{
				MailingAddressLine1 = "422 sixth st",
				Phone = "604-295-6655",
				PhysicalAddressLine1 = "422",
				MailingCity = "surrey",
				MailingPostalCode = "V4R4J8",
				MailingRegionId = "1",
				MailingCountryId = "2",
				PhysicalAddressLine2 = "422",
				PhysicalCity = "Surrey",
				PhysicalPostalCode = "V4R4J8",
				PhysicalRegionId = "12",
				PhysicalCountryId = "7",
				Title = "Software",
				PhoneAreaCode = "604",
				PhoneExchange = "888",
				PhoneNumber = "384-0726",
				PhoneExtension = "604",
				Subscribe = false
			};
			helper.MockDbSet(user);
			
			var standAloneTransaction = true;

			// Act
			service.UpdateUserDetails(user.Id, userProfileDetails, standAloneTransaction); //Testing BindBusinessUserToEntity via UpdateUserDetails because it's Private.

			// Assert
			user.JobTitle.Equals(userProfileDetails.Title);
			user.PhoneNumber.Equals($"({userProfileDetails.PhoneAreaCode}) {userProfileDetails.PhoneExchange}-{userProfileDetails.PhoneNumber}");
			user.PhoneExtension.Equals(userProfileDetails.PhoneExtension);
			user.IsSubscriberToEmail.Equals(userProfileDetails.Subscribe);
			user.PhysicalAddress.Equals(
				new Address
				{
					AddressLine1 = userProfileDetails.PhysicalAddressLine1,
					AddressLine2 = userProfileDetails.PhysicalAddressLine2,
					City = userProfileDetails.PhysicalCity,
					PostalCode = userProfileDetails.PhysicalPostalCode,
					RegionId = userProfileDetails.PhysicalRegionId,
					CountryId = userProfileDetails.PhysicalCountryId
				});
			user.MailingAddress.Equals(user.PhysicalAddress);
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void GetConfirmationDetails_User_Is_Mapped_And_Returns_UserProfileConfirmationModel()
		{
			// Arrange
			var externalUser = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(externalUser);
			var helper = new ServiceHelper(typeof(UserService), identity);
			var service = helper.Create<UserService>();

			var bCEIDGuid = Guid.NewGuid();
			var user = new User()
			{
				Id = 1,
				FirstName = "FirstName1",
				LastName = "LastName1",
				BCeIDGuid = bCEIDGuid,
				EmailAddress = "v.s@avocette.com",
				PhoneNumber = "604-298-0216"
			};
			var mockIBCeIDService = helper.GetMock<IBCeIDService>();
			mockIBCeIDService.Setup(x => x.GetBusinessUserInfo(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BCeIDAccountTypeCodes>())).Returns(user);

			// Act
			var result = service.GetConfirmationDetails(user.BCeIDGuid);

			// Assert
			result.FirstName.Should().NotBeNullOrEmpty();
			result.LastName.Should().NotBeNullOrEmpty();
			result.EmailAddress.Should().NotBeNullOrEmpty();
			result.PhoneNumber.Should().NotBeNullOrEmpty();
		}

		[TestMethod, TestCategory("User"), TestCategory("Service")]
		public void UserProfileDetailModel_User_Is_Mapped_And_Returns_UserProfileDetailModel()
		{
			// Arrange
			var dbContextMock = new Mock<IDataContext>();
			var externalUser = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(externalUser);
			var helper = new ServiceHelper(typeof(UserService), identity);
			var service = helper.Create<UserService>();
			var user = new User()
			{
				Id = 1,
				PhysicalAddress = new Address {
					AddressLine1 = "422 sixth st",
					Region = new Region { Id = "1" },
					Country = new Country { Id = "1" }
				},
				BCeIDGuid = Guid.NewGuid()
			};

			var provinces = new List<Region>
			{
				new Region { Country = new Country {Id = "1", Name = "CA" }, CountryId = "23", Id = "1"}
			};
			helper.MockDbSet(user);
			
			// Act
			var result = service.GetUserProfileDetails(user.Id);

			// Assert
			result.PhysicalAddressLine1.Should().NotBeNullOrEmpty();
		}
	}
}
