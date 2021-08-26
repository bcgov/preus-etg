using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using NLog;
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
