using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Application.Business.Models
{
	public abstract class RequestBatchPDFBaseModel
	{
		#region Properties
		public string Name => $"Batch Number: {Model.BatchNumber}";
		public string DateRequested => Model.IssuedDate.ToLocalMorning().ToString("MMMM dd, yyyy");

		public PaymentRequestBatch Model { get; set; }
		public List<RequestPDFBaseModel> PaymentRequests { get; set; } = new List<RequestPDFBaseModel>();
		#endregion

		#region Constructors
		public RequestBatchPDFBaseModel(PaymentRequestBatch batch)
		{
			if (batch == null) throw new ArgumentNullException(nameof(batch));

			this.Model = new PaymentRequestBatch()
			{
				IssuedBy = batch.IssuedBy,
				IssuedDate = batch.IssuedDate,
				BatchNumber = batch.BatchNumber,
				BatchType = batch.BatchType
			};
		}
		#endregion
	}
}
