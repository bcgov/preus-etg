using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="Log"/> class, provides the ORM a way to manage log messages.
    /// </summary>
    public class Log : EntityBase
    {
        #region Properties
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// get/set - The logging level of the message [Trace|Debug|Info|Warn|Error|Fatal].
        /// </summary>
        [Required, MaxLength(20)]
        public string Level { get; set; }

        /// <summary>
        /// get/set - The name of the application posting the log entry.
        /// </summary>
        [MaxLength(150)]
        public string Application { get; set; }

        /// <summary>
        /// get/set - The message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// get/set - The exception.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// get/set - The user name responsible for the message.
        /// </summary>
        [MaxLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// get/set - The server name.
        /// </summary>
        [MaxLength(100)]
        public string ServerName { get; set; }

        /// <summary>
        /// get/set - The URL of the request.
        /// </summary>
        [MaxLength(500)]
        public string Url { get; set; }

        /// <summary>
        /// get/set - The remote address which originated the request.
        /// </summary>
        [MaxLength(500)]
        public string RemoteAddress { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="Log"/> object.
        /// </summary>
        public Log()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="Log"/> object.
        /// </summary>
        /// <param name="level">The Logging level.</param>
        /// <param name="message">The message.</param>
        public Log(string level, string message) : this(level, System.AppDomain.CurrentDomain.FriendlyName, message)
        {
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="Log"/> object.
        /// </summary>
        /// <param name="level">The Logging level.</param>
        /// <param name="applicationName">The name of the application posting the log entry.</param>
        /// <param name="message">The message.</param>
        public Log(string level, string applicationName, string message)
        {
            this.Level = level;
            this.Application = applicationName;
            this.Message = message;
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="Log"/> object.
        /// </summary>
        /// <param name="level">The Logging level.</param>
        /// <param name="exception">The exception.</param>
        public Log(string level, Exception exception) : this(level, System.AppDomain.CurrentDomain.FriendlyName, exception)
        {
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="Log"/> object.
        /// </summary>
        /// <param name="level">The Logging level.</param>
        /// <param name="applicationName">The name of the application posting the log entry.</param>
        /// <param name="exception">The exception.</param>
        public Log(string level, string applicationName, Exception exception)
        {
            this.Level = level;
            this.Application = applicationName;
            this.Message = exception.Message;
            this.Exception = exception.StackTrace;
        }
        #endregion

        #region Methods
        #endregion
    }
}
