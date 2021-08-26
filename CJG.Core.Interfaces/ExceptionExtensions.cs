using System;
using System.Text;


namespace CJG.Core.Interfaces
{
    /// <summary>
    /// <typeparamref name="ExceptionExtensions"/> static class, provides extension methods for <typeparamref name="Exception"/> objects.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Returns all the exception messages, even the inner exceptions as a single string.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="deliminator"></param>
        /// <returns></returns>
        public static string GetAllMessages(this Exception ex, string deliminator = " ")
        {
            var msg = new StringBuilder();
            msg.Append(ex.Message);

            if (ex.InnerException != null)
                msg.Append(deliminator + ex.InnerException.GetAllMessages());

            return msg.ToString();
        }

        /// <summary>
        /// Get the last inner exception.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception GetLastInnerException(this Exception ex)
        {
            if (ex.InnerException != null)
                return ex.InnerException.GetLastInnerException();

            return ex;
        }

        /// <summary>
        /// Get the inner exception of the specified type.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Exception GetExceptionOfType(this Exception ex, Type type)
        {
            if (ex.GetType() == type)
                return ex;

            if (ex.InnerException == null)
                return null;

            return ex.InnerException.GetExceptionOfType(type);
        }
    }
}
