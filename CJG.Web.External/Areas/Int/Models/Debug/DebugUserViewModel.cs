using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models
{
    public class DebugUserViewModel
    {
        public InternalUser InternalUser { get; set; }
        public SiteMinderInfoViewModel SiteMinderInfo { get; set; }
    }
}