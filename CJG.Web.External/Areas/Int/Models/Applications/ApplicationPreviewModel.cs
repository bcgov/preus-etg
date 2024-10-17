using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using DeliveryPartnerService = CJG.Core.Entities.DeliveryPartnerService;

namespace CJG.Web.External.Areas.Int.Models.Applications
{
	public class ApplicationPreviewModel : BaseViewModel
	{
		public int GrantProgramId { get; set; }
		public ApplicationStateInternal ApplicationStateInternal { get; set; }

		public struct TestEntities
		{
			public GrantApplication GrantApplication;
			public GrantOpening GrantOpening;
			public GrantProgram GrantProgram;
			public FiscalYear FiscalYear;
			public User Applicant;
			public Organization Organization;
			public TrainingProgram TrainingProgram;
			public TrainingProvider TrainingProvider;
		}

		public TestEntities GenerateTestEntities(IPrincipal user, IUserService userService, IGrantProgramService grantProgramService, IFiscalYearService fiscalYearService)
		{
			var now = AppDateTime.UtcNow;
			var internalUser = userService.GetInternalUser(user.GetUserId().Value);
			var grantProgram = grantProgramService.Get(GrantProgramId);
			var fiscalYear = fiscalYearService.GetFiscalYear(now);
			var applicantOrganization = new Organization(new OrganizationType("Business"), Guid.NewGuid(), "Organization Name", new LegalStructure("Legal Structure"), 2000, 50, 25, 5000, 5)
			{
				HeadOfficeAddress = new Address("1234 Street", "", "Vancouver", "V9V9V9", new Region("BC", "British Columbia", new Country("CA", "Canada")))
			};

			var applicant = new User(Guid.NewGuid(), internalUser.FirstName, internalUser.LastName, internalUser.Email, applicantOrganization, applicantOrganization.HeadOfficeAddress);
			var grantOpening = new GrantOpening(new GrantStream("Test Grant Stream", "Test Objective", grantProgram), new TrainingPeriod(fiscalYear, "Test Training Period", now, now.AddMonths(6), now, now), 1000);
			var applicationType = new ApplicationType("Test Application Type");

			var grantApplication = new GrantApplication(grantOpening, applicant, applicationType)
			{
				Id = 1,
				ApplicationStateInternal = ApplicationStateInternal,
				ApplicationStateExternal = ApplicationStateInternal.GetExternalState(),
				FileNumber = $"{fiscalYear.EndDate:yy}{++fiscalYear.NextAgreementNumber:d5}",
				StartDate = now,
				EndDate = now.AddDays(10),
				MaxReimbursementAmt = 10000,
				ReimbursementRate = 2D / 3D,
				PrioritySector = new PrioritySector("Priority Sector"),
				UsedDeliveryPartner = true,
				DeliveryPartner = new DeliveryPartner(grantProgram, "Delivery Partner"),
				DeliveryPartnerServices = new[] { new DeliveryPartnerService(grantProgram, "Delivery Partner Service") },
				PrioritizationScoreBreakdown = new PrioritizationScoreBreakdown
				{
					FirstTimeApplicantScore = 1,
					SmallBusinessScore = 1,
					IndustryScore = 0,
					RegionalScore = 3
				}
			};

			grantApplication.TrainingCost = new TrainingCost(grantApplication, 10);

			var trainingProgram = new TrainingProgram(grantApplication)
			{
				Id = 1,
				CourseTitle = "Test Training Program",
				StartDate = now,
				EndDate = now.AddDays(10),
				DeliveryMethods = new[] { new DeliveryMethod("Delivery Method") },
				InDemandOccupation = new InDemandOccupation("In Demand Occupation"),
				SkillFocus = new SkillsFocus("Skill Focus"),
				SkillLevel = new SkillLevel("Skill Level"),
				TotalTrainingHours = 40,
				TrainingLevel = new TrainingLevel("Training Level"),
				TrainingProgramState = TrainingProgramStates.Complete
			};

			var trainingProvider = new TrainingProvider(trainingProgram)
			{
				Id = 1,
				Name = "Training Provider Name",
				ContactEmail = "provider@test.com",
				ContactFirstName = "John",
				ContactLastName = "Doe",
				ContactPhoneNumber = "(123) 123-1232",
				TrainingAddress = new ApplicationAddress(applicant.MailingAddress),
				TrainingProviderState = TrainingProviderStates.Complete,
				TrainingProviderType = new TrainingProviderType("Training Provider Type")
			};

			switch (grantProgram.ProgramTypeId)
			{
				case (ProgramTypes.WDAService):
					var provider = new TrainingProvider(grantApplication)
					{
						Id = 2,
						Name = "Service Provider Name",
						ContactEmail = "provider@test.com",
						ContactFirstName = "John",
						ContactLastName = "Doe",
						ContactPhoneNumber = "(123) 123-1232",
						TrainingAddress = new ApplicationAddress(applicant.MailingAddress),
						TrainingProviderState = TrainingProviderStates.Complete,
						TrainingProviderType = new TrainingProviderType("Training Provider Type")
					};
					var skillsTrainingServiceCategory = new ServiceCategory("Skills Training", ServiceTypes.SkillsTraining, true, false, 0, 0, 1, 5) { Id = 1 };
					var skillsTrainingExpenseType = new EligibleExpenseType(skillsTrainingServiceCategory, ExpenseTypes.NotParticipantLimited);
					var skillsTrainingCost = new EligibleCost(grantApplication, skillsTrainingExpenseType, 3333.33m, 10);
					skillsTrainingCost.Breakdowns.Add(new EligibleCostBreakdown(skillsTrainingCost, new EligibleExpenseBreakdown(new ServiceLine("Service Line", "Service Breakdowns", 1) { ServiceCategory = skillsTrainingServiceCategory }, skillsTrainingExpenseType), 3333.33m));
					trainingProgram.EligibleCostBreakdown = skillsTrainingCost.Breakdowns.First();
					var essCost = new EligibleCost(grantApplication, new EligibleExpenseType(new ServiceCategory("Employment Services and Supports", ServiceTypes.EmploymentServicesAndSupports, true, false, 0, 0, 1, 5), ExpenseTypes.NotParticipantLimited), 5555.55m, 10);
					provider.EligibleCost = essCost;
					var adminCost = new EligibleCost(grantApplication, new EligibleExpenseType(new ServiceCategory("Administration", ServiceTypes.Administration, true, false, 0, 0, 1, 5), ExpenseTypes.NotParticipantLimited), 560.55m, 2);
					grantApplication.TrainingCost.EligibleCosts.Add(skillsTrainingCost);
					grantApplication.TrainingCost.EligibleCosts.Add(essCost);
					grantApplication.TrainingCost.EligibleCosts.Add(adminCost);
					grantApplication.TrainingCost.RecalculateEstimatedCosts();
					break;

				case (ProgramTypes.EmployerGrant):
				default:
					grantApplication.TrainingCost.EligibleCosts.Add(new EligibleCost(grantApplication, new EligibleExpenseType("Tuition", new ExpenseType(ExpenseTypes.ParticipantAssigned)), 3333.33m, 10));
					grantApplication.TrainingCost.EligibleCosts.Add(new EligibleCost(grantApplication, new EligibleExpenseType("Exam Fees", new ExpenseType(ExpenseTypes.ParticipantAssigned)), 5555.55m, 10));
					grantApplication.TrainingCost.EligibleCosts.Add(new EligibleCost(grantApplication, new EligibleExpenseType("Books", new ExpenseType(ExpenseTypes.ParticipantAssigned)), 560.55m, 2));
					grantApplication.TrainingCost.RecalculateEstimatedCosts();
					break;
			}

			switch (ApplicationStateInternal)
			{
				case ApplicationStateInternal.Draft:
				case ApplicationStateInternal.New:
				case ApplicationStateInternal.Unfunded:
				case ApplicationStateInternal.ApplicationWithdrawn:
				case ApplicationStateInternal.ApplicationDenied:
					break;

				case ApplicationStateInternal.PendingAssessment:
				case ApplicationStateInternal.UnderAssessment:
				case ApplicationStateInternal.ReturnedToAssessment:
				case ApplicationStateInternal.RecommendedForDenial:
					AddAssessment(grantApplication);
					break;

				case ApplicationStateInternal.RecommendedForApproval:
					AddStateChange(grantApplication, ApplicationStateInternal.RecommendedForApproval, "Your application was excellent.");
					AddAssessment(grantApplication);
					break;

				case ApplicationStateInternal.OfferIssued:
					AddStateChange(grantApplication, ApplicationStateInternal.RecommendedForApproval, "Your application was excellent.<br>And GOod.");
					AddAgreement(grantApplication);
					break;

				case ApplicationStateInternal.CancelledByMinistry:
				case ApplicationStateInternal.CancelledByAgreementHolder:
				case ApplicationStateInternal.OfferWithdrawn:
				case ApplicationStateInternal.AgreementAccepted:
				case ApplicationStateInternal.AgreementRejected:
				case ApplicationStateInternal.ChangeRequestDenied:
					AddAgreement(grantApplication);
					break;

				case ApplicationStateInternal.ChangeRequest:
				case ApplicationStateInternal.ChangeForApproval:
				case ApplicationStateInternal.ChangeForDenial:
				case ApplicationStateInternal.ChangeReturned:
					AddParticipants(grantApplication);

					new TrainingProvider(trainingProvider)
					{
						Id = 3,
						Name = "Change Training Provider Name",
						ContactEmail = "change@test.com",
						ContactFirstName = "John",
						ContactLastName = "Doe",
						ContactPhoneNumber = "(123) 123-1232",
						TrainingAddress = new ApplicationAddress(applicant.MailingAddress),
						TrainingProviderType = new TrainingProviderType("Training Provider Type")
					};
					break;

				case ApplicationStateInternal.NewClaim:
				case ApplicationStateInternal.ClaimAssessEligibility:
				case ApplicationStateInternal.ClaimAssessReimbursement:
				case ApplicationStateInternal.ClaimDenied:
					AddParticipants(grantApplication);
					AddClaim(grantApplication);
					break;

				case ApplicationStateInternal.ClaimReturnedToApplicant:
					AddParticipants(grantApplication);
					AddClaim(grantApplication);
					SetupReturnedClaim(grantApplication);
					break;

				case ApplicationStateInternal.ClaimApproved:
					AddParticipants(grantApplication);
					AddClaim(grantApplication);
					break;

				case ApplicationStateInternal.CompletionReporting:
				case ApplicationStateInternal.Closed:
					AddParticipants(grantApplication);
					AddPaymentRequest(grantApplication);
					break;
			}

			return new TestEntities
			{
				GrantApplication = grantApplication,
				GrantOpening = grantOpening,
				GrantProgram = grantProgram,
				FiscalYear = fiscalYear,
				Applicant = applicant,
				Organization = applicantOrganization,
				TrainingProgram = trainingProgram,
				TrainingProvider = trainingProvider
			};
		}

