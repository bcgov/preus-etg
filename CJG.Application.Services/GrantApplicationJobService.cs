using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;

namespace CJG.Application.Services
{
	public class GrantApplicationJobService : Service, IGrantApplicationJobService
	{
		private readonly INotificationService _notificationService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly INoteService _noteService;
		private readonly IUserService _userService;

		public GrantApplicationJobService(
			INotificationService notificationService,
			IGrantOpeningService grantOpeningService,
			INoteService noteService,
			IUserService userService,
			IDataContext context, HttpContextBase httpContext, ILogger logger)
			: base(context, httpContext, logger)
		{
			_notificationService = notificationService;
			_grantOpeningService = grantOpeningService;
			_noteService = noteService;
			_userService = userService;
		}

		public IEnumerable<GrantApplication> GetUnassessedGrantApplications(int daysSinceArrival = 60)
		{
			var defaultGrantProgramId = GetDefaultGrantProgramId();
			var sendBackDate = AppDateTime.UtcNow.AddDays(-60);

			var unassessedApplications = _dbContext.GrantApplications
				.Where(ga => ga.GrantOpening.GrantStream.GrantProgram.Id == defaultGrantProgramId)
				.Where(ga => ga.ApplicationStateInternal == ApplicationStateInternal.New)
				.Where(ga => ga.DateSubmitted < sendBackDate);

			return unassessedApplications;
		}

		public void ReturnUnassessed(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			CreateWorkflowStateMachine(grantApplication).ReturnApplicationUnassessed();
		}

		private ApplicationJobWorkflowStateMachine CreateWorkflowStateMachine(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			return new ApplicationJobWorkflowStateMachine(grantApplication, _dbContext, _notificationService, _grantOpeningService, _noteService, _userService, _httpContext, _logger);
		}
	}
}