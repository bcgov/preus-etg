using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using NLog;

namespace CJG.Application.Services
{
	public class ClaimService : Service, IClaimService
	{
		private readonly IGrantAgreementService _grantAgreementService;
		private readonly IUserService _userService;
		private readonly INotificationService _notificationService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IPrioritizationService _prioritizationService;
		private readonly INoteService _noteService;
		private readonly IFiscalYearService _fiscalYearService;

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ClaimService"/> and initializes the specified properties.
		/// </summary>
		/// <param name="userService"></param>
		/// <param name="notificationService"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="noteService"></param>
		/// <param name="grantAgreementService"></param>
		/// <param name="prioritizationService"></param>
		/// <param name="fiscalYearService"></param>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ClaimService(
			IUserService userService,
			INotificationService notificationService,
			IGrantOpeningService grantOpeningService,
			INoteService noteService,
			IGrantAgreementService grantAgreementService,
			IPrioritizationService prioritizationService,
			IFiscalYearService fiscalYearService,
			IDataContext context,
			HttpContextBase httpContext,
			ILogger logger) : base(context, httpContext, logger)
		{
			_userService = userService;
			_notificationService = notificationService;
			_grantOpeningService = grantOpeningService;
			_prioritizationService = prioritizationService;
			_noteService = noteService;
			_grantAgreementService = grantAgreementService;
			_fiscalYearService = fiscalYearService;
		}

		#region Claims
		/// <summary>
		/// Create and add a new <typeparamref name="Claim"/> to the datastore linked to a grant application.
		/// This will always create the first version of the specific claim number if no claims exist.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Cannot create and add a new claim if one already exists for the training program.</exception>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public Claim AddNewClaim(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (grantApplication.Claims.Any() && grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
				throw new InvalidOperationException("Cannot create and add a new claim for this training program, one already exists.");

