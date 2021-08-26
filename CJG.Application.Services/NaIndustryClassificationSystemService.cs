using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class NaIndustryClassificationSystemService : Service, INaIndustryClassificationSystemService
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public NaIndustryClassificationSystemService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public void AddNaIndustryClassificationSystem(NaIndustryClassificationSystem newNaIndustryClassificationSystem)
		{
			_dbContext.NaIndustryClassificationSystems.Add(newNaIndustryClassificationSystem);
			_dbContext.Commit();
		}

		public void UpdateNaIndustryClassificationSystem(NaIndustryClassificationSystem naIndustryClassificationSystem)
		{
			_dbContext.Update<NaIndustryClassificationSystem>(naIndustryClassificationSystem);
			_dbContext.Commit();
		}

		/// <summary>
		/// Returns parent NACS node at specified level if there is no parent found then return given node
		/// </summary>
		/// <param name="naicsId">Child naics node Id</param>
		/// <param name="maxLevel">Max parent level that will be retrived</param>
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

			if (level <= 0)
				level = parent.Level + 1;

			var naIndustryClassificationSystems = _dbContext.NaIndustryClassificationSystems.Where(naics => naics.Level == level && naics.Left > parent.Left && naics.Right < parent.Right).OrderBy(naics => naics.Code);

			return naIndustryClassificationSystems;
		}

		public IEnumerable<NaIndustryClassificationSystem> GetNaIndustryClassificationSystemLevel(int level)
		{

			var naIndustryClassificationSystems = _dbContext.NaIndustryClassificationSystems.Where(naics => naics.Level == level).OrderBy(naics => naics.Code);

			return naIndustryClassificationSystems;
		}

		/// <summary>
		/// Returns a collection of all the NAICS objects that represent the tree up to and including the NAICS object represented by the specified Id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IEnumerable<NaIndustryClassificationSystem> GetNaIndustryClassificationSystems(int? id)
		{
			if (!id.HasValue)
				return Enumerable.Empty<NaIndustryClassificationSystem>();

			return (from p in _dbContext.NaIndustryClassificationSystems
					from c in _dbContext.NaIndustryClassificationSystems
					where p.Id == id
						&& p.Left >= c.Left
						&& p.Right <= c.Right
						&& c.Level > 0
					select c);
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

			return new int?[] { naics.FirstOrDefault(n => n.Level == 1)?.Id, naics.FirstOrDefault(n => n.Level == 2)?.Id, naics.FirstOrDefault(n => n.Level == 3)?.Id };
		}
		#endregion
	}
}
