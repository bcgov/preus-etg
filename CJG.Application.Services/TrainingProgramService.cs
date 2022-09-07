using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="TrainingProgramService"/> class, provides methods to manage training programs in the datasource.
	/// </summary>
	public class TrainingProgramService : Service, ITrainingProgramService
	{
		#region Variables
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantAgreementService _grantAgreementService;
		private readonly INoteService _noteService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProgramService"/> class.
		/// </summary>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantAgreementService"></param>
		/// <param name="noteService"></param>
		/// <param name="dbContext"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public TrainingProgramService(IGrantApplicationService grantApplicationService,
									  IGrantAgreementService grantAgreementService,
									  INoteService noteService,
									  IDataContext dbContext,
									  HttpContextBase httpContext,
									  ILogger logger) : base(dbContext, httpContext, logger)
		{
			_grantApplicationService = grantApplicationService;
			_grantAgreementService = grantAgreementService;
			_noteService = noteService;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Get the training program eligible cost section in the database
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public TrainingProgram Get(int id)
		{
			var trainingProgram = Get<TrainingProgram>(id);

			if (!_httpContext.User.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.ViewApplication))
				throw new NotAuthorizedException($"User does not have permission to access application {trainingProgram.GrantApplicationId}.");

			return trainingProgram;
		}

		/// <summary>
		/// Adds and removes delivery methods from the specified training program.
		/// This method does not make the change to the datasource, you need to commit after this.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <param name="selectedDeliveryMethodIds"></param>
		public void UpdateDeliveryMethods(TrainingProgram trainingProgram, int[] selectedDeliveryMethodIds)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));

			if (selectedDeliveryMethodIds == null)
			{
				trainingProgram.DeliveryMethods.Clear();
				return;
			}

			var removeIds = trainingProgram.DeliveryMethods.Where(dm => !selectedDeliveryMethodIds.Contains(dm.Id)).Select(dm => dm.Id).ToArray();
			var addIds = selectedDeliveryMethodIds.Where(id => !trainingProgram.DeliveryMethods.Any(dm => dm.Id == id));

			removeIds.ForEach(id =>
			{
				var deliveryMethod = trainingProgram.DeliveryMethods.FirstOrDefault(dm => dm.Id == id);
				trainingProgram.DeliveryMethods.Remove(deliveryMethod);
			});

			addIds.ForEach(id =>
			{
				var deliveryMethod = _dbContext.DeliveryMethods.FirstOrDefault(dm => dm.Id == id);
				if (deliveryMethod != null) trainingProgram.DeliveryMethods.Add(deliveryMethod);
			});
		}

		/// <summary>
		/// Add the specified training program to the datasource.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public TrainingProgram Add(TrainingProgram trainingProgram)
		{
			if (trainingProgram == null)
				throw new ArgumentNullException(nameof(trainingProgram));

			if (!_httpContext.User.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram))
				throw new NotAuthorizedException($"User does not have permission to edit application {trainingProgram.GrantApplicationId}.");

			// If it's an EmployerTraining grant then link the training provider to the program.
			if (trainingProgram.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.EmployerGrant && trainingProgram.TrainingProvider == null)
			{
				var trainingProvider = trainingProgram.GrantApplication.TrainingProviders.FirstOrDefault();
				if (trainingProvider != null)
				{
					trainingProgram.TrainingProviders.Add(trainingProvider);
				}
			}

			if (trainingProgram.TrainingProvider?.GrantApplication != null)
			{
				trainingProgram.TrainingProvider.GrantApplication = null;
				trainingProgram.TrainingProvider.GrantApplicationId = null;
				trainingProgram.GrantApplication.TrainingProviders.Remove(trainingProgram.TrainingProvider);
			}

			// When this is a skills training program we must update the associated eligible costs.
			if (trainingProgram.EligibleCostBreakdown?.EligibleExpenseBreakdown?.ServiceLine?.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining)
			{
				var breakdown = trainingProgram.EligibleCostBreakdown;
				breakdown.EligibleCost.AgreedMaxCost = breakdown.EligibleCost.CalculateAgreedMaxCost();
				breakdown.EligibleCost.RecalculateAgreedCosts();
				breakdown.EligibleCost.TrainingCost.RecalculateAgreedCosts();

				// If there is an active claim being worked on by the applicant, update it with the changes made to this skills training component.
				var claim = trainingProgram.GrantApplication.GetCurrentClaim();
				if (claim?.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete) ?? false)
				{
					var claimEligibleCost = claim.EligibleCosts.FirstOrDefault(cec => cec.EligibleCostId == breakdown.EligibleCost.Id);
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

			_dbContext.TrainingPrograms.Add(trainingProgram);

			// Keep the Training Program and Application dates the same
			trainingProgram.GrantApplication.StartDate = trainingProgram.StartDate;
			trainingProgram.GrantApplication.EndDate = trainingProgram.EndDate;

			var accountType = _httpContext.User.GetAccountType();
			if (accountType == AccountTypes.Internal && _grantAgreementService.AgreementUpdateRequired(trainingProgram.GrantApplication))
			{
				_grantAgreementService.UpdateAgreement(trainingProgram.GrantApplication);
				_dbContext.Update(trainingProgram.GrantApplication);
			}

			_noteService.GenerateUpdateNote(trainingProgram.GrantApplication);
			CommitTransaction();

			return trainingProgram;
		}

		/// <summary>
		/// Update the specified training program in the datasource.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public TrainingProgram Update(TrainingProgram trainingProgram)
		{
			if (trainingProgram == null)
				throw new ArgumentNullException(nameof(trainingProgram));

			if (!_httpContext.User.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.EditTrainingProgram))
				throw new NotAuthorizedException($"User does not have permission to edit application {trainingProgram.GrantApplicationId}.");

			// If it's an EmployerTraining grant then link the training provider to the program.
			if (trainingProgram.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.EmployerGrant && trainingProgram.TrainingProvider == null)
			{
				var trainingProvider = trainingProgram.GrantApplication.TrainingProviders.FirstOrDefault();
				if (trainingProvider != null)
				{
					trainingProgram.TrainingProviders.Add(trainingProvider);
				}
			}

			if (trainingProgram.TrainingProvider?.GrantApplication != null)
			{
				trainingProgram.TrainingProvider.GrantApplication = null;
				trainingProgram.TrainingProvider.GrantApplicationId = null;
				trainingProgram.GrantApplication.TrainingProviders.Remove(trainingProgram.TrainingProvider);
			}

			// When this is a skills training program we must update the associated eligible costs.
			if (trainingProgram.EligibleCostBreakdown?.EligibleExpenseBreakdown?.ServiceLine?.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining)
			{
				var breakdown = trainingProgram.EligibleCostBreakdown;
				var originalAssessedCost = (decimal)_dbContext.Entry(breakdown).OriginalValues[nameof(breakdown.AssessedCost)];
				if (breakdown.AssessedCost != originalAssessedCost)
				{
					breakdown.EligibleCost.AgreedMaxCost = breakdown.EligibleCost.CalculateAgreedMaxCost();
					breakdown.EligibleCost.RecalculateAgreedCosts();
				}

				// If there is an active claim being worked on by the applicant, update it with the changes made to this skills training component.
				var claim = trainingProgram.GrantApplication.GetCurrentClaim();
				if (claim?.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete) ?? false)
				{
					var claimEligibleCost = claim.EligibleCosts.FirstOrDefault(cec => cec.EligibleCostId == breakdown.EligibleCost.Id);
					var claimBreakdown = claimEligibleCost?.Breakdowns.FirstOrDefault(b => b.EligibleCostBreakdownId == breakdown.Id);
					if (claimBreakdown != null && !claimBreakdown.EligibleCostBreakdown.IsEligible)
					{
						claimEligibleCost.Breakdowns.Remove(claimBreakdown);
						claim.RecalculateClaimedCosts();
						claim.ClaimState = ClaimState.Incomplete;
					}
					else if (claimBreakdown == null && claimBreakdown.EligibleCostBreakdown.IsEligible)
					{
						// Add the new breakdown (skills training component) to the claim eligible cost.
						var claimBreakdownCost = new ClaimBreakdownCost(trainingProgram.EligibleCostBreakdown, claimEligibleCost);
						claimEligibleCost.Breakdowns.Add(claimBreakdownCost);
						claim.RecalculateClaimedCosts();
						claim.ClaimState = ClaimState.Incomplete;
					}
				}
			}

			// Keep the Training Program and Application dates the same
			trainingProgram.GrantApplication.StartDate = trainingProgram.StartDate;
			trainingProgram.GrantApplication.EndDate = trainingProgram.EndDate;

			_dbContext.Update(trainingProgram);

			var accountType = _httpContext.User.GetAccountType();
			if (accountType == AccountTypes.Internal && _grantAgreementService.AgreementUpdateRequired(trainingProgram.GrantApplication))
			{
				_grantAgreementService.UpdateAgreement(trainingProgram.GrantApplication);
				_dbContext.Update(trainingProgram.GrantApplication);
			}

			_noteService.GenerateUpdateNote(trainingProgram.GrantApplication);
			CommitTransaction();

			return trainingProgram;
		}

		/// <summary>
		/// Update the training program dates.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <returns></returns>
		public void UpdateProgramDates(TrainingProgram trainingProgram)
		{
			if (trainingProgram == null)
				throw new ArgumentNullException(nameof(trainingProgram));

			if (!_httpContext.User.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.SubmitChangeRequest))
				throw new NotAuthorizedException("The dates cannot be changed until the decision on the service provider change request is made.");

			var grantApplication = trainingProgram.GrantApplication;
			_grantAgreementService.UpdateAgreement(grantApplication);

			_dbContext.Update(trainingProgram);

			var originalStartDate = OriginalValue(trainingProgram, ga => ga.StartDate);
			var originalEndDate = OriginalValue(trainingProgram, ga => ga.EndDate);
			if (trainingProgram.StartDate != originalStartDate)
			{
				_noteService.AddDateChangedNote(grantApplication, "Training Start Date", originalStartDate, trainingProgram.StartDate);
			}

			if (trainingProgram.EndDate != originalEndDate)
			{
				_noteService.AddDateChangedNote(grantApplication, "Training End Date", originalEndDate, trainingProgram.EndDate);
			}

			CommitTransaction();
		}

		/// <summary>
		/// Change the eligibility of the specified training program.
		/// This will modify the eligible breakdown cost line item.
		/// This will remove/add the associated eligible cost breakdown to an active claim.
		/// </summary>
		/// <param name="trainingProgram"></param>
		public void ChangeEligibility(TrainingProgram trainingProgram)
		{
			if (!trainingProgram.EligibleCostBreakdownId.HasValue) throw new InvalidOperationException("Cannot change the eligibility of this program.");
			if (trainingProgram.TrainingProvider.RequestedTrainingProvider != null) throw new InvalidOperationException("Cannot change the eligibility of this program because the applicant is currently creating a change request.");

			var breakdown = trainingProgram.EligibleCostBreakdown;
			var grantApplication = breakdown.EligibleCost.TrainingCost.GrantApplication;
			var claim = grantApplication.GetCurrentClaim();

			if (grantApplication.HasSubmittedAClaim()) throw new InvalidOperationException("Cannot change the eligibility once a claim has been submitted.");

			breakdown.IsEligible = !breakdown.IsEligible;
			breakdown.AssessedCost = breakdown.IsEligible ? (!breakdown.AddedByAssessor ? breakdown.EstimatedCost : 1) : 0; // Must default to $1 because skills training components added by assessors have no estimated cost.
			breakdown.EligibleCost.AgreedMaxCost = breakdown.EligibleCost.Breakdowns.Where(x => x.IsEligible).Sum(x => x.AssessedCost);
			breakdown.EligibleCost.RecalculateAgreedCosts();
			breakdown.EligibleCost.TrainingCost.RecalculateAgreedCosts();

			// If there is an active claim associated with this training program, it will need to be updated.
			if (claim?.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete) ?? false)
			{
				if (breakdown.IsEligible)
				{
					// Add the eligible cost breakdown to the claim.
					var claimEligibleCost = _dbContext.ClaimEligibleCosts.First(cec => cec.ClaimId == claim.Id && cec.ClaimVersion == claim.ClaimVersion && cec.EligibleCostId == breakdown.EligibleCostId);
					var claimBreakdown = new ClaimBreakdownCost(breakdown, claimEligibleCost, 0);
					_dbContext.ClaimBreakdownCosts.Add(claimBreakdown);
					_dbContext.Update(claimEligibleCost);
				}
				else
				{
					// Remove the eligible cost breakdown from the claim.
					var claimBreakdown = breakdown.ClaimBreakdownCosts.First(cbc => cbc.ClaimEligibleCost.ClaimId == claim.Id && cbc.ClaimEligibleCost.ClaimVersion == claim.ClaimVersion);
					var claimEligibleCost = claimBreakdown.ClaimEligibleCost;
					claimEligibleCost.Breakdowns.Remove(claimBreakdown);

					var claimCost = claimEligibleCost.Breakdowns.Where(b => b.EligibleCostBreakdown.IsEligible).Sum(b => b.ClaimCost);

					if (claimCost > claimEligibleCost.AssessedCost)
					{
						// Set all the breakdown to $0 because it currently exceeds the approved limit.
						claimEligibleCost.Breakdowns.ForEach(b =>
						{
							b.ClaimCost = 0;
						});
						claimEligibleCost.ClaimCost = 0;
					}
					else
					{
						claimEligibleCost.ClaimCost = claimCost;
					}

					claimEligibleCost.RecalculateClaimCost();

					claim.RecalculateClaimedCosts();
					_dbContext.ClaimBreakdownCosts.Remove(claimBreakdown);
				}
			}

			if (_grantAgreementService.AgreementUpdateRequired(grantApplication))
			{
				_grantApplicationService.Update(grantApplication);
			}
			else
			{
				_dbContext.Update(breakdown);
				_dbContext.CommitTransaction();
			}
		}

		/// <summary>
		/// Delete the specified training program from the datasource.
		/// </summary>
		/// <param name="trainingProgram"></param>
		public void Delete(TrainingProgram trainingProgram)
		{
			if (trainingProgram == null)
				throw new ArgumentNullException(nameof(trainingProgram));

			if (!_httpContext.User.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.EditTrainingProgram))
				throw new NotAuthorizedException($"User does not have permission to delete application '{trainingProgram.GrantApplicationId}'.");

			trainingProgram.DeliveryMethods.Clear();
			trainingProgram.UnderRepresentedGroups.Clear();
			var ids = trainingProgram.TrainingProviders.Select(x => x.Id).ToArray();
			trainingProgram.TrainingProviders.Clear();
			foreach (var id in ids)
			{
				var provider = Get<TrainingProvider>(id);

				var attachments = new[] { provider.BusinessCaseDocument, provider.CourseOutlineDocument, provider.ProofOfQualificationsDocument }.Where(a => a != null);
				foreach (var attachment in attachments)
				{
					var versions = _dbContext.VersionedAttachments.Where(a => a.AttachmentId == attachment.Id).ToArray();
					foreach (var version in versions)
					{
						_dbContext.VersionedAttachments.Remove(version);
					}
					_dbContext.Attachments.Remove(attachment);
				}

				if (provider.TrainingAddress != null)
					_dbContext.ApplicationAddresses.Remove(provider.TrainingAddress);

				_dbContext.ApplicationAddresses.Remove(provider.TrainingProviderAddress);
				_dbContext.TrainingProviders.Remove(provider);
			}

			// If there is an eligible cost breakdown associated with this, then it too needs to be removed.
			if (trainingProgram.EligibleCostBreakdownId.HasValue)
			{
				var eligibleCost = trainingProgram.EligibleCostBreakdown.EligibleCost;
				var claimEligibleCosts = _dbContext.ClaimEligibleCosts.Where(cec => cec.Breakdowns.Any(b => b.EligibleCostBreakdownId == trainingProgram.EligibleCostBreakdownId)).ToArray();

				// If a prior approved/denied claim is created then this training program cannot be removed.
				if (claimEligibleCosts.Any(cec => !cec.Claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))) throw new InvalidOperationException("You cannot delete this component because it is part of a prior claim.");

				// If there there is a claim then we need to regenerate the agreement, but this will have to be done outside of this function.
				foreach (var claimEligibleCost in claimEligibleCosts)
				{
					// Delete the claim breakdown cost.
					var breakdown = claimEligibleCost.Breakdowns.FirstOrDefault(b => b.EligibleCostBreakdownId == trainingProgram.EligibleCostBreakdownId);
					claimEligibleCost.Breakdowns.Remove(breakdown);
					_dbContext.ClaimBreakdownCosts.Remove(breakdown);
					claimEligibleCost.ClaimCost = claimEligibleCost.Breakdowns.Sum(b => b.ClaimCost);
					claimEligibleCost.RecalculateClaimCost();
					claimEligibleCost.AssessedCost = claimEligibleCost.Breakdowns.Sum(b => b.AssessedCost);
					claimEligibleCost.RecalculateAssessedCost();
				}
				_dbContext.EligibleCostBreakdowns.Remove(trainingProgram.EligibleCostBreakdown);

				// If an external user is deleting a training program then the estimated costs must be updated.
				var accountType = _httpContext.User.GetAccountType();
				if (accountType == AccountTypes.External)
				{
					eligibleCost.EstimatedCost = eligibleCost.CalculateEstimateCost();
					eligibleCost.RecalculateEstimatedCost();
					eligibleCost.TrainingCost.RecalculateEstimatedCosts();
				}

				eligibleCost.AgreedMaxCost = eligibleCost.CalculateAgreedMaxCost();
				eligibleCost.RecalculateAgreedCosts();
				eligibleCost.TrainingCost.RecalculateAgreedCosts();
			}

			_dbContext.TrainingPrograms.Remove(trainingProgram);
			_dbContext.CommitTransaction();
		}
		#endregion
	}
}
