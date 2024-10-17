using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;

namespace CJG.Application.Services
{
	public class NationalOccupationalClassificationService : Service, INationalOccupationalClassificationService
	{
		public string UseNocVersion { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public NationalOccupationalClassificationService(
			IDataContext dbContext,
			HttpContextBase httpContext,
			ILogger logger) : base(dbContext, httpContext, logger)
		{
			var configNocSetting = ConfigurationManager.AppSettings["NocVersion"];

			if (configNocSetting == null)
				UseNocVersion = "2016";

			var validVersions = new List<string> { "2016", "2021" };

			if (configNocSetting != null && validVersions.Contains(configNocSetting))
				UseNocVersion = configNocSetting;
		}

		public void AddNationalOccupationalClassification(NationalOccupationalClassification newNationalOccupationalClassification)
		{
			try
			{
				_dbContext.NationalOccupationalClassifications.Add(newNationalOccupationalClassification);
				_dbContext.Commit();
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't add NOC: {0}", newNationalOccupationalClassification.Code);
				throw;
			}
		}

		public void UpdateNationalOccupationalClassification(NationalOccupationalClassification nationalOccupationalClassification)
		{
			try
			{
				_dbContext.Update(nationalOccupationalClassification);
				_dbContext.Commit();
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't update NOC ID: {0}", nationalOccupationalClassification.Id);
				throw;
			}
		}

		/// <summary>
		/// Returns a collection of all the NOC objects that are children of the specified parent, up to the specified level.
		/// If no level is specified it will default to the next level from the parent.
		/// </summary>
		/// <param name="parentId">The parent NOC Id, or by default the root.</param>
		/// <param name="level">The level you want to go to in the tree.  By default it will get the next level from the parent.</param>
		/// <returns></returns>
		public IEnumerable<NationalOccupationalClassification> GetNationalOccupationalClassificationChildren(int parentId = 0, int level = 0)
		{
			try
			{
				// Find the parent for the NOC Version we are defaulting to
				if (parentId == 0)
				{
					var rootNoc = _dbContext.NationalOccupationalClassifications
						.Where(n => n.Level == 0)
						.FirstOrDefault(n => n.NOCVersion == UseNocVersion);

					if (rootNoc != null)
						parentId = rootNoc.Id;
				}

				var parent = GetNationalOccupationalClassification(parentId);

				if (level <= 0)
					level = parent.Level + 1;

				var nationalOccupationalClassifications = _dbContext.NationalOccupationalClassifications
					.Where(n => n.Level == level)
					.Where(n => n.ParentId == parentId)
					.Where(n => n.NOCVersion == UseNocVersion)
					.OrderBy(noc => noc.Code);

				return nationalOccupationalClassifications;
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't get NOC Children for parent ID: {0}", parentId);
				throw;
			}
		}

		public IEnumerable<NationalOccupationalClassification> GetNationalOccupationalClassificationLevel(int level)
		{
			try
			{
				var nationalOccupationalClassifications = _dbContext.NationalOccupationalClassifications
					.Where(noc => noc.Level == level)
					.Where(noc => noc.NOCVersion == UseNocVersion)
					.OrderBy(noc => noc.Code);

				return nationalOccupationalClassifications;
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't get NOCs for level: {0}", level);
				throw;
			}
		}

		/// <summary>
		/// Returns a collection of all the NOC objects that represent the tree up to and including the NOC object represented by the specified Id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IEnumerable<NationalOccupationalClassification> GetNationalOccupationalClassifications(int? id)
		{
			try
			{
				if (!id.HasValue)
					return Enumerable.Empty<NationalOccupationalClassification>();

				var targetNoc = _dbContext.NationalOccupationalClassifications.Find(id);
				if (targetNoc == null)
					return Enumerable.Empty<NationalOccupationalClassification>();

				var nocChain = new List<NationalOccupationalClassification> { targetNoc };
				var parentId = targetNoc.ParentId;

				while (parentId > 0)
				{
					var parentNoc = _dbContext.NationalOccupationalClassifications.Find(parentId);
					if (parentNoc == null)
					{
						parentId = 0;
						continue;
					}

					nocChain.Add(parentNoc);
					parentId = parentNoc.ParentId;
				}

				return nocChain.OrderBy(n => n.Level);
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't get NOCs for ID: {0}", id);
				throw;
			}
		}

		/// <summary>
		/// Returns the NOC object with the specified ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public NationalOccupationalClassification GetNationalOccupationalClassification(int? id)
		{
			try
			{
				if (!id.HasValue)
					return null;

				var naIndustryClassificationSystem = _dbContext.NationalOccupationalClassifications.Find(id);

				return naIndustryClassificationSystem;
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't get NOC ID: {0}", id);
				throw;
			}
		}
	}
}
