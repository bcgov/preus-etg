using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models
{
    /// <summary>
    /// <typeparamref name="DebugViewModel"/> class, View Model for debugging information.
    /// </summary>
    public class DebugViewModel
    {
        #region Properties
        public User User { get; set; }
        public SiteMinderInfoViewModel SiteMinderInfo { get; set; }
        #endregion

        #region Constructors
        public DebugViewModel()
        {

        }

        public DebugViewModel(User user, SiteMinderInfoViewModel info)
        {
            this.User = user;
            this.SiteMinderInfo = info;
        }
        #endregion
    }
}