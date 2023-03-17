using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface INationalOccupationalClassificationService : IService
	{
		string UseNocVersion { get; set; }

		void AddNationalOccupationalClassification(NationalOccupationalClassification newNationalOccupationalClassification);

		void UpdateNationalOccupationalClassification(NationalOccupationalClassification nationalOccupationalClassification);

		/// <summary>
		/// Returns a collection of all the NOC objects that are children of the specified parent, up to the specified level.
		/// If no level is specified it will default to the next level from the parent.
		/// </summary>
		/// <param name="parentId">The parent NOC Id, or by default the root.</param>
		/// <param name="level">The level you want to go to in the tree.  By default it will get the next level from the parent.</param>
		/// <returns></returns>
		IEnumerable<NationalOccupationalClassification> GetNationalOccupationalClassificationChildren(int parentId = 0, int level = 0);
		IEnumerable<NationalOccupationalClassification> GetNationalOccupationalClassificationLevel(int level);

		/// <summary>
		/// Returns a collection of all the NOC objects that represent the tree up to and including the NOC object represented by the specified Id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		IEnumerable<NationalOccupationalClassification> GetNationalOccupationalClassifications(int? id);

		/// <summary>
		/// Returns the NOC object with the specified ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		NationalOccupationalClassification GetNationalOccupationalClassification(int? id);
	}
}