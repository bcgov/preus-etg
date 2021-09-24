using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Int.Models
{
	public class CommunityViewModel : BaseViewModel
	{
		#region Properties
		[Required(ErrorMessage = "Caption is required."), MaxLength(250)]
		public string Caption { get; set; }
		public bool Active { get; set; } = true;
		#endregion

		#region Constructors
		public CommunityViewModel() { }

		public CommunityViewModel(Community community) {
			this.Id = community.Id;
			this.Caption = community.Caption;
			this.Active = community.IsActive;
		}
		#endregion
	}
}