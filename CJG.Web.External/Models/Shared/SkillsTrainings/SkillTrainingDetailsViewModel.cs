using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using Constants = CJG.Core.Entities.Constants;

namespace CJG.Web.External.Models.Shared.SkillsTrainings
{
    public class SkillTrainingDetailsViewModel : BaseTrainingProgramViewModel
	{
		public int? EligibleExpenseTypeId { get; set; }

		[Required(ErrorMessage = "Skill Training Focus is required")]
		public int? EligibleExpenseBreakdownId { get; set; }

		[Required(ErrorMessage = "Essential Skills Type is required")]
		public int? ServiceLineBreakdownId { get; set; }
		public string ServiceLineBreakdownCaption { get; set; }
		public SkillTrainingProviderDetailsViewModel TrainingProvider { get; set; }
		public decimal TotalCost { get; set; }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="SkillTrainingDetailsViewModel"/> object.
		/// </summary>
		public SkillTrainingDetailsViewModel() { }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="SkillTrainingDetailsViewModel"/> class and initializes it.
		/// </summary>
		/// <param name="grantApplication"></param>
		public SkillTrainingDetailsViewModel(GrantApplication grantApplication)
		{
			GrantApplicationId = grantApplication.Id;
			DeliveryStartDate = grantApplication.StartDate.ToLocalTime();
			DeliveryEndDate = grantApplication.EndDate.ToLocalTime();
			TrainingProvider = new SkillTrainingProviderDetailsViewModel();
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="SkillTrainingDetailsViewModel"/> class and initializes it.
		/// </summary>
		/// <param name="trainingProgram"></param>
		public SkillTrainingDetailsViewModel(TrainingProgram trainingProgram)
		{
			Id = trainingProgram.Id;
			EligibleExpenseTypeId = trainingProgram.EligibleCostBreakdown.EligibleExpenseBreakdown.EligibleExpenseTypeId;
			RowVersion = Convert.ToBase64String(trainingProgram.RowVersion);
			GrantApplicationId = trainingProgram.GrantApplicationId;
			EligibleExpenseBreakdownId = trainingProgram.EligibleCostBreakdown?.EligibleExpenseBreakdownId;
			ServiceLineBreakdownId = trainingProgram.ServiceLineBreakdownId;
			ServiceLineBreakdownCaption = (trainingProgram.ServiceLine != null) ? trainingProgram.ServiceLine.BreakdownCaption : "";
			ExpectedQualificationId = trainingProgram.ExpectedQualificationId;
			SkillLevelId = trainingProgram.SkillLevelId;
			CourseTitle = trainingProgram.CourseTitle;
			TotalTrainingHours = trainingProgram.TotalTrainingHours;
			TitleOfQualification = trainingProgram.TitleOfQualification;
			StartDate = trainingProgram.StartDate.ToLocalTime();
			StartYear = StartDate.Value.Year;
			StartMonth = StartDate.Value.Month;
			StartDay = StartDate.Value.Day;
			EndDate = trainingProgram.EndDate.ToLocalTime();
			EndYear = EndDate.Value.Year;
			EndMonth = EndDate.Value.Month;
			EndDay = EndDate.Value.Day;
			SelectedDeliveryMethodIds = trainingProgram.DeliveryMethods.Select(x => x.Id).ToArray();
			DeliveryStartDate = trainingProgram.GrantApplication.StartDate.ToLocalTime();
			DeliveryEndDate = trainingProgram.GrantApplication.EndDate.ToLocalTime();
			TotalCost = trainingProgram.EligibleCostBreakdown?.EstimatedCost ?? 0;
			TrainingProvider = new SkillTrainingProviderDetailsViewModel(trainingProgram.TrainingProvider)
			{
				IsApproved = trainingProgram.RequestedTrainingProvider?.Id > 0
			};
		}

		/// <summary>
		/// Map the model properties to the appropriate training program.
		/// Also Add/Update the eligible cost breakdown and attachments associated with this component.
		/// </summary>
		/// <param name="grantApplicationService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="trainingProviderService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="applicationAddressService"></param>
		/// <param name="eligibleExpenseBreakdownService"></param>
		/// <param name="serviceLineBreakdownService"></param>
		/// <param name="staticDataService"></param>
		/// <param name="user"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		public TrainingProgram MapProperties(IGrantApplicationService grantApplicationService,
										  ITrainingProgramService trainingProgramService,
										  ITrainingProviderService trainingProviderService,
										  IAttachmentService attachmentService,
										  IApplicationAddressService applicationAddressService,
										  IEligibleExpenseBreakdownService eligibleExpenseBreakdownService,
										  IServiceLineBreakdownService serviceLineBreakdownService,
										  IStaticDataService staticDataService,
										  IPrincipal user,
										  HttpPostedFileBase[] files = null)
		{
			if (grantApplicationService == null)
				throw new ArgumentNullException(nameof(grantApplicationService));

			if (trainingProgramService == null)
				throw new ArgumentNullException(nameof(trainingProgramService));

			if (trainingProviderService == null)
				throw new ArgumentNullException(nameof(trainingProviderService));

			if (attachmentService == null)
				throw new ArgumentNullException(nameof(attachmentService));

			if (applicationAddressService == null)
				throw new ArgumentNullException(nameof(applicationAddressService));

			if (staticDataService == null)
				throw new ArgumentNullException(nameof(staticDataService));

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			var create = Id == 0;
			var trainingProgram = !create ? trainingProgramService.Get(Id) : new TrainingProgram();
			var grantApplication = !create ? trainingProgram.GrantApplication : grantApplicationService.Get(GrantApplicationId);

			if (create)
			{
				if (!user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram))
					throw new NotAuthorizedException($"User does not have permission add the training program in Grant Application '{grantApplication?.Id}'.");
			}
			else
			{
				if (!user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditTrainingProgram))
					throw new NotAuthorizedException($"User does not have permission edit the training program in Grant Application '{grantApplication?.Id}'.");
			}

