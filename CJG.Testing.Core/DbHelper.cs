using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using System;
using System.Linq;

namespace CJG.Testing.Core
{
	public static class DbHelper
	{
		/// <summary>
		/// Creates and returns a new <typeparamref name="GrantOpening"/> object.
		/// Initializes the <typeparamref name="GrantStream"/>, <typeparamref name="Stream"/>, <typeparamref name="StreamCriteria"/>, <typeparamref name="StreamObjective"/>, <typeparamref name="TrainingPeriod"/>, and <typeparamref name="FiscalYear"/>
		/// </summary>
		/// <returns></returns>
		public static GrantOpening CreateGrantOpening(this IDataContext context, DateTime fiscalYear)
		{
			var guid = Guid.NewGuid();
			var fiscal = fiscalYear.ToUniversalTime();
			var fiscal_year = context.FiscalYears.First(fy => fy.StartDate <= fiscal && fy.EndDate >= fiscal);
			var trainingPeriod = context.TrainingPeriods.OrderBy(tp => tp.StartDate).First(tp => tp.FiscalYearId == fiscal_year.Id);
			var grantOpening = new GrantOpening(
				new GrantStream(
					$"Stream - {guid}",
					$"Stream Criteria - {guid}",
					new GrantProgram(
						"name",
						"abbr",
						ProgramTypes.EmployerGrant,
						new DocumentTemplate(),
						new DocumentTemplate(),
						new DocumentTemplate(),
						new DocumentTemplate(),
						new DocumentTemplate(),
						new InternalUser(),
						new AccountCode()
						))
				{
					MaxReimbursementAmt = 1000,
					ReimbursementRate = 2d / 3d,
					DefaultDeniedRate = 0.051,
					DefaultWithdrawnRate = 0.045,
					DefaultReductionRate = 0.025,
					DefaultSlippageRate = 0.165,
					DefaultCancellationRate = 0.035
				},
				trainingPeriod,
				1000000);

			grantOpening.GrantOpeningIntake = new GrantOpeningIntake(grantOpening);
			grantOpening.GrantOpeningFinancial = new GrantOpeningFinancial(grantOpening);

			return grantOpening;
		}

		/// <summary>
		/// Creates and returns a new <typeparamref name="GrantOpening"/> object.
		/// Initializes the <typeparamref name="GrantStream"/>, <typeparamref name="Stream"/>, <typeparamref name="StreamCriteria"/>, <typeparamref name="StreamObjective"/>, <typeparamref name="TrainingPeriod"/>, and <typeparamref name="FiscalYear"/>
		/// </summary>
		/// <returns></returns>
		public static GrantOpening AddGrantOpening(this IDataContext context, DateTime fiscalYear, DateTime now)
		{
			var grantOpening = context.CreateGrantOpening(fiscalYear);

			AppDateTime.SetNow(now);
			context.GrantOpenings.Add(grantOpening);
			context.SaveChanges();

			return grantOpening;
		}

		public static User AddExternalUser(this IDataContext context)
		{
			return context.AddExternalUser(Guid.NewGuid());
		}

		public static User AddExternalUser(this IDataContext context, Guid guid)
		{
			var user = context.CreateExternalUser(guid);
			context.Users.Add(user);
			context.SaveChanges();
			return user;
		}

		public static User CreateExternalUser(this IDataContext context)
		{
			return context.CreateExternalUser(Guid.NewGuid());
		}

		public static User CreateExternalUser(this IDataContext context, Guid guid)
		{
			var user = EntityHelper.CreateExternalUser(guid);
			user.Organization.LegalStructure = context.LegalStructures.First();
			user.Organization.OrganizationType = context.OrganizationTypes.First();
			user.PhysicalAddress.Region = context.Regions.Find("BC", "CA");
			user.PhysicalAddress.Country = user.PhysicalAddress.Region.Country;
			user.Organization.HeadOfficeAddress.Region = context.Regions.Find("BC", "CA");
			user.Organization.HeadOfficeAddress.Country = user.Organization.HeadOfficeAddress.Region.Country;
			return user;
		}

		public static ApplicationUser CreateInternalUser(this IDataContext context, string idir = null)
		{
			var user = EntityHelper.CreateInternalUser(idir: idir);
			var appUser = new ApplicationUser(user);
			
			return appUser;
		}

