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
		[DataRow("Bread, Colin & Bread, Milary", "BREAD, COLIN & BREAD, MILARY", true)]
		[DataRow("Colin Bread & Milary Bread", "BREAD, COLIN & BREAD, MILARY", false)]
		[DataRow("Org Name", "oRG NaMe", true)]
		[DataRow(" Space Man ", "Space Man", true)]
		[DataRow("Other Space Man", " Other Space Man ", true)]
		[DataRow("New Line", "New Line\n", true)]
		[DataRow("Other New Line\n", "Other New Line", true)]
		[DataRow("CAPITAL SOURCE", "Capital Source", true)]
		[DataRow("Capital Target", "CAPITAL TARGET", true)]
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