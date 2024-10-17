using CJG.Application.Business.Models;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class UserProfileViewModel : BaseViewModel
    {
        public int UserId { get; set; }
        public string RowVersion { get; set; }
        public UserProfileConfirmationModel UserProfileConfirmation { get; set; }
        public UserProfileDetailModel UserProfileDetails { get; set; }
    }
}