			if (!create)
			{
				trainingProgram.RowVersion = Convert.FromBase64String(RowVersion);
			}

			trainingProgram.StartDate = new DateTime(StartYear, StartMonth, StartDay, 0, 0, 0, DateTimeKind.Local).ToUtcMorning();
			trainingProgram.EndDate = new DateTime(EndYear, EndMonth, EndDay, 0, 0, 0, DateTimeKind.Local).ToUtcMidnight();
			trainingProgram.CourseTitle = CourseTitle;

			// Only add/remove the specified delivery methods.
			if (SelectedDeliveryMethodIds != null && SelectedDeliveryMethodIds.Any())
			{
				var thisIds = SelectedDeliveryMethodIds.ToArray();
				var currentIds = trainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
				var removeIds = currentIds.Except(thisIds);
				var addIds = thisIds.Except(currentIds).Except(removeIds);

				foreach (var removeId in removeIds)
				{
					var deliveryMethod = staticDataService.GetDeliveryMethod(removeId);
					trainingProgram.DeliveryMethods.Remove(deliveryMethod);
				}

				foreach (var addId in addIds)
				{
					var deliveryMethod = staticDataService.GetDeliveryMethod(addId);
					trainingProgram.DeliveryMethods.Add(deliveryMethod);
				}
			}
			else
			{
				trainingProgram.DeliveryMethods.Clear();
			}

			trainingProgram.TotalTrainingHours = TotalTrainingHours.Value;
			trainingProgram.SkillLevelId = SkillLevelId.Value;

			trainingProgram.ExpectedQualificationId = ExpectedQualificationId.Value;
			if (new[] { Constants.ExpectedQualifications_None }.Contains(ExpectedQualificationId.GetValueOrDefault()))
			{
				trainingProgram.TitleOfQualification = null;
			}
			else
			{
				trainingProgram.TitleOfQualification = TitleOfQualification;
			}

			// TrainingProvider
			var trainingProvider = TrainingProvider.Id > 0 ? trainingProviderService.Get(TrainingProvider.Id) : new TrainingProvider(trainingProgram);