		private static void AddStateChange(GrantApplication grantApplication, ApplicationStateInternal state, string message)
		{
			if (grantApplication.StateChanges == null)
				grantApplication.StateChanges = new List<GrantApplicationStateChange>();

			grantApplication.StateChanges.Add(new GrantApplicationStateChange
			{
				FromState = ApplicationStateInternal.UnderAssessment,
				ToState = state,
				Reason = message,
				DateAdded = AppDateTime.UtcNow
			});
		}

		private static void AddAssessment(GrantApplication grantApplication)
		{
			grantApplication.TrainingCost.CopyEstimatedIntoAgreed();
			grantApplication.TrainingCost.RecalculateAgreedCosts();
		}

		private static void AddAgreement(GrantApplication grantApplication)
		{
			AddAssessment(grantApplication);
			grantApplication.GrantAgreement = new GrantAgreement(grantApplication)
			{
				StartDate = grantApplication.StartDate,
				EndDate = grantApplication.EndDate,
				CompletionReportingDueDate = grantApplication.StartDate.AddDays(5),
				ReimbursementClaimDueDate = grantApplication.StartDate.AddDays(6)
			};
		}

		private static void AddParticipants(GrantApplication grantApplication)
		{
			AddAgreement(grantApplication);

			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()) { FirstName = "John", LastName = "Doe", EmailAddress = "john.doe@test.com" });
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()) { FirstName = "Jane", LastName = "Doe", EmailAddress = "jane.doe@test.com" });
		}

		private static Claim AddClaim(GrantApplication grantApplication, ClaimState claimState = ClaimState.Unassessed)
		{
			var claim = new Claim(1, 1, grantApplication, grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId)
			{
				ClaimState = grantApplication.ApplicationStateInternal == ApplicationStateInternal.ClaimApproved ? ClaimState.ClaimApproved : grantApplication.ApplicationStateInternal == ApplicationStateInternal.ClaimDenied ? ClaimState.ClaimDenied : claimState,
				ParticipantForms = grantApplication.ParticipantForms
			};

			grantApplication.TrainingCost.EligibleCosts.ForEach(ec =>
			{
				var cost = new ClaimEligibleCost(claim, ec)
				{
					ClaimCost = ec.AgreedMaxCost,
					ClaimParticipants = ec.AgreedMaxParticipants,
					AssessedCost = ec.AgreedMaxCost,
					AssessedParticipants = ec.AgreedMaxParticipants
				};
				claim.EligibleCosts.Add(cost);
			});

			claim.RecalculateClaimedCosts();
			claim.RecalculateAssessedCosts();

			grantApplication.Claims.Add(claim);
			return claim;
		}

		private static Claim SetupReturnedClaim(GrantApplication grantApplication, ClaimState claimState = ClaimState.Unassessed)
		{
			var claim = grantApplication.Claims.First();

			claim.ClaimState = ClaimState.Incomplete;
			claim.ClaimAssessmentNotes = $"This is a multiple line{Environment.NewLine}reason for return{Environment.NewLine}{Environment.NewLine}and we should see line breaks.";

			grantApplication.ApplicationStateExternal = ApplicationStateExternal.ClaimReturned;

			AddStateChange(grantApplication, ApplicationStateInternal.ClaimReturnedToApplicant, claim.ClaimAssessmentNotes);

			return claim;
		}

		private static void AddPaymentRequest(GrantApplication grantApplication)
		{
			var claim = AddClaim(grantApplication, ClaimState.ClaimApproved);

			claim.PaymentRequests.Add(new PaymentRequest(grantApplication, new PaymentRequestBatch(grantApplication.GrantOpening.GrantStream.GrantProgram, "batch", PaymentBatchTypes.PaymentRequest, new InternalUser()), claim));
		}
	}
}