		public static GrantApplication CreateGrantApplication(this IDataContext context, GrantOpening grantOpening, User applicationAdministrator)
		{
			var applicationType = context.ApplicationTypes.First(at => at.Caption == "Employer");
			return new GrantApplication(grantOpening, applicationAdministrator, applicationType)
			{
				ApplicationStateInternal = ApplicationStateInternal.Draft,
			   ApplicationStateExternal = ApplicationStateExternal.Incomplete
			};
		}

		public static GrantApplication AddGrantApplication(this IDataContext context, GrantOpening grantOpening, User applicationAdministrator)
		{
			var grantApplication = context.CreateGrantApplication(grantOpening, applicationAdministrator);
			var trainingProvider = context.CreateTrainingProvider(grantApplication);
			context.CreateTrainingProgram(grantApplication, trainingProvider);

			context.GrantApplications.Add(grantApplication);
			context.SaveChanges();
			return grantApplication;
		}

		public static GrantApplication AddGrantApplicationWithCosts(this IDataContext context, GrantOpening grantOpening, User applicationAdministrator)
		{
			var grantApplication = context.CreateGrantApplication(grantOpening, applicationAdministrator);
			var trainingProvider = context.CreateTrainingProvider(grantApplication);
			context.CreateTrainingProgramWithCosts(grantApplication, trainingProvider);

			context.GrantApplications.Add(grantApplication);
			context.SaveChanges();
			return grantApplication;
		}

		public static TrainingProvider CreateTrainingProvider(this IDataContext context, GrantApplication grantApplication)
		{
			var trainingProviderType = context.TrainingProviderTypes.Find(1);
			var region = context.Regions.Find("BC", "CA");
			var trainingProvider = new TrainingProvider(grantApplication)
			{
				TrainingProviderState = TrainingProviderStates.Complete,
				Name = "Training Provider",
				ContactFirstName = "First Name",
				ContactLastName = "Last Name",
				ContactEmail = "contact@email.com",
				ContactPhoneNumber = "(123) 123-1235",
				TrainingAddress = new ApplicationAddress(new Address("1224 St", null, "Victoria", "V9C0E4", region)),
				TrainingOutsideBC = false,
				TrainingProviderType = trainingProviderType
			};

			return trainingProvider;
		}

		public static TrainingProgram CreateTrainingProgram(this IDataContext context, GrantApplication grantApplication, TrainingProvider trainingProvider, int estimatedParticipants = 5)
		{
			var deliveryMethod = context.DeliveryMethods.First();
			var skillFocus = context.SkillsFocuses.First();
			var skillLevel = context.SkillLevels.First();
			var expectedQualifications = context.ExpectedQualifications.First();
			return new TrainingProgram(grantApplication, trainingProvider)
			{
				CourseTitle = "Course Title",
				StartDate = trainingProvider.GrantApplication.GrantOpening.TrainingPeriod.StartDate,
				EndDate = trainingProvider.GrantApplication.GrantOpening.ClosingDate,
				TrainingProgramState = TrainingProgramStates.Complete,
				TotalTrainingHours = 40,
				ExpectedQualification = expectedQualifications,
				SkillFocus = skillFocus,
				SkillLevel = skillLevel,                
				TitleOfQualification = "Title",
				DeliveryMethods = new [] { deliveryMethod }
			};
		}

		public static TrainingProgram CreateTrainingProgramWithCosts(this IDataContext context, GrantApplication grantApplication, TrainingProvider trainingProvider, int estimatedParticipants = 5)
		{
			var trainingProgram = context.CreateTrainingProgram(grantApplication, trainingProvider, estimatedParticipants);
			trainingProgram.GrantApplication.TrainingCost.TrainingCostState = TrainingCostStates.Complete;
			var expenseType = context.EligibleExpenseTypes.First();
			trainingProgram.GrantApplication.TrainingCost.EligibleCosts.Add(new EligibleCost(trainingProgram.GrantApplication, expenseType, 1000, 5));
			trainingProgram.GrantApplication.RecalculateEstimatedCosts();

			return trainingProgram;
		}

		public static GrantAgreement CreateGrantAgreement(this IDataContext context, GrantApplication grantApplication)
		{
			var grantAgreement = new GrantAgreement(grantApplication);
			return grantAgreement;
		}

