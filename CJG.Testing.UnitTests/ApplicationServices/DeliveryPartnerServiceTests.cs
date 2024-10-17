using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	/// <summary>
	///     Unit tests for DeliveryPartnerService
	/// </summary>
	[TestClass]
	public class DeliveryPartnerServiceTests : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod, TestCategory("Delivery Partner"), TestCategory("Service")]
		public void GetDeliveryPartner_WithDeliveryPartnerId_ReturnsMatchedDeliveryPartner()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(Application.Services.DeliveryPartnerService), identity);
			helper.MockDbSet( new[] {
				new DeliveryPartner
				{
					Id = 1,
					Caption = "Name 1"
				},
				new DeliveryPartner
				{
					Id = 2,
					Caption = "Name 2"
				},
				new DeliveryPartner
				{
					Id = 3,
					Caption = "Name 3"
				}
			});
			var service = helper.Create<Application.Services.DeliveryPartnerService>();

			// Act
			var deliveryPartner = service.GetDeliveryPartner(1);

			// Assert
			deliveryPartner.Caption.Should().Be("Name 1");
		}

		[TestMethod, TestCategory("Delivery Partner"), TestCategory("Service")]
		public void GetDeliveryPartners_WithGrantProgramId_ReturnsMatchedDeliveryPartners()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(Application.Services.DeliveryPartnerService), identity);
			var dbSetMock = helper.MockDbSet(new[] {
				new DeliveryPartner
				{
					Id = 1,
					GrantProgramId = 1
				},
				new DeliveryPartner
				{
					Id = 2,
					GrantProgramId = 1
				},
				new DeliveryPartner
				{
					Id = 3,
					GrantProgramId = 2
				}
			});
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.DeliveryPartners.AsNoTracking()).Returns(dbSetMock.Object);

			var service = helper.Create<Application.Services.DeliveryPartnerService>();

			// Act
			var result = service.GetDeliveryPartners(1);

			// Assert
			result.Should().HaveCount(2);
		}

		[TestMethod, TestCategory("Delivery Partner Service"), TestCategory("Service")]
		public void GetDeliveryPartnerService_WithDeliveryPartnerServiceId_ReturnsMatchedDeliveryPartnerService()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(Application.Services.DeliveryPartnerService), identity);
			helper.MockDbSet( new[] {
				new CJG.Core.Entities.DeliveryPartnerService
				{
					Id = 1,
					Caption = "Name 1"
				},
				new CJG.Core.Entities.DeliveryPartnerService
				{
					Id = 2,
					Caption = "Name 2"
				},
				new CJG.Core.Entities.DeliveryPartnerService
				{
					Id = 3,
					Caption = "Name 3"
				}
			});
			var service = helper.Create<Application.Services.DeliveryPartnerService>();

			// Act
			var deliveryPartnerService = service.GetDeliveryPartnerService(1);

			// Assert
			deliveryPartnerService.Caption.Should().Be("Name 1");
		}

		[TestMethod, TestCategory("Delivery Partner Service"), TestCategory("Service")]
		public void GetDeliveryPartnerServices_WithGrantProgramId_ReturnsMatchedDeliveryPartnerServices()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(Application.Services.DeliveryPartnerService), identity);
			var dbSetMock = helper.MockDbSet( new[] {
				new CJG.Core.Entities.DeliveryPartnerService
				{
					Id = 1,
					GrantProgramId = 1
				},
				new CJG.Core.Entities.DeliveryPartnerService
				{
					Id = 2,
					GrantProgramId = 1
				},
				new CJG.Core.Entities.DeliveryPartnerService
				{
					Id = 3,
					GrantProgramId = 2
				}
			});
			var service = helper.Create<Application.Services.DeliveryPartnerService>();
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.DeliveryPartnerServices.AsNoTracking()).Returns(dbSetMock.Object);

			// Act
			var result = service.GetDeliveryPartnerServices(1);

			// Assert
			result.Should().HaveCount(2);
		}
	}
}
