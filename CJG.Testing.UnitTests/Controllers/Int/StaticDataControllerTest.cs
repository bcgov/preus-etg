using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class StaticDataControllerTest
	{

		#region GetOrganizationTypes
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetOrganizationTypes()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var organizationType = EntityHelper.CreateOrganizationType();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetOrganizationTypes())
				.Returns(new List<OrganizationType>() { organizationType });

			// Act
			var result = controller.GetOrganizationTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var data = result.Data as IEnumerable<KeyValuePair<int, string>>;
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(organizationType.Id);
			data.First().Value.Should().Be(organizationType.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetOrganizationTypes(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetOrganizationTypes_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetOrganizationTypes()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetOrganizationTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetLegalStructures
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetLegalStructures()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var legalStructures = EntityHelper.CreateLegalStructure();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetLegalStructures())
				.Returns(new List<LegalStructure>() { legalStructures });

			// Act
			var result = controller.GetLegalStructures();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var data = result.Data as IEnumerable<KeyValuePair<int, string>>;
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(legalStructures.Id);
			data.First().Value.Should().Be(legalStructures.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetLegalStructures(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetLegalStructures_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetLegalStructures()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetLegalStructures();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetRiskClassifications
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetRiskClassifications()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var riskClassification = EntityHelper.CreateRiskClassification();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRiskClassifications())
				.Returns(new List<RiskClassification>() { riskClassification });

			// Act
			var result = controller.GetRiskClassifications();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<RiskClassification[]>();
			var data = result.Data as RiskClassification[];
			data.Count().Should().Be(1);
			data[0].Id.Should().Be(riskClassification.Id);
			data[0].Caption.Should().Be(riskClassification.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetRiskClassifications(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetRiskClassifications_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetRiskClassifications()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetRiskClassifications();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetDeliveryMethods
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetDeliveryMethods()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var deliveryMethod = EntityHelper.CreateDeliveryMethod("test");
			helper.GetMock<IStaticDataService>().Setup(m => m.GetDeliveryMethods())
				.Returns(new List<DeliveryMethod>() { deliveryMethod });

			// Act
			var result = controller.GetDeliveryMethods();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValueListItem<int, string>[]>();
			var data = result.Data as KeyValueListItem<int, string>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(deliveryMethod.Id);
			data[0].Value.Should().Be(deliveryMethod.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetDeliveryMethods(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetDeliveryMethods_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetDeliveryMethods()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetDeliveryMethods();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetSkillLevels
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetSkillLevels()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var skillLevel = EntityHelper.CreateSkillLevel("test");
			helper.GetMock<IStaticDataService>().Setup(m => m.GetSkillLevels())
				.Returns(new List<SkillLevel>() { skillLevel });

			// Act
			var result = controller.GetSkillLevels();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValueListItem<int, string>[]>();
			var data = result.Data as KeyValueListItem<int, string>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(skillLevel.Id);
			data[0].Value.Should().Be(skillLevel.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetSkillLevels(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetSkillLevels_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetSkillLevels()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetSkillLevels();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetInDemandOccupations
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetInDemandOccupations()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var inDemandOccupation = EntityHelper.CreateInDemandOccupation("InDemandOccupation");
			helper.GetMock<IStaticDataService>().Setup(m => m.GetInDemandOccupations())
				.Returns(new List<InDemandOccupation>() { inDemandOccupation });

			// Act
			var result = controller.GetInDemandOccupations();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValueListItem<int, string>[]>();
			var data = result.Data as KeyValueListItem<int, string>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(inDemandOccupation.Id);
			data[0].Value.Should().Be(inDemandOccupation.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetInDemandOccupations(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetInDemandOccupations_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetInDemandOccupations()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetInDemandOccupations();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetTrainingLevels
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetTrainingLevels()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var trainingLevels = EntityHelper.CreateTrainingLevel("TrainingLevel");
			helper.GetMock<IStaticDataService>().Setup(m => m.GetTrainingLevels())
				.Returns(new List<TrainingLevel>() { trainingLevels });

			// Act
			var result = controller.GetTrainingLevels();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValueListItem<int, string>[]>();
			var data = result.Data as KeyValueListItem<int, string>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(trainingLevels.Id);
			data[0].Value.Should().Be(trainingLevels.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingLevels(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetTrainingLevels_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetTrainingLevels()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingLevels();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetUnderRepresentedGroups
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetUnderRepresentedGroups()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var underRepresentedGroup = EntityHelper.CreateUnderRepresentedGroup("UnderRepresentedGroup");
			underRepresentedGroup.IsActive = true;
			helper.GetMock<IStaticDataService>().Setup(m => m.GetUnderRepresentedGroups())
				.Returns(new List<UnderRepresentedGroup>() { underRepresentedGroup });

			// Act
			var result = controller.GetUnderRepresentedGroups();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValueListItem<int, string>[]>();
			var data = result.Data as KeyValueListItem<int, string>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(underRepresentedGroup.Id);
			data[0].Value.Should().Be(underRepresentedGroup.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetUnderRepresentedGroups(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetUnderRepresentedGroups_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetUnderRepresentedGroups()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetUnderRepresentedGroups();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetTrainingProviderTypes
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetTrainingProviderTypes()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var trainingProviderType = EntityHelper.CreateTrainingProviderType("TrainingProviderType");
			trainingProviderType.IsActive = true;
			trainingProviderType.PrivateSectorValidationType = TrainingProviderPrivateSectorValidationTypes.Always;
			helper.GetMock<IStaticDataService>().Setup(m => m.GetTrainingProviderTypes())
				.Returns(new List<TrainingProviderType>() { trainingProviderType });

			// Act
			var result = controller.GetTrainingProviderTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValueParent<int, string, TrainingProviderPrivateSectorValidationTypes>[]>();
			var data = result.Data as KeyValueParent<int, string, TrainingProviderPrivateSectorValidationTypes>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(trainingProviderType.Id);
			data[0].Value.Should().Be(trainingProviderType.Caption);
			data[0].Parent.Should().Be(trainingProviderType.PrivateSectorValidationType);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderTypes(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetTrainingProviderTypes_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetTrainingProviderTypes()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProviderTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetExpectedQualifications
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetExpectedQualifications()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var expectedQualification = EntityHelper.CreateExpectedQualification("ExpectedQualification");
			expectedQualification.IsActive = true;
			helper.GetMock<IStaticDataService>().Setup(m => m.GetExpectedQualifications())
				.Returns(new List<ExpectedQualification>() { expectedQualification });

			// Act
			var result = controller.GetExpectedQualifications();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValueListItem<int, string>[]>();
			var data = result.Data as KeyValueListItem<int, string>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(expectedQualification.Id);
			data[0].Value.Should().Be(expectedQualification.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetExpectedQualifications(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetExpectedQualifications_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetExpectedQualifications()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetExpectedQualifications();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetProgramTypes
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetProgramTypes()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var programType = EntityHelper.CreateProgramType();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetProgramTypes())
				.Returns(new List<ProgramType>() { programType });

			// Act
			var result = controller.GetProgramTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<ProgramTypes, string>[]>();
			var data = result.Data as KeyValuePair<ProgramTypes, string>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(programType.Id);
			data[0].Value.Should().Be(programType.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetProgramTypes(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetProgramTypes_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetProgramTypes()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetProgramTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetExpenseTypes
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetExpenseTypes()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var expenseType = EntityHelper.CreateExpenseType();
			helper.GetMock<IExpenseTypeService>().Setup(m => m.GetAll())
				.Returns(new List<ExpenseType>() { expenseType });

			// Act
			var result = controller.GetExpenseTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<ExpenseTypes, string>[]>();
			var data = result.Data as KeyValuePair<ExpenseTypes, string>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(expenseType.Id);
			data[0].Value.Should().Be(expenseType.Caption);
			helper.GetMock<IExpenseTypeService>().Verify(m => m.GetAll(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetExpenseTypes_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IExpenseTypeService>().Setup(m => m.GetAll()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetExpenseTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetRates
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetRates()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var rateFormats = EntityHelper.CreateRateFormat();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRateFormats())
				.Returns(new List<RateFormat>() { rateFormats });

			// Act
			var result = controller.GetRates();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<double, string>[]>();
			var data = result.Data as KeyValuePair<double, string>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(rateFormats.Rate);
			data[0].Value.Should().Be(rateFormats.Format);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetRateFormats(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetRates_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetRateFormats()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetRates();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetClaimTypes
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetClaimTypes()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);
			var controller = helper.Create();

			var claimType = EntityHelper.CreateClaimType(ProgramTypes.EmployerGrant);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetClaimTypes())
				.Returns(new List<ClaimType>() { claimType });

			// Act
			var result = controller.GetClaimTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<ClaimTypes, string>[]>();
			var data = result.Data as KeyValuePair<ClaimTypes, string>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(claimType.Id);
			data[0].Value.Should().Be(claimType.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetClaimTypes(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(StaticDataController))]
		public void GetClaimTypes_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<StaticDataController>(user, Roles.Assessor);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetClaimTypes()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetClaimTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion
	}
}
