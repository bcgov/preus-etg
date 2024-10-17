using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class NaIndustryClassificationSystemService : Service, INaIndustryClassificationSystemService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public NaIndustryClassificationSystemService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		public void AddNaIndustryClassificationSystem(NaIndustryClassificationSystem newNaIndustryClassificationSystem)
		{
			_dbContext.NaIndustryClassificationSystems.Add(newNaIndustryClassificationSystem);
			_dbContext.Commit();
		}

		public void UpdateNaIndustryClassificationSystem(NaIndustryClassificationSystem naIndustryClassificationSystem)
		{
			_dbContext.Update(naIndustryClassificationSystem);
			_dbContext.Commit();
		}

		/// <summary>
		/// Returns parent NAICS node at specified level if there is no parent found then return given node
		/// </summary>
		/// <param name="naicsId">Child naics node Id</param>
		/// <param name="maxLevel">Max parent level that will be retrieved</param>
		/// <returns></returns>
		public NaIndustryClassificationSystem GetNaIndustryClassificationSystemParentByLevel(int naicsId, int maxLevel = 6)
		{
			return GetNaIndustryClassificationSystems(naicsId)
				.Where(x => x.Level <= maxLevel)
				.OrderByDescending(x => x.Level)
				.FirstOrDefault();
		}

		/// <summary>
		/// Returns a collection of all the NAICS objects that are children of the specified parent, up to the specified level.
		/// If no level is specified it will default to the next level from the parent.
		/// </summary>
		/// <param name="parentId">The parent NAICS Id, or by default the root.</param>
		/// <param name="level">The level you want to go to in the tree.  By default it will get the next level from the parent.</param>
		/// <returns></returns>
		public IEnumerable<NaIndustryClassificationSystem> GetNaIndustryClassificationSystemChildren(int parentId = 0, int level = 0)
		{
			var parent = GetNaIndustryClassificationSystem(parentId);

			var cutOffDate = GetCutoffDate();
			var currentDate = AppDateTime.Now;
			var naicsVersion = 2012; // old NAICS codes

			if (parentId == 0 && currentDate >= cutOffDate)
			{
				naicsVersion = 2017; // new NAICS codes

				parentId = _dbContext.NaIndustryClassificationSystems
					.Where(n => n.NAICSVersion == naicsVersion && n.Level == 0)
					.Select(n => n.Id)
					.FirstOrDefault();
			}

			if (level <= 0)
				level = parent.Level + 1;

			var naIndustryClassificationSystems = _dbContext.NaIndustryClassificationSystems
				.Where(n => n.Level == level && n.ParentId == parentId)
				.ToList();

			return naIndustryClassificationSystems;
		}

		public IEnumerable<NaIndustryClassificationSystem> GetNaIndustryClassificationSystemLevel(int level)
		{
			var cutOffDate = GetCutoffDate();
			var currentDate = AppDateTime.Now;
			int naicsVersion = 2012;  // old NAICS codes

			if (currentDate >= cutOffDate)
			{
				naicsVersion = 2017; // new NAICS codes
			}

			var naIndustryClassificationSystems = _dbContext.NaIndustryClassificationSystems
				.Where(naics => naics.Level == level && naics.NAICSVersion == naicsVersion)
				.OrderBy(naics => naics.Code);

			return naIndustryClassificationSystems;
		}

		/// <summary>
		/// Returns a collection of all the NAICS objects that represent the tree up to and including the NAICS object represented by the specified Id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IEnumerable<NaIndustryClassificationSystem> GetNaIndustryClassificationSystems(int? id)
		{
			var naics = new NaIndustryClassificationSystem();
			var naicsList = new List<NaIndustryClassificationSystem>();
			var naicsRecord = GetNaIndustryClassificationSystem(id);

			int? level = naicsRecord?.Level;
			int? parentId = id;

			for (var i = 1; i <= level; i++)
			{
				naics = (from n1 in _dbContext.NaIndustryClassificationSystems
						 join n2 in _dbContext.NaIndustryClassificationSystems
						 on n1.ParentId equals n2.Id
						 where n1.Id == parentId
						 select n1).FirstOrDefault();

				parentId = naics.ParentId;

				naicsList.Add(naics);
			}

			return naicsList;
		}

		/// <summary>
		/// Returns the NAICS object with the specified ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public NaIndustryClassificationSystem GetNaIndustryClassificationSystem(int? id)
		{
			if (!id.HasValue)
				return null;

			var naIndustryClassificationSystem = _dbContext.NaIndustryClassificationSystems.Find(id);

			return naIndustryClassificationSystem;
		}

		public int?[] GetNaIndustryClassificationSystemIds(int? id)
		{
			if (!id.HasValue)
				return new int?[] { null, null, null };

			var naics = GetNaIndustryClassificationSystems(id);

			return new[]
			{
				naics.FirstOrDefault(n => n.Level == 1)?.Id,
				naics.FirstOrDefault(n => n.Level == 2)?.Id,
				naics.FirstOrDefault(n => n.Level == 3)?.Id
			};
		}

		public int GetRootNaicsID(int naicsVersion)
		{
			var rootParentId = _dbContext.NaIndustryClassificationSystems
				.Where(n => n.NAICSVersion == naicsVersion && n.Level == 0)
				.Select(n => n.Id)
				.FirstOrDefault();

			return rootParentId;
		}

		private static DateTime GetCutoffDate()
		{
			var validCutoff = DateTime.TryParseExact(ConfigurationManager.AppSettings["CutOffDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var cutoffDate);
			return validCutoff ? cutoffDate : AppDateTime.Now.Date.AddYears(-1);  // If we fail to get configuration cutoff, roll back the date so we use the new NAICS coding.
		}
	}
}