			trainingProvider.Name = TrainingProvider.Name;
			trainingProvider.TrainingProviderTypeId = TrainingProvider.TrainingProviderTypeId.Value;
			trainingProvider.TrainingProviderState = TrainingProviderStates.Complete;
			trainingProvider.ContactPhoneNumber = TrainingProvider.ContactPhone;
			trainingProvider.ContactPhoneExtension = TrainingProvider.ContactPhoneExtension;
			trainingProvider.ContactEmail = TrainingProvider.ContactEmail;
			trainingProvider.ContactFirstName = TrainingProvider.ContactFirstName;
			trainingProvider.ContactLastName = TrainingProvider.ContactLastName;

			if (SelectedDeliveryMethodIds == null)
				SelectedDeliveryMethodIds = new int[0];

			MapProviderAndLocationAddress(applicationAddressService, trainingProvider);

			trainingProvider.TrainingOutsideBC = TrainingProvider.TrainingOutsideBC.Value;

			if (!trainingProvider.TrainingOutsideBC)
			{
				TrainingProvider.BusinessCaseDocument.Id = 0;
				TrainingProvider.BusinessCase = "";
			}

			var trainingProviderType = staticDataService.GetTrainingProviderType(trainingProvider.TrainingProviderTypeId);
			if (trainingProviderType.PrivateSectorValidationType == TrainingProviderPrivateSectorValidationTypes.Never)
			{
				TrainingProvider.CourseOutlineDocument.Id = 0;
				TrainingProvider.ProofOfQualificationsDocument.Id = 0;
			}

			trainingProvider.BusinessCase = TrainingProvider.BusinessCase;
			trainingProvider.TrainingProviderType = trainingProviderType;
			TrainingProvider.UpdateAttachment(trainingProvider, attachmentService, files);

			if (trainingProvider.Id != 0)
				trainingProvider.RowVersion = Convert.FromBase64String(TrainingProvider.RowVersion);

			// TrainingCost
			if (grantApplication.TrainingCost == null)
				grantApplication.TrainingCost = new TrainingCost(grantApplication, 1);

			var eligibleExpenseBreakdown = eligibleExpenseBreakdownService.Get(EligibleExpenseBreakdownId.Value);

			var eligibleCost = grantApplication.TrainingCost.EligibleCosts.FirstOrDefault(x => x.EligibleExpenseTypeId == eligibleExpenseBreakdown.EligibleExpenseTypeId);
			if (eligibleCost == null)
			{
				eligibleCost = new EligibleCost(grantApplication.TrainingCost, eligibleExpenseBreakdown.ExpenseType, 0, grantApplication.TrainingCost.EstimatedParticipants);
				grantApplication.TrainingCost.EligibleCosts.Add(eligibleCost);
			}

			var internalUser = user.GetAccountType() != AccountTypes.External;

