namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="AttachmentExtensions" static class, provides extension methods for attachments.
    /// </summary>
    public static class AttachmentExtensions
    {
        /// <summary>
        /// Creates a copy of the specified attachment.
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public static Attachment CreateCopy(this Attachment attachment)
        {
            return new Attachment(attachment);
        }
    }
}
