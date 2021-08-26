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
	/// <summary>
	/// <typeparamref name="ProgramConfigurationService"/> class, provides a way to manage program configurations.
	/// </summary>
	public class ProgramConfigurationService : Service, IProgramConfigurationService
	{
		#region Variables

		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ProgramConfigurationService"/> object.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ProgramConfigurationService(
			IDataContext context,
			HttpContextBase httpContext,
			ILogger logger) : base(context, httpContext, logger)
		{

		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the program configuration for the specified 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ProgramConfiguration Get(int id)
		{
			return Get<ProgramConfiguration>(id);
		}

		/// <summary>
		/// Returns all the program configurations in the system.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ProgramConfiguration> GetAll()
		{
			return _dbContext.ProgramConfigurations.ToList();
		}

		/// <summary>
		/// Returns all the program configurations in the system filtered by IsActive.
		/// </summary>
		/// <param name="isActive"></param>
		/// <returns></returns>
		public IEnumerable<ProgramConfiguration> GetAll(bool isActive)
		{
			return _dbContext.ProgramConfigurations.AsNoTracking().Where(t => t.IsActive == isActive).ToList();
		}

		/// <summary>
		/// Delete Program Configuration from datasource.
		/// </summary>
		/// <param name="programConfiguration"></param>
		public void Delete(ProgramConfiguration programConfiguration)
		{
			Remove(programConfiguration);
			CommitTransaction();
		}

		/// <summary>
		/// Remove Program Configuration
		/// </summary>
		/// <param name="programConfiguration"></param>
		public void Remove(ProgramConfiguration programConfiguration)
		{
			var hasApplications = _dbContext.ProgramConfigurations.Any(pc => pc.Id == programConfiguration.Id && pc.EligibleExpenseTypes.Any(eet => eet.EligibleCosts.Any()));
			if (hasApplications)
				throw new InvalidOperationException("Cannot delete this program configuration because it is in use by applications.");

			foreach (var expenseType in programConfiguration.EligibleExpenseTypes.ToArray())
			{
				foreach (var breakdown in expenseType.Breakdowns.ToArray())
				{
					_dbContext.EligibleExpenseBreakdowns.Remove(breakdown);
				}
				_dbContext.EligibleExpenseTypes.Remove(expenseType);
			}
			_dbContext.ProgramConfigurations.Remove(programConfiguration);
		}

		/// <summary>
		/// Generate a new program configuration for the specified grant stream, based on the parent grant program's configuration.
		/// </summary>
		/// <param name="grantStream"></param>
		/// <returns></returns>
		public ProgramConfiguration Generate(GrantStream grantStream)
		{
			var grantProgramConfiguration = grantStream.GrantProgram.ProgramConfiguration;
			var programConfiguration = new ProgramConfiguration($"{grantProgramConfiguration.Caption} - {grantStream.Name}", grantProgramConfiguration.ClaimTypeId)
			{
				ESSMaxEstimatedParticipantCost = grantProgramConfiguration.ESSMaxEstimatedParticipantCost,
				SkillsTrainingMaxEstimatedParticipantCosts = grantProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts
			};

			foreach (var eet in grantProgramConfiguration.EligibleExpenseTypes)
			{
				var eligibleExpenseType = eet.ServiceCategory != null ? new EligibleExpenseType(eet.ServiceCategory, eet.ExpenseTypeId) : new EligibleExpenseType(eet.Caption, eet.Description, eet.ExpenseTypeId, eet.RowSequence)
				{
					IsActive = eet.IsActive
				};

				foreach (var eeb in eet.Breakdowns)
				{
					var breakdown = eeb.ServiceLine != null ? new EligibleExpenseBreakdown(eeb.ServiceLine, eligibleExpenseType) : new EligibleExpenseBreakdown(eeb.Caption, eeb.ExpenseType, eeb.IsActive, eeb.RowSequence);
					eligibleExpenseType.Breakdowns.Add(breakdown);
					_dbContext.EligibleExpenseBreakdowns.Add(breakdown);
				}

				programConfiguration.EligibleExpenseTypes.Add(eligibleExpenseType);
				_dbContext.EligibleExpenseTypes.Add(eligibleExpenseType);
			}

			_dbContext.ProgramConfigurations.Add(programConfiguration);
			grantStream.ProgramConfiguration = programConfiguration;

			return programConfiguration;
		}

		/// <summary>
		/// Synchronize the specified program configuration with the master WDA Service Descriptions.
		/// This will add new service categories and service lines.
		/// </summary>
		/// <param name="programConfiguration"></param>
		public void SyncWDAService(ProgramConfiguration programConfiguration)
		{
			if (programConfiguration == null) throw new ArgumentNullException(nameof(programConfiguration));

			var scIds = programConfiguration.EligibleExpenseTypes.Select(eet => eet.ServiceCategoryId).Distinct();

			// Update existing Service Categories
			foreach (var eligibleExpenseType in programConfiguration.EligibleExpenseTypes)
			{
				var serviceCategory = eligibleExpenseType.ServiceCategory;
				var slIds = eligibleExpenseType.Breakdowns.Select(b => b.ServiceLineId).ToArray();
				var serviceLines = serviceCategory.ServiceLines.Where(sl => !slIds.Contains(sl.Id));

				foreach (var serviceLine in serviceLines)
				{
					var eligibleExpenseBreakdown = new EligibleExpenseBreakdown(serviceLine, eligibleExpenseType);
					_dbContext.EligibleExpenseBreakdowns.Add(eligibleExpenseBreakdown);
				}
			}

			// Add new Service Categories.
			var serviceCategories = _dbContext.ServiceCategories.Where(sc => !scIds.Contains(sc.Id));
			ExpenseTypes expenseType = ExpenseTypes.ParticipantAssigned;
			foreach (var serviceCategory in serviceCategories)
			{
				switch (serviceCategory.ServiceTypeId)
				{
					case (ServiceTypes.SkillsTraining):
					case (ServiceTypes.EmploymentServicesAndSupports):
						expenseType = ExpenseTypes.ParticipantLimited;
						break;
					case (ServiceTypes.Administration):
						expenseType = ExpenseTypes.NotParticipantLimited;
						break;
				};
				var eligibleExpenseType = new EligibleExpenseType(serviceCategory, expenseType);

				foreach (var serviceLine in serviceCategory.ServiceLines)
				{
					var eligibleExpenseBreakdown = new EligibleExpenseBreakdown(serviceLine, eligibleExpenseType);
					_dbContext.EligibleExpenseBreakdowns.Add(eligibleExpenseBreakdown);
				}
				_dbContext.EligibleExpenseTypes.Add(eligibleExpenseType);
				programConfiguration.EligibleExpenseTypes.Add(eligibleExpenseType);
			}

			CommitTransaction();
		}
		#endregion
	}
}