			if (!grantApplication.IsApplicationAdministrator(_httpContext.User.GetUserId().Value) && !_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.AmendClaim))
				throw new NotAuthorizedException("User does not have permission to add a new Claim.");

			var newClaimId = new ClaimId();
			_dbContext.ClaimIds.Add(newClaimId);
			_dbContext.Commit();

			var claim = CreateNewClaim(newClaimId.Id, 1, grantApplication);

			// Update the grant application state if a new claim is created and in the right state
			if (claim.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments && grantApplication.ApplicationStateExternal.In(ApplicationStateExternal.ClaimApproved, ApplicationStateExternal.ClaimDenied))
			{
				grantApplication.ApplicationStateInternal = ApplicationStateInternal.AgreementAccepted;
				grantApplication.ApplicationStateExternal = ApplicationStateExternal.Approved;
			}

			_dbContext.CommitTransaction();
			return claim;
		}

		/// <summary>
		/// Create and add a new claim version for the specified claim.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">A new claim version can only be created if the current claim is in an appropriate state.</exception>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public Claim CreateNewClaimVersion(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (!grantApplication.Claims.Any())
				throw new InvalidOperationException("Cannot create and add a new claim version if no claims currently exist.");

			if (!(_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.AmendClaim)
				|| _httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EnableClaimReporting)))
				throw new NotAuthorizedException($"User does not have permission to amend the claim.");

			if (grantApplication.Claims.FirstOrDefault(o => o.ClaimState.In(ClaimState.Incomplete, ClaimState.Unassessed)) != null)
				// This may be unreachable code
				throw new InvalidOperationException("Cannot create and add a new claim version if unassessed claim currently exist.");

			var currentClaim = grantApplication.GetCurrentClaim();
			var id = currentClaim.Id;
			var version = currentClaim.ClaimVersion + 1;

			return CreateNewClaim(id, version, grantApplication);
		}

		/// <summary>
		/// Creates and adds a new claim to the datastore.
		/// If there has never been a claim created, it will create the first version and copy the line items from the estimate.
		/// If there has been only been a prior denied claim, it will create a new version and copy the line items from the estimate.
		/// If there has been a prior approved claim, it will create a new version and copy the line items from the prior approved claim.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		private Claim CreateNewClaim(int id, int version, GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			var claim = new Claim(id, version, grantApplication);

			if (grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
			{
				//Pull the most recent approved version of the claim if it exists
				var priorApprovedClaim = claim.GetPriorApprovedClaim();

				// If there is no prior approved claim, it means this is the first claim.
				if (priorApprovedClaim == null)
				{
					// Add line items to the claim for every line item in the estimate that has an AgreedMaxCost > 0.
					foreach (var eligibleCost in grantApplication.TrainingCost.EligibleCosts.Where(ec => ec.AgreedMaxCost > 0))
					{
						// Copy eligible cost agreed values into Claim eligible cost assessed values
						var claimEligibleCost = new ClaimEligibleCost(claim, eligibleCost);
						claim.EligibleCosts.Add(claimEligibleCost);
					}
				}
				else
				{
					// Copy the previously approved claim.
					var eligibleCosts = _dbContext.ClaimEligibleCosts.Where(ec => ec.ClaimId == priorApprovedClaim.Id && ec.ClaimVersion == priorApprovedClaim.ClaimVersion);
					foreach (var eligibleCost in eligibleCosts)
					{
						// Copy prior claim eligible costs.
						var claimEligibleCost = new ClaimEligibleCost(claim, eligibleCost);
						claim.EligibleCosts.Add(claimEligibleCost);
					}

					// Copy attachment links into the new Claim.
					foreach (var attachment in priorApprovedClaim.Receipts)
					{
						claim.Receipts.Add(attachment);
					}
				}

				claim.RecalculateClaimedCosts();
				claim.RecalculateAssessedCosts();

				foreach (var eligibleCost in claim.EligibleCosts)
					eligibleCost.AssessedParticipants = eligibleCost.ClaimParticipants;

				_dbContext.Claims.Add(claim);
			}
			else
			{
				foreach (var eligibleCost in grantApplication.TrainingCost.EligibleCosts.Where(ec => ec.AgreedMaxCost > 0))
				{
					// Copy eligible cost agreed values into Claim eligible cost assessed values
					var claimEligibleCost = new ClaimEligibleCost(claim, eligibleCost)
					{
						ClaimParticipants = grantApplication.ParticipantForms.Count(pf => !pf.IsExcludedFromClaim)
					};
					claim.EligibleCosts.Add(claimEligibleCost);
				}
				claim.RecalculateClaimedCosts();
				claim.RecalculateAssessedCosts();

				_dbContext.Claims.Add(claim);
			}
			return claim;
		}

		/// <summary>
		/// Update the specified <typeparamref name="Claim"/> in the datastore.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="overrideRates">Whether to override the reimbursement rates.  This is only allowed if the user has the AM4 privilege.</param>
		public Claim Update(Claim claim, bool overrideRates = false)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim)
				|| (overrideRates && !_httpContext.User.HasPrivilege(Privilege.AM4)))
				throw new NotAuthorizedException("User does not have permission to update claim.");

			claim.RecalculateClaimedCosts();
			claim.RecalculateAssessedCosts(overrideRates);

			_dbContext.Update(claim);
			var accountType = _httpContext.User.GetAccountType();
			if (accountType == AccountTypes.Internal)
			{
				_noteService.GenerateUpdateNote(claim.GrantApplication);
			}
			_dbContext.CommitTransaction();

			return claim;
		}

		/// <summary>
		/// Clear all draft claims for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public void ClearDraftClaims(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (!(_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.CloseClaimReporting)))
				throw new NotAuthorizedException($"User does not have permission to close claim reporting.");

			var lockReceipts = grantApplication.Claims.Where(c => c.ClaimState.In(ClaimState.ClaimApproved, ClaimState.ClaimPaid, ClaimState.PaymentRequested, ClaimState.AmountOwing, ClaimState.AmountReceived)).SelectMany(c => c.Receipts.Select(r => r.Id)).Distinct().ToArray();
			var claims = grantApplication.Claims.Where(o => o.ClaimState.In(ClaimState.Incomplete)).ToList();

			foreach (var claim in claims)
			{
				var eligibleCosts = claim.EligibleCosts.ToArray();
				var receipts = claim.Receipts.Where(r => !lockReceipts.Contains(r.Id)).ToArray();

				foreach (var item in eligibleCosts)
				{
					var participantCosts = item.ParticipantCosts.ToArray();

					foreach (var cost in participantCosts)
						_dbContext.ParticipantCosts.Remove(cost);
					_dbContext.ClaimEligibleCosts.Remove(item);
				}

				foreach (var item in receipts)
					_dbContext.Attachments.Remove(item);

				claim.Payments.Clear();
				claim.PaymentRequests.Clear();
				claim.EligibleCosts.Clear();
				claim.Receipts.Clear();

				_dbContext.Claims.Remove(claim);
			}
		}

		/// <summary>
		/// Get the specific <typeparamref name="Claim"/>.
		/// </summary>
		/// <param name="id">The unique Id.</param>
		/// <param name="version">The specific version.</param>
		/// <returns>The <typeparamref name="Claim"/> that matches the specified id and version.</returns>
		public Claim Get(int id, int version = 0)
		{
			Claim claim = null;
			if (version == 0)
			{
				claim = _dbContext.Claims.Where(x => x.Id == id).OrderByDescending(x => x.ClaimVersion).FirstOrDefault();
			}
			else
			{
				claim = Get<Claim>(id, version);
			}

			if (claim == null)
				throw new NoContentException($"User does not have permission to view this claim, or the claim does not exist.");

			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
				throw new NotAuthorizedException("User does not have permission to view this claim.");

			return claim;
		}

		/// <summary>
		/// Get all the <typeparamref name="Claim"/>'s for the specific Id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>A new instance of a Collection of <typeparamref name="Claim"/>, or an empty collection.</returns>
		public IEnumerable<Claim> GetClaims(int id)
		{
			var claims = _dbContext.Claims.Where(c => c.Id == id).OrderBy(c => c.ClaimVersion).ToList();

			if (claims.Any() && !_httpContext.User.CanPerformAction(claims.First().GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
				throw new NotAuthorizedException("User does not have permission to view this claim.");

			return claims;
		}

		/// <summary>
		/// Get all the <typeparamref name="Claim"/>'s in the specified <typeparamref name="ClaimState"/>.
		/// </summary>
		/// <param name="state">The <typeparamref name="ClaimState"/>.</param>
		/// <returns></returns>
		public IEnumerable<Claim> GetClaims(ClaimState state)
		{
			var claims = _dbContext.Claims.Where(c => c.ClaimState == state).ToList();

			if (claims.Any() && !_httpContext.User.CanPerformAction(claims.First().GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
				throw new NotAuthorizedException("User does not have permission to retrieve all claims.");

			return claims;
		}

		/// <summary>
		/// Get the total number of <typeparamref name="Claim"/>'s in the specified <typeparamref name="ClaimState"/>.
		/// </summary>
		/// <param name="state">The <typeparamref name="ClaimState"/>.</param>
		/// <returns></returns>
		public int GetTotalClaims(ClaimState state)
		{
			return _dbContext.Claims.Count(c => c.ClaimState == state);
		}

		/// <summary>
		///Get all the Approved <typeparamref name="Claim"/>'s where the TotalAssessedReimbursement is greater than the sum of all existing payment requests for the its TrainingProgram.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Claim> GetApprovedClaimsForPaymentRequest(int grantProgramId)
		{
			var claims = GetApprovedClaimsForGrantProgram(grantProgramId).ToArray();

			if (claims.Any() && !_httpContext.User.CanPerformAction(claims.First().GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
				throw new NotAuthorizedException("User does not have permission to retrieve all claims.");

			return claims.Where(c => c.AmountPaidOrOwing() >= 0).ToArray();
		}

		/// <summary>
		///Get all the Approved <typeparamref name="Claim"/>'s where the TotalAssessedReimbursement is less than the sum of all existing payment requests for the its TrainingProgram. 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Claim> GetApprovedClaimsForAmountOwing(int grantProgramId)
		{
			var claims = GetApprovedClaimsForGrantProgram(grantProgramId).ToArray();

			if (claims.Any() && !_httpContext.User.CanPerformAction(claims.First().GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
				throw new NotAuthorizedException("User does not have permission to retrieve all claims.");

			return claims.Where(c => c.AmountPaidOrOwing() < 0).ToArray();
		}

		/// <summary>
		/// Get all of the approved claims for the specified grant program filterd by whether a request has been held.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <param name="requestHold"></param>
		/// <returns></returns>
		private IQueryable<Claim> GetApprovedClaimsForGrantProgram(int grantProgramId, bool requestHold)
		{
			return _dbContext.Claims.Where(c => c.ClaimState == ClaimState.ClaimApproved &&
															 c.GrantApplication.HoldPaymentRequests == requestHold &&
															 c.GrantApplication.GrantOpening.GrantStream.GrantProgramId == grantProgramId);
		}

		/// <summary>
		/// Get all of the approved claims for the grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		private IEnumerable<Claim> GetApprovedClaimsForGrantProgram(int grantProgramId)
		{
			// Single claims are grouped, so that payments are singular.
			var singleClaims = GetApprovedClaimsForGrantProgram(grantProgramId, false)
				.Where(c => c.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
				.GroupBy(c => c.GrantApplicationId)
				.Select(x => x.OrderBy(y => y.ClaimVersion).FirstOrDefault())
				.ToArray();

			// Multiple claims are independent, so that payments are independent.
			var multipleClams = GetApprovedClaimsForGrantProgram(grantProgramId, false)
				.Where(c => c.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments)
				.ToArray();

			var results = new List<Claim>(singleClaims);
			results.AddRange(multipleClams);
			return results;
		}

		/// <summary>
		/// Gets all <typeparamref name="Claim"/>'s by value of HoldPaymentRequests property in their associated <typeparamref name="TrainingProgram"/>. 
		/// </summary>
		/// <param name="requestHold">Boolean representing the desired query value for HoldPaymentRequests.</param>
		/// <returns></returns>
		public RequestOnHoldModel GetClaimsByPaymentRequestHold(int grantProgramId, bool requestHold)
		{
			var model = new RequestOnHoldModel();
			var startDate = GetStartDate();

			model.FilesWithClaim = GetApprovedClaimsForGrantProgram(grantProgramId, requestHold).ToArray()
									.Select(c => new RequestOnHoldClaimModel(c, startDate)).ToList();

			model.FilesWithoutClaim = _dbContext.GrantApplications.Where(t => t.GrantOpening.GrantStream.GrantProgramId == grantProgramId &&
																			  !t.Claims.Any(c => c.ClaimState == ClaimState.ClaimApproved) &&
																			   t.HoldPaymentRequests == requestHold).ToArray()
																				.Select(t => new RequestOnHoldItemModel(t)).ToList();

			return model;
		}

		/// <summary>
		/// Get the start date of the fiscal user.
		/// </summary>
		/// <returns></returns>
		private DateTime GetStartDate()
		{
			var utcNow = AppDateTime.UtcNow;
			return _fiscalYearService.GetFiscalYear(utcNow)?.StartDate ?? utcNow;
		}

		/// <summary>
		/// Get all the <typeparamref name="Claim"/>'s for the specified GrantApplication.
		/// </summary>
		/// <param name="grantApplicationId">The grant applciation Id.</param>
		/// <returns></returns>
		public IEnumerable<Claim> GetClaimsForGrantApplication(int grantApplicationId)
		{
			var claims = _dbContext.Claims.Where(x => x.GrantApplicationId == grantApplicationId);

			if (claims.Count() > 0 && !_httpContext.User.CanPerformAction(claims.First().GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
				throw new NotAuthorizedException("User does not have permission to view this claim.");

			return claims;
		}

		/// <summary>
		/// Get the current Claim Version # (the most recent/latest).
		/// </summary>
		/// <param name="id">The Claim Id.</param>
		/// <returns>The Claim Version #, or 0 if no Claim exists.</returns>
		public int GetCurrentClaimVersion(int id)
		{
			try
			{
				var claims = _dbContext.Claims.Where(c => c.Id == id).Select(c => new { c.Id, c.ClaimVersion });

				if (claims == null || claims.Count() == 0)
					return 0;

				return claims.Max(c => c.ClaimVersion);
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't get the current claim version for claim ID: {0}", id);
				throw;
			}
		}

		/// <summary>
		/// Get the current 'active' Claim.
		/// The application workflow only allows one 'active' Claim at any given point in time.
		/// The most recent one will always be the current Claim.
		/// </summary>
		/// <param name="grantApplicationId">Grant application Id.</param>
		/// <returns>The current <typeparamref name="Claim"/>, or null if none.</returns>
		public Claim GetCurrentClaim(int grantApplicationId)
		{
			var grantApplication = Get<GrantApplication>(grantApplicationId);
			var claim = grantApplication.GetCurrentClaim();

			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
				throw new NotAuthorizedException("User does not have permission to view this claim.");

			return claim;
		}
		#endregion

		#region ClaimEligibleCosts

		/// <summary>
		/// Get the specified claim eligible cost.
		/// </summary>
		/// <param name="claimEligibleCostId"></param>
		/// <returns></returns>
		public ClaimEligibleCost GetClaimEligibleCost(int claimEligibleCostId)
		{
			var claimEligibleCost = Get<ClaimEligibleCost>(claimEligibleCostId);

			if (!_httpContext.User.CanPerformAction(claimEligibleCost.Claim?.GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
			{
				throw new NotAuthorizedException($"User does not have permission to access Claim '{claimEligibleCost?.ClaimId}'.");
			}

			return claimEligibleCost;
		}

		/// <summary>
		/// Removes the ClaimEligibleCost from the Claim.
		/// This method does not save the changes to the database.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <param name="claimRowVersion"></param>
		/// <param name="trigger"></param>
		public Claim Remove(ClaimEligibleCost claimEligibleCost, byte[] claimRowVersion, ApplicationWorkflowTrigger trigger = ApplicationWorkflowTrigger.EditTrainingCosts)
		{
			if (claimEligibleCost == null)
				throw new ArgumentNullException(nameof(claimEligibleCost));

			var claimEligibleCostToUpdate = _dbContext.ClaimEligibleCosts.Find(claimEligibleCost.Id);

			var claim = _dbContext.Claims.Find(claimEligibleCostToUpdate.ClaimId, claimEligibleCostToUpdate.ClaimVersion);

			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, trigger))
				throw new NotAuthorizedException($"User does not have permission to access Claim.");

			_dbContext.ClaimEligibleCosts.Remove(claimEligibleCost);

			foreach (var participantCost in claimEligibleCost.ParticipantCosts)
			{
				_dbContext.ParticipantCosts.Remove(participantCost);
			}

			claim.EligibleCosts.Remove(claimEligibleCost);
			claimEligibleCost.ParticipantCosts.Clear();

			claim.RecalculateClaimedCosts();
			claim.RecalculateAssessedCosts();

			return claim;
		}

		/// <summary>
		/// Updates the claim eligible cost in the datasource.
		/// </summary>
		/// <param name="claimEligibleCost"></param>
		/// <param name="claimRowVersion"></param>
		/// <param name="commitTransaction"></param>
		/// <param name="trigger"></param>
		public Claim Update(ClaimEligibleCost claimEligibleCost, byte[] claimRowVersion, bool commitTransaction = true, ApplicationWorkflowTrigger trigger = ApplicationWorkflowTrigger.EditClaim)
		{
			if (claimEligibleCost == null)
				throw new ArgumentNullException(nameof(claimEligibleCost));

			var claimEligibleCostToUpdate = Get<ClaimEligibleCost>(claimEligibleCost.Id);
			var claim = Get<Claim>(claimEligibleCostToUpdate.ClaimId, claimEligibleCostToUpdate.ClaimVersion);

			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, trigger))
				throw new NotAuthorizedException($"User does not have permission to access Claim.");

			claimEligibleCostToUpdate.EligibleCostId = claimEligibleCost.EligibleCostId;
			claimEligibleCostToUpdate.EligibleCost = commitTransaction ? null : claimEligibleCostToUpdate.EligibleCost;
			claimEligibleCostToUpdate.EligibleExpenseTypeId = claimEligibleCost.EligibleExpenseTypeId;
			claimEligibleCostToUpdate.EligibleExpenseType = commitTransaction ? null : claimEligibleCostToUpdate.EligibleExpenseType;

			claimEligibleCostToUpdate.ClaimCost = claimEligibleCost.ClaimCost;
			claimEligibleCostToUpdate.ClaimParticipants = claimEligibleCost.ClaimParticipants;
			claimEligibleCostToUpdate.RecalculateClaimCost();

			claimEligibleCostToUpdate.AssessedCost = claimEligibleCost.AssessedCost;
			claimEligibleCostToUpdate.AssessedParticipants = claimEligibleCost.AssessedParticipants;
			claimEligibleCostToUpdate.AssessedMaxParticipantReimbursementCost = claimEligibleCost.AssessedMaxParticipantReimbursementCost;
			claimEligibleCostToUpdate.RecalculateAssessedCost();
			claimEligibleCostToUpdate.RowVersion = claimEligibleCost.RowVersion;

			_dbContext.Update(claimEligibleCostToUpdate);

			if (claimEligibleCost.ParticipantCosts != null)
			{
				foreach (var updatedParticipantCost in claimEligibleCost.ParticipantCosts)
				{
					var participantCostToUpdate =
						claimEligibleCostToUpdate.ParticipantCosts.FirstOrDefault(p => p.Id == updatedParticipantCost.Id) ?? new ParticipantCost(claimEligibleCostToUpdate, updatedParticipantCost.ParticipantForm);

					participantCostToUpdate.ClaimParticipantCost = updatedParticipantCost.ClaimParticipantCost;
					participantCostToUpdate.RecalculateClaimCost();

					participantCostToUpdate.AssessedParticipantCost = updatedParticipantCost.AssessedParticipantCost;
					participantCostToUpdate.AssessedReimbursement = updatedParticipantCost.AssessedReimbursement;
					participantCostToUpdate.RecalculatedAssessedCost();
					participantCostToUpdate.RowVersion = updatedParticipantCost.RowVersion;

					if (participantCostToUpdate.Id == 0)
						_dbContext.ParticipantCosts.Add(participantCostToUpdate);
					else
						_dbContext.Update(participantCostToUpdate);
				}
			}

			claim.RecalculateClaimedCosts();
			claim.RecalculateAssessedCosts();
			claim.RowVersion = claimRowVersion;

			_dbContext.Update(claim);

			if (commitTransaction)
			{
				CommitTransaction();
			}

			return claim;
		}

		/// <summary>
		/// Add the eligible cost to the claim.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="eligibleCostTypeId"></param>
		/// <param name="commitTransaction"></param>
		/// <param name="claimEligibleCostToAdd"></param>
		/// <returns></returns>
		public string AddEligibleCost(Claim claim, int eligibleCostTypeId, bool commitTransaction = true, ClaimEligibleCost claimEligibleCostToAdd = null)
		{
			var grantApplication = claim.GrantApplication;
			var participantForms = grantApplication.ParticipantForms;

			var participantsOnClaim = (from pc in claim.EligibleCosts.SelectMany(ec => ec.ParticipantCosts)
									   select pc.ParticipantFormId).ToArray().Distinct();

			var claimEligibleCost = claimEligibleCostToAdd ?? new ClaimEligibleCost(claim)
			{
				EligibleExpenseTypeId = eligibleCostTypeId,
				AddedByAssessor = true,
			};

			foreach (var participantForm in participantForms)
			{
				if (!participantForm.Id.In(participantsOnClaim.ToArray<int>()))
				{
					continue;
				}

				claimEligibleCost.ParticipantCosts.Add(new ParticipantCost(claimEligibleCost, participantForm));
			}
			claim.EligibleCosts.Add(claimEligibleCost);

			if (commitTransaction)
			{
				Update(claim);
				return System.Convert.ToBase64String(claimEligibleCost.RowVersion);
			}

			return string.Empty;
		}

		/// <summary>
		/// Delete the eligible cost from the claim.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="claimEligibleCostId"></param>
		/// <returns></returns>
		public string DeleteEligibleCost(Claim claim, int claimEligibleCostId)
		{
			var claimEligibleCost = GetClaimEligibleCost(claimEligibleCostId);

			foreach (var participantCost in claimEligibleCost.ParticipantCosts.ToArray())
			{
				_dbContext.ParticipantCosts.Remove(participantCost);
			}

			_dbContext.ClaimEligibleCosts.Remove(claimEligibleCost);

			claim.RecalculateClaimedCosts();
			claim.RecalculateAssessedCosts();

			Update(claim);

			return System.Convert.ToBase64String(claimEligibleCost.RowVersion);
		}

		/// <summary>
		/// Remove all eligible cost line items added by the assessor.
		/// </summary>
		/// <param name="claim"></param>
		public Claim RemoveEligibleCostsAddedByAssessor(Claim claim)
		{
			if (!claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete, ClaimState.Unassessed))
				throw new InvalidOperationException($"Cannot remove eligible costs from a claim in this state '{claim.ClaimState.GetDescription()}'.");

			var eligibleCostsAddedByAssessor = claim.EligibleCosts.Where(x => x.AddedByAssessor).ToArray();

			for (var ic = 0; ic < eligibleCostsAddedByAssessor.Count(); ic++)
			{
				var participantCosts = eligibleCostsAddedByAssessor[ic].ParticipantCosts.ToArray();

				for (var ip = 0; ip < participantCosts.Count(); ip++)
				{
					_dbContext.ParticipantCosts.Remove(participantCosts[ip]);
				}
				_dbContext.ClaimEligibleCosts.Remove(eligibleCostsAddedByAssessor[ic]);
			}

			claim.RecalculateClaimedCosts();
			claim.RecalculateAssessedCosts(true);

			return claim;
		}
		#endregion

		#region ParticipantCosts
		/// <summary>
		/// Get the participant cost for the specified Id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ParticipantCost GetParticipantCost(int id)
		{
			var participantCost = Get<ParticipantCost>(id);

			if (!_httpContext.User.CanPerformAction(participantCost.ClaimEligibleCost.Claim?.GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
				throw new NotAuthorizedException("User does not have permission to view claim.");

			return participantCost;
		}

		/// <summary>
		/// Get the participant costs for the specified claimed eligible cost.
		/// </summary>
		/// <param name="claimEligibleCostId"></param>
		/// <returns></returns>
		public IEnumerable<ParticipantCost> GetParticipantCosts(int claimEligibleCostId)
		{
			var claimEligibleCost = Get<ClaimEligibleCost>(claimEligibleCostId);

			if (!_httpContext.User.CanPerformAction(claimEligibleCost.Claim.GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
				throw new NotAuthorizedException("User does not have permission to access Claim.");

			return claimEligibleCost.ParticipantCosts.Count > 0 ? claimEligibleCost.ParticipantCosts : null;
		}

		/// <summary>
		/// Update the specified <typeparamref name="Claim"/> in the datastore.
		/// </summary>
		/// <param name="claim"></param>
		public Claim UpdateParticipants(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditParticipants))
				throw new NotAuthorizedException("User does not have permission to update participants.");

			claim.RecalculateClaimedCosts();
			claim.RecalculateAssessedCosts();

			_dbContext.Update(claim);
			_dbContext.CommitTransaction();

			return claim;
		}

		/// <summary>
		/// Updates the participant costs.
		/// </summary>
		/// <param name="claimEligibleCostId"></param>
		/// <param name="participantCosts"></param>
		/// <param name="claimRowVersion"></param>
		public Claim UpdateParticipantCosts(int claimEligibleCostId, IEnumerable<ParticipantCost> participantCosts, byte[] claimRowVersion)
		{
			var claimEligibleCost = Get<ClaimEligibleCost>(claimEligibleCostId);

			if (claimEligibleCost == null)
				throw new ArgumentNullException(nameof(claimEligibleCost));

			var claim = Get<Claim>(claimEligibleCost.ClaimId, claimEligibleCost.ClaimVersion);

			if (_httpContext.User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim))
				throw new NotAuthorizedException($"User does not have permission to access Claim '{claimEligibleCost?.ClaimId}'.");

			if (participantCosts != null)
			{
				foreach (var updatedParticipantCost in participantCosts)
				{
					var participantCostToUpdate =
						claimEligibleCost.ParticipantCosts.FirstOrDefault(p => p.Id == updatedParticipantCost.Id) ?? new ParticipantCost(claimEligibleCost, updatedParticipantCost.ParticipantForm);

					participantCostToUpdate.ClaimParticipantCost = updatedParticipantCost.ClaimParticipantCost;
					participantCostToUpdate.RecalculateClaimCost();

					participantCostToUpdate.AssessedParticipantCost = updatedParticipantCost.AssessedParticipantCost;
					participantCostToUpdate.AssessedReimbursement = updatedParticipantCost.AssessedReimbursement;
					participantCostToUpdate.RecalculatedAssessedCost();
					participantCostToUpdate.RowVersion = updatedParticipantCost.RowVersion;

					if (updatedParticipantCost.Id == 0)
						_dbContext.ParticipantCosts.Add(participantCostToUpdate);
					else
						_dbContext.Update(participantCostToUpdate);
				}

				claim.RecalculateClaimedCosts();
				claim.RecalculateAssessedCosts();
				claim.RowVersion = claimRowVersion;

				CommitTransaction();
			}

			return claim;
		}
		#endregion

		#region Attachments

		/// <summary>
		/// Get the attachment for the specified 'attachmentId' and verify the current user has access to the grant application.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		public Attachment GetAttachment(int claimId, int claimVersion, int attachmentId)
		{
			var claim = Get<Claim>(claimId, claimVersion);

			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.ViewApplication) || !claim.Receipts.Any(a => a.Id == attachmentId))
			{
				throw new NotAuthorizedException($"User does not have permission to view application '{claim.GrantApplicationId}'.");
			}

			return Get<Attachment>(attachmentId);
		}

		/// <summary>
		/// Add a new <typeparamref name="Attachment"/> to the specified Claim.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="attachment"></param>
		public void AddReceipt(int id, Attachment attachment)
		{
			if (attachment == null)
				throw new ArgumentNullException(nameof(attachment));

			var claim = Get(id);

			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim))
				throw new NotAuthorizedException("User does not have permission to update claim.");

			// If attachment belongs to prior Claim, need to create a new Attachment.
			if (claim.GrantApplication.Claims.Count(c => c.Id != id && c.Receipts.Any(r => r.Id == attachment.Id)) == 0)
			{
				if (claim.Receipts.Any(r => r.Id == attachment.Id))
				{
					// If the receipt already exist in the current Claim, update it.
					_dbContext.Update(attachment);
				}
				else
				{
					// Only add a receipt if it doesn't exist.
					claim.Receipts.Add(attachment);
				}
			}
			else
			{
				// Delete the prior attachment reference so that it doesn't delete the attachment itself.
				claim.Receipts.Remove(attachment);
				// Now add a new receipt.
				var newAttachment = new Attachment(attachment);
				claim.Receipts.Add(newAttachment);
			}

			Update(claim);
		}

		/// <summary>
		/// Add a new <typeparamref name="Attachment"/> to the specified Claim.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <param name="attachment"></param>
		public Attachment AddReceipt(int claimId, int claimVersion, Attachment attachment)
		{
			if (attachment == null)
				throw new ArgumentNullException(nameof(attachment));

			var claim = Get(claimId, claimVersion);

			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim))
				throw new NotAuthorizedException("User does not have permission to update claim.");

			// If attachment belongs to prior Claim, need to create a new Attachment.
			if (claim.GrantApplication.Claims.Count(c => c.Id != claimId && c.Receipts.Any(r => r.Id == attachment.Id)) == 0)
			{
				if (claim.Receipts.Any(r => r.Id == attachment.Id))
				{
					// If the receipt already exist in the current Claim, update it.
					_dbContext.Update(attachment);
				}
				else
				{
					// Only add a receipt if it doesn't exist.
					claim.Receipts.Add(attachment);
					_dbContext.Attachments.Add(attachment);
				}
			}
			else
			{
				// Delete the prior attachment reference so that it doesn't delete the attachment itself.
				claim.Receipts.Remove(attachment);
				_dbContext.Attachments.Remove(attachment);
				// Now add a new receipt.
				var newAttachment = new Attachment(attachment);
				claim.Receipts.Add(newAttachment);
				_dbContext.Attachments.Add(newAttachment);
				attachment = newAttachment;
			}

			Update(claim);

			return attachment;
		}
		/// <summary>
		/// Removes the association of the <typeparamref name="Attachment"/> from the specified Claim.
		/// This will only delete the attachment if it isn't associated with any other Claim.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="attachment"></param>
		public void RemoveReceipt(int id, Attachment attachment)
		{
			if (attachment == null)
				throw new ArgumentNullException(nameof(attachment));

			var claim = Get(id);

			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim))
				throw new NotAuthorizedException("User does not have permission to update claim.");

			// If the attachment is not associated with a prior claim, we can delete it.
			if (claim.GrantApplication.Claims.Count(c => (c.Id == id && c.ClaimVersion < claim.ClaimVersion) && c.Receipts.Any(r => r.Id == attachment.Id)) == 0)
			{
				var versions = _dbContext.VersionedAttachments.Where(va => va.AttachmentId == attachment.Id).ToArray();
				versions.ForEach(va => _dbContext.VersionedAttachments.Remove(va));
				_dbContext.Attachments.Remove(attachment);
			}

			claim.Receipts.Remove(attachment);

			Update(claim);
		}

		public void RemoveReceipt(int claimId, int claimVersion, Attachment attachment)
		{
			if (attachment == null)
				throw new ArgumentNullException(nameof(attachment));

			var claim = Get(claimId, claimVersion);

			if (!_httpContext.User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.EditClaim))
				throw new NotAuthorizedException("User does not have permission to update claim.");

			// If the attachment is not associated with a prior claim, we can delete it.
			if (claim.GrantApplication.Claims.Count(c => (c.Id == claimId && c.ClaimVersion < claimVersion) && c.Receipts.Any(r => r.Id == attachment.Id)) == 0)
			{
				var versions = _dbContext.VersionedAttachments.Where(va => va.AttachmentId == attachment.Id).ToArray();
				versions.ForEach(va => _dbContext.VersionedAttachments.Remove(va));
				_dbContext.Attachments.Remove(attachment);
			}

			claim.Receipts.Remove(attachment);

			Update(claim);
		}


		#endregion

		#region Workflow
		/// <summary>
		/// Submit the specified claim.
		/// </summary>
		/// <param name="claim"></param>
		public void SubmitClaim(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			CreateWorkflowStateMachine(claim.GrantApplication).SubmitClaim(claim);
		}

		/// <summary>
		/// Withdraw the specified claim.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="reason"></param>
		public void WithdrawClaim(Claim claim, string reason)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			CreateWorkflowStateMachine(claim.GrantApplication).WithdrawClaim(claim, reason, this);
		}

		/// <summary>
		/// Select the specified claim for assessment.
		/// </summary>
		/// <param name="claim"></param>
		public void SelectClaimForAssessment(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			CreateWorkflowStateMachine(claim.GrantApplication).SelectClaimForAssessment(claim);
		}

		/// <summary>
		/// Remove the specified claim from assessment.
		/// </summary>
		/// <param name="claim"></param>
		public void RemoveClaimFromAssessment(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			CreateWorkflowStateMachine(claim.GrantApplication).RemoveClaimFromAssessment(claim);
		}

		/// <summary>
		/// Select the specified claim for claim reimbursement assessment.
		/// </summary>
		/// <param name="claim"></param>
		public void AssessClaimReimbursement(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			CreateWorkflowStateMachine(claim.GrantApplication).AssessClaimReimbursement(claim);
		}

		/// <summary>
		/// Select the specified claim for claim eligibility assessment.
		/// </summary>
		/// <param name="claim"></param>
		public void AssessClaimEligibility(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			CreateWorkflowStateMachine(claim.GrantApplication).AssessClaimEligibility(claim);
		}

		/// <summary>
		/// Approve the specified claim.
		/// </summary>
		/// <param name="claim"></param>
		public void ApproveClaim(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			CreateWorkflowStateMachine(claim.GrantApplication).ApproveClaim(claim);
			if (claim.IsFinalClaim)
				this.CloseClaimReporting(claim.GrantApplication);
		}

		/// <summary>
		/// Deny the specified claim.
		/// </summary>
		/// <param name="claim"></param>
		public void DenyClaim(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			CreateWorkflowStateMachine(claim.GrantApplication).DenyClaim(claim);
		}

		/// <summary>
		/// Return the specified claim to the applicant.
		/// </summary>
		/// <param name="claim"></param>
		public void ReturnClaimToApplicant(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			CreateWorkflowStateMachine(claim.GrantApplication).ReturnClaimToApplicant(claim, claim.ClaimAssessmentNotes, this);
		}


		/// <summary>
		/// Initialize the Claim Amendment for the specified grant application.
		/// </summary>
		/// <param name="claim"></param>
		public void InitiateClaimAmendment(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			CreateWorkflowStateMachine(claim.GrantApplication).AmendClaim(claim, CreateNewClaimVersion);
		}

		/// <summary>
		/// Close the specified grant application for reporting.
		/// </summary>
		/// <param name="grantApplication"></param>
		public void CloseClaimReporting(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			CreateWorkflowStateMachine(grantApplication).CloseClaimReporting(ClearDraftClaims);
		}

		/// <summary>
		/// Enable the specified grant application for reporting.
		/// </summary>
		/// <param name="grantApplication"></param>
		public void EnableClaimReporting(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
			{
				var claim = grantApplication.GetCurrentClaim();
				if (claim == null)
				{
					CreateWorkflowStateMachine(grantApplication).EnableClaimReporting(null);
				}
				else
				{
					CreateWorkflowStateMachine(grantApplication).EnableClaimReporting(CreateNewClaimVersion);
				}
			}
			else
			{
				CreateWorkflowStateMachine(grantApplication).EnableClaimReporting(null);
			}
		}
		#endregion

		#region Helpers
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ApplicationWorkflowStateMachine"/> object.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		private ApplicationWorkflowStateMachine CreateWorkflowStateMachine(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			return new ApplicationWorkflowStateMachine(grantApplication,
													   _dbContext, 
													   _notificationService, 
													   _grantAgreementService, 
													   _grantOpeningService,
													   _prioritizationService,
													   _noteService,
													   _userService,
													   _httpContext,
													   _logger);
		}
		#endregion
	}
}
