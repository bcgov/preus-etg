using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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