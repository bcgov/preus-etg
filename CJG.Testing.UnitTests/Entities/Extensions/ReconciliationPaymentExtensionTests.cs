using CJG.Core.Entities;
using CJG.Core.Entities.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.Entities.Extensions
{
	[TestClass]
	public class ReconciliationPaymentExtensionTests
	{
		[TestMethod]
		[DataRow("Fred Smith", "Fred Smith", true)]
		[DataRow("Fred Smith", "Smith, Fred", true)]
		[DataRow("Fred Smith", "Smith Fred", false)]
		[DataRow("", "Smith Fred", false)]
		[DataRow("Fred Smith", "", false)]
		[DataRow("Fred Smith", "Fred, Johnny, Frank and Co.", false)]
		[DataRow("Org Name", "oRG NaMe", true)]
		[DataRow("No Match", "Smith, Fred", false)]
		public void IsSupplierNameValid(string organizationName, string supplierName, bool expectMatch)
		{
			var reconciliationPayment = new ReconciliationPayment { SupplierName = supplierName };
			var grantApplication = new GrantApplication { OrganizationLegalName = organizationName };

			Assert.AreEqual(reconciliationPayment.IsValidSupplierName(grantApplication), expectMatch);
		}

		[TestMethod]
		public void IsValidSupplierNameNotExplosive()
		{
			var reconciliationPayment = new ReconciliationPayment { SupplierName = "Testing" };
			Assert.AreEqual(reconciliationPayment.IsValidSupplierName(null), false);
		}
	}
}