using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJG.Application.Business.Models
{
    public class AmountOwingPDFModel : RequestPDFBaseModel
    {
        public string DocumentNumber { get; set; }

        public string ProgramName { get; set; }

        public AmountOwingPDFModel(PaymentRequest model, bool duplicate) : base(model, duplicate)
        {
            var application = model.GrantApplication;

            RecipientName = application.ApplicantFirstName;
            DocumentNumber = model.DocumentNumber;
            ProgramName = application.GrantOpening.GrantStream.GrantProgram.Name;
        }
    }
}
