using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface ICommunityService : IService
    {
        IEnumerable<Community> GetCommunities(int[] ids);
        IEnumerable<Community> GetAll();
        Community Get(int id);
        Community Get(string caption);
		Community Add(Community community);
        Community Update(Community community);
        void Delete(Community community);
    }
}
