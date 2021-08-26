using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="GrantOpeningManageScheduledService"/> class, provides a way to automatically update the state of Grant Openings based on their scheduled dates.
	/// </summary>
	public class GrantOpeningManageScheduledService : Service, IGrantOpeningManageScheduledService
	{
		#region Variables
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IFiscalYearService _fiscalYearService;

		private int _repeatCounter = 0;

		private int _maxConcurrencyAttempts = 0;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantOpeningManageScheduledService"/>.
		/// </summary>
		/// <param name="grantOpeningService"></param>
		/// <param name="fiscalYearService"></param>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public GrantOpeningManageScheduledService(IGrantOpeningService grantOpeningService, IFiscalYearService fiscalYearService, IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
			_grantOpeningService = grantOpeningService;
			_fiscalYearService = fiscalYearService;

			if (ConfigurationManager.AppSettings["MaxConcurrencyAttempts"] != null)
			{
				int.TryParse(ConfigurationManager.AppSettings["MaxConcurrencyAttempts"].ToString(), out _maxConcurrencyAttempts);
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Fetches the specified <typeparamref name="GrantOpening"/> and updates the state based on the schedule dates.  
		/// Will attempt to update the Grant Opening on concurrency failures up to the configured MaxConcurrencyAttempts.
		/// </summary>
		/// <param name="fiscalYearId"></param>
		public void ManageStateTransitions(int fiscalYearId)
		{
			var fiscalYear = _fiscalYearService.Get(fiscalYearId);

			var grantOpenings = _grantOpeningService.GetGrantOpenings(fiscalYear);

			try
			{
				foreach (var grantOpening in grantOpenings.Where(go => !go.State.In(GrantOpeningStates.Closed, GrantOpeningStates.OpenForSubmit, GrantOpeningStates.Unscheduled)))
				{
					ManageStateTransition(grantOpening);
				}
				Commit();
			}
			catch (DbUpdateConcurrencyException e)
			{
				do
				{
					ManageStateTransitions(fiscalYearId);
					_logger.Error(e, $"Manage State transition failed to update Grant Openings.");
					_repeatCounter++;
				} while (_repeatCounter < _maxConcurrencyAttempts);
			}
			catch (Exception e)
			{
				_logger.Error(e, $"Manage State transition failed to update Grant Openings.");
			}

			// clear counter
			_repeatCounter = 0;
		}

		/// <summary>
		/// Update the state of the specified <typeparamref name="GrantOpening"/> based on the scheduled dates.
		/// </summary>
		/// <param name="grantOpening">The Grant Opening to update.</param>
		public void ManageStateTransition(GrantOpening grantOpening)
		{
			switch (grantOpening.State)
			{
				case GrantOpeningStates.Unscheduled:
					break;
				case GrantOpeningStates.Scheduled:
					if (grantOpening.PublishDate <= AppDateTime.UtcNow)
					{
						grantOpening.State = GrantOpeningStates.Published;
					}
					break;
				case GrantOpeningStates.Published:
					if (grantOpening.OpeningDate <= AppDateTime.UtcNow)
					{
						grantOpening.State = GrantOpeningStates.Open;
					}
					break;
				case GrantOpeningStates.Open:
					if (grantOpening.ClosingDate <= AppDateTime.UtcNow)
					{
						grantOpening.State = GrantOpeningStates.Closed;
					}
					break;
				case GrantOpeningStates.Closed:
					break;
				default:
					break;
			}
		}
		#endregion  
	}
}
