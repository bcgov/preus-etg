using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="AttachmentService"/> class, provides a way to manage attachments in the datasource.
	/// </summary>
	public class AttachmentService : Service, IAttachmentService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="AttachmentService"/> object, and initializes it.
		/// </summary>
		/// <param name="httpContext"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public AttachmentService(
			IDataContext dbContext,
			HttpContextBase httpContext,
			ILogger logger) : base(dbContext, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the attachment for the specified 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Attachment Get(int id)
		{
			return Get<Attachment>(id);
		}

		/// <summary>
		/// Get the attachment for the specified 'attachmentId' and verify that the current user has access to the grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		public Attachment GetAttachment(int grantApplicationId, int attachmentId)
		{
			var grantApplication = Get<GrantApplication>(grantApplicationId);

			if (!_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ViewApplication))
			{
				throw new NotAuthorizedException($"User does not have permission to view application '{grantApplicationId}'.");
			}

			return Get<Attachment>(attachmentId);
		}

		/// <summary>
		/// Updates the current attachment and creates a versioned copy of the previous attachment.  Updates the datasource.
		/// </summary>
		/// <param name="attachment"></param>
		/// <param name="commit"></param>
		/// <returns></returns>
		public Attachment Update(Attachment attachment, bool commit = false)
		{
			if (attachment == null) throw new ArgumentNullException(nameof(attachment));

			var existingAttachment = Get<Attachment>(attachment.Id);
			existingAttachment.RowVersion = attachment.RowVersion ?? existingAttachment.RowVersion;
			existingAttachment.CreateNewVersion(attachment.FileName, attachment.Description, attachment.FileExtension, attachment.AttachmentData, attachment.AttachmentType);
			_dbContext.Update(existingAttachment);

			if (commit)
				_dbContext.CommitTransaction();
			return existingAttachment;
		}

		/// <summary>
		/// Adds the attachment to the datasource.
		/// </summary>
		/// <param name="attachment"></param>
		/// <param name="commit"></param>
		/// <returns></returns>
		public Attachment Add(Attachment attachment, bool commit = false)
		{
			if (attachment == null)
				throw new ArgumentNullException(nameof(attachment));

			_dbContext.Attachments.Add(attachment);

			if (commit)
				_dbContext.CommitTransaction();
			return attachment;
		}

		/// <summary>
		/// Removes the specified attachment and all versions from the datasource.
		/// </summary>
		/// <param name="attachment"></param>
		public void Delete(Attachment attachment)
		{
			if (attachment == null)
				throw new ArgumentNullException(nameof(attachment));

			foreach (var version in attachment.Versions)
			{
				_dbContext.VersionedAttachments.Remove(version);
			}
			_dbContext.Attachments.Remove(attachment);
			_dbContext.CommitTransaction();
		}
		#endregion
	}
}
