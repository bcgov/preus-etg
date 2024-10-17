using CJG.Core.Entities;
using System;

namespace CJG.Core.Interfaces.Service
{
    public interface ITempDataService : IService
    {
        TempData Get(Type parentType, int parentId, Type dataType);
        void Add(TempData data);
        void AddOrUpdate(TempData data);
        void Update(TempData data);
        void Delete(TempData data);
        void Delete(Type parentType, int parentId, Type dataType);
    }
}
