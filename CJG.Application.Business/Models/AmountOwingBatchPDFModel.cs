using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJG.Application.Business.Models
{
    public class AmountOwingBatchPDFModel : RequestBatchPDFBaseModel
    {
        public AmountOwingBatchPDFModel(PaymentRequestBatch model, bool duplicate) : base(model)
        {
            foreach (var paymentRequest in model.PendingRequests)
                PaymentRequests.Add(new AmountOwingPDFModel(paymentRequest, duplicate));
        }
    }
}
