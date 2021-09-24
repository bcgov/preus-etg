using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Ext.Models.Attachments
{
	public class GrantApplicationBusinessCaseViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public string BusinessCaseUserGuidance { get; set; }
		public string BusinessCaseHeader { get; set; }
		public string BusinessCaseTemplateURL { get; set; }
		public BusinessCaseModel BusinessCaseDocument { get; set; } = new BusinessCaseModel();
		#endregion

		#region Constructors
		public GrantApplicationBusinessCaseViewModel()
		{
		}

		public GrantApplicationBusinessCaseViewModel(GrantApplication grantApplication) : base()
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.GrantApplicationId = grantApplication.Id;
			this.BusinessCaseUserGuidance = grantApplication.GrantOpening.GrantStream.BusinessCaseUserGuidance;
			this.BusinessCaseHeader = grantApplication.GrantOpening.GrantStream.BusinessCaseExternalHeader;
			this.BusinessCaseTemplateURL = grantApplication.GrantOpening.GrantStream.BusinessCaseTemplateURL;
			if (grantApplication.BusinessCaseDocument != null)
			{
				this.BusinessCaseDocument.Id = grantApplication.BusinessCaseDocument.Id;
				this.BusinessCaseDocument.FileName = grantApplication.BusinessCaseDocument?.FileName;
				this.BusinessCaseDocument.Description = grantApplication.BusinessCaseDocument?.Description;
				this.BusinessCaseDocument.RowVersion = grantApplication.BusinessCaseDocument.RowVersion != null ? Convert.ToBase64String(grantApplication.BusinessCaseDocument.RowVersion) : null;
			}
		}
		#endregion
	}
}
