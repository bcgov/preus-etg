using System;
using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="ProgramNotificationService"/> class, provides a way to manage program notifications.
	/// </summary>
	public class ProgramNotificationService : Service, IProgramNotificationService
	{
		#region Variables

		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ProgramNotificationService"/> object.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ProgramNotificationService(
			IDataContext context,
			HttpContextBase httpContext,
			ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the program notification for the specified 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ProgramNotification Get(int id)
		{
			return Get<ProgramNotification>(id);
		}

		/// <summary>
		/// Add the specified Program Notification to the datasource.
		/// </summary>
		/// <param name="programNotification"></param>
		/// <returns></returns>
		public ProgramNotification Add(ProgramNotification programNotification)
		{
			_dbContext.ProgramNotifications.Add(programNotification);
			CommitTransaction();

			return programNotification;
		}

		/// <summary>
		/// Update the specified Program Notification in the datasource.
		/// </summary>
		/// <param name="programNotification"></param>
		/// <returns></returns>
		public ProgramNotification Update(ProgramNotification programNotification)
		{
			_dbContext.Update(programNotification);
			CommitTransaction();

			return programNotification;
		}

		/// <summary>
		/// Delete Program Notification from datasource.
		/// </summary>
		/// <param name="programNotification"></param>
		public void Delete(ProgramNotification programNotification)
		{
			var recipients = programNotification.ProgramNotificationRecipients.ToArray();
			foreach (var recipient in recipients)
				_dbContext.ProgramNotificationRecipients.Remove(recipient);
			_dbContext.NotificationTemplates.Remove(programNotification.NotificationTemplate);
			_dbContext.ProgramNotifications.Remove(programNotification);
			CommitTransaction();
		}

		/// <summary>
		/// Delete Program Notification Recipient from the context.
		/// </summary>
		/// <param name="programNotificationRecipient"></param>
		public void DeleteRecipient(ProgramNotificationRecipient programNotificationRecipient)
		{
			_dbContext.ProgramNotificationRecipients.Remove(programNotificationRecipient);
		}

		/// <summary>
		/// Get program notifications.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		public PageList<ProgramNotification> GetProgramNotifications(int page, int quantity, string search)
		{
			var filtered = _dbContext.ProgramNotifications
				.Where(x => string.IsNullOrEmpty(search) || x.Caption.Contains(search))
				.OrderBy(x => x.Caption);
			var total = filtered.Count();
			var result = filtered.Skip((page - 1) * quantity).Take(quantity);
			return new PageList<ProgramNotification>(page, quantity, total, result.ToArray());
		}

		/// <summary>
		/// Check if the specified Program Notification caption exists in the datasource.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="caption"></param>
		/// <returns></returns>
		public bool Exists(int id, string caption)
		{
			return _dbContext.ProgramNotifications.Any(o => o.Id != id && o.Caption.Equals(caption));
		}

		/// <summary>
		/// Returns the number of registered external users.
		/// </summary>
		/// <returns></returns>
		public int GetNumberOfApplicants()
		{
			return _dbContext.Users.Count();
		}

		//TODO: Possibly remove this since we only have one program now?
		/// <summary>
		/// Returns the number of applicants who have shown interest, and the number of applicants who have submitted an application in each grant program.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<GrantProgramApplicantModel> GetNumberOfApplicantsPerGrantProgram()
		{
			var defaultGrantProgramId = GetDefaultGrantProgramId();

			return _dbContext.GrantPrograms.Where(o => o.State == GrantProgramStates.Implemented)
				.Where(o => o.Id == defaultGrantProgramId)
				.ToArray()
				.Select(o => new GrantProgramApplicantModel(o, _dbContext.BusinessContactRoles
					.Where(a => a.GrantApplication.GrantOpening.GrantStream.GrantProgramId == o.Id &&
					            a.GrantApplication.DateSubmitted != null)
					.Select(b => b.UserId).Distinct().Count()));
		}

		/// <summary>
		/// Adds program notifications to the notification queue for all the specified applicants.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public void Send(int id)
		{
			var programNotification = Get(id);
			var applicants = GetProgramNotificationApplicants(programNotification);
			var sender = System.Configuration.ConfigurationManager.AppSettings["EmailFromAddress"];

			_dbContext.NotificationQueue.AddRange(applicants.Select(o => new NotificationQueue(programNotification, o, sender)));

			CommitTransaction();
		}

		/// <summary>
		/// Returns the applicants of program notification recipients.
		/// </summary>
		/// <param name="programNotification"></param>
		/// <returns></returns>
		[Obsolete("We need to figure out if we have to remove this or not. Split doesn't require a count of different programs.")]
		private IEnumerable<User> GetProgramNotificationApplicants(ProgramNotification programNotification)
		{
			if (programNotification.AllApplicants)
				return _dbContext.Users.ToArray();
			return (from recipient in _dbContext.ProgramNotificationRecipients
					where recipient.ProgramNotificationId == programNotification.Id && recipient.SubscriberOnly
					join preference in _dbContext.UserGrantProgramPreferences
					on recipient.GrantProgramId equals preference.GrantProgramId
					where preference.IsSelected
					join user in _dbContext.Users
					on preference.UserId equals user.Id
					select user)
			 .Union(from recipient in _dbContext.ProgramNotificationRecipients
					where recipient.ProgramNotificationId == programNotification.Id && recipient.ApplicantOnly
					join contact in _dbContext.BusinessContactRoles
					on recipient.GrantProgramId equals contact.GrantApplication.GrantOpening.GrantStream.GrantProgramId
					where contact.GrantApplication.DateSubmitted != null
					join user in _dbContext.Users
					on contact.UserId equals user.Id
					select user)
			.Distinct().ToArray();
		}
		#endregion
	}
}
