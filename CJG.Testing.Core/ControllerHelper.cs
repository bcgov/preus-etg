using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Ext;
using CJG.Web.External.Areas.Int;
using CJG.Web.External.Helpers;
using Moq;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CJG.Testing.Core
{
	public class ControllerHelper<T> : ServiceHelper
		where T : Controller
	{
		#region Constructors
		public ControllerHelper() : base(typeof(T))
		{
			InitializeController();
		}

		public ControllerHelper(System.Security.Principal.IPrincipal user) : base(typeof(T), user)
		{
			InitializeController();
		}

		public ControllerHelper(System.Security.Principal.IPrincipal user, params object[] args) : base(typeof(T), user, args)
		{
			InitializeController();
		}

		public ControllerHelper(User applicationAdministrator, params object[] args) : base(typeof(T), applicationAdministrator, args)
		{
			InitializeController();
		}

		public ControllerHelper(InternalUser user, string role, params object[] args) : base(typeof(T), user, role, args)
		{
			InitializeController();
		}

		public ControllerHelper(InternalUser user, Roles role, params object[] args) : base(typeof(T), user, role, args)
		{
			InitializeController();
		}

		public ControllerHelper(InternalUser user, Privilege privilege, params object[] args) : base(typeof(T), user, privilege, args)
		{
			InitializeController();
		}

		public ControllerHelper(InternalUser user, Privilege[] privileges, params object[] args) : base(typeof(T), user, privileges, args)
		{
			InitializeController();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Initialize the base controller services.
		/// </summary>
		private void InitializeController()
		{
			var controllerService = this.GetMock<IControllerService>();
			controllerService.Setup(cs => cs.StaticDataService).Returns(this.GetMock<IStaticDataService>().Object);
			controllerService.Setup(cs => cs.UserService).Returns(this.GetMock<IUserService>().Object);
			controllerService.Setup(cs => cs.SiteMinderService).Returns(this.GetMock<ISiteMinderService>().Object);
			controllerService.Setup(cs => cs.Logger).Returns(this.GetMock<ILogger>().Object);
		}

		/// <summary>
		/// Creates a new instance of <typeparamref name="T"/> and initializes it with either the args specified or the mocked objects in this helper.
		/// This always uses the constructor that was initialized for this service to create the object.
		/// </summary>
		/// <param name="args"></param>
		/// <returns>A new instance of <typeparamref name="T"/> object.</returns>
		public T Create(params object[] args)
		{
			return this.CreateController<T>(args);
		}

		/// <summary>
		/// Creates a new instance of <typeparamref name="T"/> and initializes it with either the args specified or the mocked objects in this helper.
		/// This always uses the constructor that was initialized for this service to create the object.
		/// This provides a way to bind to the model validation errors.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="args"></param>
		/// <returns>A new instance of <typeparamref name="T"/> object.</returns>
		public T CreateWithModel(object model, params object[] args)
		{
			return CreateControllerWithModel<T>(model, args);
		}

		/// <summary>
		/// Creates a new instance of <typeparamref name="CT"/> and initializes it with either the args specified or the mocked objects in this helper.
		/// This always uses the constructor that was initialized for this service to create the object.
		/// This provides a way to bind to the model validation errors.
		/// </summary>
		/// <typeparam name="CT"></typeparam>
		/// <param name="model"></param>
		/// <param name="args"></param>
		/// <returns>A new instance of <typeparamref name="CT"/> object.</returns>
		public CT CreateControllerWithModel<CT>(object model, params object[] args)
			where CT : Controller
		{
			var controller = CreateController<CT>(args);

			// Model binding
			var modelBinder = new ModelBindingContext()
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ValueProvider = new NameValueCollectionValueProvider(new NameValueCollection(), CultureInfo.InvariantCulture)
			};
			var binder = new DefaultModelBinder().BindModel(controller.ControllerContext, modelBinder);
			controller.ModelState.Clear();
			controller.ModelState.Merge(modelBinder.ModelState);

			return controller;
		}

		/// <summary>
		/// Creates a new instance of <typeparamref name="CT"/> and initializes it with either the args specified or the mocked objects in this helper.
		/// This always uses the constructor that was initialized for this service to create the object.
		/// </summary>
		/// <param name="args"></param>
		/// <returns>A new instance of <typeparamref name="CT"/> object.</returns>
		public CT CreateController<CT>(params object[] args)
			where CT : Controller
		{
			var mockHttpContext = this.GetMock<HttpContextBase>();
			mockHttpContext.Setup(c => c.Request.QueryString).Returns(new NameValueCollection());
			var httpContext = mockHttpContext.Object;

			// set up routes
			RouteTable.Routes.Clear();
			AreaRegistration areaRegistration;
			if (httpContext.User.GetAccountType() == AccountTypes.External)
				areaRegistration = new ExtAreaRegistration();
			else
				areaRegistration = new IntAreaRegistration();

			areaRegistration.RegisterArea(new AreaRegistrationContext(areaRegistration.AreaName, RouteTable.Routes));

			var routes = RouteTable.Routes;
			MapMvcAttributeRoutes(routes, typeof(CT).Assembly);
			var requestContext = new RequestContext()
			{
				HttpContext = httpContext,
				RouteData = routes.GetRouteData(httpContext)
			};

			// create controller instance
			var controller = this.Create<CT>(args) as CT;

			// set controller context and URL
			controller.ControllerContext = new ControllerContext(requestContext, controller);
			controller.Url = new UrlHelper(requestContext, routes);


			// Configure model validation
			var mockForm = new FormCollection();
			controller.ValueProvider = mockForm.ToValueProvider();
			return controller;
		}

		public void EndpointThrows<ET>(Expression<Func<T, object>> endpoint)
			where ET : Exception
		{
			try
			{
				var action = endpoint.Compile();
				action.Invoke(null);
			}
			catch (ET)
			{
				// Success
			}
		}

		private static void MapMvcAttributeRoutes(RouteCollection routeCollection, Assembly controllerAssembly)
		{
			var controllerTypes = controllerAssembly.GetExportedTypes()
				.Where(x => x != null &&
				   x.IsPublic &&
				   x.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) &&
				   !x.IsAbstract &&
				   typeof(IController).IsAssignableFrom(x)).ToList();

			var attributeRoutingMapperType = typeof(RouteCollectionAttributeRoutingExtensions).Assembly.GetType("System.Web.Mvc.Routing.AttributeRoutingMapper");

			var mapAttributeRoutesMethod = attributeRoutingMapperType.GetMethod(
				"MapAttributeRoutes",
				BindingFlags.Public | BindingFlags.Static,
				null,
				new[] { typeof(RouteCollection), typeof(IEnumerable<Type>) },
				null);

			mapAttributeRoutesMethod.Invoke(null, new object[] { routeCollection, controllerTypes });
		}

		public Mock<IStaticDataService> MockStaticDataService(Mock<IStaticDataService> mockStaticDataService)
		{
			var skillLevels = new SkillLevel[] { EntityHelper.CreateSkillLevel("skill level") };
			var skillsFocuses = new SkillsFocus[] { EntityHelper.CreateSkillFocus("skill focus") };
			var trainingLevels = new TrainingLevel[] { EntityHelper.CreateTrainingLevel("training level") };
			var inDemandOccupations = new InDemandOccupation[] { EntityHelper.CreateInDemandOccupation("in demand occupation") };
			var expectedQualifications = new ExpectedQualification[] { EntityHelper.CreateExpectedQualification("expected qualification") };
			var trainingProviderTypes = new TrainingProviderType[] { EntityHelper.CreateTrainingProviderType("Training Provider Type") };
			var provinces = new Region[] { EntityHelper.CreateRegion("BC") };
			var prioritySectors = new PrioritySector[] { EntityHelper.CreatePrioritySector("my priority sector") };
			var rateFormats = new[] { new RateFormat { Rate = 99d, Format = "5 %" } };
			var claimTypes = new[] { EntityHelper.CreateClaimType(ProgramTypes.EmployerGrant) };
			var programTypes = new[] { EntityHelper.CreateProgramType() };
			var fiscalYear = EntityHelper.CreateFiscalYear();
			var trainingPeriod = EntityHelper.CreateTrainingPeriod(DateTime.Today, DateTime.Today.AddMonths(3));
			mockStaticDataService = Mock.Get(Mock.Of<IStaticDataService>(x =>
				x.GetSkillLevels() == skillLevels &&
				x.GetSkillsFocuses() == skillsFocuses &&
				x.GetTrainingLevels() == trainingLevels &&
				x.GetInDemandOccupations() == inDemandOccupations &&
				x.GetExpectedQualifications() == expectedQualifications &&
				x.GetTrainingProviderTypes() == trainingProviderTypes &&
				x.GetProvinces() == provinces &&
				x.GetPrioritySectors() == prioritySectors &&
				x.GetRateFormats() == rateFormats &&
				x.GetClaimTypes() == claimTypes &&
				x.GetProgramTypes() == programTypes &&
				x.GetFiscalYear(It.IsAny<int>()) == fiscalYear &&
				x.GetTrainingPeriod(It.IsAny<int>()) == trainingPeriod &&
				x.GetTrainingPeriodsForFiscalYear(It.IsAny<int>()) == new[] { trainingPeriod }));
			mockStaticDataService.Setup(m => m.GetCountry(It.IsAny<string>())).Returns(new Country("CA", "Canada"));
			mockStaticDataService.Setup(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(new Region("BC", "British Columbia", new Country("CA", "Canada")));

			return mockStaticDataService;
		}

		public Mock<IStaticDataService> MockStaticDataService_Exception(Exception ex)
		{
			var mockStaticDataService = new Mock<IStaticDataService>();
			mockStaticDataService.Setup(x => x.GetApplicantOrganizationTypes()).Throws(ex);
			mockStaticDataService.Setup(x => x.GetParticipantEmploymentStatuses()).Throws(ex);
			mockStaticDataService.Setup(x => x.GetCountry(It.IsAny<string>())).Throws(ex);
			mockStaticDataService.Setup(x => x.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Throws(ex);

			return mockStaticDataService;
		}

		public Mock<IUserService> MockUserService(Mock<IUserService> mockUserService, User user)
		{
			mockUserService = new Mock<IUserService>();
			mockUserService.Setup(x => x.GetUser(It.IsAny<Guid>())).Returns(user);
			mockUserService.Setup(x => x.GetUser(It.IsAny<int>())).Returns(user);
			return mockUserService;
		}

		public Mock<ISiteMinderService> MockSiteMinderService(Mock<ISiteMinderService> mockSiteMinderService, Guid guid)
		{
			mockSiteMinderService = new Mock<ISiteMinderService>();
			mockSiteMinderService.Setup(x => x.CurrentUserGuid).Returns(guid);
			return mockSiteMinderService;
		}

		public Mock<IControllerService> MockControllerService(Mock<IControllerService> mockControllerService, Mock<ISiteMinderService> mockSiteMinderService, Mock<IUserService> mockUserService, Mock<IStaticDataService> mockStaticDataService)
		{
			mockControllerService = new Mock<IControllerService>();
			mockControllerService.Setup(x => x.SiteMinderService).Returns(mockSiteMinderService.Object);
			mockControllerService.Setup(x => x.UserService).Returns(mockUserService.Object);
			mockControllerService.Setup(x => x.StaticDataService).Returns(mockStaticDataService.Object);
			var logger = new Mock<ILogger>();
			mockControllerService.Setup(x => x.Logger).Returns(logger.Object);

			return mockControllerService;
		}

		public Mock<IEligibleCostService> MockEligibleCostService(Mock<IEligibleCostService> mockEligibleCostService, EligibleCost eligibleCost)
		{
			mockEligibleCostService = new Mock<IEligibleCostService>();
			mockEligibleCostService.Setup(x => x.Get(It.IsAny<int>())).Returns(eligibleCost);
			return mockEligibleCostService;
		}

		public Mock<IEligibleCostBreakdownService> MockEligibleCostBreakdownService(Mock<IEligibleCostBreakdownService> mockEligibleCostBreakdownService, EligibleCostBreakdown eligibleCostBreakdown)
		{
			mockEligibleCostBreakdownService = new Mock<IEligibleCostBreakdownService>();
			mockEligibleCostBreakdownService.Setup(x => x.Get(It.IsAny<int>())).Returns(eligibleCostBreakdown);
			return mockEligibleCostBreakdownService;
		}

		public Mock<IEligibleExpenseTypeService> MockEligibleExpenseTypeService(Mock<IEligibleExpenseTypeService> mockEligibleExpenseTypeService, EligibleExpenseType eligibleExpenseType)
		{
			mockEligibleExpenseTypeService = new Mock<IEligibleExpenseTypeService>();
			mockEligibleExpenseTypeService.Setup(x => x.Get(It.IsAny<int>())).Returns(eligibleExpenseType);
			return mockEligibleExpenseTypeService;
		}

		public Mock<IGrantOpeningService> MockGrantOpeningService(Mock<IGrantOpeningService> mockGrantOpeningService, GrantOpening grantOpening)
		{
			mockGrantOpeningService = new Mock<IGrantOpeningService>();
			mockGrantOpeningService.Setup(x => x.Get(It.IsAny<int>())).Returns(grantOpening);
			mockGrantOpeningService.Setup(x => x.GetGrantOpening(It.IsAny<int>(), It.IsAny<int>())).Returns(grantOpening);
			mockGrantOpeningService.Setup(x => x.CheckGrantOpeningByFiscalAndStream(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
			mockGrantOpeningService.Setup(x => x.CanDeleteGrantOpening(It.IsAny<GrantOpening>())).Returns(true);
			return mockGrantOpeningService;
		}

		public Mock<IUserManagerAdapter> MockUserManagerAdapter(Mock<IUserManagerAdapter> mockUserManagerAdapter, ApplicationUser user)
		{
			mockUserManagerAdapter = new Mock<IUserManagerAdapter>();
			mockUserManagerAdapter.Setup(x => x.FindById(It.IsAny<string>())).Returns(user);

			return mockUserManagerAdapter;
		}

		public Mock<IOrganizationService> MockOrganizationService(Mock<IOrganizationService> mockOrganizationService)
		{
			mockOrganizationService = new Mock<IOrganizationService>();
			mockOrganizationService.Setup(x => x.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			return mockOrganizationService;
		}

		public Mock<INationalOccupationalClassificationService> MockNationalOccupationalClassificationService(Mock<INationalOccupationalClassificationService> mockNationalOccupationalClassificationService)
		{
			mockNationalOccupationalClassificationService = new Mock<INationalOccupationalClassificationService>();

			return mockNationalOccupationalClassificationService;
		}

		public Mock<INationalOccupationalClassificationService> MockNationalOccupationalClassificationService_Exception(Exception ex)
		{
			var mockNationalOccupationalClassificationService = new Mock<INationalOccupationalClassificationService>();
			mockNationalOccupationalClassificationService.Setup(x => x.GetNationalOccupationalClassificationChildren(It.IsAny<int>(), It.IsAny<int>())).Throws(ex);
			return mockNationalOccupationalClassificationService;
		}

		public Mock<IVulnerableGroupService> MockVulnerableGroupService(Mock<IVulnerableGroupService> mockVulnerableGroupService)
		{
			mockVulnerableGroupService = new Mock<IVulnerableGroupService>();
			mockVulnerableGroupService.Setup(x => x.GetVulnerableGroups(It.IsAny<int[]>())).Returns(new Mock<IEnumerable<VulnerableGroup>>().Object);
			return mockVulnerableGroupService;
		}

		public Mock<IProgramConfigurationService> MockProgramConfigurationService(Mock<IProgramConfigurationService> mockProgramConfigurationService, ProgramConfiguration programConfiguration)
		{
			mockProgramConfigurationService = new Mock<IProgramConfigurationService>();
			mockProgramConfigurationService.Setup(x => x.Get(It.IsAny<int>())).Returns(programConfiguration);
			mockProgramConfigurationService.Setup(x => x.GetAll()).Returns(new[] { programConfiguration });
			mockProgramConfigurationService.Setup(x => x.GetAll(It.IsAny<bool>())).Returns(new[] { programConfiguration });
			return mockProgramConfigurationService;
		}

		public Mock<ITrainingProviderService> MockTrainingProviderService(Mock<ITrainingProviderService> mockTrainingProviderService, TrainingProvider trainingProvider)
		{
			mockTrainingProviderService = new Mock<ITrainingProviderService>();
			mockTrainingProviderService.Setup(x => x.Get(It.IsAny<int>())).Returns(trainingProvider);
			mockTrainingProviderService.Setup(x => x.ValidateTrainingProvider(It.IsAny<TrainingProvider>(), It.IsAny<int>())).Returns(trainingProvider);
			mockTrainingProviderService.Setup(x => x.Update(It.IsAny<TrainingProvider>())).Returns(trainingProvider);

			return mockTrainingProviderService;
		}

		public Mock<IClaimService> MockClaimService(Mock<IClaimService> mockClaimService, ref Claim claim)
		{
			mockClaimService = new Mock<IClaimService>();
			mockClaimService.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>())).Returns(claim);

			return mockClaimService;
		}

		public Mock<ISettingService> MockSettingService(Mock<ISettingService> mockSettingService, Setting setting)
		{
			mockSettingService = new Mock<ISettingService>();
			mockSettingService.Setup(x => x.Get(It.IsAny<string>())).Returns(setting);

			return mockSettingService;
		}

		public Mock<IEligibleExpenseBreakdownService> MockEligibleExpenseBreakdownService(Mock<IEligibleExpenseBreakdownService> mockEligibleExpenseBreakdownService, List<EligibleExpenseBreakdown> listEligibleExpenseBreakdown)
		{
			mockEligibleExpenseBreakdownService = new Mock<IEligibleExpenseBreakdownService>();
			mockEligibleExpenseBreakdownService.Setup(x => x.GetAllActiveForEligibleExpenseType(It.IsAny<int>())).Returns(listEligibleExpenseBreakdown);
			mockEligibleExpenseBreakdownService.Setup(x => x.Get(It.IsAny<int>())).Returns(listEligibleExpenseBreakdown.First());
			return mockEligibleExpenseBreakdownService;
		}

		public Mock<IGrantAgreementService> MockGrantAgreementService(Mock<IGrantAgreementService> mockGrantAgreementService, GrantApplication grantApplication)
		{
			mockGrantAgreementService = new Mock<IGrantAgreementService>();
			mockGrantAgreementService.Setup(x => x.Get(It.IsAny<int>())).Returns(grantApplication.GrantAgreement);
			mockGrantAgreementService.Setup(x => x.AcceptGrantAgreement(It.IsAny<GrantApplication>()));
			mockGrantAgreementService.Setup(x => x.RejectGrantAgreement(It.IsAny<GrantApplication>(), It.IsAny<string>()));
			mockGrantAgreementService.Setup(x => x.Update(It.IsAny<GrantAgreement>())).Returns(grantApplication.GrantAgreement);
			mockGrantAgreementService.Setup(x => x.CancelGrantAgreement(It.IsAny<GrantApplication>(), It.IsAny<string>()));
			mockGrantAgreementService.Setup(x => x.RejectGrantAgreement(It.IsAny<GrantApplication>(), It.IsAny<string>()));
			mockGrantAgreementService.Setup(x => x.GenerateDocuments(It.IsAny<GrantApplication>()));
			return mockGrantAgreementService;
		}

		public Mock<INoteService> MockNoteService(Mock<INoteService> mockNoteService, Note note)
		{
			mockNoteService = new Mock<INoteService>();
			mockNoteService.Setup(x => x.AddSystemNote(It.IsAny<GrantApplication>(), It.IsAny<string>(), It.IsAny<Attachment>()));

			return mockNoteService;
		}

		public Mock<ITrainingProviderInventoryService> MockTrainingProviderInventoryService(Mock<ITrainingProviderInventoryService> mockTrainingProviderInventoryService, TrainingProviderInventory trainingProviderInventory)
		{
			mockTrainingProviderInventoryService = new Mock<ITrainingProviderInventoryService>();
			mockTrainingProviderInventoryService.Setup(x => x.GetInventory(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null)).Returns(new CJG.Core.Entities.Helpers.PageList<TrainingProviderInventory>() { Items = new[] { trainingProviderInventory } });
			return mockTrainingProviderInventoryService;
		}

		public Mock<IExpenseTypeService> MockExpenseTypeService(Mock<IExpenseTypeService> mockExpenseTypeService, ExpenseType expenseType)
		{
			mockExpenseTypeService = new Mock<IExpenseTypeService>();
			mockExpenseTypeService.Setup(x => x.GetAll()).Returns(new[] { expenseType });
			return mockExpenseTypeService;
		}

		public Mock<IServiceLineService> MockServiceLineService(Mock<IServiceLineService> mockServiceLineService, ServiceLine serviceLine)
		{
			mockServiceLineService = new Mock<IServiceLineService>();
			mockServiceLineService.Setup(x => x.Get(It.IsAny<int>())).Returns(serviceLine);
			return mockServiceLineService;
		}

		public Mock<IServiceLineBreakdownService> MockServiceLineBreakdownService(Mock<IServiceLineBreakdownService> mockServiceLineBreakdownService, List<ServiceLineBreakdown> serviceLineBreakdown)
		{
			mockServiceLineBreakdownService = new Mock<IServiceLineBreakdownService>();
			mockServiceLineBreakdownService.Setup(x => x.GetAllForServiceLine(It.IsAny<int>(), It.IsAny<bool>())).Returns(serviceLineBreakdown);
			return mockServiceLineBreakdownService;
		}

		public Mock<IServiceCategoryService> MockServiceCategoryService(Mock<IServiceCategoryService> mockServiceCategoryService, ServiceCategory serviceCategory)
		{
			mockServiceCategoryService = new Mock<IServiceCategoryService>();
			mockServiceCategoryService.Setup(x => x.Get(It.IsAny<int>())).Returns(serviceCategory);
			mockServiceCategoryService.Setup(x => x.Get(null)).Returns(new ServiceCategory[] { serviceCategory });
			return mockServiceCategoryService;
		}

		public Mock<IAccountCodeService> MockAccountCodeService(Mock<IAccountCodeService> mockAccountCodeService)
		{
			mockAccountCodeService = new Mock<IAccountCodeService>();

			return mockAccountCodeService;
		}

		public Mock<IFiscalYearService> MockFiscalYearService(Mock<IFiscalYearService> mockFiscalYearService)
		{
			mockFiscalYearService = new Mock<IFiscalYearService>();

			return mockFiscalYearService;
		}

		public Mock<IApplicationAddressService> MockApplicationAddressService(Mock<IApplicationAddressService> mockApplicationAddressService)
		{
			mockApplicationAddressService = new Mock<IApplicationAddressService>();
			mockApplicationAddressService.Setup(m => m.VerifyOrCreateRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(new Region("BC", "British Columbia", new Country("CA", "Canada")));
			return mockApplicationAddressService;
		}

		public Mock<ITempDataService> MockTempDataService(Mock<ITempDataService> mockTempDataService)
		{
			mockTempDataService = new Mock<ITempDataService>();

			return mockTempDataService;
		}

		public Mock<IGrantStreamService> MockGrantStreamService(Mock<IGrantStreamService> mockGrantStreamService, GrantStream grantStream)
		{
			mockGrantStreamService = new Mock<IGrantStreamService>();
			mockGrantStreamService.Setup(x => x.Get(It.IsAny<int>())).Returns(grantStream);
			var another = EntityHelper.CreateGrantStream(grantStream.GrantProgram);
			another.Id = 2;
			mockGrantStreamService.Setup(x => x.GetAll()).Returns(new GrantStream[] { grantStream, another });
			mockGrantStreamService.Setup(x => x.GetGrantStreamsForProgram(It.IsAny<int>(), true)).Returns(new[] { grantStream });
			return mockGrantStreamService;
		}

		public Mock<INaIndustryClassificationSystemService> MockNaIndustryClassificationSystemService(Mock<INaIndustryClassificationSystemService> mockNaIndustryClassificationSystemService)
		{
			var naIndustryClassificationSystems = new NaIndustryClassificationSystem[] { new Mock<NaIndustryClassificationSystem>().Object };
			mockNaIndustryClassificationSystemService = new Mock<INaIndustryClassificationSystemService>();
			mockNaIndustryClassificationSystemService.Setup(x => x.GetNaIndustryClassificationSystemParentByLevel(It.IsAny<int>(), It.IsAny<int>())).Returns(new Mock<NaIndustryClassificationSystem>().Object);
			mockNaIndustryClassificationSystemService.Setup(x => x.GetNaIndustryClassificationSystems(It.IsAny<int>())).Returns(naIndustryClassificationSystems);
			return mockNaIndustryClassificationSystemService;
		}

		public Mock<INaIndustryClassificationSystemService> MockNaIndustryClassificationSystemService_Exception(Exception ex)
		{
			var mockNaIndustryClassificationSystemService = new Mock<INaIndustryClassificationSystemService>();
			mockNaIndustryClassificationSystemService.Setup(x => x.GetNaIndustryClassificationSystemParentByLevel(It.IsAny<int>(), It.IsAny<int>())).Throws(ex);
			mockNaIndustryClassificationSystemService.Setup(x => x.GetNaIndustryClassificationSystemChildren(It.IsAny<int>(), It.IsAny<int>())).Throws(ex);
			mockNaIndustryClassificationSystemService.Setup(x => x.GetNaIndustryClassificationSystems(It.IsAny<int>())).Throws(ex);
			return mockNaIndustryClassificationSystemService;
		}

		public Mock<IGrantOpeningManageScheduledService> MockGrantOpeningManageScheduledService(Mock<IGrantOpeningManageScheduledService> mockGrantOpeningManageScheduledService)
		{
			mockGrantOpeningManageScheduledService = new Mock<IGrantOpeningManageScheduledService>();

			return mockGrantOpeningManageScheduledService;
		}

		public Mock<IAttachmentService> MockAttachmentService(Mock<IAttachmentService> mockAttachmentService, Attachment attachment)
		{
			mockAttachmentService = new Mock<IAttachmentService>();
			mockAttachmentService.Setup(x => x.Get(It.IsAny<int>())).Returns(attachment);
			return mockAttachmentService;
		}

		public Mock<IGrantApplicationService> MockGrantApplicationService(Mock<IGrantApplicationService> mockGrantApplicationService, ref GrantApplication grantApplication)
		{
			mockGrantApplicationService = new Mock<IGrantApplicationService>();
			mockGrantApplicationService.Setup(x => x.Get(It.IsAny<int>())).Returns(grantApplication);
			mockGrantApplicationService.Setup(x => x.GetDefaultApplicationType()).Returns(grantApplication.ApplicationType);
			mockGrantApplicationService.Setup(x => x.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null)).Returns(grantApplication);
			var users = new List<User>
			{
				EntityHelper.CreateExternalUser()
			};
			mockGrantApplicationService.Setup(x => x.GetAvailableApplicationContacts(It.IsAny<GrantApplication>())).Returns(users);
			List<CJG.Application.Business.Models.AttachmentModel> listAttachments = new List<Application.Business.Models.AttachmentModel>();
			AttachmentModel attachmentModel = new AttachmentModel
			{
				Description = grantApplication.Attachments.First().Description,
				Id = grantApplication.Attachments.First().Id,
				Name = grantApplication.Attachments.First().FileName,
				RowVersion = System.Convert.ToBase64String(grantApplication.RowVersion),
				Sequence = 1
			};
			listAttachments.Add(attachmentModel);
			mockGrantApplicationService.Setup(x => x.GetAttachments(It.IsAny<int>())).Returns(listAttachments);
			return mockGrantApplicationService;
		}

		public Mock<IGrantApplicationService> MockGrantApplicationService_Exception(ref GrantApplication grantApplication, Exception ex)
		{
			var mockGrantApplicationService = new Mock<IGrantApplicationService>();
			mockGrantApplicationService.Setup(x => x.Get(It.IsAny<int>())).Throws(ex);
			mockGrantApplicationService.Setup(x => x.GetDefaultApplicationType()).Throws(ex);
			mockGrantApplicationService.Setup(x => x.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null)).Throws(ex);
			var users = new List<User>
			{
				EntityHelper.CreateExternalUser()
			};
			mockGrantApplicationService.Setup(x => x.GetAvailableApplicationContacts(It.IsAny<GrantApplication>())).Throws(ex);
			List<CJG.Application.Business.Models.AttachmentModel> listAttachments = new List<Application.Business.Models.AttachmentModel>();
			AttachmentModel attachmentModel = new AttachmentModel
			{
				Description = grantApplication.Attachments.First().Description,
				Id = grantApplication.Attachments.First().Id,
				Name = grantApplication.Attachments.First().FileName,
				RowVersion = System.Convert.ToBase64String(grantApplication.RowVersion),
				Sequence = 1
			};
			listAttachments.Add(attachmentModel);
			mockGrantApplicationService.Setup(x => x.GetAttachments(It.IsAny<int>())).Throws(ex);
			return mockGrantApplicationService;
		}

		public Mock<ITrainingProgramService> MockTrainingProgramService(Mock<ITrainingProgramService> mockTrainingProgramService, TrainingProgram trainingProgram)
		{
			mockTrainingProgramService = new Mock<ITrainingProgramService>();
			mockTrainingProgramService.Setup(x => x.Get(It.IsAny<int>())).Returns(trainingProgram);
			return mockTrainingProgramService;
		}

		public Mock<IUnderRepresentedPopulationService> MockUnderRepresentedPopulationService(Mock<IUnderRepresentedPopulationService> mockUnderRepresentedPopulationService)
		{
			mockUnderRepresentedPopulationService = new Mock<IUnderRepresentedPopulationService>();
			mockUnderRepresentedPopulationService.Setup(x => x.GetUnderRepresentedPopulations(It.IsAny<int[]>())).Returns(new Mock<IEnumerable<UnderRepresentedPopulation>>().Object);
			return mockUnderRepresentedPopulationService;
		}

		public Mock<ICommunityService> MockCommunityService(Mock<ICommunityService> mockCommunityService)
		{
			var communities = new Community[] { EntityHelper.CreateCommunity("New Community") };
			mockCommunityService = new Mock<ICommunityService>();
			mockCommunityService.Setup(x => x.GetAll()).Returns(communities);
			return mockCommunityService;
		}

		public Mock<IGrantProgramService> MockGrantProgramService(Mock<IGrantProgramService> mockGrantProgramService, GrantProgram grantProgram, InternalUser user)
		{
			mockGrantProgramService = new Mock<IGrantProgramService>();
			mockGrantProgramService.Setup(x => x.Get(It.IsAny<int>())).Returns(grantProgram);
			mockGrantProgramService.Setup(x => x.GetAll(null)).Returns(new[] { grantProgram });
			mockGrantProgramService.Setup(x => x.Add(It.IsAny<GrantProgram>())).Returns(grantProgram);
			mockGrantProgramService.Setup(x => x.Update(It.IsAny<GrantProgram>())).Returns(grantProgram);
			mockGrantProgramService.Setup(x => x.GetExpenseAuthorities()).Returns(new[] { user });
			mockGrantProgramService.Setup(x => x.GenerateApplicantDeclarationBody(It.IsAny<GrantApplication>())).Returns("test");
			mockGrantProgramService.Setup(x => x.GetForFiscalYear(It.IsAny<int?>())).Returns(new[] { grantProgram });
			mockGrantProgramService.Setup(x => x.GenerateApplicantCoverLetterBody(It.IsAny<GrantApplication>())).Returns("coverletterbody");
			mockGrantProgramService.Setup(x => x.GenerateAgreementScheduleABody(It.IsAny<GrantApplication>())).Returns("<div></div>");
			return mockGrantProgramService;
		}

		public Mock<IFiscalYearService> MockFiscalYearService(Mock<IFiscalYearService> mockFiscalYearService, FiscalYear fiscalYear)
		{
			mockFiscalYearService = new Mock<IFiscalYearService>();
			mockFiscalYearService.Setup(x => x.GetFiscalYear(It.IsAny<DateTime>())).Returns(fiscalYear);
			return mockFiscalYearService;
		}

		public Mock<IClaimEligibleCostService> MockClaimEligibleCostService(ClaimEligibleCost claimEligibleCost)
		{
			var mockClaimEligibleCostService = new Mock<IClaimEligibleCostService>();
			mockClaimEligibleCostService.Setup(x => x.Get(It.IsAny<int>())).Returns(claimEligibleCost);
			mockClaimEligibleCostService.Setup(x => x.Update(It.IsAny<List<ClaimEligibleCostModel>>(), It.IsAny<bool?>(), It.IsAny<bool?>()));
			return mockClaimEligibleCostService;
		}

		public Mock<IAuthorizationService> MockAuthorizationService()
		{
			var mockAuthorizationService = new Mock<IAuthorizationService>();
			IEnumerable<InternalUser> userObjects = new List<InternalUser>()
			{
				new InternalUser() {Id = 1, LastName = "AAAA", FirstName = "aaaa"},
				new InternalUser() {Id = 2, LastName = "BBBB", FirstName = "BBBB"}
			};

			mockAuthorizationService.Setup(x => x.GetAssessors()).Returns(userObjects);
			return mockAuthorizationService;
		}

		public Mock<IRiskClassificationService> MockRiskClassificationService()
		{
			var mockRiskClassificationService = new Mock<IRiskClassificationService>();
			IEnumerable<RiskClassification> riskClassificationObjects = new List<RiskClassification>()
			{
				new RiskClassification() {Id = 1, Caption = "AAAA"},
				new RiskClassification() {Id = 2, Caption = "BBBB"}
			};

			mockRiskClassificationService.Setup(x => x.GetAll()).Returns(riskClassificationObjects);
			return mockRiskClassificationService;
		}

		public Mock<IDeliveryPartnerService> MockDeliveryPartnerService(Mock<IDeliveryPartnerService> mockDeliveryPartnerService, DeliveryPartner deliveryPartner, CJG.Core.Entities.DeliveryPartnerService deliveryPartnerService)
		{
			mockDeliveryPartnerService = new Mock<IDeliveryPartnerService>();
			mockDeliveryPartnerService.Setup(x => x.GetDeliveryPartner(It.IsAny<int>())).Returns(deliveryPartner);
			mockDeliveryPartnerService.Setup(x => x.GetDeliveryPartnerService(It.IsAny<int>())).Returns(deliveryPartnerService);
			mockDeliveryPartnerService.Setup(x => x.GetDeliveryPartners(It.IsAny<int>())).Returns(new[] { deliveryPartner });
			mockDeliveryPartnerService.Setup(x => x.GetDeliveryPartnerServices(It.IsAny<int>())).Returns(new[] { deliveryPartnerService });
			return mockDeliveryPartnerService;
		}
		#endregion
	}

	public class ControllerInfo
	{
		public Type ControllerType;
		public string ActionName;
		public Type ReturnType;
	}
}
