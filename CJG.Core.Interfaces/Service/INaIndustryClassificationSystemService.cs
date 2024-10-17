using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface INaIndustryClassificationSystemService : IService
    {
        void AddNaIndustryClassificationSystem(NaIndustryClassificationSystem newNaIndustryClassificationSystem);

        void UpdateNaIndustryClassificationSystem(NaIndustryClassificationSystem naIndustryClassificationSystem);

        /// <summary>
        /// Returns a collection of all the NAICS objects that are children of the specified parent, up to the specified level.
        /// If no level is specified it will default to the next level from the parent.
        /// </summary>
        /// <param name="parentId">The parent NAICS Id, or by default the root.</param>
        /// <param name="level">The level you want to go to in the tree.  By default it will get the next level from the parent.</param>
        /// <returns></returns>
        IEnumerable<NaIndustryClassificationSystem> GetNaIndustryClassificationSystemChildren(int parentId = 0, int level = 0);
        IEnumerable<NaIndustryClassificationSystem> GetNaIndustryClassificationSystemLevel(int level);

        /// <summary>
        /// Returns a collection of all the NAICS objects that represent the tree up to and including the NAICS object represented by the specified Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<NaIndustryClassificationSystem> GetNaIndustryClassificationSystems(int? id);

        /// <summary>
        /// Returns the NAICS object with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        NaIndustryClassificationSystem GetNaIndustryClassificationSystem(int? id);

        int?[] GetNaIndustryClassificationSystemIds(int? id);

        NaIndustryClassificationSystem GetNaIndustryClassificationSystemParentByLevel(int naicsId, int maxLevel = 6);

		int GetRootNaicsID(int naicsVersion);

	}
}
