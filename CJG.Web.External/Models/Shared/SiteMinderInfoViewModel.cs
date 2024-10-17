using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Models.Shared
{
    /// <summary>
    /// <typeparamref name="SiteMinderInfoViewModel"/> class, View Model for SiteMinder information.
    /// </summary>
    public class SiteMinderInfoViewModel
    {
        #region Properties
        public Guid CurrentUserGuid { get; set; }
        public BCeIDAccountTypeCodes CurrentUserType { get; set; }
        #endregion

        #region Constructors
        public SiteMinderInfoViewModel()
        {

        }

        public SiteMinderInfoViewModel(Guid currentUser, BCeIDAccountTypeCodes accountType)
        {
            this.CurrentUserGuid = currentUser;
            this.CurrentUserType = accountType;
        }
        #endregion
    }
}
