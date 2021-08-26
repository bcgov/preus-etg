using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface ILogService : IService
    {
        void Add(Log log);
        Log Get(int id);
        IEnumerable<Log> GetLogs(int page = 1, int numberOfItems = 25, string level = "*");
        IEnumerable<Log> Filter(string level = "*", DateTime? dateAdded = null, string message = null, string userName = null, int page = 1, int numberOfItems = 25);
        void Update(Log log);
        void Delete(Log log);
        void Delete(DateTime before);
    }
}
