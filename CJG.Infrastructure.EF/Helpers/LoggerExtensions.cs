using NLog;
using System;

namespace CJG.Infrastructure.EF
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs the SQL statement with the specified calling type name to identify it.
        /// </summary>
        /// <param name="sql"></param>
        public static void LogSql(this Logger logger, string sql)
        {
            if (!String.IsNullOrEmpty(sql) && sql != "\r\n")
            {
                logger.Debug(sql);
            }
        }
    }
}
