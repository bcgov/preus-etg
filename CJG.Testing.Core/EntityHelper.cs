using CJG.Core.Entities;
using CJG.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Testing.Core
{
	/// <summary>
	/// <typeparamref name="EntityHelper"/> static class, provides helper methods to create entity objects for unit tests.
	/// </summary>
	public static class EntityHelper
	{
		#region Variables
		private static Random random = new Random();
		#endregion

		#region Entities
		/// <summary>
		/// Set the RowVersion property on the specified entity.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		public static T SetRowVersion<T>(this T entity, string rowVersion = "AgQGCAoMDhASFA==")
			where T : EntityBase
		{
			entity.RowVersion = Convert.FromBase64String(rowVersion);
			return entity;
		}

		/// <summary>
		/// Creates and returns a new GrantProgram object.
		/// Initializes the ProgramConfiguration, and DocumentTemplate(s).
		/// </summary>
		/// <param name="programName"></param>
		/// <param name="programCode"></param>
		/// <param name="programType"></param>
		/// <returns></returns>
		public static GrantProgram CreateGrantProgram(string programName = "testProgram", string programCode = "TEST", ProgramTypes programType = ProgramTypes.EmployerGrant)
		{
			var programConfiguration = programType == ProgramTypes.EmployerGrant
				? new ProgramConfiguration("SingleAmendableClaim", new ClaimType(ClaimTypes.SingleAmendableClaim) { Id = ClaimTypes.SingleAmendableClaim })
				: new ProgramConfiguration("MultipleClaimsWithoutAmendments", new ClaimType(ClaimTypes.MultipleClaimsWithoutAmendments) { Id = ClaimTypes.MultipleClaimsWithoutAmendments });
			return new GrantProgram(programName, programCode, programType,
				new DocumentTemplate(DocumentTypes.ApplicantDeclaration, "declaration", "test") { Id = 1 },
				new DocumentTemplate(DocumentTypes.GrantAgreementCoverLetter, "approval", "test") { Id = 2 },
				new DocumentTemplate(DocumentTypes.GrantAgreementScheduleA, "schedulea", "test") { Id = 3 },
				new DocumentTemplate(DocumentTypes.GrantAgreementScheduleB, "scheduleb", "test") { Id = 4 },
				new DocumentTemplate(DocumentTypes.ParticipantConsent, "consent", "test") { Id = 5 })
			{
				Id = 1,
				ProgramConfiguration = programConfiguration,
				IncludeDeliveryPartner = true,
				UseFIFOReservation = true,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		public static GrantProgram CreateGrantProgram(ProgramConfiguration programConfiguration)
		{
			var grantProgram = CreateGrantProgram();
			grantProgram.ProgramConfiguration = programConfiguration;
			grantProgram.ProgramConfigurationId = programConfiguration.Id;
			grantProgram.ProgramType = new ProgramType()
			{
				Id = ProgramTypes.EmployerGrant,
				Description = "ProgramType"
			};
			programConfiguration.GrantPrograms.Add(grantProgram);
			grantProgram.ApplicantScheduleATemplate = new DocumentTemplate { Body = "<div></div>" };
			return grantProgram;
		}

		/// <summary>
		/// Creates and returns a new <typeparamref name="GrantOpening"/> object.
		/// Initializes the <typeparamref name="GrantStream"/>, <typeparamref name="Stream"/>, <typeparamref name="StreamCriteria"/>, <typeparamref name="StreamObjective"/>, <typeparamref name="TrainingPeriod"/>, and <typeparamref name="FiscalYear"/>
		/// </summary>
		/// <param name="streamName"></param>
		/// <param name="budgetAllocation"></param>
		/// <param name="maxReimbursementAmount"></param>
		/// <param name="reimbursementRate"></param>
		/// <returns></returns>
		public static GrantOpening CreateGrantOpening(string streamName = "Stream", decimal budgetAllocation = 1000000, decimal maxReimbursementAmount = 10000, double reimbursementRate = 2d / 3d)
		{
			// This is the fiscal year
			var startDate = new DateTime(AppDateTime.UtcNow.Year, 4, 1, 0, 0, 0, DateTimeKind.Local).ToUniversalTime(); // April
			var endDate = startDate.AddYears(1).ToLocalMidnight().ToUniversalTime(); // March

			if (AppDateTime.UtcNow < startDate)
			{
				startDate = startDate.AddYears(-1);
				endDate = endDate.AddYears(-1);
			}

			// Training periods
			var period1Start = startDate; // April
			var period1End = period1Start.AddMonths(4).AddDays(31);
			var period2Start = period1End.AddDays(1); // Sept
			var period2End = period2Start.AddMonths(3).AddDays(31);
			var period3Start = period2End.AddDays(1); // Jan
			var period3End = period3Start.AddMonths(2).AddDays(31);

			var periodStart = (AppDateTime.UtcNow <= period1End) ? period1Start : (AppDateTime.UtcNow <= period2End) ? period2Start : period3Start;
			var periodEnd = (AppDateTime.UtcNow <= period1End) ? period1End : (AppDateTime.UtcNow <= period2End) ? period2End : period3End;

			var grantOpening = new GrantOpening(
				new GrantStream(
					streamName,
					$"Stream Criteria - {Guid.NewGuid()}",
					CreateGrantProgram(),
					new AccountCode())
				{
					MaxReimbursementAmt = maxReimbursementAmount,
					ReimbursementRate = reimbursementRate,
					DefaultDeniedRate = 0.051,
					DefaultWithdrawnRate = 0.045,
					DefaultReductionRate = 0.025,
					DefaultSlippageRate = 0.165,
					DefaultCancellationRate = 0.035
				},
				new TrainingPeriod(
					new FiscalYear($"{startDate.ToLocalMorning().Year}/{endDate.ToLocalMidnight().Year}", startDate, endDate),
					"Training Period",
					periodStart,
					periodEnd,
					periodStart.AddMonths(-1),
					periodStart),
				budgetAllocation)
			{
				Id = 1,
				State = GrantOpeningStates.Open
			};
			grantOpening.GrantOpeningIntake = new GrantOpeningIntake(grantOpening);
			grantOpening.GrantOpeningFinancial = new GrantOpeningFinancial(grantOpening);

			return grantOpening;
		}

		public static ProgramType CreateProgramType(ProgramTypes id = ProgramTypes.EmployerGrant)
		{
			return new ProgramType
			{
				Id = id,
				Caption = "Program Type Caption"
			};
		}

		public static ExpectedQualification CreateExpectedQualification(string caption, int id = 1)
		{
			return new ExpectedQualification
			{
				Id = id,
				Caption = caption
			};
		}

		public static InDemandOccupation CreateInDemandOccupation(string caption, int id = 1)
		{
			return new InDemandOccupation
			{
				Id = id,
				Caption = caption
			};
		}

		public static TrainingLevel CreateTrainingLevel(string caption, int id = 1)
		{
			return new TrainingLevel
			{
				Id = id,
				Caption = caption
			};
		}

		public static UnderRepresentedGroup CreateUnderRepresentedGroup(string caption, int id = 1)
		{
			return new UnderRepresentedGroup
			{
				Id = id,
				Caption = caption
			};
		}

		public static DeliveryMethod CreateDeliveryMethod(string caption, int id = 1)
		{
			return new DeliveryMethod
			{
				Id = id,
				Caption = caption
			};
		}

		public static DeliveryPartnerService CreateDeliveryPartnerService(string caption, int id = 1)
		{
			return new DeliveryPartnerService
			{
				Id = id,
				Caption = caption
			};
		}

		public static TrainingProviderInventory CreateTrainingProviderInventory(int id = 1)
		{
			return new TrainingProviderInventory
			{
				Id = id,
				Name = "Training Provider Inventory"
			};
		}

		public static SkillsFocus CreateSkillFocus(string caption, int id = 1)
		{
			return new SkillsFocus
			{
				Id = id,
				Caption = caption
			};
		}

		public static SkillLevel CreateSkillLevel(string caption, int id = 1)
		{
			return new SkillLevel
			{
				Id = id,
				Caption = caption
			};
		}

		public static Region CreateRegion(string name)
		{
			return new Region
			{
				Id = name,
				Name = name
			};
		}

		public static Country CreateCountry(string name)
		{
			return new Country
			{
				Id = name,
				Name = name
			};
		}


		/// <summary>
		/// Creates and returns a new <typeparamref name="GrantOpening"/> object.
		/// Initializes the <typeparamref name="decimal"/>
		/// </summary>
		/// <param name="intakeAmt"></param>
		/// <returns>GrantOpening</returns>
		public static GrantOpening CreateGrantOpeningWithGrantApplications(decimal intakeAmt = 1)
		{
			var grantOpening = new GrantOpening
			{
				Id = 1,
				State = GrantOpeningStates.Open,
				GrantOpeningFinancial = new GrantOpeningFinancial(),
				IntakeTargetAmt = intakeAmt,
				GrantOpeningIntake = new GrantOpeningIntake(),
				GrantStream = new GrantStream("stream", "objective", CreateGrantProgram(), new AccountCode())
			};
			var grantApplications = new List<GrantApplication>
				{
					new GrantApplication
					{
						Id = 1,
						GrantOpeningId = 1,
						DateSubmitted = DateTime.UtcNow.AddDays(-5),
						ApplicationStateInternal = ApplicationStateInternal.New,
						GrantOpening = grantOpening,
						TrainingPrograms = new List<TrainingProgram>
						{
							new TrainingProgram { }
						},
						TrainingCost = new TrainingCost
						{
							TotalEstimatedReimbursement = 2,
							AgreedCommitment = 1
						}
					},
					new GrantApplication
					{
						Id = 2,
						GrantOpeningId = 1,
						DateSubmitted = DateTime.UtcNow.AddDays(-5),
						ApplicationStateInternal = ApplicationStateInternal.Closed,
						GrantOpening = grantOpening,
						TrainingPrograms = new List<TrainingProgram>
						{
							new TrainingProgram { }
						},
						TrainingCost = new TrainingCost
						{
							TotalEstimatedReimbursement = 3,
							AgreedCommitment = 1
						}
					},
				};
			grantOpening.GrantApplications = grantApplications;
			return grantOpening;
		}

		public static FiscalYear CreateFiscalYear()
		{
			return new FiscalYear
			{
				Id = 1
			};
		}

		public static Attachment CreateAttachment(int id = 1)
		{
			return new Attachment
			{
				Id = id,
				FileName = "File Name",
				FileExtension = ".pdf",
				AttachmentData = Convert.FromBase64String("AgQGCAoMDhASFA=="),
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		public static EligibleCost CreateEligibleCost(GrantApplication grantApplication, int id = 1)
		{
			return new EligibleCost(grantApplication, new EligibleExpenseType("ExpenseType", new ExpenseType(ExpenseTypes.ParticipantAssigned)), 1, 1)
			{
				Id = id,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		public static EligibleCostBreakdown CreateEligibleCostBreakdown(EligibleCost eligibleCost, EligibleExpenseBreakdown eligibleExpenseBreakdown, int id = 1)
		{
			return new EligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown, 1)
			{
				Id = id,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		public static EligibleExpenseBreakdown CreateEligibleExpenseBreakdown(EligibleExpenseType eligibleExpenseType)
		{
			return new EligibleExpenseBreakdown()
			{
				EligibleExpenseTypeId = eligibleExpenseType.Id,
				ExpenseType = eligibleExpenseType,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		public static GrantApplication CreateGrantApplicationWithClaim(GrantOpening grantOpening = null)
		{
			var Claims = new List<Claim>
			{
				new Claim
				{
					TotalClaimReimbursement = 1,
					TotalAssessedReimbursement = 1,
					ClaimVersion = 1,
					EligibleCosts = new List<ClaimEligibleCost>
					{
						new ClaimEligibleCost()
					},
				}
			};
			return new GrantApplication
			{
				Id = 1,
				GrantOpeningId = 1,
				DateSubmitted = DateTime.UtcNow.AddDays(10),
				ApplicationStateInternal = ApplicationStateInternal.New,
				GrantOpening = grantOpening ?? CreateGrantOpening(),
				Claims = Claims,
				TrainingPrograms = new List<TrainingProgram>(),
				TrainingCost = new TrainingCost
				{
					TotalEstimatedReimbursement = 2,
					AgreedCommitment = 1,
				}
			};
		}

		/// <summary>
		/// Creates and returns a new <typeparamref name="GrantOpening"/> object.
		/// Initializes the <typeparamref name="GrantStream"/>, <typeparamref name="Stream"/>, <typeparamref name="StreamCriteria"/>, <typeparamref name="StreamObjective"/>, <typeparamref name="TrainingPeriod"/>, and <typeparamref name="FiscalYear"/>
		/// </summary>
		/// <param name="startDate"></param>
		/// <returns></returns>
		public static GrantOpening CreateGrantOpening(DateTime startDate)
		{
			if (startDate == null)
				throw new ArgumentNullException(nameof(startDate));

			var guid = Guid.NewGuid();
			var endDate = startDate.AddYears(1).ToLocalMidnight();
			var grantOpening = new GrantOpening(
				new GrantStream(
						$"Stream - {guid}",
						$"Stream Criteria - {guid}",
						new GrantProgram(),
						new AccountCode())
				{
					MaxReimbursementAmt = 10000,
					ReimbursementRate = 2d / 3d,
					DefaultDeniedRate = 0.051,
					DefaultWithdrawnRate = 0.045,
					DefaultReductionRate = 0.025,
					DefaultSlippageRate = 0.165,
					DefaultCancellationRate = 0.035
				},
				new TrainingPeriod(
					new FiscalYear($"{startDate.ToLocalMorning().Year}/{endDate.ToLocalMidnight().Year}", startDate, endDate),
					"Training Period",
					startDate,
					startDate.AddMonths(4).ToLocalMidnight(),
					startDate.AddMonths(-1),
					startDate),
				1000000);

			grantOpening.GrantOpeningIntake = new GrantOpeningIntake(grantOpening);
			grantOpening.GrantOpeningFinancial = new GrantOpeningFinancial(grantOpening);

			return grantOpening;
		}

		/// <summary>
		/// Changes the state of the incomplete application and it's components to Complete.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static GrantApplication CompleteGrantApplication(this GrantApplication grantApplication)
		{
			grantApplication.TrainingCost.TrainingCostState = TrainingCostStates.Complete;
			grantApplication.TrainingProviders.Where(tp => tp.TrainingProviderState == TrainingProviderStates.Incomplete).ForEach(tp => tp.TrainingProviderState = TrainingProviderStates.Complete);
			grantApplication.TrainingPrograms.Where(tp => tp.TrainingProgramState == TrainingProgramStates.Incomplete).ForEach(tp =>
			{
				tp.TrainingProgramState = TrainingProgramStates.Complete;
				if (tp.TrainingProvider.TrainingProviderState == TrainingProviderStates.Incomplete)
					tp.TrainingProvider.TrainingProviderState = TrainingProviderStates.Complete;
			});

			return grantApplication;
		}

		/// <summary>
		/// Creates and returns a new GrantApplication object.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <param name="applicationAdministrator"></param>
		/// <param name="assessor"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplication(GrantOpening grantOpening, User applicationAdministrator, InternalUser assessor, ApplicationStateInternal internalState = ApplicationStateInternal.Draft)
		{
			var organizationType = new OrganizationType("test") { Id = 1 };
			var legalStructure = new LegalStructure("test") { Id = 1 };
			var organizationAddress = new ApplicationAddress(new Address("test", null, "city", "v9c9c9", new Region("BC", "British Columbia", new Country("CA", "Canada")))) { Id = 1 };
			var naics = new NaIndustryClassificationSystem("TEST", "test", 1, null, 0, 1, 2012) { Id = 1 };
			var grantApplication = new GrantApplication(grantOpening, applicationAdministrator, new ApplicationType("ApplicationType"))
			{
				Id = 1,
				ApplicationStateInternal = internalState,
				ApplicationStateExternal = internalState.GetExternalState(),
				AssessorId = assessor?.Id,
				Assessor = assessor,
				TrainingCost = new TrainingCost()
				{
					AgreedParticipants = 1
				},
				OrganizationType = organizationType,
				OrganizationTypeId = organizationType.Id,
				OrganizationLegalStructure = legalStructure,
				OrganizationLegalStructureId = legalStructure.Id,
				OrganizationAnnualTrainingBudget = 10000,
				Organization = new Organization(organizationType, Guid.NewGuid(), "Org", legalStructure, 2000, 10, 10, 10000, 2) { Id = 1 },
				OrganizationId = 1,
				OrganizationAddress = organizationAddress,
				OrganizationAddressId = organizationAddress.Id,
				NAICS = naics,
				NAICSId = naics.Id,
				StartDate = AppDateTime.UtcMorning,
				EndDate = AppDateTime.UtcNow.ToUtcMidnight(),
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
			grantApplication.BusinessContactRoles.First().GrantApplicationId = grantApplication.Id;
			grantApplication.TrainingCost.GrantApplication = grantApplication;
			return grantApplication;
		}

		public static GrantApplication CreateGrantApplication(User applicant = null)
		{
			// set up GrantProgram
			var programConfiguration = CreateProgramConfiguration();
			var grantProgram = CreateGrantProgram(programConfiguration);
			// set up GrantStream
			var grantStream = CreateGrantStream(grantProgram);
			// set up DeliveryPartner
			var deliveryPartner = CreateDeliveryPartner(grantProgram);
			var deliveryPartnerService = CreateDeliveryPartnerService(grantProgram);
			grantProgram.DeliveryPartners.Add(deliveryPartner);
			grantProgram.DeliveryPartnerServices.Add(deliveryPartnerService);
			// set up AccountCode
			var accountCode = CreateAccountCode(2);
			grantProgram.AccountCode = accountCode;
			grantProgram.AccountCodeId = accountCode.Id;
			grantStream.AccountCode = accountCode;
			grantStream.AccountCodeId = accountCode.Id;
			accountCode.GrantStreams.Add(grantStream);
			accountCode.GrantPrograms.Add(grantProgram);
			// set up GrantOpening
			var trainingPeriod = CreateTrainingPeriod(AppDateTime.UtcMorning.AddDays(-1), AppDateTime.UtcNow.AddDays(1).ToUtcMidnight());
			trainingPeriod.FiscalYear = CreateFiscalYear();

			var grantOpening = CreateGrantOpening(grantStream, trainingPeriod);
			// set up GrantApplication
			var grantApplication = CreateGrantApplication(grantOpening, applicant ?? CreateExternalUser());
			grantApplication.DateAdded = AppDateTime.UtcNow;
			grantApplication.StartDate = AppDateTime.UtcMorning;
			grantApplication.EndDate = AppDateTime.UtcNow.AddDays(1).ToUtcMidnight();
			grantApplication.RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==");
			grantApplication.NAICS = CreateNaIndustryClassificationSystem();
			grantApplication.ApplicationType = new ApplicationType("ApplicationType");
			grantApplication.EligibilityConfirmed = true;
			// set up ProgramDescription
			var programDescription = CreateProgramDescription();
			programDescription.GrantApplication = grantApplication;
			programDescription.GrantApplicationId = grantApplication.Id;
			grantApplication.ProgramDescription = programDescription;
			// set up Attachments
			var attachment = CreateAttachment();
			attachment.GrantApplications.Add(grantApplication);
			grantApplication.Attachments.Add(attachment);
			// set up EligibleExpenseType
			var eligibleExpenseType = CreateEligibleExpenseType();
			var expenseType = CreateExpenseType();
			expenseType.EligibleExpenseTypes.Add(eligibleExpenseType);
			eligibleExpenseType.ExpenseType = expenseType;

			var serviceLine = CreateServiceLine();
			serviceLine.ServiceLineBreakdowns.Add(CreateServiceLineBreakdown());

			var serviceCategory = CreateServiceCategory();
			serviceLine.ServiceCategory = serviceCategory;
			serviceLine.ServiceCategoryId = serviceCategory.Id;
			serviceCategory.ServiceLines.Add(serviceLine);

			var eligibleExpenseBreakdown = new EligibleExpenseBreakdown(serviceLine, eligibleExpenseType)
			{
				ServiceLine = serviceLine
			};
			serviceLine.EligibleExpenseBreakdowns.Add(eligibleExpenseBreakdown);
			eligibleExpenseType.Breakdowns.Add(eligibleExpenseBreakdown);
			serviceCategory.EligibleExpenseTypes.Add(eligibleExpenseType);
			eligibleExpenseType.ServiceCategory = serviceCategory;
			eligibleExpenseType.ServiceCategoryId = serviceCategory.Id;

			programConfiguration.EligibleExpenseTypes.Add(eligibleExpenseType);
			eligibleExpenseType.ProgramConfigurations.Add(programConfiguration);
			// set up TrainingCost
			var trainingCost = CreateTrainingCost(grantApplication);
			grantApplication.TrainingCost = trainingCost;
			// set up EligibleCost
			var eligibleCost = CreateEligibleCost(grantApplication);
			eligibleExpenseType.EligibleCosts.Add(eligibleCost);
			eligibleCost.EligibleExpenseType = eligibleExpenseType;
			eligibleCost.EligibleExpenseTypeId = eligibleExpenseType.Id;

			var eligibleCostBreakdown = CreateEligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown);
			eligibleCost.Breakdowns.Add(eligibleCostBreakdown);

			trainingCost.EligibleCosts.Add(eligibleCost);
			eligibleCost.TrainingCost = trainingCost;
			// set up TrainingProvider
			var trainingProvider = CreateTrainingProvider(grantApplication);
			eligibleCost.TrainingProviders.Add(trainingProvider);
			trainingProvider.EligibleCost = eligibleCost;

			grantApplication.TrainingProviders.Add(trainingProvider);

			// set up TrainingProgram
			var trainingProgram = CreateTrainingProgram(grantApplication);
			trainingProgram.EligibleCostBreakdownId = eligibleCostBreakdown.Id;
			trainingProgram.EligibleCostBreakdown = eligibleCostBreakdown;
			eligibleCostBreakdown.TrainingPrograms.Add(trainingProgram);

			// set up GrantAgreement
			grantApplication.GrantAgreement = CreateGrantAgreement(grantApplication);

			// set up Claims
			grantApplication.Claims = new[] { CreateClaim(grantApplication) };

			return grantApplication;
		}

		public static Community CreateCommunity(string caption, int id = 1)
		{
			return new Community
			{
				Id = id,
				Caption = caption,
				IsActive = true
			};
		}

		public static PrioritySector CreatePrioritySector(string caption, int id = 1)
		{
			return new PrioritySector
			{
				Id = id,
				Caption = caption
			};
		}

		public static ApplicationType CreateApplicationType(int id = 1)
		{
			return new ApplicationType
			{
				Id = id
			};
		}

		public static NaIndustryClassificationSystem CreateNaIndustryClassificationSystem(int id = 1, string code = "test", int level = 1)
		{
			return new NaIndustryClassificationSystem
			{
				Id = id,
				Code = code,
				Level = level,
				Left = 0,
				Right = 1
			};
		}

		public static NationalOccupationalClassification CreateNationalOccupationalClassification(int id = 1)
		{
			return new NationalOccupationalClassification
			{
				Id = id,
				Code = "test_" + id,
				Description = "test description " + id
			};
		}

		public static GrantStream CreateGrantStream(GrantProgram grantProgram)
		{
			var grantStream = new GrantStream
			{
				Id = 1,
				Name = "Grant Stream Name",
				Objective = "Grant Stream Objective",
				IsActive = true,
				GrantProgram = grantProgram,
				GrantProgramId = grantProgram.Id,
				IncludeDeliveryPartner = true,
				MaxReimbursementAmt = 10000,
				ReimbursementRate = 2d / 3d,
				DefaultDeniedRate = 0.051,
				DefaultWithdrawnRate = 0.045,
				DefaultReductionRate = 0.025,
				DefaultSlippageRate = 0.165,
				DefaultCancellationRate = 0.035,
				EligibilityEnabled = true,
				EligibilityRequirements = "Eligibility Requirements",
				EligibilityQuestion = "Eligibility Question",
				EligibilityRequired = true,
				ProgramConfiguration = grantProgram.ProgramConfiguration,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
			grantProgram.GrantStreams.Add(grantStream);
			return grantStream;
		}

		public static DeliveryPartnerService CreateDeliveryPartnerService(GrantProgram grantProgram, int id = 1)
		{
			return new DeliveryPartnerService
			{
				Id = id,
				Caption = "Delivery Partner",
				GrantProgram = grantProgram,
				GrantProgramId = grantProgram.Id,
				IsActive = true,
				RowSequence = id,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		public static AccountCode CreateAccountCode(int id = 1)
		{
			return new AccountCode
			{
				Id = id,
				GLProjectCode = "GLProjectCode"
			};
		}

		public static ProgramConfiguration CreateProgramConfiguration(ProgramTypes programTypes = ProgramTypes.EmployerGrant, int id = 1)
		{
			var claimType = CreateClaimType(programTypes);
			return new ProgramConfiguration
			{
				Id = id,
				IsActive = true,
				ClaimType = claimType,
				ClaimTypeId = claimType.Id,
				Caption = programTypes == ProgramTypes.EmployerGrant ? "SingleAmendableClaim" : "MultipleClaimsWithoutAmendments",
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		public static ClaimType CreateClaimType(ProgramTypes programTypes)
		{
			return new ClaimType
			{
				Id = programTypes == ProgramTypes.EmployerGrant ? ClaimTypes.SingleAmendableClaim : ClaimTypes.MultipleClaimsWithoutAmendments,
				IsActive = true,
				IsAmendable = true,
				Caption = "Claim Type Caption"
			};
		}

		public static EligibleExpenseType CreateEligibleExpenseType(int id = 1)
		{
			return new EligibleExpenseType
			{
				Id = id,
				Caption = "Eligible Expense Type Caption"
			};
		}

		public static ExpenseType CreateExpenseType(ExpenseTypes id = ExpenseTypes.AutoLimitEstimatedCosts)
		{
			return new ExpenseType
			{
				Id = id,
				IsActive = true,
				Caption = "Expense Type Caption",
				Description = "Expense Type Description"
			};
		}

		public static ServiceCategory CreateServiceCategory(int id = 1)
		{
			return new ServiceCategory
			{
				Id = id,
				IsActive = true,
				Caption = "Service Category Caption",
				ServiceTypeId = ServiceTypes.SkillsTraining
			};
		}

		public static ServiceLine CreateServiceLine(int id = 1)
		{
			return new ServiceLine
			{
				Id = id,
				Caption = "Service Line Caption",
				Description = "Service Line Description",
				BreakdownCaption = "Service Line Breakdown Caption"
			};
		}

		public static ServiceLineBreakdown CreateServiceLineBreakdown(int id = 1)
		{
			return new ServiceLineBreakdown
			{
				Id = id,
				Caption = "Service Line Breakdown Caption",
				Description = "Service Line Breakdown Description"
			};
		}

		public static TrainingPeriod CreateTrainingPeriod(DateTime startDate, DateTime endDate, int id = 1)
		{
			return new TrainingPeriod
			{
				StartDate = startDate,
				EndDate = endDate,
				Caption = "Training Period Caption",
				Id = id
			};
		}

		public static GrantOpening CreateGrantOpening(GrantStream grantStream, TrainingPeriod trainingPeriod, GrantOpeningStates grantOpeningState = GrantOpeningStates.Open, int id = 1)
		{
			return new GrantOpening
			{
				Id = id,
				State = grantOpeningState,
				GrantStream = grantStream,
				GrantStreamId = grantStream.Id,
				TrainingPeriod = trainingPeriod,
				TrainingPeriodId = trainingPeriod.Id,
				BudgetAllocationAmt = 99,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		public static TrainingCost CreateTrainingCost(GrantApplication grantApplication, TrainingCostStates trainingCostState = TrainingCostStates.Complete, int agreedParticipants = 1)
		{
			return new TrainingCost
			{
				TrainingCostState = trainingCostState,
				AgreedParticipants = agreedParticipants,
				GrantApplication = grantApplication,
				GrantApplicationId = grantApplication.Id,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		public static ProgramDescription CreateProgramDescription(ProgramDescriptionStates programDescriptionState = ProgramDescriptionStates.Complete)
		{
			return new ProgramDescription { DescriptionState = programDescriptionState };
		}

		/// <summary>
		/// Creates and returns a new GrantApplication object.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <param name="applicationAdministrator"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplication(GrantOpening grantOpening, User applicationAdministrator, ApplicationStateInternal internalState = ApplicationStateInternal.Draft)
		{
			return CreateGrantApplication(grantOpening, applicationAdministrator, null, internalState);
		}

		/// <summary>
		/// Creates and returns a new GrantApplication object.
		/// </summary>
		/// <param name="applicationAdministrator"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplication(User applicationAdministrator, ApplicationStateInternal internalState = ApplicationStateInternal.Draft)
		{
			return CreateGrantApplication(CreateGrantOpening(), applicationAdministrator, null, internalState);
		}

		/// <summary>
		/// Creates and returns a new GrantApplication object.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <param name="assessor"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplication(GrantOpening grantOpening, InternalUser assessor, ApplicationStateInternal internalState = ApplicationStateInternal.Draft)
		{
			return CreateGrantApplication(grantOpening, CreateExternalUser(), assessor, internalState);
		}

		/// <summary>
		/// Creates and returns a new GrantApplication object.
		/// </summary>
		/// <param name="assessor"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplication(InternalUser assessor, ApplicationStateInternal internalState = ApplicationStateInternal.Draft)
		{
			return CreateGrantApplication(CreateExternalUser(), assessor, internalState);
		}

		/// <summary>
		/// Creates and returns a new GrantApplication object.
		/// </summary>
		/// <param name="applicationAdministrator"></param>
		/// <param name="assessor"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplication(User applicationAdministrator, InternalUser assessor, ApplicationStateInternal internalState = ApplicationStateInternal.Draft)
		{
			return CreateGrantApplication(CreateGrantOpening(), applicationAdministrator, assessor, internalState);
		}

		/// <summary>
		/// Creates <typeparamref name="TrainingProvider"/> and <typeparamref name="TrainingProgram"/> and associates them with the specified GrantApplication.
		/// </summary>
		/// <param name="applicationAdministrator"></param>
		/// <param name="assessor"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplicationWithCosts(User applicationAdministrator, InternalUser assessor, ApplicationStateInternal internalState = ApplicationStateInternal.Draft)
		{
			return CreateGrantApplicationWithCosts(CreateGrantOpening(), applicationAdministrator, assessor, internalState);
		}

		/// <summary>
		/// Creates <typeparamref name="TrainingProvider"/> and <typeparamref name="TrainingProgram"/> and associates them with the specified GrantApplication.
		/// </summary>
		/// <param name="applicationAdministrator"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplicationWithCosts(User applicationAdministrator, ApplicationStateInternal internalState = ApplicationStateInternal.Draft)
		{
			return CreateGrantApplicationWithCosts(CreateGrantOpening(), applicationAdministrator, null, internalState);
		}

		/// <summary>
		/// Creates <typeparamref name="TrainingProvider"/> and <typeparamref name="TrainingProgram"/> and associates them with the specified GrantApplication.
		/// </summary>
		/// <param name="assessor"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplicationWithCosts(InternalUser assessor, ApplicationStateInternal internalState = ApplicationStateInternal.Draft)
		{
			return CreateGrantApplicationWithCosts(CreateGrantOpening(), CreateExternalUser(), assessor, internalState);
		}

		/// <summary>
		/// Creates <typeparamref name="TrainingProvider"/> and <typeparamref name="TrainingProgram"/> and associates them with the specified GrantApplication.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <param name="applicationAdministrator"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplicationWithCosts(GrantOpening grantOpening, User applicationAdministrator, ApplicationStateInternal internalState = ApplicationStateInternal.Draft)
		{
			return CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, null, internalState);
		}

		/// <summary>
		/// Creates <typeparamref name="TrainingProvider"/> and associates it with the specified GrantApplication.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static TrainingProvider CreateTrainingProvider(GrantApplication grantApplication)
		{
			var attachment = CreateAttachment();

			return new TrainingProvider(grantApplication)
			{
				Id = 1,
				GrantApplicationId = grantApplication.Id,
				GrantApplication = grantApplication,
				TrainingProviderState = TrainingProviderStates.Complete,
				Name = "Training Provider",
				ContactFirstName = "First Name",
				ContactLastName = "Last Name",
				ContactEmail = "contact@email.com",
				ContactPhoneNumber = "(123) 123-1235",
				TrainingAddress = new ApplicationAddress(new Address("1224 St", null, "Victoria", "V9C0E4", new Region("BC", "British Columbia", new Country("CA", "Canada")))),
				TrainingOutsideBC = false,
				TrainingProviderType = new TrainingProviderType("Training Provider Type"),
				CourseOutlineDocument = attachment,
				CourseOutlineDocumentId = attachment.Id,
				DateAdded = AppDateTime.UtcNow,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		/// <summary>
		/// Creates <typeparamref name="TrainingProvider"/> and associates it with the specified GrantApplication.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public static TrainingProvider CreateTrainingProvider(TrainingProgram trainingProgram)
		{
			var attachment = CreateAttachment();

			var trainingProvider = new TrainingProvider(trainingProgram)
			{
				Id = 1,
				TrainingProviderState = TrainingProviderStates.Complete,
				Name = "Training Provider",
				ContactFirstName = "First Name",
				ContactLastName = "Last Name",
				ContactEmail = "contact@email.com",
				ContactPhoneNumber = "(123) 123-1235",
				TrainingAddress = new ApplicationAddress(new Address("1224 St", null, "Victoria", "V9C0E4", new Region("BC", "British Columbia", new Country("CA", "Canada")))),
				TrainingOutsideBC = false,
				TrainingProviderType = new TrainingProviderType("Training Provider Type"),
				TrainingProviderTypeId = 1,
				CourseOutlineDocument = attachment,
				CourseOutlineDocumentId = attachment.Id,
				DateAdded = AppDateTime.UtcNow,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
			return trainingProvider;
		}

		public static Note CreateNote()
		{
			var attachment = CreateAttachment();

			return new Note
			{
				Id = 1,
				Attachment = attachment,
				AttachmentId = attachment.Id,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		public static TrainingProviderType CreateTrainingProviderType(string caption, int id = 1)
		{
			return new TrainingProviderType
			{
				Id = id,
				Caption = caption
			};
		}

		public static TrainingProgram CreateTrainingProgram(GrantApplication grantApplication, int estimatedParticipants = 5)
		{
			var trainingProgram = new TrainingProgram(grantApplication)
			{
				Id = 1,
				CourseTitle = "Course Title",
				StartDate = AppDateTime.UtcNow,
				EndDate = AppDateTime.UtcNow.AddMonths(3),
				TrainingProgramState = TrainingProgramStates.Complete,
				DeliveryMethods = new[] { new DeliveryMethod { Id = 1, Caption = "Delivery Methods" } },
				TotalTrainingHours = 40,
				TitleOfQualification = "test Qualification",
				DateAdded = AppDateTime.UtcNow,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
			var trainingProvider = CreateTrainingProvider(trainingProgram);
			return trainingProgram;
		}

		/// <summary>
		/// Creates <typeparamref name="TrainingProgram"/> and associates it with the specified <typeparamref name="TrainingProvider"/>.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <param name="estimatedParticipants"></param>
		/// <returns></returns>
		public static TrainingProgram CreateTrainingProgram(GrantApplication grantApplication, TrainingProvider trainingProvider, int estimatedParticipants = 5)
		{
			var trainingProgram = new TrainingProgram(grantApplication, trainingProvider)
			{
				Id = 1,
				CourseTitle = "Course Title",
				StartDate = AppDateTime.UtcNow,
				EndDate = AppDateTime.UtcNow.AddMonths(3),
				TrainingProgramState = TrainingProgramStates.Complete,
				DeliveryMethods = new[] { new DeliveryMethod { Id = 1, Caption = "Delivery Methods" } },
				TotalTrainingHours = 40,
				DateAdded = AppDateTime.UtcNow,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
			return trainingProgram;
		}

		/// <summary>
		/// Creates <typeparamref name="TrainingProvider"/> and <typeparamref name="TrainingProgram"/> and associates them with the specified GrantApplication.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <param name="applicationAdministrator"></param>
		/// <param name="assessor"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplicationWithCosts(GrantOpening grantOpening, User applicationAdministrator, InternalUser assessor, ApplicationStateInternal internalState, decimal estimatedCost = 10000, int estimatedParticipants = 5, int numberOfEligibleCosts = 1)
		{
			var grantApplication = CreateGrantApplication(grantOpening, applicationAdministrator, assessor, internalState);
			var trainingProvider = CreateTrainingProvider(grantApplication);
			var trainingProgram = CreateTrainingProgram(grantApplication, trainingProvider, estimatedParticipants);
			var costs = DivideAmount(estimatedCost, numberOfEligibleCosts);
			var trainingCost = new TrainingCost()
			{
				GrantApplicationId = grantApplication.Id,
				GrantApplication = grantApplication,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA=="),
				EstimatedParticipants = estimatedParticipants
			};
			for (var i = 0; i < numberOfEligibleCosts; i++)
			{
				grantApplication.TrainingCost = trainingCost;
				grantApplication.TrainingCost.EligibleCosts.Add(
					new EligibleCost(
						grantApplication,
						new EligibleExpenseType(
							"ExpenseType",
							new ExpenseType(ExpenseTypes.ParticipantAssigned)
							{
								Id = ExpenseTypes.ParticipantAssigned,
								IsActive = true
							})
						{
							Id = 1,
							IsActive = true
						},
						costs[i], estimatedParticipants)
						);
			}
			grantApplication.RecalculateEstimatedCosts();

			return grantApplication;
		}

		/// <summary>
		/// Creates a new DeliveryPartner for the specified grant program.
		/// </summary>
		/// <param name="grantProgram"></param>
		/// <param name="caption"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static DeliveryPartner CreateDeliveryPartner(GrantProgram grantProgram, string caption = "Delivery Partner", int id = 1)
		{
			return new DeliveryPartner(grantProgram, caption)
			{
				Id = id,
				Caption = caption,
				GrantProgram = grantProgram,
				GrantProgramId = grantProgram.Id,
				IsActive = true,
				RowSequence = id,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		/// <summary>
		/// Split the amount and ensure the remainder is added to the final value.
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="divisor"></param>
		/// <returns></returns>
		private static decimal[] DivideAmount(decimal amount, int divisor)
		{
			var value = Math.Round(amount / divisor, 2);
			var remainder = Math.Round(amount % divisor, 2);

			var values = new List<decimal>();

			for (var i = 0; i < divisor; i++)
			{
				if (i != divisor - 1)
					values.Add(value);
				else
					values.Add(value + remainder);
			}

			return values.ToArray();
		}

		/// <summary>
		/// Creates a new instance of a GrantApplication and initalizes it up to the <typeparamref name="GrantAgreement"/>.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <param name="applicationAdministrator"></param>
		/// <param name="assessor"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplicationWithAgreement(GrantOpening grantOpening, User applicationAdministrator, InternalUser assessor, ApplicationStateInternal internalState)
		{
			var grantApplication = CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, assessor, internalState);
			grantApplication.CopyEstimatedIntoAgreed();
			grantApplication.RecalculateAgreedCosts();
			CreateGrantAgreement(grantApplication);
			return grantApplication;
		}

		/// <summary>
		/// Creates a new instance of a GrantApplication and initalizes it up to the <typeparamref name="GrantAgreement"/>.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <param name="applicationAdministrator"></param>
		/// <param name="assessor"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplicationWithAgreement(User applicationAdministrator, InternalUser assessor, ApplicationStateInternal internalState)
		{
			return CreateGrantApplicationWithAgreement(CreateGrantOpening(), applicationAdministrator, assessor, internalState);
		}

		/// <summary>
		/// Creates a new instance of a GrantApplication and initalizes it up to the <typeparamref name="GrantAgreement"/>.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <param name="applicationAdministrator"></param>
		/// <param name="assessor"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplicationWithAgreement(User applicationAdministrator, ApplicationStateInternal internalState)
		{
			return CreateGrantApplicationWithAgreement(CreateGrantOpening(), applicationAdministrator, null, internalState);
		}

		/// <summary>
		/// Creates a new instance of a GrantApplication and initalizes it up to the <typeparamref name="GrantAgreement"/>.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <param name="applicationAdministrator"></param>
		/// <param name="assessor"></param>
		/// <param name="internalState"></param>
		/// <returns></returns>
		public static GrantApplication CreateGrantApplicationWithAgreement(InternalUser assessor, ApplicationStateInternal internalState)
		{
			return CreateGrantApplicationWithAgreement(CreateGrantOpening(), CreateExternalUser(), assessor, internalState);
		}

		/// <summary>
		/// Creates <typeparamref name="GrantAgreement"/> for the specified GrantApplication.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static GrantAgreement CreateGrantAgreement(GrantApplication grantApplication)
		{
			grantApplication.GrantAgreement = new GrantAgreement(grantApplication)
			{
				ScheduleA = new Document { Title = "scheduleA", Body = "ScheduleA" },
				ScheduleB = new Document { Title = "scheduleB", Body = "ScheduleB" },
				CoverLetter = new Document { Title = "title", Body = "coverletter" }
			};
			return grantApplication.GrantAgreement;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProvider"/> in the requested state and associates it with the specified GrantApplication.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static TrainingProvider CreateTrainingProviderRequest(GrantApplication grantApplication)
		{
			var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
			var trainingProvider = new TrainingProvider(trainingProgram)
			{
				Id = 2,
				TrainingProviderState = TrainingProviderStates.Requested,
				DateAdded = DateTime.UtcNow.AddMinutes(15)
			};
			//grantApplication.TrainingProviders.Add(trainingProvider);
			trainingProgram.TrainingProviders.Add(trainingProvider);
			return trainingProvider;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Claim"/> and includes costs.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		public static Claim CreateClaim(GrantApplication grantApplication, int claimId = 1, int claimVersion = 1)
		{
			var claim = new Claim(claimId, claimVersion, grantApplication);
			var expenseType = new ExpenseType(ExpenseTypes.ParticipantAssigned);
			var eligibleExpenseType = new EligibleExpenseType("ExpenseType", expenseType)
			{
				ExpenseType = expenseType
			};
			var eligibleCost = new ClaimEligibleCost(claim, new EligibleCost(grantApplication, eligibleExpenseType, 1000, 5));
			var participantForm1 = new ParticipantForm(grantApplication, Guid.NewGuid()) { Id = 1 };
			var participantForm2 = new ParticipantForm(grantApplication, Guid.NewGuid()) { Id = 2 };
			var participantForm3 = new ParticipantForm(grantApplication, Guid.NewGuid()) { Id = 3 };
			var participantForm4 = new ParticipantForm(grantApplication, Guid.NewGuid()) { Id = 4 };
			var participantForm5 = new ParticipantForm(grantApplication, Guid.NewGuid()) { Id = 5 };
			eligibleCost.ParticipantCosts.Add(new ParticipantCost(eligibleCost, participantForm1, 200) { RowVersion = new byte[9999] });
			eligibleCost.ParticipantCosts.Add(new ParticipantCost(eligibleCost, participantForm2, 200) { RowVersion = new byte[9999] });
			eligibleCost.ParticipantCosts.Add(new ParticipantCost(eligibleCost, participantForm3, 200) { RowVersion = new byte[9999] });
			eligibleCost.ParticipantCosts.Add(new ParticipantCost(eligibleCost, participantForm4, 200) { RowVersion = new byte[9999] });
			eligibleCost.ParticipantCosts.Add(new ParticipantCost(eligibleCost, participantForm5, 200) { RowVersion = new byte[9999] });
			claim.EligibleCosts.Add(eligibleCost);
			claim.RecalculateClaimedCosts();
			claim.RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==");
			return claim;
		}

		/// <summary>
		/// Creates and returns a new <typeparamref name="User"/> object.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static User CreateExternalUser(int id = 1, AccountTypes accountType = AccountTypes.External)
		{
			return CreateExternalUser(Guid.NewGuid(), id, accountType);
		}

		/// <summary>
		/// Creates and returns a new <typeparamref name="User"/> object.
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static User CreateExternalUser(Guid guid, int id = 1, AccountTypes accountType = AccountTypes.External)
		{
			var naics = CreateNaIndustryClassificationSystem();

			return new User(
				guid,
				"FirstName",
				"LastName",
				$"{guid}@email.com",
				new Organization(
					new OrganizationType("Type"),
					Guid.NewGuid(), "Organization",
					new LegalStructure("Corporation"), 2017, 20, 20, 0, 0)
				{
					Id = 1,
					HeadOfficeAddress = new Address("1234 St", "", "Victoria", "V9V9V9", new Region("BC", "British Columbia", new Country("CAN", "Canada"))),
					Naics = naics,
					NaicsId = naics.Id,
					BusinessDescription = "Description of business",
					RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
				},
					new Address("1234 St", "", "Victoria", "V9V9V9",
						new Region("BC", "British Columbia",
						new Country("CAN", "Canada")))
					{ Id = 1 }
				)
			{
				Id = id,
				PhoneNumber = "(123) 123-1234",
				AccountType = accountType,
				PhysicalAddressId = 1,
				MailingAddressId = 1,
				OrganizationId = 1
			};
		}

		/// <summary>
		/// Creates and returns a new <typeparamref name="InternalUser"/> object.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="idir"></param>
		/// <returns></returns>
		public static InternalUser CreateInternalUser(int id = 1, string idir = null)
		{
			idir = idir ?? RandomString(20);
			return new InternalUser(idir, "FirstName", "LastName", $"{idir}@test.com") { Id = id };
		}

		/// <summary>
		/// Creates a random string.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string RandomString(int length)
		{
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var stringChars = new char[8];
			var random = new Random();

			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}

			return new String(stringChars);
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantApplicationStateChange"/> with the specified stateChangeReason.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="fromState"></param>
		/// <param name="toState"></param>
		/// <param name="applicationAdministrator"></param>
		/// <param name="user"></param>
		/// <param name="stateChangeReason"></param>
		/// <returns></returns>
		public static GrantApplicationStateChange CreateStateChangeLogEntry(GrantApplication grantApplication, ApplicationStateInternal fromState, ApplicationStateInternal toState, User applicationAdministrator, InternalUser user, string stateChangeReason)
		{
			var stateChange = new GrantApplicationStateChange()
			{
				GrantApplicationId = grantApplication.Id,
				FromState = fromState,
				ToState = toState,
				ChangedDate = AppDateTime.UtcNow,
				ApplicationAdministrator = applicationAdministrator,
				ApplicationAdministratorId = applicationAdministrator?.Id,
				Assessor = user,
				AssessorId = user?.Id,
				Reason = stateChangeReason
			};

			return stateChange;
		}

		public static CompletionReportGroup CreateCompletionReportGroup()
		{
			var completionReportGroup = new CompletionReportGroup()
			{
				Id = 1,
				Title = "Completion Report Group Test",
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
			return completionReportGroup;
		}

		public static CompletionReport CreateCompletionReport()
		{
			var completionReport = new CompletionReport()
			{
				Id = 1,
				Caption = "Completion Report Test",
				Description = "Completion Report Description Test",
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
			return completionReport;
		}

		public static CompletionReportQuestion CreateCompletionReportQuestion()
		{
			var completionReportQuestion = new CompletionReportQuestion()
			{
				Id = 1,
				Question = "Completion Report Question Test",
				Description = "Completion Report Question Description Test",
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
			return completionReportQuestion;
		}

		public static ParticipantForm CreateParticipantForm(GrantApplication grantApplication)
		{
			return new ParticipantForm
			{
				Id = 1,
				GrantApplicationId = grantApplication.Id,
				FirstName = "First Name Test",
				LastName = "Last Name Test",
				GrantApplication = grantApplication,
				RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
			};
		}

		public static Region CreateRegion(string id = "BC", string name = "British Columbia", string countryId = "CA", string countryName = "Canada")
		{
			return new Region(id, name, new Country(countryId, countryName));
		}

		public static OrganizationType CreateOrganizationType(string caption = "Type", int id = 1)
		{
			return new OrganizationType() {
				Id = id,
				Caption = caption
			};
		}
		public static Organization CreateOrganization(string organizationType = "Type", int id = 1)
		{
			var naics = new NaIndustryClassificationSystem("TEST", "test", 1, null, 0, 1, 2012) { Id = 1 };
			return new Organization(
					new OrganizationType(organizationType),
					Guid.NewGuid(), "Organization",
					new LegalStructure("Corporation"), 2017, 20, 20, 0, 0)
					{
					Id = 1,
					HeadOfficeAddress = new Address("1234 St", "", "Victoria", "V9V9V9", new Region("BC", "British Columbia", new Country("CAN", "Canada"))),
					Naics = naics,
					NaicsId = naics.Id,
					RowVersion = Convert.FromBase64String("AgQGCAoMDhASFA==")
					};
		}
		public static LegalStructure CreateLegalStructure(string caption = "Corporation", int id = 1)
		{
			return new LegalStructure()
			{
				Id = id,
				Caption = caption
			};
		}

		public static RiskClassification CreateRiskClassification(string caption = "RiskClassification", int id = 1)
		{
			return new RiskClassification()
			{
				Id = id,
				Caption = caption
			};
		}

		public static RateFormat CreateRateFormat(string Format = "Format", double Rate = 10.78)
		{
			return new RateFormat()
			{
				Rate = Rate,
				Format = Format
			};
		}
		#endregion
	}
}
