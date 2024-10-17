using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	/// <summary>
	/// Summary description for CompletionReportServiceTest
	/// </summary>
	[TestClass]
	public class CompletionReportServiceTest : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod]
		public void ClosedGrantApplicationReturnsSubmittedCompletionReportStatus()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(CompletionReportService), applicationAdministrator);
			var service = helper.Create<CompletionReportService>();
			var forms = new List<ParticipantForm>() { new ParticipantForm() { Id = 3 } };
			var grantApplication = new GrantApplication {
				Id = 2,
				ApplicationStateExternal = ApplicationStateExternal.Closed,
				ParticipantForms = forms
			};
			var trainingProgram = new TrainingProgram(grantApplication)
			{
				Id = 1
			};
			
			helper.MockDbSet(grantApplication);
			helper.MockDbSet(trainingProgram);

			// Act
			var completionReportStatus = service.GetCompletionReportStatus(grantApplication.Id);

			// Assert
			completionReportStatus.Should().Be("Submitted");
		}

		[TestMethod]
		public void ZeroParticipantEnrollmentsForReportReturnsNotStartedCompletionReportStatus()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(CompletionReportService), applicationAdministrator);
			var service = helper.Create<CompletionReportService>();
			var grantApplication = new GrantApplication
			{
				Id = 1,
				ApplicationStateExternal = ApplicationStateExternal.Approved
			};
			var trainingProgram = new TrainingProgram(grantApplication)
			{
				Id = 1
			};
			helper.MockDbSet(grantApplication);
			helper.MockDbSet(trainingProgram);

			// Act
			var completionReportStatus = service.GetCompletionReportStatus(grantApplication.Id);

			// Assert
			completionReportStatus.Should().Be("Not Started");
		}

		[TestMethod]
		public void ParticipantsAnswersRecordedReturnsIncompleteCompletionReportStatus()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(CompletionReportService), applicationAdministrator);
			var grantApplication = new GrantApplication {
				Id = 1,
				ApplicationStateExternal = ApplicationStateExternal.Approved,
				ParticipantForms = new List<ParticipantForm>
				{
					new ParticipantForm
					{
						Id = 1
					}
				},
				GrantOpening = new GrantOpening() { GrantStream = new GrantStream() { GrantProgram = new GrantProgram() { ProgramType = new ProgramType() { Id = ProgramTypes.EmployerGrant } } } },
				CompletionReportId = 1
			};
			var trainingProgram = new TrainingProgram(grantApplication)
			{
				Id = 1
			};
			var participantAnswers = new List<ParticipantCompletionReportAnswer> { new ParticipantCompletionReportAnswer { AnswerId = 1, QuestionId = 1, ParticipantFormId = 1 } };
			var employerAnswers = new List<EmployerCompletionReportAnswer> { new EmployerCompletionReportAnswer { GrantApplicationId = 1 } };
			var questions = new List<CompletionReportQuestion> { new CompletionReportQuestion { Id = 1, CompletionReportId = 1, GroupId = 0, IsActive = true } };
			var report = new CompletionReport { Id = 1 };
			var reports = new List<CompletionReport> { new CompletionReport { Id = 1 } };
			var group = new CompletionReportGroup() { CompletionReportId = 1, CompletionReport = report, ProgramTypeId = ProgramTypes.EmployerGrant, RowSequence = 1 };
			var participantForms = new List<ParticipantForm>();
			participantForms.Add(new ParticipantForm { Id = 1 });

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.MockDbSet(grantApplication);
			helper.MockDbSet(trainingProgram);
			var groupDbSet = helper.MockDbSet(group);
			var answersDbSet = helper.MockDbSet(participantAnswers);
			var answersEmpDbSet = helper.MockDbSet(employerAnswers);
			var questionsDbSet = helper.MockDbSet(questions);
			var reportDbSet = helper.MockDbSet(report);
			helper.MockDbSet(reports);

			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.ParticipantCompletionReportAnswers.AsNoTracking()).Returns(answersDbSet.Object);
			dbContextMock.Setup(x => x.CompletionReports.AsNoTracking()).Returns(reportDbSet.Object);
			dbContextMock.Setup(x => x.EmployerCompletionReportAnswers.AsNoTracking()).Returns(answersEmpDbSet.Object);
			dbContextMock.Setup(x => x.CompletionReportQuestions.AsNoTracking()).Returns(questionsDbSet.Object);
			dbContextMock.Setup(x => x.CompletionReportGroups).Returns(groupDbSet.Object);
			dbContextMock.Setup(x => x.CompletionReportGroups.AsNoTracking()).Returns(groupDbSet.Object);
			var service = helper.SetMockAs<CompletionReportService, ICompletionReportService>();

			// Act
			var completionReportStatus = service.Object.GetCompletionReportStatus(grantApplication.Id);

			// Assert
			completionReportStatus.Should().Be("Complete");
		}

		[TestMethod]
		public void EmployerAnswersRecordedReturnsCompleteCompletionReportStatus()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(CompletionReportService), applicationAdministrator);
			var report = new CompletionReport { Id = 1 };
			var reports = new List<CompletionReport> { new CompletionReport { Id = 1 } };
			var group = new CompletionReportGroup() { CompletionReportId = 1, CompletionReport = report, ProgramTypeId = ProgramTypes.EmployerGrant, RowSequence = 1 };
			var grantApplication = new GrantApplication {
				Id = 1,
				ApplicationStateExternal = ApplicationStateExternal.Approved,
				ParticipantForms = new List<ParticipantForm>
				{
					new ParticipantForm {
						Id = 1
					}
				},
				GrantOpening = new GrantOpening() { GrantStream = new GrantStream() { GrantProgram = new GrantProgram() { ProgramType = new ProgramType() { Id = ProgramTypes.EmployerGrant } } } },
				CompletionReport = report,
				CompletionReportId = 1
			};
			var trainingProgram = new TrainingProgram(grantApplication)
			{
				Id = 1
			};
			var participantAnswers = new List<ParticipantCompletionReportAnswer> { new ParticipantCompletionReportAnswer { AnswerId = 1, QuestionId= 1, ParticipantFormId = 1 } };
			var employerAnswers = new List<EmployerCompletionReportAnswer>();
			var questions = new List<CompletionReportQuestion> { new CompletionReportQuestion { Id = 1, CompletionReportId = 1, GroupId = 0, IsActive = true } };
			var participantEnrollments = new List<ParticipantForm>();
			participantEnrollments.Add(new ParticipantForm { Id = 1 });

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.MockDbSet(grantApplication);
			helper.MockDbSet(trainingProgram);
			var groupDbSet = helper.MockDbSet(group);
			var answersDbSet = helper.MockDbSet(participantAnswers);
			var answersEmpDbSet = helper.MockDbSet(employerAnswers);
			var questionsDbSet = helper.MockDbSet(questions);
			var reportDbSet = helper.MockDbSet(report);
			helper.MockDbSet(reports);

			var dbContextMock = helper.GetMock<IDataContext>();

			dbContextMock.Setup(x => x.ParticipantCompletionReportAnswers.AsNoTracking()).Returns(answersDbSet.Object);
			dbContextMock.Setup(x => x.CompletionReports.AsNoTracking()).Returns(reportDbSet.Object);
			dbContextMock.Setup(x => x.CompletionReportGroups).Returns(groupDbSet.Object);
			dbContextMock.Setup(x => x.CompletionReportGroups.AsNoTracking()).Returns(groupDbSet.Object);
			dbContextMock.Setup(x => x.EmployerCompletionReportAnswers.AsNoTracking()).Returns(answersEmpDbSet.Object);
			dbContextMock.Setup(x => x.CompletionReportQuestions.AsNoTracking()).Returns(questionsDbSet.Object);

			var service = helper.SetMockAs<CompletionReportService, ICompletionReportService>();

			// Act
			var completionReportStatus = service.Object.GetCompletionReportStatus(grantApplication.Id);

			// Assert
			completionReportStatus.Should().Be("Complete");
		}

		[TestMethod]
		public void GetEmployerParticipantEnrollmentsWhenNoClaimExists()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(CompletionReportService), applicationAdministrator);

			var employerParticipantForm = new ParticipantForm { Id = 1 };

			var grantApplication = new GrantApplication
			{
				Id = 1,
				ApplicationStateExternal = ApplicationStateExternal.Approved,
				ParticipantForms = new List<ParticipantForm>
				{
					employerParticipantForm
				}
			};

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.MockDbSet( new[] { grantApplication });

			var service = helper.SetMockAs<CompletionReportService, ICompletionReportService>();

			// Act
			var employerParticipantEnrollments = service.Object.GetParticipantsForReport(grantApplication.Id);

			// Assert
			employerParticipantEnrollments.Should().Contain(employerParticipantForm);
		}

		[TestMethod]
		public void GetEligibleCostParticipantsWhenClaimExists()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(CompletionReportService), applicationAdministrator);

			var claimParticipantForm = new ParticipantForm { Id = 1 };
			var employerParticipantForm = new ParticipantForm { Id = 2 };

			var grantApplication = new GrantApplication
			{
				Id = 1,
				ApplicationStateExternal = ApplicationStateExternal.Approved,
				ParticipantForms = new List<ParticipantForm>
				{
					employerParticipantForm
				},
				Claims = new List<Claim>
				{
					new Claim {
						EligibleCosts = new List<ClaimEligibleCost> {
							new ClaimEligibleCost
							{
								ParticipantCosts = new List<ParticipantCost>
								{
									new ParticipantCost
									{
										ParticipantForm = claimParticipantForm
									}
								}
							}
						}
					}
				}
			};

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.MockDbSet( new[] { grantApplication });

			var service = helper.SetMockAs<CompletionReportService, ICompletionReportService>();

			// Act
			var participants = service.Object.GetParticipantsForReport(grantApplication.Id);

			// Assert
			participants.Should().Contain(claimParticipantForm);
			participants.Should().NotContain(employerParticipantForm);
		}

		[TestMethod]
		public void GetQuestionsForStep()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(CompletionReportService), applicationAdministrator);
			var firstQuestion = new CompletionReportQuestion
			{
				Id = 1,
				GroupId = 1,
				CompletionReportId = 1,
				IsActive = true,
				Sequence = 1
			};
			var secondQuestion = new CompletionReportQuestion
			{
				Id = 2,
				GroupId = 1,
				CompletionReportId = 1,
				IsActive = true,
				Sequence = 2
			};
			var questions = new List<CompletionReportQuestion>
			{
				firstQuestion,
				secondQuestion
			};
			var questionsDbSet = helper.MockDbSet(questions);
			var dbContextMock = helper.GetMock<IDataContext>();

			dbContextMock.Setup(x => x.CompletionReportQuestions.AsNoTracking()).Returns(questionsDbSet.Object);

			var service = helper.SetMockAs<CompletionReportService, ICompletionReportService>();

			// Act
			var completionReportQuestionsForStep = service.Object.GetCompletionReportQuestionsForStep(1, 1);

			// Assert
			completionReportQuestionsForStep.First().Sequence.Should().Be(1);
			completionReportQuestionsForStep.Should().Contain(secondQuestion);
		}

		[TestMethod]
		public void CheckAllParticipantsHaveCompletedReport()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(CompletionReportService), applicationAdministrator);
			var participantForm = new ParticipantForm() { Id = 1 };
			var participantAnswers = new List<ParticipantCompletionReportAnswer>
			{
				new ParticipantCompletionReportAnswer
				{
					GrantApplicationId = 1,
					ParticipantForm = participantForm,
					QuestionId = 1,
					ParticipantFormId = 1
				},
				new ParticipantCompletionReportAnswer
				{
					GrantApplicationId = 2,
					ParticipantForm = participantForm,
					QuestionId = 1,
					ParticipantFormId = 2
				},
				new ParticipantCompletionReportAnswer
				{
					GrantApplicationId = 3,
					ParticipantForm = participantForm,
					QuestionId = 1,
					ParticipantFormId = 3
				},
				new ParticipantCompletionReportAnswer
				{
					GrantApplicationId = 4,
					ParticipantForm = participantForm,
					QuestionId = 1,
					ParticipantFormId = 4
				}
			};
			var firstQuestion = new CompletionReportQuestion
			{
				Id = 1,
				GroupId = 1,
				CompletionReportId = 1,
				IsActive = true,
				Sequence = 1
			};
			var questions = new List<CompletionReportQuestion>
			{
				firstQuestion
			};
			var questionsDbSet = helper.MockDbSet(questions);
			var answersDbSet = helper.MockDbSet(participantAnswers);
			var dbContextMock = helper.GetMock<IDataContext>();

			dbContextMock.Setup(x => x.CompletionReportQuestions.AsNoTracking()).Returns(questionsDbSet.Object);
			dbContextMock.Setup(x => x.ParticipantCompletionReportAnswers.AsNoTracking()).Returns(answersDbSet.Object);

			var service = helper.SetMockAs<CompletionReportService, ICompletionReportService>();

			// Act
			var allParticipantsHaveCompletedReport = service.Object.AllParticipantsHaveCompletedReport(new int[4] { 1, 2, 3, 4 }, 1, 1);

			// Assert
			allParticipantsHaveCompletedReport.Should().Be(true);
		}
	}
}