		public static void SetFileNumber(this GrantApplication grantApplication, IDataContext context)
		{
			using (var trans = context.Database.BeginTransaction())
			{
				try
				{
					// need to get the next sequential number for the fiscal year in order to create the agreement number
					var fiscalYear = context.FiscalYears.First(x => x.Id == grantApplication.GrantOpening.TrainingPeriod.FiscalYearId);
					var lastNumber = fiscalYear.NextAgreementNumber++;
					grantApplication.FileNumber = $"{fiscalYear.EndDate:yy}{lastNumber:d5}";
					context.SaveChanges();
					trans.Commit();
				}
				catch (Exception)
				{
					trans.Rollback();
					throw;
				}
			}
		}

		public static void Schedule(this GrantOpening grantOpening, IDataContext context)
		{
			grantOpening.State = GrantOpeningStates.Scheduled;
			context.SaveChanges();
		}

		public static void Publish(this GrantOpening grantOpening, IDataContext context)
		{
			grantOpening.State = GrantOpeningStates.Published;
			context.SaveChanges();
		}

		public static void Open(this GrantOpening grantOpening, IDataContext context)
		{
			grantOpening.State = GrantOpeningStates.Open;
			context.SaveChanges();
		}

		public static void Complete(this GrantApplication grantApplication, IDataContext context)
		{
			grantApplication.ApplicationStateExternal = ApplicationStateExternal.Complete;
			grantApplication.TrainingCost.TrainingCostState = TrainingCostStates.Complete;
			grantApplication.TrainingPrograms.FirstOrDefault().TrainingProgramState = TrainingProgramStates.Complete;
			grantApplication.TrainingPrograms.FirstOrDefault().TrainingProvider.TrainingProviderState = TrainingProviderStates.Complete;
			context.SaveChanges();
		}

		public static void Submit(this GrantApplication grantApplication, IDataContext context)
		{
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.New;
			grantApplication.ApplicationStateExternal = grantApplication.ApplicationStateInternal.GetExternalState();
			grantApplication.CopyEstimatedIntoAgreed();
			grantApplication.DateSubmitted = AppDateTime.UtcNow;
			grantApplication.SetFileNumber(context);
			context.SaveChanges();
		}

		public static void SelectForAssessment(this GrantApplication grantApplication, IDataContext context)
		{
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.PendingAssessment;
			grantApplication.ApplicationStateExternal = grantApplication.ApplicationStateInternal.GetExternalState();
			context.SaveChanges();
		}

		public static void BeginAssessment(this GrantApplication grantApplication, IDataContext context, InternalUser assessor)
		{
			grantApplication.AssessorId = assessor?.Id;
			grantApplication.Assessor = assessor;
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.UnderAssessment;
			grantApplication.ApplicationStateExternal = grantApplication.ApplicationStateInternal.GetExternalState();
			context.SaveChanges();
		}

		public static void RecommendForApproval(this GrantApplication grantApplication, IDataContext context)
		{
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.RecommendedForApproval;
			grantApplication.ApplicationStateExternal = grantApplication.ApplicationStateInternal.GetExternalState();
			context.SaveChanges();
		}

		public static void IssueOffer(this GrantApplication grantApplication, IDataContext context)
		{
			context.CreateGrantAgreement(grantApplication);
			context.SaveChanges();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.OfferIssued;
			grantApplication.ApplicationStateExternal = grantApplication.ApplicationStateInternal.GetExternalState();
			context.SaveChanges();
		}

		public static void AcceptAgreement(this GrantApplication grantApplication, IDataContext context)
		{
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.AgreementAccepted;
			grantApplication.ApplicationStateExternal = grantApplication.ApplicationStateInternal.GetExternalState();
			grantApplication.GrantAgreement.DateAccepted = AppDateTime.UtcNow;
			grantApplication.GrantAgreement.CoverLetterConfirmed = true;
			grantApplication.GrantAgreement.ScheduleAConfirmed = true;
			grantApplication.GrantAgreement.ScheduleBConfirmed = true;
			grantApplication.GrantAgreement.StartDate = grantApplication.TrainingPrograms.FirstOrDefault().StartDate;
			grantApplication.GrantAgreement.EndDate = grantApplication.TrainingPrograms.FirstOrDefault().EndDate;
			context.SaveChanges();
		}
	}
}
