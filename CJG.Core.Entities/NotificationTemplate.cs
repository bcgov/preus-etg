using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// NotificationTemplate class, provides a way to manage all the notification templates.
    /// </summary>
    public class NotificationTemplate : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key uses IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// get/set - The alert caption to use for this notification.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required."), MaxLength(250)]
        public string Caption { get; set; }

        /// <summary>
        /// get/set - The email subject line.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Subject is required."), MaxLength(500)]
        public string EmailSubject { get; set; }

        /// <summary>
        /// get/set - The email subject body.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Template is required.")]
        public string EmailBody { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a NotificationTemplate object.
        /// </summary>
        public NotificationTemplate()
        {

        }

        /// <summary>
        /// Creates a new instance of a NotificationTemplate object and initializes it.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="defaultExpiryDays"></param>
        public NotificationTemplate(string caption, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(caption))
                throw new ArgumentNullException(nameof(caption));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentNullException(nameof(body));

            this.Caption = caption;
            this.EmailSubject = subject;
            this.EmailBody = body;
        }
        #endregion
    }
}
