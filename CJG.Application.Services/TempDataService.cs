using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Web;

namespace CJG.Application.Services
{
	public class TempDataService : Service, ITempDataService
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public TempDataService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public TempData Get(Type parentType, int parentId, Type dataType)
		{
			if (parentId == 0)
				return null;

			try
			{
				return _dbContext.TempData.Find(parentType.Name, parentId, dataType.FullName);
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't find temporary data for parent ID: {0}", parentId);
				throw;
			}
		}

		public void Add(TempData data)
		{
			if (data.ParentId <= 0)
				throw new ArgumentException($"The argument '{nameof(data)}'.'{nameof(data.ParentId)}' cannot be less than or equal to 0.");

			try
			{
				_dbContext.TempData.Add(data);
				Commit();
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't add temporary data parent ID: {0}", data.ParentId);
				throw;
			}
		}

		public void AddOrUpdate(TempData data)
		{
			if (data.ParentId <= 0)
				throw new ArgumentException($"The argument '{nameof(data)}'.'{nameof(data.ParentId)}' cannot be less than or equal to 0.");

			var temp = _dbContext.TempData.Find(data.ParentType, data.ParentId, data.DataType);
			if (temp == null)
			{
				Add(data);
			}
			else
			{
				temp.Data = data.Data;
				Update(temp);
			}
		}

		public void Update(TempData data)
		{
			try
			{
				_dbContext.Update<TempData>(data);
				_dbContext.Commit();
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't update temporary data ID: {0}", data.ParentId);
				throw;
			}
		}

		public void Delete(TempData data)
		{
			try
			{
				Remove(data);
				_dbContext.Commit();
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't delete temporary data ID: {0}", data.ParentId);
				throw;
			}
		}

		public void Delete(Type parentType, int parentId, Type dataType)
		{
			try
			{
				var data = Get(parentType, parentId, dataType);
				if (data != null)
				{
					_dbContext.TempData.Remove(data);
					_dbContext.Commit();
				}
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't delete temporary data ID: {0}", parentId);
				throw;
			}
		}
		#endregion
	}
}
