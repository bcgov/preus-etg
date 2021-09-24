using System;
using System.Collections.Generic;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using System.Linq;
using NLog;
using System.Web;
using CJG.Infrastructure.Entities;
using CJG.Core.Interfaces;

namespace CJG.Application.Services
{
	/// <summary>
	/// ServiceCategoryService class, provides a way to manage service categories.
	/// </summary>
	public class ServiceCategoryService : Service, IServiceCategoryService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ServiceCategoryService object and initializes it.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ServiceCategoryService(
			IDataContext context,
			HttpContextBase httpContext,
			ILogger logger
			) : base(context, httpContext, logger)
		{

		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the service category for the specified 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ServiceCategory Get(int id)
		{
			return Get<ServiceCategory>(id);
		}

		/// <summary>
		/// Add the specified service category to the datasource.
		/// </summary>
		/// <param name="serviceCategory"></param>
		/// <returns></returns>
		public ServiceCategory Add(ServiceCategory serviceCategory)
		{
			_dbContext.ServiceCategories.Add(serviceCategory);
			_dbContext.CommitTransaction();

			return serviceCategory;
		}

		/// <summary>
		/// Delete the specified service category from the datasource.
		/// </summary>
		/// <param name="serviceCategory"></param>
		public void Delete(ServiceCategory serviceCategory)
		{
			this.Remove(serviceCategory);
			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Remove the service category but do not commit to the datasource.
		/// </summary>
		/// <param name="serviceCategory"></param>
		public void Remove(ServiceCategory serviceCategory)
		{
			var entity = _dbContext.ServiceCategories.Find(serviceCategory.Id);

			if (entity.EligibleExpenseTypes.Any())
			{
				if (_dbContext.EligibleCosts.Any(ec => ec.EligibleExpenseType.ServiceCategoryId == serviceCategory.Id))
					throw new InvalidOperationException($"Cannot delete the '{serviceCategory.Caption}' service category because it is currently associated to a grant application.");

				foreach (var expenseType in entity.EligibleExpenseTypes.ToArray())
				{
					foreach (var config in _dbContext.ProgramConfigurations.ToArray())
					{
						config.EligibleExpenseTypes.Remove(expenseType);
					}
					_dbContext.EligibleExpenseTypes.Remove(expenseType);
				}
			}

			foreach (var serviceLine in serviceCategory.ServiceLines.ToArray())
			{
				if (_dbContext.TrainingPrograms.Any(tp => tp.ServiceLineId == serviceLine.Id))
					throw new InvalidOperationException($"Cannot delete the '{serviceLine.Caption}' service line because it is currently associated to a grant application.");

				foreach (var breakdown in serviceLine.ServiceLineBreakdowns.ToArray())
				{
					if (_dbContext.TrainingPrograms.Any(tp => tp.ServiceLineBreakdownId == breakdown.Id))
						throw new InvalidOperationException($"Cannot delete the '{breakdown.Caption}' service line breakdown because it is currently associated to a grant application.");

					_dbContext.ServiceLineBreakdowns.Remove(breakdown);
				}
				_dbContext.ServiceLines.Remove(serviceLine);
			}

			_dbContext.ServiceCategories.Remove(serviceCategory);
		}

		/// <summary>
		/// Return service categories from the datasource and filters them based on the isActive property.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ServiceCategory> Get(bool? isActive = null)
		{
			return _dbContext.ServiceCategories.Where(sc => isActive == null || sc.IsActive == isActive).OrderBy(sc => sc.RowSequence).ThenBy(sc => sc.Caption).ToArray();
		}

		/// <summary>
		/// Update the specified service category from the datasource.
		/// </summary>
		/// <param name="serviceCategory"></param>
		/// <returns></returns>
		public ServiceCategory Update(ServiceCategory serviceCategory)
		{
			_dbContext.Update(serviceCategory);
			_dbContext.CommitTransaction();

			return serviceCategory;
		}

		/// <summary>
		/// Update all of the service categories.
		/// </summary>
		/// <param name="serviceCategories"></param>
		/// <returns></returns>
		public IEnumerable<ServiceCategory> BulkAddUpdate(IEnumerable<ServiceCategory> serviceCategories)
		{
			foreach (var serviceCategory in serviceCategories)
			{
				if (serviceCategory.Id == 0)
				{
					_dbContext.ServiceCategories.Add(serviceCategory);
				}
				else
				{
					// Update related EligibleExpenseTypes
					foreach (var eet in serviceCategory.EligibleExpenseTypes)
					{
						eet.MinProviders = serviceCategory.MinProviders;
						eet.MaxProviders = serviceCategory.MaxProviders;
						eet.RowSequence = serviceCategory.RowSequence;

						// Only update eligible expenses that are not associated with submitted grant applications.
						if (!eet.EligibleCosts.Any(ec => ec.TrainingCost.GrantApplication.ApplicationStateInternal >= ApplicationStateInternal.New && ec.TrainingCost.GrantApplication.ApplicationStateInternal != ApplicationStateInternal.ApplicationWithdrawn))
						{
							var originalServiceCategoryIsActive = (bool)_dbContext.Entry(serviceCategory).OriginalValues[nameof(serviceCategory.IsActive)];
							eet.Caption = serviceCategory.Caption;
							eet.Description = serviceCategory.Description;
							eet.IsActive = serviceCategory.IsActive != originalServiceCategoryIsActive ? serviceCategory.IsActive : eet.IsActive;

							// Any existing grant application that isn't submitted yet will need to become incomplete.
							eet.EligibleCosts.ForEach(ec =>
							{
								ec.TrainingCost.TrainingCostState = TrainingCostStates.Incomplete;
								ec.TrainingCost.GrantApplication.TrainingProviders.Where(tp => tp.EligibleCostId == ec.Id).ForEach(tp => tp.TrainingProviderState = TrainingProviderStates.Incomplete);
							});

							foreach (var serviceLine in serviceCategory.ServiceLines)
							{
								// Update EligibleExpenseBreakdowns
								foreach (var eeb in serviceLine.EligibleExpenseBreakdowns)
								{
									var originalServiceLineIsActive = (bool)_dbContext.Entry(serviceLine).OriginalValues[nameof(serviceLine.IsActive)];
									eeb.RowSequence = serviceLine.RowSequence;
									eeb.Caption = serviceLine.Caption;
									eeb.Description = serviceLine.Description;
									eeb.IsActive = serviceLine.IsActive != originalServiceLineIsActive ? serviceLine.IsActive : eeb.IsActive;
									eeb.EnableCost = serviceLine.EnableCost;

									_dbContext.Update(eeb);

									// Any existing grant application that isn't submitted yet will need to become incomplete.
									eeb.EligibleCostBreakdowns.ForEach(ecb => ecb.TrainingPrograms.ForEach(tp => tp.TrainingProgramState = TrainingProgramStates.Incomplete));
								}

								foreach (var breakdown in serviceLine.ServiceLineBreakdowns)
								{
									if (breakdown.Id == 0)
									{
										_dbContext.ServiceLineBreakdowns.Add(breakdown);
									}
									else
									{
										_dbContext.Update(breakdown);
									}
								}

								if (serviceLine.Id == 0)
								{
									_dbContext.ServiceLines.Add(serviceLine);
								}
								else
								{
									_dbContext.Update(serviceLine);
								}
							}
						}
						else
						{
							eet.IsActive = serviceCategory.IsActive;
							foreach (var serviceLine in serviceCategory.ServiceLines)
							{
								// Update EligibleExpenseBreakdowns
								foreach (var eeb in serviceLine.EligibleExpenseBreakdowns)
								{
									var originalServiceLineIsActive = (bool)_dbContext.Entry(serviceLine).OriginalValues[nameof(serviceLine.IsActive)];
									eeb.RowSequence = serviceLine.RowSequence;
									eeb.IsActive = serviceLine.IsActive != originalServiceLineIsActive ? serviceLine.IsActive : eeb.IsActive;
									eeb.EnableCost = serviceLine.EnableCost;
									_dbContext.Update(eeb);
								}

								foreach (var breakdown in serviceLine.ServiceLineBreakdowns)
								{
									if (breakdown.Id == 0)
									{
										_dbContext.ServiceLineBreakdowns.Add(breakdown);
									}
									else
									{
										_dbContext.Update(breakdown);
									}
								}


								if (serviceLine.Id == 0)
								{
									_dbContext.ServiceLines.Add(serviceLine);
								}
								else
								{
									_dbContext.Update(serviceLine);
								}
							}
						}
						_dbContext.Update(eet);
					}

					_dbContext.Update(serviceCategory);
				}
			}

			_dbContext.CommitTransaction();

			return Get();
		}
		#endregion
	}
}