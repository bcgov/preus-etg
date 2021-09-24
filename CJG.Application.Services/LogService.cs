using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class LogService : Service, ILogService
	{
		#region Constructors
		public LogService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public Log Get(int id)
		{
			if (id <= 0)
				return null;

			return _dbContext.Logs.Find(id);
		}

		public IEnumerable<Log> GetLogs(int page = 1, int numberOfItems = 25, string level = "*")
		{
			if (page < 1) page = 1;
			if (numberOfItems < 1) numberOfItems = 25;

			return _dbContext.Logs.AsNoTracking().AsNoTracking().Where(l => (level == "*" || l.Level == level)).OrderByDescending(l => l.DateAdded).Skip((page - 1) * numberOfItems).Take(numberOfItems).ToArray();
		}

		public IEnumerable<Log> Filter(string level = "*", DateTime? dateAdded = null, string message = null, string userName = null, int page = 1, int numberOfItems = 25)
		{
			if (page < 1) page = 1;
			if (numberOfItems < 1) numberOfItems = 25;

			return _dbContext.Logs.AsNoTracking().Where(l => 
				(level == "*" || l.Level == level)
				&& (dateAdded == null || l.DateAdded >= dateAdded)
				&& (message == null || l.Message.Contains(message))
				&& (userName == null || l.UserName.Contains(userName))
			).OrderByDescending(l => l.DateAdded).Skip((page - 1) * numberOfItems).Take(numberOfItems).ToArray();
		}

		public void Add(Log log)
		{
			_dbContext.Logs.Add(log);
			Commit();
		}

		public void Update(Log log)
		{
			_dbContext.Update<Log>(log);
			Commit();
		}

		public void Delete(Log log)
		{
			_dbContext.Logs.Remove(log);
			_dbContext.Commit();
		}

		public void Delete(DateTime before)
		{
			var logs = _dbContext.Logs.AsNoTracking().Where(l => l.DateAdded < before);
			foreach (var log in logs)
			{
				_dbContext.Logs.Remove(log);
			}
			_dbContext.Commit();
		}
		#endregion
	}
}
