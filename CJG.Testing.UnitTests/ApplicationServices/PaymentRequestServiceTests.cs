using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class PaymentRequestServiceTests : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		/// <summary>
		/// GeneratePaymentRequestBatch should return a <typeparamref name="PaymentRequestBatch"/>.
		/// </summary>
		[TestMethod, TestCategory("Payment Request")]
		public void GeneratePaymentRequestBatch_ShouldReturnPaymentRequestBatch()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(PaymentRequestService), identity);
			var service = helper.Create<PaymentRequestService>();

			var grantProgram = new GrantProgram {
				Id = 1,
				RequestedBy = "Bob",
				ProgramPhone = "666-666-6661",
				DocumentPrefix = "1",
				ExpenseAuthority = new InternalUser { FirstName = "Alex", LastName = "Kent" },
				BatchRequestDescription = "Batch_01"
			};

			var fiscalYear = new FiscalYear { StartDate = AppDateTime.Now };

			helper.GetMock<IGrantProgramService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantProgram);
			helper.GetMock<IFiscalYearService>().Setup(m => m.GetFiscalYear(It.IsAny<DateTime>())).Returns(fiscalYear);
			helper.MockDbSet<PaymentRequestBatch>();
			helper.MockDbSet<Claim>();

			var grantProgramId = 1;
			var claims = new List<Claim> {
				new Claim {
					TotalAssessedReimbursement = 10,
					ClaimNumber = "123",
					GrantApplication = new GrantApplication {
						Organization = new Organization {
							BusinessNumber = "123"
						},
						GrantOpening = new GrantOpening {
							GrantStream = new GrantStream {
								AccountCode = new AccountCode {
									GLClientNumber = "Test1",
									GLRESP = "Test2",
									GLServiceLine = "Test3",
									GLSTOBNormal = "Test4",
									GLProjectCode = "Test5"
								},
								GrantProgram = grantProgram
							}
						}
					}
				}
			};

			var batchType = PaymentBatchTypes.PaymentRequest;

			// Act
			var result = service.GeneratePaymentRequestBatch(grantProgramId, claims, batchType);

			// Assert
			var paymentRequest = result.PaymentRequests.First();
			var accountCode = claims.FirstOrDefault().GrantApplication.GrantOpening.GrantStream.AccountCode;
			Assert.AreEqual(paymentRequest.GLClientNumber, accountCode.GLClientNumber);
			Assert.AreEqual(paymentRequest.GLRESP, accountCode.GLRESP);
			Assert.AreEqual(paymentRequest.GLServiceLine, accountCode.GLServiceLine);
			Assert.AreEqual(paymentRequest.GLProjectCode, accountCode.GLProjectCode);
		}

		/// <summary>
		/// GeneratePaymentRequestBatch without an ExpenseAuthority should throw a <typeparamref name="NullReferenceException"/>.
		/// </summary>
		[TestMethod, TestCategory("Payment Request")]
		public void GeneratePaymentRequestBatch_WithoutExpenseAuthorityShouldThrowNullReferenceException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var expenseAuthority = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(PaymentRequestService), identity);
			var service = helper.Create<PaymentRequestService>();
			var template = new DocumentTemplate(DocumentTypes.ApplicantDeclaration, "test", "test");
			var grantProgram = new GrantProgram();
			var fiscalYear = new FiscalYear { StartDate = AppDateTime.Now };

			helper.GetMock<IGrantProgramService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantProgram);
			helper.GetMock<IFiscalYearService>().Setup(m => m.GetFiscalYear(It.IsAny<DateTime>())).Returns(fiscalYear);
			helper.MockDbSet<Claim>();
			helper.MockDbSet<PaymentRequestBatch>();

			var grantProgramId = 1;
			var claims = new List<Claim> { new Claim() };
			var batchType = PaymentBatchTypes.PaymentRequest;

			// Act
			Action action = () => service.GeneratePaymentRequestBatch(grantProgramId, claims, batchType);

			// Assert
			action.Should().Throw<InvalidOperationException>().WithMessage("An Expense Authority must be specified.");
		}
	}
}
