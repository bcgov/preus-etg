using System;
using CJG.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass]
	public class DateTimeExtensionTests
	{
		[TestMethod, TestCategory("Date Time"), TestCategory("Extension")]
		public void DateFormatsMorningUtcCorrectly()
		{
			var sampleDate = new DateTime(2022, 4, 1, 7, 0, 0, DateTimeKind.Utc);  // What we'd get back from the db for Start Date
			Assert.AreEqual("2022-04-01", sampleDate.ToStringLocalTime());
		}

		[TestMethod, TestCategory("Date Time"), TestCategory("Extension")]
		public void DateFormatsMidnightUtcCorrectly()
		{
			var sampleDate = new DateTime(2022, 7, 1, 6, 59, 59, DateTimeKind.Utc);  // What we'd get back from the db for End Date
			Assert.AreEqual("2022-06-30", sampleDate.ToStringLocalTime());
		}
	}
}