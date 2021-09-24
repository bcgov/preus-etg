using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;
using System;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class PaymentValidationTests
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
    }
}