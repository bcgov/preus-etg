using CJG.Core.Entities;
using System;
using System.Globalization;

namespace CJG.Application.Business.Models
{
	public abstract class RequestPDFBaseModel
	{
		#region Properties
		public string RecipientName { get; set; }

		public string InvoiceAmount { get; set; }

		public string ContractNumber { get; set; }

		public string ClientNumb { get; set; }
		public string RESP { get; set; }
		public string ServiceLine { get; set; }
		public string STOB { get; set; }
		public string ProjectCode { get; set; }

		public bool IsAccrual { get; set; }

		public bool Duplicate { get; set; }
		#endregion

		#region Constructors

		public RequestPDFBaseModel(PaymentRequest paymentRequest, bool duplicate)
		{
			if (paymentRequest == null) throw new ArgumentNullException(nameof(paymentRequest));

			this.InvoiceAmount = Math.Abs(paymentRequest.PaymentAmount).ToString("C2", new CultureInfo("en-US", false));
			this.ContractNumber = paymentRequest.GrantApplication.FileNumber;
			this.Duplicate = duplicate;
			this.IsAccrual = paymentRequest.PaymentType == PaymentTypes.Accrual;
			this.ClientNumb = paymentRequest.GLClientNumber;
			this.RESP = paymentRequest.GLRESP;
			this.ServiceLine = paymentRequest.GLServiceLine;
			this.STOB = paymentRequest.GLSTOB;
			this.ProjectCode = paymentRequest.GLProjectCode;
		}
		#endregion
	}
}
