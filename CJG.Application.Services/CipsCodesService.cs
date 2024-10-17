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
	/// Cips Codes Service class, provides a way to manage communities within the datasource.
	/// </summary>
	public class CipsCodesService : Service, ICipsCodesService
	{
		#region Constructors
		/// <summary>
		/// Creates a new instance of a CommunityService object, and initializes it with the specified parameters.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public CipsCodesService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets all CIPS codes from the database
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ClassificationOfInstructionalProgram> GetAll()
		{
			return _dbContext.ClassificationOfInstructionalPrograms.Where(x => x.Level != 0).OrderBy(x => x.Id);
		}

		/// <summary>
		/// Get the  CIPS Code records for an ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ClassificationOfInstructionalProgram Get(int id)
		{
			return Get<ClassificationOfInstructionalProgram>(id);
		}

		public IEnumerable<ClassificationOfInstructionalProgram> GetListOfCipsCodes(int id)
		{

			ClassificationOfInstructionalProgram cipsCode = new ClassificationOfInstructionalProgram();
			List<ClassificationOfInstructionalProgram> lstCipsCode = new List<ClassificationOfInstructionalProgram>();
			var cipsCodeRecord = GetClassificationOfInstructionalProgram(id);

			int? level = cipsCodeRecord?.Level;
			int? parentId = id;

			for (int i = 1; i <= level; i++)
			{
				cipsCode = (from n1 in _dbContext.ClassificationOfInstructionalPrograms
							join n2 in _dbContext.ClassificationOfInstructionalPrograms
							on n1.ParentId equals n2.Id
							where n1.Id == parentId
							select n1).FirstOrDefault();

				parentId = cipsCode.ParentId;

				lstCipsCode.Add(cipsCode);
			}

			return lstCipsCode;
		}

		/// <summary>
		/// Returns the CIPS Code object with the specified ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ClassificationOfInstructionalProgram GetClassificationOfInstructionalProgram(int? id)
		{
			if (!id.HasValue)
				return null;

			var cipsCode = _dbContext.ClassificationOfInstructionalPrograms.Find(id);

			return cipsCode;
		}

		/// <summary>
		/// updates the selected CIPS Code
		/// </summary>
		/// <param name="cipsCode"></param>
		/// <returns></returns>
		public ClassificationOfInstructionalProgram Update(ClassificationOfInstructionalProgram cipsCode)
		{
			_dbContext.Update<ClassificationOfInstructionalProgram>(cipsCode);
			CommitTransaction();

			return cipsCode;
		}

		public ClassificationOfInstructionalProgram Add(ClassificationOfInstructionalProgram cipsCode)
		{
			_dbContext.ClassificationOfInstructionalPrograms.Add(cipsCode);
			CommitTransaction();

			return cipsCode;
		}

		public bool ParentIdExists(int? parentId)
		{
			var parent = _dbContext.ClassificationOfInstructionalPrograms.Where(c => c.ParentId == parentId).FirstOrDefault();

			if (parent == null)
				return false;
			else
				return true;
		}


		public IEnumerable<ClassificationOfInstructionalProgram> GetCipsCodeChildren(int parentId = 0, int level = 0)
		{
			var parent = GetClassificationOfInstructionalProgram(parentId);

			if (parentId == 0)
			{
				parentId = (from n in _dbContext.ClassificationOfInstructionalPrograms
							where n.Level == 0
							select n.Id).FirstOrDefault();
			}

			if (level <= 0)
				level = parent.Level + 1;

			var cipsCodes = (from n in _dbContext.ClassificationOfInstructionalPrograms
												   where n.Level == level && n.ParentId == parentId
												   select n).ToList();

			return cipsCodes;
		}

		#endregion


	}
}
