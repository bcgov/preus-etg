namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="AttachmentExtensions" static class, provides extension methods for attachments.
	/// </summary>
	public static class AttachmentExtensions
	{
		public static Attachment Clone(this Attachment attachment)
		{
			var clone = new Attachment
			{
				FileName = attachment.FileName,
				Description = attachment.Description,
				FileExtension = attachment.FileExtension,
				AttachmentData = attachment.AttachmentData
			};

			return clone;
		}
	}
}