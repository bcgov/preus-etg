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
	/// Community Service class, provides a way to manage communities within the datasource.
	/// </summary>
	public class CommunityService : Service, ICommunityService
	{
		#region Constructors
		/// <summary>
		/// Creates a new instance of a CommunityService object, and initializes it with the specified parameters.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public CommunityService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the community specified by the 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Community Get(int id)
		{
			return Get<Community>(id);
		}

		/// <summary>
		/// Get the community specified by the 'caption'.
		/// </summary>
		/// <param name="caption"></param>
		/// <returns></returns>
		public Community Get(string caption)
		{
			return _dbContext.Communities.FirstOrDefault(x => x.Caption == caption);
		}

		/// <summary>
		/// Get all communities.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Community> GetAll()
		{
			return _dbContext.Communities.OrderBy(x => x.RowSequence).ThenBy(x => x.Caption);
		}

		/// <summary>
		/// Get all of the communities specified by the 'ids'.
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public IEnumerable<Community> GetCommunities(int[] ids)
		{
			return _dbContext.Communities.Where(e => e.IsActive && ids.Contains(e.Id));
		}

		/// <summary>
		/// Add the specified community to the datasource.
		/// </summary>
		/// <param name="community"></param>
		/// <returns></returns>
		public Community Add(Community community)
		{
			_dbContext.Communities.Add(community);
			CommitTransaction();

			return community;
		}

		/// <summary>
		/// Update the specified community in the datasource.
		/// </summary>
		/// <param name="community"></param>
		/// <returns></returns>
		public Community Update(Community community)
		{
			_dbContext.Update<Community>(community);
			CommitTransaction();

			return community;
		}

		/// <summary>
		/// Delete the specified community from the datasource.
		/// </summary>
		/// <param name="community"></param>
		public void Delete(Community community)
		{
			if (community == null)
				throw new ArgumentNullException(nameof(community));

			var entity = Get(community.Id);
			_dbContext.Communities.Remove(entity);

			CommitTransaction();
		}
		#endregion
	}
}
