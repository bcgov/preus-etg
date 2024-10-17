using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using NLog;
using System;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="ProgramDescriptionService"/> class, provides a way to manage grant applications.
	/// </summary>
	public class ProgramDescriptionService : Service, IProgramDescriptionService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ProgramDescriptionService"/> object.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ProgramDescriptionService(
			IDataContext context,
			HttpContextBase httpContext,
			ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public void Add(ProgramDescription programDescription)
		{
			if (!_httpContext.User.CanPerformAction(programDescription.GrantApplication, ApplicationWorkflowTrigger.EditApplication))
				throw new NotAuthorizedException("User does not have permission to edit application '{id}'.");
			_dbContext.ProgramDescriptions.Add(programDescription);
			_dbContext.Commit();
		}

		public void ClearApplicationNaics(Guid userBCeID, int orgdId)
		{
			var grantApplication = (from p in _dbContext.ProgramDescriptions
									   join g in _dbContext.GrantApplications
									   on p.GrantApplicationId equals g.Id
									   join n in _dbContext.NaIndustryClassificationSystems
									   on p.TargetNAICSId equals n.Id
									   where n.NAICSVersion == 2012 && g.ApplicantBCeID == userBCeID && g.OrganizationId == orgdId &&
									   p.DescriptionState != ProgramDescriptionStates.Incomplete &&
									  (g.ApplicationStateExternal == ApplicationStateExternal.Complete || g.ApplicationStateExternal == ApplicationStateExternal.Incomplete)
									   select g).ToArray();



			for (var i = 0; i < grantApplication.Count(); i++)
			{
				if (grantApplication[i].ProgramDescription != null)
				{
					var programDescription = grantApplication[i].ProgramDescription;

					programDescription.ClearNaics();

					_dbContext.Update<ProgramDescription>(programDescription);
				}
			}


			_dbContext.Commit();
		}

		public void Update(ProgramDescription programDescription)
		{
			if (!_httpContext.User.CanPerformAction(programDescription.GrantApplication, ApplicationWorkflowTrigger.EditApplication))
				throw new NotAuthorizedException("User does not have permission to edit application '{id}'.");
			_dbContext.Update<ProgramDescription>(programDescription);
			_dbContext.Commit();
		}
		#endregion
	}
}
