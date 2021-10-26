using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// SettingService class, provides a way to manage system settings that are stored in the DB.
	/// </summary>
	public class SettingService : Service, ISettingService
	{
		/// <summary>
		/// Creates a new instance of a SettingService object.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public SettingService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		/// <summary>
		/// Get the setting specified by the key.  
		/// </summary>
		/// <param name="key">Unique key to identify the setting.</param>
		/// <returns></returns>
		public Setting Get(string key)
		{
			return _dbContext.Settings.Find(key);
		}

		/// <summary>
		/// Get all the settings from the datasource without tracking.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Setting> GetAll()
		{
			return _dbContext.Settings.AsNoTracking().Where(s => true);
		}

		/// <summary>
		/// Add or update the setting in the datasource.
		/// </summary>
		/// <param name="setting"></param>
		public void AddOrUpdate(Setting setting)
		{
			var exists = _dbContext.Settings.FirstOrDefault(s => s.Key == setting.Key);
			if (exists != null)
			{
				exists.Value = setting.Value;
				exists.ValueType = setting.ValueType;
				Update(exists);
			}
			else
				Add(setting);
		}

		/// <summary>
		/// Add the setting to the datasource.
		/// </summary>
		/// <param name="setting"></param>
		public void Add(Setting setting)
		{
			_dbContext.Settings.Add(setting);
			_dbContext.Commit();
		}

		/// <summary>
		/// Update the setting in the datasource.
		/// </summary>
		/// <param name="setting"></param>
		public void Update(Setting setting)
		{
			_dbContext.Update(setting);
			_dbContext.Commit();
		}

		/// <summary>
		/// Delete the setting from the datasource.
		/// </summary>
		/// <param name="setting"></param>
		public void Delete(Setting setting)
		{
			if (setting == null)
				return;

			var exists = _dbContext.Settings.FirstOrDefault(s => s.Key == setting.Key);
			if (exists == null)
				return;

			Remove(exists);
			_dbContext.Commit();
		}
	}
}
