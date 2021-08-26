using CJG.Application.Business.Models;
using CJG.Web.External.Models.Shared;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class UserProfileViewModel : BaseViewModel
    {
        public int UserId { get; set; }
        public string RowVewsion { get; set; }
        public UserProfileConfirmationModel UserProfileConfirmation { get; set; }
        public UserProfileDetailModel UserProfileDetails { get; set; }
        public List<UserGrantProgramPreferenceModel> PreferencePrograms { get; set; } = new List<UserGrantProgramPreferenceModel>();

    }
}