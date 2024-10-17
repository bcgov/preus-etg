using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq.Expressions;

namespace CJG.Web.External.Areas.Ext.Models.Agreements
{
	public class GrantAgreementDocumentViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }

		public string Body { get; set; }

		public bool Confirmation { get; set; }
		#endregion

		#region Constructors
		public GrantAgreementDocumentViewModel() { }
		public GrantAgreementDocumentViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
		}

		public GrantAgreementDocumentViewModel(GrantApplication grantApplication, string documentName) : this(grantApplication)
		{
			switch (documentName)
			{
				case ("CoverLetter"):
					this.Body = grantApplication.GrantAgreement?.CoverLetter?.Body;
					this.Confirmation = grantApplication.GrantAgreement.CoverLetterConfirmed;
					break;
				case ("ScheduleA"):
					this.Body = grantApplication.GrantAgreement?.ScheduleA?.Body;
					this.Confirmation = grantApplication.GrantAgreement.ScheduleAConfirmed;

					// either replace the placeholders with the view or an empty string
					this.Body = this.Body.Replace("::RequestChangeTrainingProvider::", "");
					this.Body = this.Body.Replace("::RequestChangeTrainingDates::", "");
					break;
				case ("ScheduleB"):
					this.Body = grantApplication.GrantAgreement?.ScheduleB?.Body;
					this.Confirmation = grantApplication.GrantAgreement.ScheduleBConfirmed;
					break;
			}
		}

		public GrantAgreementDocumentViewModel(GrantApplication grantApplication, Expression<Func<GrantAgreement, Document>> expression) : this(grantApplication)
		{
			var document = expression.Compile().Invoke(grantApplication.GrantAgreement);
			this.Body = document.Body;
		}
		#endregion
	}
}