			// This is a new Training Program and requires a matching Eligible Cost Breakdown.
			if (trainingProgram.EligibleCostBreakdownId == null && trainingProgram.EligibleCostBreakdown == null)
			{
				if (internalUser) trainingProgram.EligibleCostBreakdown = new EligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown, 0) { AssessedCost = TotalCost };
				else trainingProgram.EligibleCostBreakdown = new EligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown, TotalCost);
				eligibleCost.Breakdowns.Add(trainingProgram.EligibleCostBreakdown);

				// If there is an active claim being worked on by the applicant, update it with the changes made to this skills training component.
				var claim = grantApplication.GetCurrentClaim();
				if (claim?.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete) ?? false)
				{
					// This should never return a null value.  
					var claimEligibleCost = claim.EligibleCosts.FirstOrDefault(cec => cec.EligibleCostId == eligibleCost.Id);
					if (claimEligibleCost != null)
					{
						// Add the new breakdown (skills training component) to the claim eligible cost.
						var claimBreakdownCost = new ClaimBreakdownCost(trainingProgram.EligibleCostBreakdown, claimEligibleCost);
						claimEligibleCost.Breakdowns.Add(claimBreakdownCost);
						claim.RecalculateClaimedCosts();
						claim.ClaimState = ClaimState.Incomplete;
					}
				}
			}
			else
			{
				if (internalUser) trainingProgram.EligibleCostBreakdown.AssessedCost = TotalCost;
				else trainingProgram.EligibleCostBreakdown.EstimatedCost = TotalCost;
				trainingProgram.EligibleCostBreakdown.EligibleExpenseBreakdownId = EligibleExpenseBreakdownId.Value;
			}

			if (internalUser && trainingProgram.EligibleCostBreakdown.IsEligible && TotalCost == 0)
				throw new InvalidOperationException("Program total assessed cost is required.");

			if (!internalUser &&
				grantApplication.TrainingCost.TrainingCostState == TrainingCostStates.Complete &&
				(trainingProgram.EligibleCostBreakdownId == null && trainingProgram.EligibleCostBreakdown == null ||
				 trainingProgram.EligibleCostBreakdown.EstimatedCost != TotalCost))
				grantApplication.TrainingCost.TrainingCostState = TrainingCostStates.Incomplete;

			if (trainingProgram.ServiceLineId == null && trainingProgram.ServiceLine == null)
			{
				trainingProgram.ServiceLine = eligibleExpenseBreakdown.ServiceLine;
			}
			else
			{
				trainingProgram.ServiceLineId = eligibleExpenseBreakdown.ServiceLineId;
			}

			var serviceLineBreakdowns = ServiceLineBreakdownId > 0 ? serviceLineBreakdownService.Get(ServiceLineBreakdownId.Value) : null;

			if (trainingProgram.ServiceLineBreakdownId == null && trainingProgram.ServiceLineBreakdown == null)
			{
				trainingProgram.ServiceLineBreakdown = serviceLineBreakdowns;
			}
			else
			{
				trainingProgram.ServiceLineBreakdownId = serviceLineBreakdowns?.Id;
			}

			if (!internalUser)
			{
				eligibleCost.EstimatedCost = eligibleCost.CalculateEstimateCost();
				eligibleCost.EstimatedReimbursement = eligibleCost.CalculateEstimatedReimbursement();
				eligibleCost.EstimatedParticipantCost = eligibleCost.CalculateEstimatedParticipantCost();

				grantApplication.TrainingCost.TotalEstimatedCost = grantApplication.TrainingCost.CalculatedTotalEstimatedCost();
				grantApplication.TrainingCost.TotalEstimatedReimbursement = grantApplication.TrainingCost.CalculateTotalEstimatedReimbursement();
			}
			else
			{
				eligibleCost.AgreedMaxCost = eligibleCost.CalculateAgreedMaxCost();
				eligibleCost.AgreedMaxReimbursement = eligibleCost.CalculateAgreedReimbursement();
				eligibleCost.AgreedEmployerContribution = eligibleCost.CalculateAgreedEmployerContribution();

				grantApplication.TrainingCost.TotalAgreedMaxCost = grantApplication.TrainingCost.CalculatedTotalAgreedMaxCost();
				grantApplication.TrainingCost.AgreedCommitment = grantApplication.TrainingCost.CalculateAgreedMaxReimbursement();
			}

			// Agreed commitment cannot exceed 10% unless user is a Director or Assessor
			if (grantApplication.TrainingCost.DoesAgreedCommitmentExceedEstimatedContribution() && !user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditTrainingCostOverride))
				throw new InvalidOperationException("You may not increase the assessed total government contribution more than 10% over the estimated total government contribution.");

			if ((grantApplication.ApplicationStateExternal == ApplicationStateExternal.ApplicationWithdrawn
				&& grantApplication.ApplicationStateInternal == ApplicationStateInternal.ApplicationWithdrawn)
				|| grantApplication.ApplicationStateExternal == ApplicationStateExternal.NotAccepted)
			{
				// This GrantApplication was withdrawn or returned as not accepted, and can now be edited for resubmission
				grantApplication.ApplicationStateInternal = ApplicationStateInternal.Draft;
				grantApplication.ApplicationStateExternal = ApplicationStateExternal.Incomplete;

				// remove the file number
				grantApplication.FileNumber = string.Empty;

				// also remove the assigned assessor
				grantApplication.Assessor = null;
			}

			trainingProgram.TrainingProgramState = TrainingProgramStates.Complete;

			if (trainingProgram.Id == 0)
			{
				trainingProgram.GrantApplicationId = grantApplication.Id;
				trainingProgram.GrantApplication = grantApplication;
			}

			return trainingProgram;
		}

		private void MapProviderAndLocationAddress(IApplicationAddressService applicationAddressService, TrainingProvider trainingProvider)
		{
			if (trainingProvider.TrainingProviderAddress == null)
				trainingProvider.TrainingProviderAddress = new ApplicationAddress();

			trainingProvider.TrainingProviderAddress.AddressLine1 = TrainingProvider.AddressLine1TrainingProvider;
			trainingProvider.TrainingProviderAddress.AddressLine2 = TrainingProvider.AddressLine2TrainingProvider;
			trainingProvider.TrainingProviderAddress.City = TrainingProvider.CityTrainingProvider;

			Region nonCanadianRegion = null;

			if (TrainingProvider.IsCanadianAddressTrainingProvider)
			{
				trainingProvider.TrainingProviderAddress.RegionId = TrainingProvider.RegionIdTrainingProvider;
				trainingProvider.TrainingProviderAddress.PostalCode = TrainingProvider.PostalCodeTrainingProvider;
				trainingProvider.TrainingProviderAddress.CountryId = Constants.CanadaCountryId;
			}
			else
			{
				nonCanadianRegion = applicationAddressService.VerifyOrCreateRegion(TrainingProvider.OtherRegionTrainingProvider, TrainingProvider.CountryIdTrainingProvider);
				trainingProvider.TrainingProviderAddress.RegionId = nonCanadianRegion.Id;
				trainingProvider.TrainingProviderAddress.PostalCode = TrainingProvider.OtherZipCodeTrainingProvider;
				trainingProvider.TrainingProviderAddress.CountryId = TrainingProvider.CountryIdTrainingProvider;
			}

			if (SelectedDeliveryMethodIds != null
			    && (SelectedDeliveryMethodIds.Contains(Constants.Delivery_Classroom) || SelectedDeliveryMethodIds.Contains(Constants.Delivery_Workplace)))
			{
				if (trainingProvider.TrainingAddress == null)
					trainingProvider.TrainingAddress = new ApplicationAddress();

				trainingProvider.TrainingAddress.AddressLine1 = TrainingProvider.AddressLine1;
				trainingProvider.TrainingAddress.AddressLine2 = TrainingProvider.AddressLine2;
				trainingProvider.TrainingAddress.City = TrainingProvider.City;

				if (TrainingProvider.IsCanadianAddress)
				{
					trainingProvider.TrainingAddress.RegionId = TrainingProvider.RegionId;
					trainingProvider.TrainingAddress.PostalCode = TrainingProvider.PostalCode;
					trainingProvider.TrainingAddress.CountryId = Constants.CanadaCountryId;
				}
				else
				{
					// We have to get the existing region if it's the same for both addresses, otherwise we get an EF error
					var region = GetInternationalRegion(TrainingProvider.CountryId, TrainingProvider.OtherRegion,
						TrainingProvider.CountryIdTrainingProvider, TrainingProvider.OtherRegionTrainingProvider,
						nonCanadianRegion, applicationAddressService);
					trainingProvider.TrainingAddress.RegionId = region.Id;
					trainingProvider.TrainingAddress.PostalCode = TrainingProvider.OtherZipCode;
					trainingProvider.TrainingAddress.CountryId = TrainingProvider.CountryId;
				}
			}
		}

		private Region GetInternationalRegion(string targetCountry, string targetRegion, string sourceCountry, string sourceRegion, Region existingRegion, IApplicationAddressService applicationAddressService)
		{
			if (targetCountry == sourceCountry && targetRegion.Equals(sourceRegion, StringComparison.CurrentCultureIgnoreCase))
				return existingRegion;

			return applicationAddressService.VerifyOrCreateRegion(targetRegion, targetCountry);
		}
	}
}
