using CJG.Application.Services;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models.IntakePeriods;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class IntakePeriodsControllersTests
	{
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(IntakePeriodsController))]
		public void IntakePeriodsViews()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<IntakePeriodsController>(user, Privilege.SM);
			var controller = helper.Create();
			var grantProgram = EntityHelper.CreateGrantProgram();
			var fiscalYear = EntityHelper.CreateFiscalYear();
			var grantStream = EntityHelper.CreateGrantStream(grantProgram);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetFiscalYear(It.IsAny<int>())).Returns(fiscalYear);

			// Act
			var result = controller.IntakePeriodsView(fiscalYear.Id, grantProgram.Id, grantStream.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantProgram.Id.Should().Be(controller.ViewBag.GrantProgramId);
			fiscalYear.Id.Should().Be(controller.ViewBag.FiscalYearId);
			grantStream.Id.Should().Be(controller.ViewBag.GrantStreamId);

			helper.GetMock<IStaticDataService>().Verify(m => m.GetFiscalYear(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(IntakePeriodsController))]
		public void GetIntakePeriods()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<IntakePeriodsController>(user, Privilege.AM4);
			var controller = helper.Create();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantProgram = EntityHelper.CreateGrantProgram();
			var grantStream = EntityHelper.CreateGrantStream(grantProgram);
			var fiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IGrantOpeningService>().Setup(m => m.GetGrantOpening(It.IsAny<int>(), It.IsAny<int>())).Returns(grantOpening);
			helper.GetMock<IGrantProgramService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantProgram);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantStream);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetFiscalYear(It.IsAny<int>())).Returns(fiscalYear);

			// Act
			var result = controller.GetIntakePeriods(fiscalYear.Id, grantProgram.Id, grantStream.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<IntakePeriodsListModel>();
			var model = result.Data as IntakePeriodsListModel;
			helper.GetMock<IStaticDataService>().Verify(m => m.GetFiscalYear(It.IsAny<int>()), Times.Never);
		}
	}
}
