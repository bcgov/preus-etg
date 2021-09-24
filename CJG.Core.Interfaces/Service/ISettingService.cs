using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface ISettingService : IService
    {
        Setting Get(string key);
        IEnumerable<Setting> GetAll();

        void Add(Setting setting);

        void AddOrUpdate(Setting setting);

        void Update(Setting setting);

        void Delete(Setting setting);
    }
}
