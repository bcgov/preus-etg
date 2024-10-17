using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class GrantProgramServiceTests : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GetAllGrantPrograms()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var programs = new List<GrantProgram>
			{
				 new GrantProgram
				 {
					 Id = 1,
					 Name = "Program",
					 ProgramCode = "ETG"

				}
			 };
			helper.MockDbSet(programs);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			// Act
			var results = service.GetAll();

			// Assert
			results.Count().Should().Be(1);
		}


		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GetGrantProgram_WithGrantProgramId_ReturnsProgramWithMatchedProgramId()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var programs = new List<GrantProgram>()
			{
				 new GrantProgram
				 {
					 Id = 1,
					 Name = "Program",
					 ProgramCode = "ETG"

				}
			 };
			helper.MockDbSet( programs);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			// Act
			var results = service.Get(1);

			// Assert
			results.Id.Should().Be(1);
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void SaveGrantProgram_WithGrantProgram_ReturnsProgram()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ServiceHelper(typeof(GrantProgramService), user, "Director");
			var programs = new List<GrantProgram>()
			{
				 
			 };

			var templates = new List<DocumentTemplate>()
			{
				new DocumentTemplate
				{
					Id = 10,
					IsActive = true
				}
			};
			var programConfigs = new List<ProgramConfiguration>()
			{
				new ProgramConfiguration()
			};
			helper.MockDbSet( programs);
			helper.MockDbSet(templates);
			helper.MockDbSet(programConfigs);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			helper.GetMock<IStaticDataService>().Setup(m => m.GetClaimTypes()).Returns(new List<ClaimType>() { new ClaimType() });
			var service = helper.Create<GrantProgramService>();

			var programToAdd = new GrantProgram
			{
				Id = 1,
				Name = "Program 1",
				ProgramCode = "PGM"
			};

			// Act
			var results = service.Add(programToAdd);

			// Assert
			results.Name.Should().Be("Program 1");
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void UpdateGrantProgram_WithGrantProgram_ReturnsProgram()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var programs = new List<GrantProgram>
			{
				 new GrantProgram
				 {
					 Id = 1,
					 Name = "Program",
					 ProgramConfiguration = new ProgramConfiguration("SingleAmendableClaim", new ClaimType(ClaimTypes.SingleAmendableClaim))
				}
			 };
			helper.MockDbSet( programs);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			var programToUpdate = programs.First();
			programToUpdate.Name = "Program 1";

			// Act
			var results = service.Update(programToUpdate);

			// Assert
			results.Name.Should().Be("Program 1");
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void ImplementGrantProgram_WithGrantProgram_ReturnsProgram()
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

			var templates = new List<DocumentTemplate>()
			{
				new DocumentTemplate()
				{
					Id =10,
					IsActive = true
				}
			};
			helper.MockDbSet( programs);
			helper.MockDbSet(templates);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			var programToUpdate = new GrantProgram()
			{
				Id = 1
			};

			// Act
			service.Implement(programToUpdate);

			// Assert
			programToUpdate.State.Should().Be(GrantProgramStates.Implemented);
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void TerminateGrantProgram_WithGrantProgram_ReturnsProgram()
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
					 Name = "Program"

				}
			 };

			var grantStreams = new List<GrantStream>()
			{
			  
			};
			helper.MockDbSet( programs);
			helper.MockDbSet(grantStreams);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			var programToTerminate = new GrantProgram()
			{
				Id = 1,
				State = GrantProgramStates.Implemented
			};

			// Act
			service.Terminate(programToTerminate);

			// Assert
			programToTerminate.State.Should().Be(GrantProgramStates.NotImplemented);
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GetReportRates_WithGrantProgram_ReturnsRates()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var rates = new List<ReportRate>()
			{
				new ReportRate()
				{
					GrantProgramId = 1
				}
			};
			helper.MockDbSet(rates);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			// Act
			var results = service.GetReportRates(1);

			// Assert
			results.Count().Should().Be(1);
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GetExpenseAuthorities_ReturnsExpenseAuthorities()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var applicationClaims = new List<ApplicationClaim>()
			{
				new ApplicationClaim()
				{
					ClaimType = "Privilege",
					ClaimValue = "IA1"
				}
			};
			var applicationRoles = new List<ApplicationRole>()
			{
				 new ApplicationRole()
				 {
					 Id = "1",
					 Name = "Assessor",
					 ApplicationClaims = applicationClaims
				}
			 };
			helper.MockDbSet(applicationClaims);
			helper.MockDbSet(applicationRoles);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			// Act
			var results = service.GetExpenseAuthorities();

			// Assert
			results.Count().Should().Be(0);
		}


		#region document templates
		#region applicant declaration 
		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateApplicantDeclarationBody_WithNullApplicationDeclarationTemplate_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{

						}
					}
				}
			};

			// Act
			Action action = () =>  service.GenerateApplicantDeclarationBody(grantApplication);

			// Assert
			action.Should().Throw<InvalidOperationException>().Where(x => x.Message.Contains("declaration"));
		}

		[Ignore, TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateApplicantDeclarationBody_WithApplicationDeclarationTemplate_ReturnsTemplateBody()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				ApplicantPhysicalAddress = new ApplicationAddress(),
				Organization = new Organization(),
				TrainingPrograms = new List<TrainingProgram>() { new TrainingProgram() { Id = 1, GrantApplication = new GrantApplication() { ApplicationStateInternal = ApplicationStateInternal.RecommendedForDenial } } },
				TrainingCost = new TrainingCost() { GrantApplicationId = 1 },
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{
							ApplicantDeclarationTemplate = new DocumentTemplate()
							{
								Body = "EMPTY"
							}
						}
					}
				}
			};

			// Act
			var content = service.GenerateApplicantDeclarationBody(grantApplication);

			// Assert
			content.Should().Be("EMPTY");
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateApplicantDeclaration_WithNullApplication_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();


			// Act
			Action action = () => service.GenerateApplicantDeclaration(null);

			//Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateApplicantDeclaration_WithNullApplicationFileNumber_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{

						}
					}
				}
			};

			// Act
			Action action = () => service.GenerateApplicantDeclaration(grantApplication);

			// Assert
			action.Should().Throw<ArgumentNullException>().Where(x => x.Message.Contains("FileNumber"));
		}

		[Ignore, TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateApplicantDeclaration_WithApplication_ReturnsDocument()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				ApplicantPhysicalAddress = new ApplicationAddress(),
				Organization = new Organization(),
				TrainingPrograms = new List<TrainingProgram>() { new TrainingProgram() { Id = 1, GrantApplication = new GrantApplication() { ApplicationStateInternal = ApplicationStateInternal.RecommendedForDenial } } },
				TrainingCost = new TrainingCost() { GrantApplicationId = 1 },
				FileNumber = "FILE",
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{
							ApplicantDeclarationTemplate = new DocumentTemplate()
							{
							   Body  = "EMPTY"
							}
						}
					}
				}
			};

			// Act
			var content = service.GenerateApplicantDeclaration(grantApplication);

			// Assert
			content.Title.Should().Contain("FILE");
			content.Body.Should().Be("EMPTY");
		}
		#endregion

		#region applicant cover letter
		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateApplicantCoverLetterBody_WithNullApplicationCoverLetterTemplate_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{

						}
					}
				}
			};

			// Act
			Action action = () => service.GenerateApplicantCoverLetterBody(grantApplication);

			// Assert
			action.Should().Throw<InvalidOperationException>().Where(x => x.Message.Contains("letter"));
		}

		[Ignore, TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateApplicantCoverLetterBody_WithApplicationCoverLetterTemplate_ReturnsTemplateBody()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				ApplicantPhysicalAddress = new ApplicationAddress(),
				Organization = new Organization(),
				TrainingPrograms = new List<TrainingProgram>() { new TrainingProgram() { Id = 1, GrantApplication = new GrantApplication() { ApplicationStateInternal = ApplicationStateInternal.RecommendedForDenial } } },
				TrainingCost = new TrainingCost() { GrantApplicationId = 1 },
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{
							ApplicantCoverLetterTemplate = new DocumentTemplate()
							{
								Body = "EMPTY"
							}
						}
					}
				}
			};

			// Act
			var content = service.GenerateApplicantCoverLetterBody(grantApplication);

			// Assert
			content.Should().Be("EMPTY");
		}


		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateApplicantCoverLetter_WithNullApplication_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();


			// Act
			Action action = () => service.GenerateApplicantCoverLetter(null);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateApplicantCoverLetter_WithNullApplicationFileNumber_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{

						}
					}
				}
			};

			// Act
			Action action = () => service.GenerateApplicantCoverLetter(grantApplication);

			// Assert
			action.Should().Throw<ArgumentNullException>().Where(x => x.Message.Contains("FileNumber"));
		}

		[Ignore, TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateApplicantCoverLetter_WithApplication_ReturnsDocument()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				FileNumber = "FILE",
				ApplicantPhysicalAddress = new ApplicationAddress(),
				Organization = new Organization(),
				TrainingPrograms = new List<TrainingProgram>() { new TrainingProgram() { Id = 1, GrantApplication = new GrantApplication() { ApplicationStateInternal = ApplicationStateInternal.RecommendedForDenial } } },
				TrainingCost = new TrainingCost() { GrantApplicationId = 1 },
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{
							ApplicantCoverLetterTemplate = new DocumentTemplate()
							{
								Body = "EMPTY"
							}
						}
					}
				}
			};

			// Act
			var content = service.GenerateApplicantCoverLetter(grantApplication);

			// Assert
			content.Title.Should().Contain("FILE");
			content.Body.Should().Be("EMPTY");
		}
		#endregion

		#region Schedule A
		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateAgreementScheduleABody_WithNullAgreementScheduleATemplate_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{

						}
					}
				}
			};

			// Act
			Action action = () => service.GenerateAgreementScheduleABody(grantApplication);

			// Assert
			action.Should().Throw<InvalidOperationException>().Where(x => x.Message.Contains("schedule A"));
		}

		[Ignore, TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateAgreementScheduleABody_WithAgreementScheduleATemplate_ReturnsTemplateBody()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication() {
				ApplicantPhysicalAddress = new ApplicationAddress(),
				Organization = new Organization(),
				TrainingPrograms = new List<TrainingProgram>() { new TrainingProgram() { Id = 1, GrantApplication =  new GrantApplication() { ApplicationStateInternal = ApplicationStateInternal.RecommendedForDenial } } },
				TrainingCost = new TrainingCost() { GrantApplicationId = 1 },
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{
							ApplicantScheduleATemplate = new DocumentTemplate()
							{
								Body = "EMPTY"
							}
						}
					}
				}
			};

			// Act
			var content = service.GenerateAgreementScheduleABody(grantApplication);

			// Assert
			content.Should().Be("EMPTY");
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateAgreementScheduleA_WithNullApplication_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();


			// Act
			Action action = () => service.GenerateAgreementScheduleA(null);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateAgreementScheduleA_WithNullApplicationFileNumber_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{

						}
					}
				}
			};

			// Act
			Action action = () => service.GenerateAgreementScheduleA(grantApplication);

			// Assert
			action.Should().Throw<ArgumentNullException>().Where(x => x.Message.Contains("FileNumber"));
		}

		[Ignore, TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateAgreementScheduleA_WithApplication_ReturnsDocument()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				FileNumber = "FILE",
				ApplicantPhysicalAddress = new ApplicationAddress(),
				Organization = new Organization(),
				TrainingPrograms = new List<TrainingProgram>() { new TrainingProgram() { Id = 1, GrantApplication = new GrantApplication() { ApplicationStateInternal = ApplicationStateInternal.RecommendedForDenial } } },
				TrainingCost = new TrainingCost() { GrantApplicationId = 1 },
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{
							ApplicantScheduleATemplate = new DocumentTemplate()
							{
								Body = "EMPTY"
							}
						}
					}
				}
			};

			// Act
			var content = service.GenerateAgreementScheduleA(grantApplication);

			// Assert
			content.Title.Should().Contain("FILE");
			content.Body.Should().Be("EMPTY");
		}

		#endregion

		#region Schedule B
		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateAgreementScheduleBBody_WithNullAgreementScheduleBTemplate_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{

						}
					}
				}
			};

			// Act
			Action action = () => service.GenerateAgreementScheduleBBody(grantApplication);

			// Assert
			action.Should().Throw<InvalidOperationException>().Where(x => x.Message.Contains("schedule B"));
		}

		[Ignore, TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateAgreementScheduleBBody_WithAgreementScheduleBTemplate_ReturnsTemplateBody()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				ApplicantPhysicalAddress = new ApplicationAddress(),
				Organization = new Organization(),
				TrainingPrograms = new List<TrainingProgram>() { new TrainingProgram() { Id = 1, GrantApplication = new GrantApplication() { ApplicationStateInternal = ApplicationStateInternal.RecommendedForDenial } } },
				TrainingCost = new TrainingCost() { GrantApplicationId = 1 },
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{
							ApplicantScheduleBTemplate = new DocumentTemplate()
							{
								Body = "EMPTY"
							}
						}
					}
				}
			};

			// Act
			var content = service.GenerateAgreementScheduleBBody(grantApplication);

			// Assert
			content.Should().Be("EMPTY");
		}


		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateAgreementScheduleB_WithNullApplication_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();


			// Act
			Action action = () => service.GenerateAgreementScheduleB(null);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateAgreementScheduleB_WithNullApplicationFileNumber_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{

						}
					}
				}
			};

			// Act
			Action action = () => service.GenerateAgreementScheduleB(grantApplication);

			// Assert
			action.Should().Throw<ArgumentNullException>().Where(x => x.Message.Contains("FileNumber"));
		}

		[Ignore, TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateAgreementScheduleB_WithApplication_ReturnsDocument()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				FileNumber = "FILE",
				ApplicantPhysicalAddress = new ApplicationAddress(),
				Organization = new Organization(),
				TrainingPrograms = new List<TrainingProgram>() { new TrainingProgram() { Id = 1, GrantApplication = new GrantApplication() { ApplicationStateInternal = ApplicationStateInternal.RecommendedForDenial } } },
				TrainingCost = new TrainingCost() { GrantApplicationId = 1 },
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{
							ApplicantScheduleBTemplate = new DocumentTemplate()
							{
								Body = "EMPTY"
							}
						}
					}
				}
			};

			// Act
			var content = service.GenerateAgreementScheduleB(grantApplication);

			// Assert
			content.Title.Should().Contain("FILE");
			content.Body.Should().Be("EMPTY");
		}
		#endregion

		#region Conset 

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateParticipantConsentBody_WithNullParticipantConsentTemplate_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{

						}
					}
				}
			};

			// Act
			Action action = () => service.GenerateParticipantConsentBody(grantApplication);

			// Assert
			action.Should().Throw<InvalidOperationException>().Where(x => x.Message.Contains("consent"));
		}

		[Ignore, TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateParticipantConsentBody_WithParticipantConsentTemplate_ReturnsTemplateBody()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				ApplicantPhysicalAddress = new ApplicationAddress(),
				Organization = new Organization(),
				TrainingPrograms = new List<TrainingProgram>() { new TrainingProgram() { Id = 1, GrantApplication = new GrantApplication() { ApplicationStateInternal = ApplicationStateInternal.RecommendedForDenial } } },
				TrainingCost = new TrainingCost() { GrantApplicationId = 1 },
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{
							ParticipantConsentTemplate = new DocumentTemplate()
							{
								Body = "EMPTY"
							}
						}
					}
				}
			};

			// Act
			var content = service.GenerateParticipantConsentBody(grantApplication);

			// Assert
			content.Should().Be("EMPTY");
		}


		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateParticipantConsent_WithNullApplication_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();


			// Act
			Action action = () => service.GenerateParticipantConsent(null);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateParticipantConsent_WithNullApplicationFileNumber_ThrowsError()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{

						}
					}
				}
			};

			// Act
			Action action = () => service.GenerateParticipantConsent(grantApplication);

			// Assert
			action.Should().Throw<ArgumentNullException>().Where(x => x.Message.Contains("FileNumber"));
		}

		[Ignore, TestMethod]
		[TestCategory("Grant Program"), TestCategory("Service")]
		public void GenerateParticipantConsent_WithApplication_ReturnsDocument()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			GrantApplication grantApplication = new GrantApplication()
			{
				ApplicantPhysicalAddress = new ApplicationAddress(),
				Organization = new Organization(),
				TrainingPrograms = new List<TrainingProgram>() { new TrainingProgram() { Id = 1, GrantApplication = new GrantApplication() { ApplicationStateInternal = ApplicationStateInternal.RecommendedForDenial } } },
				TrainingCost = new TrainingCost() { GrantApplicationId = 1 },
				FileNumber = "FILE",
				GrantOpening = new GrantOpening()
				{
					GrantStream = new GrantStream()
					{
						GrantProgram = new GrantProgram()
						{
							ParticipantConsentTemplate = new DocumentTemplate()
							{
								Body = "EMPTY"
							}
						}
					}
				}
			};

			// Act
			var content = service.GenerateParticipantConsent(grantApplication);

			// Assert
			content.Title.Should().Contain("FILE");
			content.Body.Should().Be("EMPTY");
		}

		#endregion
		#endregion

		[TestCategory("Grant Program"), TestCategory("Service")]
		public void Delete_When_GrantStream_GrantProgramId_Is_Equal_To_GrantProgram_Id_Throw_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);

			var grantProgram = new GrantProgram { Id = 1, AccountCodeId = 1 };
			var accountCode = new List<AccountCode>() { new AccountCode { Id = 1 } };
			var grantStreams = new List<GrantStream>()
			{
				new GrantStream {GrantProgramId = 1 },
				new GrantStream {GrantProgramId = 2 },
			};
			helper.MockDbSet(grantStreams);
			helper.MockDbSet(accountCode);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();

			// Act
			Action action = ()=> service.Delete(grantProgram);

			// Assert
			action.Should().Throw<InvalidOperationException>();

		}

		[TestCategory("Grant Program"), TestCategory("Service")]
		public void Delete_When_GrantProgram_AccountCode_Id_HasValue_And_AccountCode_Exist_Remove()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantProgramService), identity);

			var grantProgram = new GrantProgram { Id = 1, AccountCodeId = 1 };
			var accountCodes = new List<AccountCode>() { new AccountCode { Id = 1 } };
			var grantStreams = new List<GrantStream>()
			{
				new GrantStream {GrantProgramId = 11 },
				new GrantStream {GrantProgramId = 22 },
			};
			helper.MockDbSet(grantStreams);
			var dbContextMock = helper.GetMock<IDataContext>();

			var grantPrograms = new List<GrantProgram>() {
				new GrantProgram { Id = 1, AccountCodeId = 1 },
				new GrantProgram { Id = 2, AccountCodeId = 2 }
			};
			helper.MockDbSet(grantPrograms);
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			helper.SetMock(new Mock<ApplicationUserManager>(userStore.Object));
			var service = helper.Create<GrantProgramService>();
			var accountCodeMock = helper.MockDbSet(accountCodes);

			// Act
			service.Delete(grantProgram);

			// Assert
			accountCodeMock.Verify(x => x.Remove(It.IsAny<AccountCode>()), Times.Exactly(1));
			dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
		}
	}
}