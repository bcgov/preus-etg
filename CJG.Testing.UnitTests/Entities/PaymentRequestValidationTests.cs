using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class PaymentRequestValidationTests
    {
        ServiceHelper helper { get; set; }

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();

            helper = new ServiceHelper(typeof(PaymentRequestService), user);
            helper.MockContext();
            helper.MockDbSet<PaymentRequest>();
        }

        [TestMethod, TestCategory("Payment Request"), TestCategory("Validate")]
        public void Validate_When_Payment_Request_Is_Required_Properties_Error()
        {
            var grantApplication = new GrantApplication()
            {
                Assessor = EntityHelper.CreateInternalUser(),
                InvitationKey = Guid.NewGuid(),
                ApplicationStateInternal = ApplicationStateInternal.AgreementAccepted
            };

            var claim = new Claim()
            {
                Id = 1,
                GrantApplication = grantApplication
            };

            PaymentRequest paymentRequest = new PaymentRequest()
            {
                Claim = claim,
                PaymentAmount = 1
            };

            helper.MockDbSet<GrantApplication>(new[] { grantApplication });
            helper.MockDbSet<Claim>(new[] { claim });

            var service = helper.Create<PaymentRequestService>();

            // Act 
            var validationResults = service.Validate(paymentRequest).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("A document number is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The general ledger client number is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The general ledger service line account number is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The general ledger STOB account number is required.")));
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The general ledger RESP account number is required.")));            
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The general ledger project code is required.")));
        }

        [TestMethod, TestCategory("Payment Request"), TestCategory("Validate")]
        public void Validate_When_Payment_Request_Payment_Amount_Is_Not_Invalid_And_Not_Equal_To_Claim_AmountPaidOrOwing()
        {
            var grantApplication = new GrantApplication()
            {
                Assessor = EntityHelper.CreateInternalUser(),
                InvitationKey = Guid.NewGuid(),
                ApplicationStateInternal = ApplicationStateInternal.AgreementAccepted
            };

            var claim = new Claim()
            {
                Id = 1,
                GrantApplication = grantApplication
            };

            PaymentRequest paymentRequest = new PaymentRequest()
            {
                Claim = claim,
                PaymentAmount = 1,
                DocumentNumber = "123",
                GLClientNumber = "CL123",
                GLServiceLine = "SL123",
                GLSTOB = "ST123",
                GLProjectCode = "PC123",
                GLRESP = "RE123"
            };

            helper.MockDbSet<GrantApplication>(new[] { grantApplication });
            helper.MockDbSet<Claim>(new[] { claim });

            var service = helper.Create<PaymentRequestService>();

            // Act 
            var validationResults = service.Validate(paymentRequest).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The payment amount is invalid and should be")));
        }

    }
}