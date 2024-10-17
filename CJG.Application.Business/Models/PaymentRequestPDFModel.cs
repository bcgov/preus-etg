using CJG.Core.Entities;
using System;

namespace CJG.Application.Business.Models
{
	public class PaymentRequestPDFModel : RequestPDFBaseModel
	{
		#region Properties
		public string Recipient { get; set; }
		public string RecipientAddress { get; set; }

		public string InvoiceDate { get; set; }
		public string InvoiceNumber { get; set; }

		public string Receiver { get; set; }

		public string PaymentType { get; set; }

		public string BusinessNumber { get; set; }
		public bool BusinessNumberVerified { get; set; }
		#endregion

		#region Constructors
		public PaymentRequestPDFModel(PaymentRequest paymentRequest, bool duplicate) : base(paymentRequest, duplicate)
		{
			if (paymentRequest == null) throw new ArgumentNullException(nameof(paymentRequest));

			var application = paymentRequest.GrantApplication;
			var address = application.ApplicantMailingAddressId == null ? application.OrganizationAddress : application.ApplicantMailingAddress;

			this.Recipient = application.OrganizationLegalName;
			this.RecipientName = $"{application.ApplicantFirstName} {application.ApplicantLastName}";
			this.RecipientAddress = $"{address.AddressLine1 + (string.IsNullOrEmpty(address.AddressLine2) ? "" : " " + address.AddressLine2)}<br />{address.City}, {address.RegionId} {address.PostalCode}";

			this.InvoiceDate = paymentRequest.Claim.DateSubmitted?.ToString("yyyy-MM-dd");
			this.InvoiceNumber = paymentRequest.DocumentNumber;

			this.Receiver = $"{paymentRequest.Claim.Assessor.FirstName} {paymentRequest.Claim.Assessor.LastName}";

			this.BusinessNumber = application.Organization.BusinessNumber;
			this.BusinessNumberVerified = application.Organization.BusinessNumberVerified == true;

			this.PaymentType = paymentRequest.PaymentType.ToString();
		}
		#endregion
	}
}
