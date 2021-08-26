using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public sealed class TrainingProviderInventoryService : Service, ITrainingProviderInventoryService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingService"/> class.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public TrainingProviderInventoryService(
							   IDataContext context,
							   HttpContextBase httpContext,
							   ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods

		public TrainingProviderInventory Get(int id)
		{
			return Get<TrainingProviderInventory>(id);
		}

		public TrainingProviderInventory Add(TrainingProviderInventory trainingProviderInventory)
		{
			_dbContext.TrainingProviderInventory.Add(trainingProviderInventory);
			_dbContext.CommitTransaction();

			return trainingProviderInventory;
		}

		public TrainingProviderInventory Update(TrainingProviderInventory trainingProviderInventory)
		{
			_dbContext.Update<TrainingProviderInventory>(trainingProviderInventory);
			_dbContext.CommitTransaction();

			return trainingProviderInventory;
		}

		public void Delete(int id, ref string strMsgType, ref string strMsgText)
		{
			var trainingProviderInventory = Get(id);
			var trainingProviders = _dbContext.TrainingProviders.AsNoTracking().Where(x => x.TrainingProviderInventoryId == id);

			if (trainingProviders.Count() == 0)
			{
				_dbContext.TrainingProviderInventory.Remove(trainingProviderInventory);
				_dbContext.Commit();
				strMsgType = "";
				strMsgText = "Inventoried Training Provider (" + trainingProviderInventory.Name + ") was deleted successfully";
			}
			else
			{
				strMsgType = "W";
				strMsgText = "Inventoried Training Provider (" + trainingProviderInventory.Name + ") has been used and cannot be deleted";
			}
		}

		public TrainingProviderInventory GetTrainingProviderFromInventory(string name)
		{
			return _dbContext.TrainingProviderInventory.FirstOrDefault(t => t.Name == name);
		}

		public PageList<TrainingProviderInventory> GetInventory(int page, int quantity, string search, bool? isActive = null)
		{
			var filtered = _dbContext.TrainingProviderInventory
				.Where(x => (string.IsNullOrEmpty(search) || x.Name.Contains(search) || x.Acronym != null && x.Acronym.Contains(search))
					&& (isActive == null || x.IsActive == isActive)
				).OrderBy(x => x.Name);
			var total = filtered.Count();
			var result = filtered.Skip((page - 1) * quantity).Take(quantity);
			return new PageList<TrainingProviderInventory>(page, quantity, total, result.ToArray());
		}

		public IEnumerable<TrainingProviderInventory> GetActiveTrainingProvidersFromInventory()
		{
			return _dbContext.TrainingProviderInventory.AsNoTracking().Where(x => x.IsActive == true).OrderBy(x => x.Name);
		}

		public bool IsTrainingProviderInventoryUsedInApplications(int trainingProviderInventoryId)
		{
			return _dbContext.TrainingProviders.AsNoTracking().Where(x => x.TrainingProviderInventoryId == trainingProviderInventoryId).Any();
		}

		public IEnumerable<TrainingProviderInventory> GetAll(int page, int quantity)
		{
			return _dbContext.TrainingProviderInventory.OrderBy(x => x.Name).Skip(page).Take(quantity);
		}

		public IEnumerable<TrainingProviderInventory> Search(string name, int page, int quantity)
		{
			return _dbContext.TrainingProviderInventory.AsNoTracking().Where(x => x.Name.Contains(name)).OrderBy(x => x.Name).Skip(page).Take(quantity);
		}
		#endregion
	}
}
