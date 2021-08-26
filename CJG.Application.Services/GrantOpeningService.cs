using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class GrantOpeningService : Service, IGrantOpeningService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public GrantOpeningService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the specified Grant Opening.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public GrantOpening Get(int id)
		{
			return Get<GrantOpening>(id);
		}

		/// <summary>
		/// Add a new Grant Opening.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <returns></returns>
		public GrantOpening Add(GrantOpening grantOpening)
		{
			var lazyLoadTemp = grantOpening.GrantStream.GrantProgram;
			if (grantOpening == null)
				throw new ArgumentNullException(nameof(grantOpening));

			grantOpening.State = GrantOpeningStates.Unscheduled;
			grantOpening.GrantOpeningIntake = new GrantOpeningIntake();
			grantOpening.GrantOpeningFinancial = new GrantOpeningFinancial();
			_dbContext.GrantOpenings.Add(grantOpening);
			_dbContext.CommitTransaction();

			return grantOpening;
		}

		/// <summary>
		/// Update a current Grant Opening.
		/// Automatically update the State based on the current date.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <returns></returns>
		public GrantOpening Update(GrantOpening grantOpening)
		{
			if (grantOpening == null)
				throw new ArgumentNullException(nameof(grantOpening));

			var originalState = (GrantOpeningStates) _dbContext.OriginalValue(grantOpening, nameof(GrantOpening.State));
			if (originalState != grantOpening.State)
				throw new InvalidOperationException("Grant opening state cannot be changed.");

			if (grantOpening.State == GrantOpeningStates.Scheduled ||
				grantOpening.State == GrantOpeningStates.Published ||
				grantOpening.State == GrantOpeningStates.Open)
			{
				if (grantOpening.PublishDate < AppDateTime.UtcNow
					 && grantOpening.OpeningDate > AppDateTime.UtcNow)
				{
					grantOpening.State = GrantOpeningStates.Published;
				}
				else if (grantOpening.OpeningDate < AppDateTime.UtcNow
					&& grantOpening.ClosingDate > AppDateTime.UtcNow.ToUtcMidnight())
				{
					grantOpening.State = GrantOpeningStates.Open;
				}
				else if (grantOpening.ClosingDate < AppDateTime.UtcNow.ToUtcMidnight()
					&& grantOpening.State != GrantOpeningStates.Unscheduled)
				{
					grantOpening.State = GrantOpeningStates.Closed;
				}
			}

			_dbContext.Update<GrantOpening>(grantOpening);
			_dbContext.CommitTransaction();

			return grantOpening;
		}

		/// <summary>
		/// Delete the Grant Opening.
		/// </summary>
		/// <param name="grantOpening"></param>
		public void Delete(GrantOpening grantOpening)
		{
			if (grantOpening == null)
				throw new ArgumentNullException(nameof(grantOpening));

			Delete(grantOpening.Id);
		}

		/// <summary>
		/// Delete the Grant Opening for the specified Id.
		/// </summary>
		/// <param name="grantOpeningId"></param>
		public void Delete(int grantOpeningId)
		{
			var grantOpening = Get<GrantOpening>(grantOpeningId);

			if (grantOpening.State != GrantOpeningStates.Unscheduled)
				throw new InvalidOperationException($"Cannot delete grant opening in state: {grantOpening.State}");

			if (grantOpening.GrantApplications.Any())
				throw new InvalidOperationException("Cannot delete grant opening with linked applications");

			if (grantOpening.GrantOpeningIntake != null)
				_dbContext.GrantOpeningIntakes.Remove(grantOpening.GrantOpeningIntake);

			if (grantOpening.GrantOpeningFinancial != null)
				_dbContext.GrantOpeningFinancials.Remove(grantOpening.GrantOpeningFinancial);

			_dbContext.GrantOpenings.Remove(grantOpening);

			_dbContext.Commit();
		}

		/// <summary>
		/// Determine whether the Grant Opening can be deleted.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <returns></returns>
		public bool CanDeleteGrantOpening(GrantOpening grantOpening)
		{
			if (grantOpening == null)
				throw new ArgumentNullException(nameof(grantOpening));

			return grantOpening.State == GrantOpeningStates.Unscheduled &&
				   !grantOpening.GrantApplications.Any();
		}

		/// <summary>
		/// Check if there is a grant opening already for this training period and grant stream.
		/// </summary>
		/// <param name="trainingPeriodId"></param>
		/// <param name="grantStreamId"></param>
		/// <returns></returns>
		public bool CheckGrantOpeningByFiscalAndStream(int trainingPeriodId, int grantStreamId)
		{
			return _dbContext.GrantOpenings.Any(x => x.TrainingPeriodId == trainingPeriodId && x.GrantStreamId == grantStreamId);
		}

		/// <summary>
		/// Get all the Grant Openings for the specified Fiscal Year.
		/// </summary>
		/// <param name="fiscalYear"></param>
		/// <returns></returns>
		public IEnumerable<GrantOpening> GetGrantOpenings(FiscalYear fiscalYear)
		{
			if (fiscalYear == null)
				throw new ArgumentNullException(nameof(fiscalYear));

			return GetGrantOpeningsForFiscalYear(fiscalYear.Id);
		}

		/// <summary>
		/// Get all the Grant Openings for the specified Grant Stream.
		/// </summary>
		/// <param name="grantStream"></param>
		/// <returns></returns>
		public IEnumerable<GrantOpening> GetGrantOpenings(GrantStream grantStream)
		{
			if (grantStream == null)
				throw new ArgumentNullException(nameof(grantStream));

			var grantOpenings = _dbContext.GrantOpenings.AsNoTracking().Where(x => x.GrantStreamId == grantStream.Id);
			return grantOpenings;
		}

		/// <summary>
		/// Get all the Grant Openings for the specified Fiscal Year Id.
		/// </summary>
		/// <param name="fiscalYearId"></param>
		/// <returns></returns>
		public IEnumerable<GrantOpening> GetGrantOpeningsForFiscalYear(int fiscalYearId)
		{
			var grantOpenings = _dbContext.GrantOpenings.Where(x => x.TrainingPeriod.FiscalYearId == fiscalYearId).OrderBy(go => go.GrantStream.Name).ThenBy(go => go.TrainingPeriod.StartDate);
			return grantOpenings;
		}

		/// <summary>
		/// Get all the Grant Openings.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<GrantOpening> GetGrantOpenings()
		{
			var grantOpenings = _dbContext.GrantOpenings.Where(go => go.Id > 0).ToList();
			return grantOpenings;
		}

		/// <summary>
		/// Determine if the specified Grant Stream is associated to any Grant Openings.
		/// </summary>
		/// <param name="grantStream"></param>
		/// <returns></returns>
		public bool AssociatedWithAGrantStream(GrantStream grantStream)
		{
			if (grantStream == null)
				throw new ArgumentNullException(nameof(grantStream));

			return _dbContext.GrantOpenings.AsNoTracking().Where(x => x.GrantStreamId == grantStream.Id).Count() > 0;
		}

		/// <summary>
		/// Get all the Grant Openings for the specified Date.
		/// </summary>
		/// <param name="forThisDate"></param>
		/// <returns></returns>
		public IEnumerable<GrantOpening> GetGrantOpenings(DateTime forThisDate)
		{
			DateTime chkDate = new AppDateTime(forThisDate.Year, forThisDate.Month, forThisDate.Day);

			return _dbContext.GrantOpenings
				.Where(o => o.GrantStream.IsActive == true
				&& o.ClosingDate >= chkDate && (o.State == GrantOpeningStates.Open || o.State == GrantOpeningStates.Published))
				.OrderBy(o => o.TrainingPeriod.StartDate)
				.ThenBy(o => o.GrantStream.Name);
		}

		/// <summary>
		/// Get all the Grant Openings for the specified Date and Grant Program Id.
		/// </summary>
		/// <param name="forThisDate"></param>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		public IEnumerable<GrantOpening> GetGrantOpenings(DateTime forThisDate, int grantProgramId)
		{
			DateTime chkDate = new AppDateTime(forThisDate.Year, forThisDate.Month, forThisDate.Day);

			return _dbContext.GrantOpenings
				.AsNoTracking().Where(o => o.GrantStream.IsActive == true
				&& o.GrantStream.GrantProgramId == grantProgramId
				&& o.GrantStream.GrantProgram.State == GrantProgramStates.Implemented
				&& o.ClosingDate >= chkDate && (o.State == GrantOpeningStates.Open || o.State == GrantOpeningStates.Published))
				.OrderBy(o => o.TrainingPeriod.StartDate)
				.ThenBy(o => o.GrantStream.Name);
		}

		/// <summary>
		/// Get all the Grant Openings for the specified Date, Grant Stream Id and Training Period Id.
		/// </summary>
		/// <param name="forThisDate"></param>
		/// <param name="selectedStreamId"></param>
		/// <param name="selectedTrainingPeriodId"></param>
		/// <returns></returns>
		public IEnumerable<GrantOpening> GetGrantOpenings(DateTime forThisDate, int selectedStreamId, int selectedTrainingPeriodId)
		{
			DateTime chkDate = new AppDateTime(forThisDate.Year, forThisDate.Month, forThisDate.Day);

			return _dbContext.GrantOpenings
				.AsNoTracking().Where(o => o.GrantStream.IsActive == true
				&& ((o.ClosingDate >= chkDate && (o.State == GrantOpeningStates.Published || o.State == GrantOpeningStates.Open))
				 || (o.State == GrantOpeningStates.OpenForSubmit && o.GrantStreamId == selectedStreamId && o.TrainingPeriodId == selectedTrainingPeriodId)))
				.OrderBy(o => o.TrainingPeriod.StartDate)
				.ThenBy(o => o.GrantStream.Name);
		}

		/// <summary>
		/// Get all the Grant Openings that are Published and in an Active Grant Stream with a Closing date greater than or equal to the specified Date.
		/// </summary>
		/// <param name="forThisDate"></param>
		/// <returns></returns>
		public IEnumerable<GrantOpening> GetPublishedGrantOpenings(DateTime forThisDate)
		{
			return _dbContext.GrantOpenings.AsNoTracking().Where(o => o.GrantStream.IsActive == true && o.ClosingDate >= forThisDate && o.State == GrantOpeningStates.Published);
		}

		/// <summary>
		/// Get the Grant Opening for the specified Grant Stream Id and Training period Id.
		/// </summary>
		/// <param name="grantStreamId"></param>
		/// <param name="trainingPeriodId"></param>
		/// <returns></returns>
		public GrantOpening GetGrantOpening(int grantStreamId, int trainingPeriodId)
		{
			return _dbContext.GrantOpenings
				.AsNoTracking().Where(x => x.TrainingPeriod.Id == trainingPeriodId && x.GrantStreamId == grantStreamId)
				.OrderBy(go => go.GrantStream.Name).ThenBy(go => go.TrainingPeriod.StartDate)
				.FirstOrDefault();
		}

		/// <summary>
		/// Get all the Grant Openings for the specified Grant Opening States.
		/// </summary>
		/// <param name="filterStates"></param>
		/// <returns></returns>
		public IEnumerable<GrantOpening> GetGrantOpeningsInStates(IEnumerable<GrantOpeningStates> filterStates)
		{
			return _dbContext.GrantOpenings.Where(x => x.GrantStream.IsActive && filterStates.Contains(x.State))
				.ToList();

		}

		/// <summary>
		/// Get all the Grant Applications that are New.
		/// </summary>
		/// <param name="grantOpeningId"></param>
		/// <param name="dateSubmitted"></param>
		/// <returns></returns>
		private IEnumerable<GrantApplication> GetNewApplications(int grantOpeningId, DateTime dateSubmitted)
		{
			return _dbContext.GrantApplications
				.Where(x => x.GrantOpening.Id == grantOpeningId
					&& x.DateSubmitted.Value <= dateSubmitted
					&& x.ApplicationStateInternal == ApplicationStateInternal.New);
		}

		public IDictionary<int, decimal> GetBudgetllocationAmountsInTrainingPeriods(int fiscalYearId, int? programId, int? streamId)
		{
			var queryable = _dbContext.GrantOpenings
				.Include(x => x.TrainingPeriod)
			   .AsNoTracking().Where(x => x.TrainingPeriod.FiscalYearId == fiscalYearId);

			if (streamId.HasValue)
			{
				queryable = queryable
					.Include(x => x.GrantStream)
					.AsNoTracking().Where(x => x.GrantStream.Id == streamId.Value);
			}

			if (programId.HasValue)
			{
				queryable = queryable
					.Include(x => x.GrantStream.GrantProgram)
					.AsNoTracking().Where(x => x.GrantStream.GrantProgram.Id == programId.Value);
			}

			return queryable.GroupBy(x => x.TrainingPeriodId)
				.Select(g => new { TrainingPeriodId = g.Key, BudgetllocationAmount = g.Sum(x => x.BudgetAllocationAmt) })
				.ToDictionary(x => x.TrainingPeriodId, x => x.BudgetllocationAmount);
		}

		#region Workflow
		/// <summary>
		/// Change the state of the Grant Opening to Scheduled.
		/// </summary>
		/// <param name="grantOpening"></param>
		public void Schedule(GrantOpening grantOpening)
		{
			if (grantOpening == null)
				throw new ArgumentNullException(nameof(grantOpening));

			if (grantOpening.ClosingDate < AppDateTime.UtcNow.ToLocalMidnight())
				throw new InvalidOperationException("Closing date cannot be before today's date.");

			if (grantOpening.PublishDate < AppDateTime.UtcNow
				&& grantOpening.OpeningDate > AppDateTime.UtcNow)
			{
				grantOpening.State = GrantOpeningStates.Published;
			}
			else if (grantOpening.OpeningDate < AppDateTime.UtcNow
				&& grantOpening.ClosingDate > AppDateTime.UtcNow.ToUtcMidnight())
			{
				grantOpening.State = GrantOpeningStates.Open;
			}
			else if (grantOpening.ClosingDate < AppDateTime.UtcNow.ToUtcMidnight()
				&& grantOpening.State != GrantOpeningStates.Unscheduled)
			{
				grantOpening.State = GrantOpeningStates.Closed;
			}
			else
			{
				grantOpening.State = GrantOpeningStates.Scheduled;
			}

			_dbContext.Update<GrantOpening>(grantOpening);
			CommitTransaction();
		}

		/// <summary>
		/// Change the state of the Grant Opening to Unscheduled.
		/// </summary>
		/// <param name="grantOpening"></param>
		public void Unschedule(GrantOpening grantOpening)
		{
			if (grantOpening == null)
				throw new ArgumentNullException(nameof(grantOpening));

			if (grantOpening.State != GrantOpeningStates.Scheduled)
				throw new InvalidOperationException("Cannot change the Grant Opening state to Unscheduled.");

			grantOpening.State = GrantOpeningStates.Unscheduled;
			_dbContext.Update<GrantOpening>(grantOpening);
			CommitTransaction();
		}

		/// <summary>
		/// Change the state of the Grant Opening to Closed.
		/// </summary>
		/// <param name="grantOpening"></param>
		public void Close(GrantOpening grantOpening)
		{
			if (grantOpening == null)
				throw new ArgumentNullException(nameof(grantOpening));

			if (grantOpening.State == GrantOpeningStates.Unscheduled)
				throw new InvalidOperationException("Cannot close an unschedule Grant Opening.");

			grantOpening.State = GrantOpeningStates.Closed;
			_dbContext.Update<GrantOpening>(grantOpening);
			CommitTransaction();
		}

		/// <summary>
		/// Change the state of the Grant Opening from Closed to Open.
		/// </summary>
		/// <param name="grantOpening"></param>
		public void Reopen(GrantOpening grantOpening)
		{
			if (grantOpening == null)
				throw new ArgumentNullException(nameof(grantOpening));

			if (grantOpening.State != GrantOpeningStates.Closed)
			{
				throw new InvalidOperationException();
			}

			if (AppDateTime.UtcNow > grantOpening.ClosingDate)
			{
				grantOpening.ClosingDate = AppDateTime.Now.ToUtcMidnight();
			}

			grantOpening.State = GrantOpeningStates.Open;
			_dbContext.Update<GrantOpening>(grantOpening);
			CommitTransaction();
		}

		/// <summary>
		/// Change the state of the Grant Opening from Closed to OpenForSubmit.
		/// </summary>
		/// <param name="grantOpening"></param>
		public void OpenForSubmit(GrantOpening grantOpening)
		{
			if (grantOpening == null)
				throw new ArgumentNullException(nameof(grantOpening));

			if (grantOpening.State != GrantOpeningStates.Closed)
			{
				throw new InvalidOperationException("Cannot change the state to Open for Submit unless the current Grant Opening is Closed.");
			}

			grantOpening.State = GrantOpeningStates.OpenForSubmit;
			_dbContext.Update<GrantOpening>(grantOpening);
			CommitTransaction();
		}

		#endregion

		public Dictionary<int, decimal[]> GetGrantOpeningFinancialsInTrainingPeriods(int fiscalYearId, int? programId, int? grantStreamId)
		{
			var queryable = _dbContext.GrantOpeningFinancials.Include(x => x.GrantOpening)
				.Include(x => x.GrantOpening.GrantOpeningFinancial)
				.Include(x => x.GrantOpening.TrainingPeriod)
				.AsNoTracking().Where(x => x.GrantOpening.TrainingPeriod.FiscalYearId == fiscalYearId);

			if (grantStreamId.HasValue)
			{
				queryable = queryable
					.Include(x => x.GrantOpening.GrantStream)
					.AsNoTracking().Where(x => x.GrantOpening.GrantStream.Id == grantStreamId.Value);
			}

			if (programId.HasValue)
			{
				queryable = queryable
					.Include(x => x.GrantOpening.GrantStream.GrantProgram)
					.AsNoTracking().Where(x => x.GrantOpening.GrantStream.GrantProgram.Id == programId.Value);
			}

			return
				queryable.GroupBy(x => x.GrantOpening.TrainingPeriodId)
					.Select(g => new
					{
						TrainingPeriodId = g.Key,
						OutstandingCommintmentsAmount = g.Sum(x => x.OutstandingCommitments),
						OutstandingCommintmentCount = g.Sum(x => x.OutstandingCommitmentCount),
						CurrentClaimsAmount = g.Sum(x => x.CurrentClaims),
						CurrentClaimCount = g.Sum(x => x.CurrentClaimCount),
						ClaimsAssessedCount = g.Sum(x => x.ClaimsAssessedCount),
						ClaimsAssessedAmount = g.Sum(x => x.ClaimsAssessed),
					})
				   .ToDictionary(x => x.TrainingPeriodId, x => new[] {
					   x.OutstandingCommintmentsAmount,
					   0,
					   0,
					   x.OutstandingCommintmentCount,
					   x.CurrentClaimCount,
					   x.ClaimsAssessedCount,
					   x.CurrentClaimsAmount ,
					   x.ClaimsAssessedAmount});
		}

		public IQueryable<GrantOpening> GetGrantOpenings(GrantProgram grantProgram)
		{
			if (grantProgram == null)
				throw new ArgumentNullException(nameof(grantProgram));

			return _dbContext.GrantOpenings.AsNoTracking().Where(x => x.GrantStream.GrantProgramId == grantProgram.Id);
		}

		#region Financial
		/// <summary>
		/// Make a reservation for the specified Grant Application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public List<int> MakeReservation(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			List<int> reservedApplicationIds = new List<int>();

			if (grantApplication.GrantOpening.GrantStream.GrantProgram.UseFIFOReservation)
			{
				decimal total_estimated_reimbursement = 0;
				var grantOpening = Get(grantApplication.GrantOpeningId);
				var newApplications = GetNewApplications(grantApplication.GrantOpeningId, grantApplication.DateSubmitted.Value).ToList();
				var applicationEstimatedCost = new Dictionary<int, decimal>();

				// Make sure to include the selected grant application.
				if (!newApplications.Any(ga => ga.Id == grantApplication.Id))
					newApplications.Add(grantApplication);

				// Loop through all estimated cost items, and accumulate the amount in the request
				foreach (GrantApplication ga in newApplications)
				{
					var amount = ga.GetEstimatedReimbursement();
					applicationEstimatedCost.Add(ga.Id, amount);
					total_estimated_reimbursement += amount;
				}

				// Checked to see if there are anough funds available in the GrantOpening to cover the request
				if (grantOpening.IntakeTargetAmt < total_estimated_reimbursement
					+ grantOpening.GrantOpeningFinancial.CurrentReservations
					+ grantOpening.GrantOpeningFinancial.AssessedCommitments)
				{
					// Funds are not available to cover the entire requests selected
					throw new DbEntityValidationException("Insufficient funds to make the reservation requested");
				}

				// Enough funds are in the Grant Opening to cover the request, update the state of all newApplications other than the one explicitly selected for assessment, and update the GrantOpening.CurrentReservations
				foreach (var ga in newApplications)
				{
					if (ga.ApplicationStateInternal == ApplicationStateInternal.New || ga.Id == grantApplication.Id)
					{
						// Change the state of all other applications, since the state machine has already handled the state change for the originally selected application
						if (ga.Id != grantApplication.Id)
						{
							ga.ApplicationStateInternal = ApplicationStateInternal.PendingAssessment;
						}

						// If there is an issue with a specific grant application stop the process and throw the validation error.
						var validationResults = Validate(ga);
						if(validationResults.Any())
						{
							throw new DbEntityValidationException($"While attempting to select application \"{ga.FileNumber}\" for assessment the following error occured. {String.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage))}");
						}

						reservedApplicationIds.Add(ga.Id);
						AdjustFinancialStatements(ga, ga.ApplicationStateInternal, ApplicationWorkflowTrigger.SelectForAssessment);
					}
				}
			}
			else
			{
				if (CanMakeReservation(grantApplication))
				{
					AdjustFinancialStatements(grantApplication, grantApplication.ApplicationStateInternal, ApplicationWorkflowTrigger.SelectForAssessment);
					reservedApplicationIds.Add(grantApplication.Id);
				}
				else
				{
					throw new DbEntityValidationException("Insufficient funds to make the reservation requested");
				}
			}
			return reservedApplicationIds;
		}

		/// <summary>
		/// Check if there are enough funds to make a reservation for the specified Grant Application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public bool CanMakeReservation(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			var grantOpening = Get(grantApplication.GrantOpeningId);
			return grantOpening.IntakeTargetAmt >= grantApplication.GetEstimatedReimbursement()
				+ grantOpening.GrantOpeningFinancial.CurrentReservations
				+ grantOpening.GrantOpeningFinancial.AssessedCommitments;
		}

		/// <summary>
		/// Update the GrantOpening.GrantOpeningIntake and GrantOpening.GrantOpeningFinancial statements.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="priorState"></param>
		/// <param name="trigger"></param>
		public void AdjustFinancialStatements(GrantApplication grantApplication, ApplicationStateInternal priorState, ApplicationWorkflowTrigger trigger)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			var grantOpening = Get(grantApplication.GrantOpeningId);
			var estimated_amount = grantApplication.GetEstimatedReimbursement();
			var agreed_amount = grantApplication.GetAgreedCommitment();

			var claim = grantApplication.GetCurrentClaim();
			var has_prior_approved_claim = claim?.HasPriorApprovedClaim() ?? false;

			switch (trigger)
			{
				case ApplicationWorkflowTrigger.SubmitApplication:
					IncreaseIntakeNew(grantOpening, estimated_amount);
					break;
				case ApplicationWorkflowTrigger.SelectForAssessment:
					DecreaseIntakeNew(grantOpening, estimated_amount);
					IncreaseIntakePending(grantOpening, estimated_amount);
					IncreaseFinancialReservation(grantOpening, estimated_amount);
					break;
				case ApplicationWorkflowTrigger.RemoveFromAssessment:
					DecreaseIntakePending(grantOpening, estimated_amount);
					IncreaseIntakeNew(grantOpening, estimated_amount);
					DecreaseFinancialReservation(grantOpening, estimated_amount);
					break;
				case ApplicationWorkflowTrigger.BeginAssessment:
					DecreaseIntakePending(grantOpening, estimated_amount);
					IncreaseIntakeUnderAssessment(grantOpening, estimated_amount);
					break;
				case ApplicationWorkflowTrigger.DenyApplication:
					DecreaseIntakeUnderAssessment(grantOpening, estimated_amount);
					IncreaseIntakeDenied(grantOpening, estimated_amount);
					DecreaseFinancialReservation(grantOpening, estimated_amount);
					break;
				case ApplicationWorkflowTrigger.ReturnToAssessment:
					if (priorState == ApplicationStateInternal.ApplicationDenied)
					{
						IncreaseIntakeUnderAssessment(grantOpening, estimated_amount);
						DecreaseIntakeDenied(grantOpening, estimated_amount);
						IncreaseFinancialReservation(grantOpening, estimated_amount);
					}
					break;
				case ApplicationWorkflowTrigger.RejectGrantAgreement:
				case ApplicationWorkflowTrigger.WithdrawOffer:
					DecreaseIntakeUnderAssessment(grantOpening, estimated_amount);
					IncreaseIntakeWithdrawn(grantOpening, estimated_amount);
					DecreaseFinancialReservation(grantOpening, estimated_amount);
					break;
				case ApplicationWorkflowTrigger.ReturnUnfundedApplications:
					DecreaseIntakeNew(grantOpening, estimated_amount);
					break;
				case ApplicationWorkflowTrigger.WithdrawApplication:
					switch (priorState)
					{
						case ApplicationStateInternal.New:
							DecreaseIntakeNew(grantOpening, estimated_amount);
							break;
						case ApplicationStateInternal.PendingAssessment:
							DecreaseIntakePending(grantOpening, estimated_amount);
							DecreaseFinancialReservation(grantOpening, estimated_amount);
							IncreaseIntakeWithdrawn(grantOpening, estimated_amount);
							break;
						case ApplicationStateInternal.UnderAssessment:
						case ApplicationStateInternal.RecommendedForApproval:
						case ApplicationStateInternal.RecommendedForDenial:
						case ApplicationStateInternal.ReturnedToAssessment:
							DecreaseIntakeUnderAssessment(grantOpening, estimated_amount);
							DecreaseFinancialReservation(grantOpening, estimated_amount);
							IncreaseIntakeWithdrawn(grantOpening, estimated_amount);
							break;
					}
					break;
				case ApplicationWorkflowTrigger.AcceptGrantAgreement:
					DecreaseIntakeUnderAssessment(grantOpening, estimated_amount);
					DecreaseFinancialReservation(grantOpening, estimated_amount);
					IncreaseIntakeReductions(grantOpening, estimated_amount - agreed_amount);
					IncreaseFinancialAssessedCommitments(grantOpening, agreed_amount);
					IncreaseFinancialOutstandingCommitments(grantOpening, agreed_amount);
					break;
				case ApplicationWorkflowTrigger.CancelAgreementHolder:
				case ApplicationWorkflowTrigger.CancelAgreementMinistry:
					DecreaseFinancialOutstandingCommitments(grantOpening, agreed_amount);
					IncreaseFinancialCancellation(grantOpening, agreed_amount);
					break;
				case ApplicationWorkflowTrigger.CloseClaimReporting:
					if (claim.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments)
					{
						var approved = grantApplication.Claims.Where(x => x.IsApproved()).Sum(x => x.TotalAssessedReimbursement);
						var prior_claim = grantApplication.GetPriorClaim();
						if (prior_claim != null)
						{
							grantOpening.GrantOpeningFinancial.OutstandingCommitments += approved - grantApplication.TrainingCost.AgreedCommitment; // IncreaseFinancialOutstandingCommitments
							grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount -= 1;
						}
						else
						{
							IncreaseFinancialOutstandingCommitments(grantOpening, grantApplication.TrainingCost.AgreedCommitment - approved); // IncreaseFinancialOutstandingCommitments
						}
					}
					break;
				case ApplicationWorkflowTrigger.EnableClaimReporting:
					if (claim.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments)
					{
						var approved = grantApplication.Claims.Where(x => x.IsApproved()).Sum(x => x.TotalAssessedReimbursement);
						IncreaseFinancialOutstandingCommitments(grantOpening, grantApplication.TrainingCost.AgreedCommitment - approved);
					}
					break;
				case ApplicationWorkflowTrigger.SubmitClaim:
					if (claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
					{
						if (!has_prior_approved_claim || claim.ClaimVersion == 1)
						{
							DecreaseFinancialOutstandingCommitments(grantOpening, agreed_amount);
							IncreaseFinancialCurrentClaims(grantOpening, claim.TotalClaimReimbursement);
						}
					}
					else if (claim.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments)
					{
						grantOpening.GrantOpeningFinancial.OutstandingCommitments -= claim.TotalClaimReimbursement; // DecreaseFinancialOutstandingCommitments
						grantOpening.GrantOpeningFinancial.CurrentClaims += claim.TotalClaimReimbursement; // IncreaseFinancialCurrentClaims
					}
					break;
				case ApplicationWorkflowTrigger.WithdrawClaim:
					if (claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
					{
						if (!has_prior_approved_claim || claim.ClaimVersion == 1)
						{
							DecreaseFinancialCurrentClaims(grantOpening, claim.TotalClaimReimbursement);
							IncreaseFinancialOutstandingCommitments(grantOpening, agreed_amount);
						}
					}
					else if (claim.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments)
					{
						grantOpening.GrantOpeningFinancial.CurrentClaims -= claim.TotalClaimReimbursement; // DecreaseFinancialCurrentClaims
						grantOpening.GrantOpeningFinancial.OutstandingCommitments += claim.TotalClaimReimbursement; // IncreaseFinancialOutstandingCommitments
					}
					break;
				case ApplicationWorkflowTrigger.ApproveClaim:
					if (claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
					{
						if (!has_prior_approved_claim || claim.ClaimVersion == 1)
						{
							DecreaseFinancialCurrentClaims(grantOpening, claim.TotalClaimReimbursement);
							IncreaseFinancialAssessedClaims(grantOpening, claim.TotalAssessedReimbursement);
						}
						else
						{
							// Get the previously approved claim.
							var prior_approved_claim = claim.GetPriorApprovedClaim();
							DecreaseFinancialAssessedClaims(grantOpening, prior_approved_claim.TotalAssessedReimbursement);
							IncreaseFinancialAssessedClaims(grantOpening, claim.TotalAssessedReimbursement);
						}
					}
					else if (claim.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments)
					{
						grantOpening.GrantOpeningFinancial.OutstandingCommitments += claim.TotalClaimReimbursement - claim.TotalAssessedReimbursement; // IncreaseFinancialOutstandingCommitments
						grantOpening.GrantOpeningFinancial.CurrentClaims -= claim.TotalClaimReimbursement; // DecreaseFinancialCurrentClaims
						grantOpening.GrantOpeningFinancial.ClaimsAssessed += claim.TotalAssessedReimbursement; // IncreaseFinancialAssessedClaims
					}
					break;
				case ApplicationWorkflowTrigger.DenyClaim:
					ZeroAssessedClaimValues(claim);
					if (claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
					{
						if (!has_prior_approved_claim || claim.ClaimVersion == 1)
						{
							DecreaseFinancialCurrentClaims(grantOpening, claim.TotalClaimReimbursement);
							IncreaseFinancialOutstandingCommitments(grantOpening, claim.GrantApplication.TrainingCost.AgreedCommitment);
							IncreaseFinancialClaimsDenied(grantOpening, claim.TotalClaimReimbursement);
						}
					}
					else if (claim.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments)
					{
						grantOpening.GrantOpeningFinancial.CurrentClaims -= claim.TotalClaimReimbursement; // DecreaseFinancialCurrentClaims
						grantOpening.GrantOpeningFinancial.OutstandingCommitments += claim.TotalClaimReimbursement; // IncreaseFinancialOutstandingCommitments
						grantOpening.GrantOpeningFinancial.ClaimsDenied += claim.TotalClaimReimbursement; // IncreaseFinancialClaimsDenied
					}
					break;
				case ApplicationWorkflowTrigger.ReturnClaimToApplicant:
					if (claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
					{
						if (!has_prior_approved_claim || claim.ClaimVersion == 1)
						{
							DecreaseFinancialCurrentClaims(grantOpening, claim.TotalClaimReimbursement);
							IncreaseFinancialOutstandingCommitments(grantOpening, claim.GrantApplication.TrainingCost.AgreedCommitment);
						}
					}
					else if (claim.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments)
					{
						grantOpening.GrantOpeningFinancial.CurrentClaims -= claim.TotalClaimReimbursement; // DecreaseFinancialCurrentClaims
						grantOpening.GrantOpeningFinancial.OutstandingCommitments += claim.TotalClaimReimbursement; // IncreaseFinancialOutstandingCommitments

					}
					break;
				default:
					break;
			}
		}

		#region Helpers
		private void IncreaseFinancialReservation(GrantOpening grantOpening, decimal estimatedGovernmentContribution)
		{
			grantOpening.GrantOpeningFinancial.CurrentReservations += estimatedGovernmentContribution;
		}

		private void DecreaseFinancialReservation(GrantOpening grantOpening, decimal estimatedGovernmentContribution)
		{
			grantOpening.GrantOpeningFinancial.CurrentReservations -= estimatedGovernmentContribution;
		}

		private void IncreaseFinancialAssessedCommitments(GrantOpening grantOpening, decimal governmentContribution)
		{
			grantOpening.GrantOpeningFinancial.AssessedCommitmentsCount += 1;
			grantOpening.GrantOpeningFinancial.AssessedCommitments += governmentContribution;
		}

		private void IncreaseFinancialOutstandingCommitments(GrantOpening grantOpening, decimal governmentContribution)
		{
			grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount += 1;
			grantOpening.GrantOpeningFinancial.OutstandingCommitments += governmentContribution;
		}

		private void IncreaseFinancialCancellation(GrantOpening grantOpening, decimal governmentContribution)
		{
			grantOpening.GrantOpeningFinancial.CancellationsCount += 1;
			grantOpening.GrantOpeningFinancial.Cancellations += governmentContribution;
		}

		private void IncreaseFinancialAssessedClaims(GrantOpening grantOpening, decimal governmentContribution)
		{
			grantOpening.GrantOpeningFinancial.ClaimsAssessedCount += 1;
			grantOpening.GrantOpeningFinancial.ClaimsAssessed += governmentContribution;
		}

		private void DecreaseFinancialAssessedClaims(GrantOpening grantOpening, decimal governmentContribution)
		{
			grantOpening.GrantOpeningFinancial.ClaimsAssessedCount -= 1;
			grantOpening.GrantOpeningFinancial.ClaimsAssessed -= governmentContribution;
		}

		private void IncreaseIntakeNew(GrantOpening grantOpening, decimal estimatedGovernmentContribution)
		{
			grantOpening.GrantOpeningIntake.NewCount += 1;
			grantOpening.GrantOpeningIntake.NewAmt += estimatedGovernmentContribution;
		}

		private void DecreaseIntakeNew(GrantOpening grantOpening, decimal estimatedGovernmentContribution)
		{
			grantOpening.GrantOpeningIntake.NewCount -= 1;
			grantOpening.GrantOpeningIntake.NewAmt -= estimatedGovernmentContribution;
		}

		private void IncreaseIntakePending(GrantOpening grantOpening, decimal estimatedGovernmentContribution)
		{
			grantOpening.GrantOpeningIntake.PendingAssessmentCount += 1;
			grantOpening.GrantOpeningIntake.PendingAssessmentAmt += estimatedGovernmentContribution;
		}

		private void DecreaseIntakePending(GrantOpening grantOpening, decimal estimatedGovernmentContribution)
		{
			grantOpening.GrantOpeningIntake.PendingAssessmentCount -= 1;
			grantOpening.GrantOpeningIntake.PendingAssessmentAmt -= estimatedGovernmentContribution;
		}

		private void IncreaseIntakeUnderAssessment(GrantOpening grantOpening, decimal estimatedGovernmentContribution)
		{
			grantOpening.GrantOpeningIntake.UnderAssessmentCount += 1;
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt += estimatedGovernmentContribution;
		}

		private void DecreaseIntakeUnderAssessment(GrantOpening grantOpening, decimal estimatedGovernmentContribution)
		{
			grantOpening.GrantOpeningIntake.UnderAssessmentCount -= 1;
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt -= estimatedGovernmentContribution;
		}

		private void IncreaseIntakeDenied(GrantOpening grantOpening, decimal estimatedGovernmentContribution)
		{
			grantOpening.GrantOpeningIntake.DeniedCount += 1;
			grantOpening.GrantOpeningIntake.DeniedAmt += estimatedGovernmentContribution;
		}

		private void DecreaseIntakeDenied(GrantOpening grantOpening, decimal estimatedGovernmentContribution)
		{
			grantOpening.GrantOpeningIntake.DeniedCount -= 1;
			grantOpening.GrantOpeningIntake.DeniedAmt -= estimatedGovernmentContribution;
		}

		private void IncreaseIntakeWithdrawn(GrantOpening grantOpening, decimal estimatedGovernmentContribution)
		{
			grantOpening.GrantOpeningIntake.WithdrawnCount += 1;
			grantOpening.GrantOpeningIntake.WithdrawnAmt += estimatedGovernmentContribution;
		}

		private void IncreaseIntakeReductions(GrantOpening grantOpening, decimal reduction)
		{
			grantOpening.GrantOpeningIntake.ReductionsAmt += reduction;
		}

		private void DecreaseIntakeReductions(GrantOpening grantOpening, decimal reduction)
		{
			grantOpening.GrantOpeningIntake.ReductionsAmt -= reduction;
		}

		private void DecreaseFinancialOutstandingCommitments(GrantOpening grantOpening, decimal governmentContribution)
		{
			grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount -= 1;
			grantOpening.GrantOpeningFinancial.OutstandingCommitments -= governmentContribution;
		}

		private void DecreaseFinancialCurrentClaims(GrantOpening grantOpening, decimal reduction)
		{
			grantOpening.GrantOpeningFinancial.CurrentClaimCount -= 1;
			grantOpening.GrantOpeningFinancial.CurrentClaims -= reduction;
		}

		private void IncreaseFinancialCurrentClaims(GrantOpening grantOpening, decimal increase)
		{
			grantOpening.GrantOpeningFinancial.CurrentClaimCount += 1;
			grantOpening.GrantOpeningFinancial.CurrentClaims += increase;
		}

		private void IncreaseFinancialClaimsDenied(GrantOpening grantOpening, decimal increase)
		{
			grantOpening.GrantOpeningFinancial.ClaimsDeniedCount += 1;
			grantOpening.GrantOpeningFinancial.ClaimsDenied += increase;
		}

		private void ZeroAssessedClaimValues(Claim claim)
		{
			claim.TotalAssessedReimbursement = 0;

			foreach (var claimEligibleCost in claim.EligibleCosts)
			{
				claimEligibleCost.AssessedCost = 0;
				claimEligibleCost.AssessedParticipants = 0;
				claimEligibleCost.AssessedMaxParticipantCost = 0;
				claimEligibleCost.AssessedMaxParticipantReimbursementCost = 0;
				claimEligibleCost.AssessedParticipantEmployerContribution = 0;

				foreach (var participantCost in (claimEligibleCost.ParticipantCosts))
				{
					participantCost.AssessedParticipantCost = 0;
					participantCost.AssessedReimbursement = 0;
					participantCost.AssessedEmployerContribution = 0;
				}
			}
		}
		#endregion
		#endregion
		#endregion
	}
}
