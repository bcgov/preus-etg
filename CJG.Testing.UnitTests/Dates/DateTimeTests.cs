using CJG.Core.Entities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CJG.Testing.UnitTests.Dates
{
	[TestClass]
	public class DateTimeTests
	{
		#region Variables
		#endregion

		#region Initialize
		[ClassInitialize]
		public static void Setup(TestContext context)
		{
		}

		[TestInitialize]
		public void Configure()
		{

		}
		#endregion

		#region Tests

		[TestMethod]
		[TestCategory("Extensions"), TestCategory("Dates")]
		public void DateTime_ToLocalMorning_Unspecified()
		{
			// Arrange
			var date = new DateTime(2017, 01, 01, 10, 10, 10);

			// Act
			var morning = date.ToLocalMorning();

			// Assert
			Assert.AreEqual(new DateTime(2017, 01, 01, 0, 0, 0, DateTimeKind.Local), morning);
		}

		[TestMethod]
		[TestCategory("Extensions"), TestCategory("Dates")]
		public void DateTime_ToLocalMorning_Utc()
		{
			// Arrange
			var date = new DateTime(2017, 01, 01, 10, 10, 10, DateTimeKind.Utc);

			// Act
			var morning = date.ToLocalMorning().ToUniversalTime();

			// Assert
			Assert.AreEqual(new DateTime(2017, 01, 01, 8, 0, 0, DateTimeKind.Utc), morning);
		}

		[TestMethod]
		[TestCategory("Extensions"), TestCategory("Dates")]
		public void DateTime_ToLocalMorning_Utc_PreviousDay()
		{
			// Arrange
			var date = new DateTime(2017, 01, 01, 7, 59, 59, DateTimeKind.Utc);

			// Act
			var morning = date.ToLocalMorning().ToUniversalTime();

			// Assert
			Assert.AreEqual(new DateTime(2016, 12, 31, 8, 0, 0, DateTimeKind.Utc), morning);
		}


		[TestMethod]
		[TestCategory("Extensions"), TestCategory("Dates")]
		public void DateTime_ToLocalMidnight_Unspecified()
		{
			// Arrange
			var date = new DateTime(2017, 01, 01, 10, 10, 10);

			// Act
			var midnight = date.ToLocalMidnight();

			// Assert
			Assert.AreEqual(new DateTime(2017, 01, 01, 23, 59, 59, DateTimeKind.Local), midnight);
		}

		[TestMethod]
		[TestCategory("Extensions"), TestCategory("Dates")]
		public void DateTime_ToLocalMidnight_Utc()
		{
			// Arrange
			var date = new DateTime(2017, 01, 01, 10, 10, 10, DateTimeKind.Utc);

			// Act
			var midnight = date.ToLocalMidnight().ToUniversalTime();

			// Assert
			Assert.AreEqual(new DateTime(2017, 01, 02, 7, 59, 59, DateTimeKind.Utc), midnight);
		}

		[TestMethod]
		[TestCategory("Extensions"), TestCategory("Dates")]
		public void DateTime_ToLocalMidnight_Utc_PreviousDay()
		{
			// Arrange
			var date = new DateTime(2017, 01, 01, 7, 59, 59, DateTimeKind.Utc);

			// Act
			var midnight = date.ToLocalMidnight().ToUniversalTime();

			// Assert
			Assert.AreEqual(new DateTime(2017, 01, 01, 7, 59, 59, DateTimeKind.Utc), midnight);
		}

		[TestMethod]
		[TestCategory("Extensions"), TestCategory("Dates")]
		public void DateTime_ToUtcMorning_Local()
		{
			// Arrange
			var date = new DateTime(2018, 8, 30, 12, 0, 0, DateTimeKind.Local);
			var expected = new DateTime(2018, 8, 30, 7, 0, 0, DateTimeKind.Utc);

			// Act
			var actual = date.ToUtcMorning();

			// Assert
			actual.Should().Be(expected);
		}

		[TestMethod]
		[TestCategory("Extensions"), TestCategory("Dates")]
		public void DateTime_ToUtcMorning_Utc()
		{
			// Arrange
			var date = new DateTime(2018, 8, 30, 1, 0, 0, DateTimeKind.Utc);
			var expected = new DateTime(2018, 8, 29, 7, 0, 0, DateTimeKind.Utc);

			// Act
			var actual = date.ToUtcMorning();

			// Assert
			actual.Should().Be(expected);
		}

		[TestMethod]
		[TestCategory("Extensions"), TestCategory("Dates")]
		public void DateTime_ToUtcMidnight_Local()
		{
			// Arrange
			var date = new DateTime(2018, 8, 30, 1, 0, 0, DateTimeKind.Local);
			var expected = new DateTime(2018, 8, 31, 6, 59, 59, DateTimeKind.Utc);

			// Act
			var actual = date.ToUtcMidnight();

			// Assert
			actual.Should().Be(expected);
		}

		[TestMethod]
		[TestCategory("Extensions"), TestCategory("Dates")]
		public void DateTime_ToUtcMidnight_Utc()
		{
			// Arrange
			var date = new DateTime(2018, 8, 30, 1, 0, 0, DateTimeKind.Utc);
			var expected = new DateTime(2018, 8, 30, 6, 59, 59, DateTimeKind.Utc);

			// Act
			var actual = date.ToUtcMidnight();

			// Assert
			actual.Should().Be(expected);
		}
		#endregion
	}
}
