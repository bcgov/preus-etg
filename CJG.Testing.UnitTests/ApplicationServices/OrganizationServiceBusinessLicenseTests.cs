using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Testing.Core;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class OrganizationServiceBusinessLicenseTests : ServiceUnitTestBase
	{
		private User _user;
		private ServiceHelper _helper;
		private DateTime _today;
		private OrganizationService _localService;

		[TestInitialize]
		public void Setup()
		{
			_today = DateTime.UtcNow;
			AppDateTime.SetNow(_today);

			_user = EntityHelper.CreateExternalUser();
			_helper = new ServiceHelper(typeof(OrganizationService), _user);

			_localService = _helper.Create<OrganizationService>();
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void OrganizationRequiresBusinessLicenseDocuments()
		{
			// Arrange
			var organization = new Organization
			{
				Id = 37,
				DateUpdated = _today.AddMonths(-6),
				BusinessLicenseDocuments = new List<Attachment>()
			};

			_helper.MockDbSet(organization);

			// Act
			var result = _localService.RequiresBusinessLicenseDocuments(organization.Id);
			Assert.AreEqual(true, result);
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void OrganizationDoesNotRequireBusinessLicenseDocuments()
		{
			// Arrange
			var organization = new Organization
			{
				Id = 37,
				DateUpdated = _today.AddMonths(-6),
				BusinessLicenseDocuments = new List<Attachment>
				{
					new Attachment
					{
						DateAdded = _today.AddDays(-5)
					}
				}
			};

			_helper.MockDbSet(organization);

			// Act
			var result = _localService.RequiresBusinessLicenseDocuments(organization.Id);
			Assert.AreEqual(false, result);
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void OrganizationRequiresBusinessLicenseDocumentUpdates()
		{
			// Arrange
			var organization = new Organization
			{
				Id = 37,
				DateUpdated = _today.AddMonths(-6),
				BusinessLicenseDocuments = new List<Attachment>
				{
					new Attachment
					{
						DateAdded = _today.AddMonths(-13)
					}
				}
			};

			_helper.MockDbSet(organization);

			// Act
			var result = _localService.RequiresBusinessLicenseDocuments(organization.Id);
			Assert.AreEqual(true, result);
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void OrganizationDoesNotRequireUpdateOnceRefreshed()
		{
			// Arrange
			var organization = new Organization
			{
				Id = 37,
				DateUpdated = _today.AddMonths(-6),
				BusinessLicenseDocuments = new List<Attachment>
				{
					new Attachment
					{
						DateAdded = _today.AddMonths(-13),
						DateUpdated = _today.AddMonths(-5)
					}
				}
			};

			_helper.MockDbSet(organization);

			// Act
			var result = _localService.RequiresBusinessLicenseDocuments(organization.Id);
			Assert.AreEqual(false, result);
		}
	}
}
