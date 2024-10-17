using CJG.Web.External.Models.Shared;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models
{
    public class SyncBCeIDAccoutsViewModel : BaseViewModel
    {
        public List<int> Ids { get; set; }
        public int NumberOfAccounts { get; set; }
        public int NumberOfUpdatedAccounts { get; set; }
        public List<FailedBCeIDAccount> Failed { get; set; } = new List<FailedBCeIDAccount>();
        public bool IsCompleted { get; set; } = false;
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    public class FailedBCeIDAccount
    {
        public string BCeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Reason { get; set; }
    }
}