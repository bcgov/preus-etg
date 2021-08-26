using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CJG.Web.External.Models.Shared.SkillsTrainings
{
	public class SkillTrainingDetailsViewModel : BaseTrainingProgramViewModel
	{
		#region Properties
		public int? EligibleExpenseTypeId { get; set; }

		[Required(ErrorMessage = "Skill Training Focus is required")]
		public int? EligibleExpenseBreakdownId { get; set; }

		[Required(ErrorMessage = "Essential Skills Type is required")]
		public int? ServiceLineBreakdownId { get; set; }
		public string ServiceLineBreakdownCaption { get; set; }
		public SkillTrainingProviderDetailsViewModel TrainingProvider { get; set; }
		public decimal TotalCost { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="SkillTrainingModel"/> object.
		/// </summary>
		public SkillTrainingDetailsViewModel() { }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="SkillTrainingModel"/> class and initializes it.
		/// </summary>
		/// <param name="grantApplication"></param>
		public SkillTrainingDetailsViewModel(GrantApplication grantApplication)
		{
			this.GrantApplicationId = grantApplication.Id;
			this.DeliveryStartDate = grantApplication.StartDate.ToLocalTime();
			this.DeliveryEndDate = grantApplication.EndDate.ToLocalTime();
			this.TrainingProvider = new SkillTrainingProviderDetailsViewModel();
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="SkillTrainingModel"/> class and initializes it.
		/// </summary>
		/// <param name="trainingProgram"></param>
		public SkillTrainingDetailsViewModel(TrainingProgram trainingProgram)
		{
			this.Id = trainingProgram.Id;
			this.EligibleExpenseTypeId = trainingProgram.EligibleCostBreakdown.EligibleExpenseBreakdown.EligibleExpenseTypeId;
			this.RowVersion = Convert.ToBase64String(trainingProgram.RowVersion);
			this.GrantApplicationId = trainingProgram.GrantApplicationId;
			this.EligibleExpenseBreakdownId = trainingProgram.EligibleCostBreakdown?.EligibleExpenseBreakdownId;
			this.ServiceLineBreakdownId = trainingProgram.ServiceLineBreakdownId;
			this.ServiceLineBreakdownCaption = (trainingProgram.ServiceLine != null) ? trainingProgram.ServiceLine.BreakdownCaption : "";
			this.ExpectedQualificationId = trainingProgram.ExpectedQualificationId;
			this.SkillLevelId = trainingProgram.SkillLevelId;
			this.CourseTitle = trainingProgram.CourseTitle;
			this.TotalTrainingHours = trainingProgram.TotalTrainingHours;
			this.TitleOfQualification = trainingProgram.TitleOfQualification;
			this.StartDate = trainingProgram.StartDate.ToLocalTime();
			this.StartYear = this.StartDate.Value.Year;
			this.StartMonth = this.StartDate.Value.Month;
			this.StartDay = this.StartDate.Value.Day;
			this.EndDate = trainingProgram.EndDate.ToLocalTime();
			this.EndYear = this.EndDate.Value.Year;
			this.EndMonth = this.EndDate.Value.Month;
			this.EndDay = this.EndDate.Value.Day;
			this.SelectedDeliveryMethodIds = trainingProgram.DeliveryMethods.Select(x => x.Id).ToArray();
			this.DeliveryStartDate = trainingProgram.GrantApplication.StartDate.ToLocalTime();
			this.DeliveryEndDate = trainingProgram.GrantApplication.EndDate.ToLocalTime();
			this.TotalCost = trainingProgram.EligibleCostBreakdown?.EstimatedCost ?? 0;
			this.TrainingProvider = new SkillTrainingProviderDetailsViewModel(trainingProgram.TrainingProvider)
			{
				IsApproved = trainingProgram.RequestedTrainingProvider?.Id > 0
			};
		}

		#endregion

		#region Methods
		/// <summary>
		/// Map the model properties to the appropirate training program.
		/// Also Add/Update the eligible cost breakdown and attachments associated with this component.
		/// </summary>
		/// <param name="_grantApplicationService"></param>
		/// <param name="_trainingProgramService"></param>
		/// <param name="_trainingProviderService"></param>
		/// <param name="_attachmentService"></param>
		/// <param name="_applicationAddressService"></param>
		/// <param name="_eligibleExpenseBreakdownService"></param>
		/// <param name="_serviceLineBreakdownService"></param>
		/// <param name="_staticDataService"></param>
		/// <param name="user"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		public TrainingProgram MapProperties(IGrantApplicationService _grantApplicationService,
										  ITrainingProgramService _trainingProgramService,
										  ITrainingProviderService _trainingProviderService,
										  IAttachmentService _attachmentService,
										  IApplicationAddressService _applicationAddressService,
										  IEligibleExpenseBreakdownService _eligibleExpenseBreakdownService,
										  IServiceLineBreakdownService _serviceLineBreakdownService,
										  IStaticDataService _staticDataService,
										  IPrincipal user,
										  HttpPostedFileBase[] files = null)
		{
			if (_grantApplicationService == null) throw new ArgumentNullException(nameof(_grantApplicationService));
			if (_trainingProgramService == null) throw new ArgumentNullException(nameof(_trainingProgramService));
			if (_trainingProviderService == null) throw new ArgumentNullException(nameof(_trainingProviderService));
			if (_attachmentService == null) throw new ArgumentNullException(nameof(_attachmentService));
			if (_applicationAddressService == null) throw new ArgumentNullException(nameof(_applicationAddressService));
			if (_staticDataService == null) throw new ArgumentNullException(nameof(_staticDataService));
			if (user == null) throw new ArgumentNullException(nameof(user));

			var create = this.Id == 0;
			var trainingProgram = !create ? _trainingProgramService.Get(this.Id) : new TrainingProgram();
			var grantApplication = !create ? trainingProgram.GrantApplication : _grantApplicationService.Get(this.GrantApplicationId);

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
				trainingProgram.RowVersion = Convert.FromBase64String(this.RowVersion);
			}

			trainingProgram.StartDate = new DateTime(this.StartYear, this.StartMonth, this.StartDay, 0, 0, 0, DateTimeKind.Local).ToUtcMorning();
			trainingProgram.EndDate = new DateTime(this.EndYear, this.EndMonth, this.EndDay, 0, 0, 0, DateTimeKind.Local).ToUtcMidnight();
			trainingProgram.CourseTitle = this.CourseTitle;

			// Only add/remove the specified delivery methods.
			if (this.SelectedDeliveryMethodIds != null && this.SelectedDeliveryMethodIds.Any())
			{
				var thisIds = this.SelectedDeliveryMethodIds.ToArray();
				var currentIds = trainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
				var removeIds = currentIds.Except(thisIds);
				var addIds = thisIds.Except(currentIds).Except(removeIds);

				foreach (var removeId in removeIds)
				{
					var deliveryMethod = _staticDataService.GetDeliveryMethod(removeId);
					trainingProgram.DeliveryMethods.Remove(deliveryMethod);
				}

				foreach (var addId in addIds)
				{
					var deliveryMethod = _staticDataService.GetDeliveryMethod(addId);
					trainingProgram.DeliveryMethods.Add(deliveryMethod);
				}
			}
			else
			{
				trainingProgram.DeliveryMethods.Clear();
			}

			trainingProgram.TotalTrainingHours = this.TotalTrainingHours.Value;
			trainingProgram.SkillLevelId = this.SkillLevelId.Value;

			trainingProgram.ExpectedQualificationId = this.ExpectedQualificationId.Value;
			if (new int[] { 5 }.Contains(this.ExpectedQualificationId.GetValueOrDefault()))
			{
				trainingProgram.TitleOfQualification = null;
			}
			else
			{
				trainingProgram.TitleOfQualification = this.TitleOfQualification;
			}

			// TrainingProvider
			var trainingProvider = this.TrainingProvider.Id > 0 ? _trainingProviderService.Get(this.TrainingProvider.Id) : new TrainingProvider(trainingProgram);

			trainingProvider.Name = this.TrainingProvider.Name;
			trainingProvider.TrainingProviderTypeId = this.TrainingProvider.TrainingProviderTypeId.Value;
			trainingProvider.TrainingProviderState = TrainingProviderStates.Complete;
			trainingProvider.ContactPhoneNumber = this.TrainingProvider.ContactPhone;
			trainingProvider.ContactPhoneExtension = this.TrainingProvider.ContactPhoneExtension;
			trainingProvider.ContactEmail = this.TrainingProvider.ContactEmail;
			trainingProvider.ContactFirstName = this.TrainingProvider.ContactFirstName;
			trainingProvider.ContactLastName = this.TrainingProvider.ContactLastName;
			if (trainingProvider.TrainingAddress == null)
				trainingProvider.TrainingAddress = new ApplicationAddress();

			trainingProvider.TrainingAddress.AddressLine1 = this.TrainingProvider.AddressLine1;
			trainingProvider.TrainingAddress.AddressLine2 = this.TrainingProvider.AddressLine2;
			trainingProvider.TrainingAddress.City = this.TrainingProvider.City;
			if (this.TrainingProvider.IsCanadianAddress)
			{
				trainingProvider.TrainingAddress.RegionId = this.TrainingProvider.RegionId;
			}
			else
			{
				var region = _applicationAddressService.VerifyOrCreateRegion(this.TrainingProvider.OtherRegion, this.TrainingProvider.CountryId);
				trainingProvider.TrainingAddress.RegionId = region.Id;
			}

			trainingProvider.TrainingAddress.PostalCode = this.TrainingProvider.IsCanadianAddress ? this.TrainingProvider.PostalCode : this.TrainingProvider.OtherZipCode;
			trainingProvider.TrainingAddress.CountryId = this.TrainingProvider.CountryId;
			trainingProvider.TrainingOutsideBC = this.TrainingProvider.TrainingOutsideBC.Value;

			if (!trainingProvider.TrainingOutsideBC)
			{
				this.TrainingProvider.BusinessCaseDocument.Id = 0;
				this.TrainingProvider.BusinessCase = "";
			}

			var trainingProviderType = _staticDataService.GetTrainingProviderType(trainingProvider.TrainingProviderTypeId);
			if (trainingProviderType.PrivateSectorValidationType == TrainingProviderPrivateSectorValidationTypes.Never)
			{
				this.TrainingProvider.CourseOutlineDocument.Id = 0;
				this.TrainingProvider.ProofOfQualificationsDocument.Id = 0;
			}

			trainingProvider.BusinessCase = this.TrainingProvider.BusinessCase;
			trainingProvider.TrainingProviderType = trainingProviderType;
			this.TrainingProvider.UpdateAttachment(trainingProvider, _attachmentService, files);

			if (trainingProvider.Id != 0)
				trainingProvider.RowVersion = Convert.FromBase64String(this.TrainingProvider.RowVersion);

			// TrainingCost
			if (grantApplication.TrainingCost == null)
				grantApplication.TrainingCost = new TrainingCost(grantApplication, 1);

			var eligibleExpenseBreakdown = _eligibleExpenseBreakdownService.Get(this.EligibleExpenseBreakdownId.Value);

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
				if (internalUser) trainingProgram.EligibleCostBreakdown = new EligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown, 0) { AssessedCost = this.TotalCost };
				else trainingProgram.EligibleCostBreakdown = new EligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown, this.TotalCost);
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
				if (internalUser) trainingProgram.EligibleCostBreakdown.AssessedCost = this.TotalCost;
				else trainingProgram.EligibleCostBreakdown.EstimatedCost = this.TotalCost;
				trainingProgram.EligibleCostBreakdown.EligibleExpenseBreakdownId = this.EligibleExpenseBreakdownId.Value;
			}

			if (internalUser && trainingProgram.EligibleCostBreakdown.IsEligible && this.TotalCost == 0)
				throw new InvalidOperationException("Program total assessed cost is required.");

			if (!internalUser &&
				grantApplication.TrainingCost.TrainingCostState == TrainingCostStates.Complete &&
				(trainingProgram.EligibleCostBreakdownId == null && trainingProgram.EligibleCostBreakdown == null ||
				 trainingProgram.EligibleCostBreakdown.EstimatedCost != this.TotalCost))
				grantApplication.TrainingCost.TrainingCostState = TrainingCostStates.Incomplete;

			if (trainingProgram.ServiceLineId == null && trainingProgram.ServiceLine == null)
			{
				trainingProgram.ServiceLine = eligibleExpenseBreakdown.ServiceLine;
			}
			else
			{
				trainingProgram.ServiceLineId = eligibleExpenseBreakdown.ServiceLineId;
			}

			var serviceLineBreakdowns = this.ServiceLineBreakdownId > 0 ? _serviceLineBreakdownService.Get(this.ServiceLineBreakdownId.Value) : null;

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

			// Agreed commitment cannot exceed 10% unless user is a Director
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
		#endregion
	}
}
