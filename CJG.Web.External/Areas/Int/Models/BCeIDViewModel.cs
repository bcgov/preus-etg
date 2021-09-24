using CJG.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Int.Models
{
    public class BCeIDViewModel
    {
        public User User { get; set; }

        [Display(Name = "User Id")]
        public string UserId { get; set; }

        [Display(Name = "BCeID GUID")]
        public Guid? BCeID { get; set; }

        [Display(Name = "Requester GUID")]
        public Guid? Requester { get; set; }

        [Display(Name = "Requester Account Type")]
        public BCeIDAccountTypeCodes? RequesterAccountType { get; set; }
    }
}