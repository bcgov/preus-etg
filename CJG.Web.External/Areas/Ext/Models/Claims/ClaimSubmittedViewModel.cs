using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
    public class ClaimSubmittedViewModel : ClaimReviewViewModel
    {
        public ClaimSubmittedViewModel()
        {

        }
        public ClaimSubmittedViewModel(Claim claim): base(claim)
        {

        }
    }
}