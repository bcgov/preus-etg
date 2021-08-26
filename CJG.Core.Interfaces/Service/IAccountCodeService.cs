using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface IAccountCodeService : IService
    {
        IEnumerable<AccountCode> GetAll();
        AccountCode Get(int id);
        void Add(AccountCode accountCode);
        void Update(AccountCode accountCode);
        void Delete(AccountCode accountCode);
    }
}
