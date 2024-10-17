using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using System;
using System.Linq;

namespace CJG.Application.Services.Helpers
{
	static class TestService
	{
		#region Methods
		public static GrantApplication CreateTestApplication(this IDataContext context, GrantProgram grantProgram)
		{
			var grantStream = grantProgram.GrantStreams.FirstOrDefault() ?? new GrantStream("Test Stream", "Stream Objective", grantProgram);
			var grantOpening = grantStream.GrantOpenings.FirstOrDefault() ?? new GrantOpening(grantStream, context.TrainingPeriods.First(), 150000);
			var grantApplication = new GrantApplication(grantOpening, new User(Guid.NewGuid(), "John", "Doe", "john.doe@gov.bc.ca", new Organization(new OrganizationType("test"), Guid.NewGuid(), "Legal Name Inc.", new LegalStructure("Legal Structure"), 2000, 20, 20, 2000, 20), new Address("1234 Test St", "", "Victoria", "V9C9C9", "BC", "CA")), new ApplicationType("Test"))
			{
				Id = 1,
				StartDate = AppDateTime.UtcNow.ToUtcMorning(),
				EndDate = AppDateTime.UtcNow.AddDays(5).ToUtcMidnight()
			};
			grantApplication.TrainingCost = new TrainingCost(grantApplication, 5);

			if (grantProgram.ProgramTypeId == ProgramTypes.EmployerGrant)
			{
				context.CreateTestTrainingProgram(grantApplication);
				grantApplication.TrainingCost.EligibleCosts.Add(new EligibleCost(grantApplication, grantProgram.ProgramConfiguration.EligibleExpenseTypes.FirstOrDefault(), 5000.99M, 5));
				grantApplication.TrainingCost.EligibleCosts.Add(new EligibleCost(grantApplication, grantProgram.ProgramConfiguration.EligibleExpenseTypes.LastOrDefault(), 3000.33M, 3));
			}
			else if (grantProgram.ProgramTypeId == ProgramTypes.WDAService)
			{
				foreach (var expenseType in grantProgram.ProgramConfiguration.EligibleExpenseTypes.Where(eet => eet.IsActive))
				{
					switch (expenseType.ServiceCategory.ServiceTypeId)
					{
						case (ServiceTypes.SkillsTraining):
							var skillCost = new EligibleCost(grantApplication.TrainingCost, expenseType, 8998.33M, 5);
							var breakdown = new EligibleCostBreakdown(skillCost, expenseType.Breakdowns.First(), 8998.33M);
							var program = context.CreateTestTrainingProgram(grantApplication, breakdown);
							breakdown.TrainingPrograms.Add(program);
							grantApplication.TrainingCost.EligibleCosts.Add(skillCost);
							break;
						case (ServiceTypes.EmploymentServicesAndSupports):
							var serviceCost = new EligibleCost(grantApplication.TrainingCost, expenseType, 2000.33M, 5);
							var provider = context.CreateTestTrainingProvider(grantApplication, serviceCost);
							grantApplication.TrainingCost.EligibleCosts.Add(serviceCost);
							break;
						case (ServiceTypes.Administration):
							grantApplication.TrainingCost.EligibleCosts.Add(new EligibleCost(grantApplication.TrainingCost, expenseType, 500.33M, 5));
							break;
					}
				}
			}

			grantApplication.RecalculateEstimatedCosts();
			grantApplication.GrantAgreement = new GrantAgreement(grantApplication);
			grantApplication.CopyEstimatedIntoAgreed();

			return grantApplication;
		}

		public static TrainingProgram CreateTestTrainingProgram(this IDataContext context, GrantApplication grantApplication, EligibleCostBreakdown eligibleCostBreakdown = null)
		{
			var trainingProgram = new TrainingProgram(grantApplication)
			{
				Id = 1,
				TrainingProgramState = TrainingProgramStates.Complete,
				StartDate = grantApplication.StartDate,
				EndDate = grantApplication.EndDate,
				CourseTitle = "Course Title",
				DeliveryMethods = context.DeliveryMethods.Take(2).ToArray(),
				UnderRepresentedGroups = context.UnderRepresentedGroups.Take(2).ToArray(),
				ExpectedQualification = context.ExpectedQualifications.FirstOrDefault(),
				InDemandOccupation = context.InDemandOccupations.FirstOrDefault(),
				SkillFocus = context.SkillsFocuses.FirstOrDefault(),
				SkillLevel = context.SkillLevels.FirstOrDefault(),
				TitleOfQualification = "Title of Qualification",
				TrainingLevel = context.TrainingLevels.FirstOrDefault(),
				TotalTrainingHours = 40,
				ServiceLine = eligibleCostBreakdown?.EligibleExpenseBreakdown?.ServiceLine,
				ServiceLineBreakdown = eligibleCostBreakdown?.EligibleExpenseBreakdown?.ServiceLine?.ServiceLineBreakdowns?.FirstOrDefault(),
				EligibleCostBreakdown = eligibleCostBreakdown
			};

			var trainingProvider = new TrainingProvider(trainingProgram)
			{
				Id = 1,
				TrainingProviderState = TrainingProviderStates.Complete,
				ContactEmail = "test@test.com",
				ContactFirstName = "John",
				ContactLastName = "Doe",
				ContactPhoneNumber = "(123) 123-1234",
				BusinessCase = "Business Case",
				Name = "Training Provider Name",
				TrainingAddress = new ApplicationAddress(new Address("1234 St", "", "Victoria", "V9V9V9", context.Regions.FirstOrDefault(r => r.Id == "BC"))),
				TrainingProviderInventory = context.TrainingProviderInventory.First(),
				TrainingProviderType = context.TrainingProviderTypes.First()
			};

			return trainingProgram;
		}

		public static TrainingProvider CreateTestTrainingProvider(this IDataContext context, GrantApplication grantApplication, EligibleCost eligibleCost = null)
		{
			var trainingProvider = new TrainingProvider(grantApplication)
			{
				Id = 2,
				TrainingProviderState = TrainingProviderStates.Complete,
				ContactEmail = "test@test.com",
				ContactFirstName = "John",
				ContactLastName = "Doe",
				ContactPhoneNumber = "(123) 123-1234",
				BusinessCase = "Business Case",
				Name = "Training Provider Name",
				TrainingAddress = new ApplicationAddress(new Address("1234 St", "", "Victoria", "V9V9V9", context.Regions.FirstOrDefault(r => r.Id == "BC"))),
				TrainingProviderInventory = context.TrainingProviderInventory.First(),
				TrainingProviderType = context.TrainingProviderTypes.First(),
				EligibleCost = eligibleCost
			};

			return trainingProvider;
		}
		#endregion
	}
}
