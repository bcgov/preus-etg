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
	public class NationalOccupationalClassificationService : Service, INationalOccupationalClassificationService
	{
		#region Constructors
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
		}
		#endregion

		#region Methods
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
				_dbContext.Update<NationalOccupationalClassification>(nationalOccupationalClassification);
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
				var parent = GetNationalOccupationalClassification(parentId);

				if (level <= 0)
					level = parent.Level + 1;

				var nationalOccupationalClassifications = _dbContext.NationalOccupationalClassifications.Where(noc => noc.Level == level && noc.Left > parent.Left && noc.Right < parent.Right).OrderBy(noc => noc.Code);

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
				var nationalOccupationalClassifications = _dbContext.NationalOccupationalClassifications.Where(noc => noc.Level == level).OrderBy(noc => noc.Code);

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

				return (from p in _dbContext.NationalOccupationalClassifications
						from c in _dbContext.NationalOccupationalClassifications
						where p.Id == id
							&& p.Left >= c.Left
							&& p.Right <= c.Right
							&& c.Level > 0
						select c);
			} catch (Exception e)
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

		public Tuple<int?, int?, int?, int?> GetNationalOccupationalClassificationIds(int? id)
		{
			try
			{
				if (!id.HasValue)
					return new Tuple<int?, int?, int?, int?>(null, null, null, null);

				var nocs = GetNationalOccupationalClassifications(id);

				return new Tuple<int?, int?, int?, int?> (nocs.FirstOrDefault(n => n.Level == 1)?.Id, nocs.FirstOrDefault(n => n.Level == 2)?.Id, nocs.FirstOrDefault(n => n.Level == 3)?.Id, nocs.FirstOrDefault(n => n.Level == 4)?.Id);
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't get NOC Ids for Parent ID: {0}", id);
				throw;
			}
		}
		#endregion
	}
}
