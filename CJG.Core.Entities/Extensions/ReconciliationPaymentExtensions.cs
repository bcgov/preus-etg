using System;
using System.Linq;

namespace CJG.Core.Entities.Extensions
{
	public static class ReconciliationPaymentExtensions
	{
		public static bool IsValidSupplierName(this ReconciliationPayment recPayment, GrantApplication grantApplication)
		{
			if (grantApplication == null)
				return false;

			var supplierName = recPayment.SupplierName?.ToLower();
			var orgName = grantApplication.OrganizationLegalName?.ToLower();

			if (string.IsNullOrWhiteSpace(supplierName) || string.IsNullOrWhiteSpace(orgName))
				return false;

			if (supplierName == orgName)
				return true;

			var splits = supplierName.Split(new [] {","}, StringSplitOptions.RemoveEmptyEntries);

			if (splits.Length != 2)
				return false;

			var flippedName = $"{splits.ElementAt(1).Trim()} {splits.ElementAt(0).Trim()}";
			if (flippedName == orgName)
				return true;

			return false;
		}
	}
}