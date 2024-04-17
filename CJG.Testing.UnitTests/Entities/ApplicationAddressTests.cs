using CJG.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass]
	public class ApplicationAddressTests
	{
		private ApplicationAddress _address;

		[TestInitialize]
		public void Setup()
		{
			_address = new ApplicationAddress
			{
				CountryId = Constants.CanadaCountryId,
			};
		}

		[TestMethod, TestCategory("ApplicationAddress"), TestCategory("Validate")]
		public void Is_Canadian_PostalCode_Invalid()
		{
			_address.PostalCode = "12345";
			Assert.AreEqual(false, ApplicationAddress.IsPostalCodeValid(_address), "Invalid Canadian Postal Code");
		}

		[TestMethod, TestCategory("ApplicationAddress"), TestCategory("Validate")]
		public void Is_Canadian_PostalCode_WithNoSpace_Valid()
		{
			_address.PostalCode = "V8A4Y4";
			Assert.AreEqual(true, ApplicationAddress.IsPostalCodeValid(_address), "Valid postal code with no space");
		}

		[TestMethod, TestCategory("ApplicationAddress"), TestCategory("Validate")]
		public void Is_Canadian_PostalCode_WithSpace_Valid()
		{
			_address.PostalCode = "V8A 4Y4";
			Assert.AreEqual(true, ApplicationAddress.IsPostalCodeValid(_address), "Valid postal code with space");
		}

		[TestMethod, TestCategory("ApplicationAddress"), TestCategory("Validate")]
		public void Is_NonCanadian_PostalCode_Validated()
		{
			_address.CountryId = "US";

			_address.PostalCode = "V8A4Y4";
			Assert.AreEqual(true, ApplicationAddress.IsPostalCodeValid(_address), "Canadian Postal Code is ok");

			_address.PostalCode = "90210";
			Assert.AreEqual(true, ApplicationAddress.IsPostalCodeValid(_address), "US Postal Code is ok");

			_address.PostalCode = "ABCDEF";
			Assert.AreEqual(true, ApplicationAddress.IsPostalCodeValid(_address), "Alpha Postal Code is ok");

			_address.PostalCode = "SW1W 0NY";
			Assert.AreEqual(true, ApplicationAddress.IsPostalCodeValid(_address), "UK Postal Code is ok");
		}

		[TestMethod, TestCategory("ApplicationAddress"), TestCategory("Validate")]
		public void HasAddressChanged()
		{
			var oldAddress = new ApplicationAddress
			{
				AddressLine1 = "1 Smith St",
				AddressLine2 = "Apt 2",
				City = "TestVille",
				RegionId = "BC",
				PostalCode = "V91 5G2",
				CountryId = "CA",
			};

			var newAddress = GetNewApplicationAddress();
			Assert.IsFalse(ApplicationAddress.IsAddressDifferent(oldAddress, newAddress), "Address should be the same");

			newAddress = GetNewApplicationAddress();
			newAddress.AddressLine1 = "2 Smith St";
			Assert.IsTrue(ApplicationAddress.IsAddressDifferent(oldAddress, newAddress), "Address should be different");

			newAddress = GetNewApplicationAddress();
			newAddress.AddressLine2 = "Suite 8";
			Assert.IsTrue(ApplicationAddress.IsAddressDifferent(oldAddress, newAddress), "Address should be different");

			newAddress = GetNewApplicationAddress();
			newAddress.City = "Vancouver";
			Assert.IsTrue(ApplicationAddress.IsAddressDifferent(oldAddress, newAddress), "Address should be different");

			newAddress = GetNewApplicationAddress();
			newAddress.PostalCode = "V72 4G5";
			Assert.IsTrue(ApplicationAddress.IsAddressDifferent(oldAddress, newAddress), "Address should be different");

			newAddress = GetNewApplicationAddress();
			newAddress.RegionId = "BA";
			Assert.IsTrue(ApplicationAddress.IsAddressDifferent(oldAddress, newAddress), "Address should be different");

			newAddress = GetNewApplicationAddress();
			newAddress.CountryId = "US";
			Assert.IsTrue(ApplicationAddress.IsAddressDifferent(oldAddress, newAddress), "Address should be different");
		}

		private static ApplicationAddress GetNewApplicationAddress()
		{
			var newAddress = new ApplicationAddress
			{
				AddressLine1 = "1 Smith St",
				AddressLine2 = "Apt 2",
				City = "TestVille",
				RegionId = "BC",
				PostalCode = "V91 5G2",
				CountryId = "CA",
			};

			return newAddress;
		}
	}
}