using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class ApplicationAddressServiceTests : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod, TestCategory("Application Address"), TestCategory("Service")]
		public void GetTwoLetterCombinations_WithThreeLetters_ReturnsThreeCombinations()
		{
			var regionIds = ApplicationAddressService.GetTwoLetterCombinations("Was".ToUpper()).ToList();

			regionIds.Should().HaveCount(3);
		}

		[TestMethod, TestCategory("Application Address"), TestCategory("Service")]
		public void TryGenerateUniqueRegionId_WithNewRegion_ReturnsTrueAndFirstTwoLetttersRegionId()
		{
			string regionId;
			var result = ApplicationAddressService.TryGenerateUniqueRegionId(new List<string>()
			{
				"BC", "AL"
			},  "Washington".ToUpper(), 
			out regionId);

			result.Should().BeTrue();
			regionId.Should().Be("WA");
		}

		[TestMethod, TestCategory("Application Address"), TestCategory("Service")]
		public void TryGenerateUniqueRegionId_WithExistingRegion_ReturnsTrueAndNewTwoLetttersRegionId()
		{
			string regionId;
			var result = ApplicationAddressService.TryGenerateUniqueRegionId(new List<string>()
			{
				"BC", "AL", "WA"
			},  "Washington".ToUpper(), 
			out regionId);

			result.Should().BeTrue();
			regionId.Should().Be("WS");
		}

		[TestMethod, TestCategory("Application Address"), TestCategory("Service")]
		public void VerifyOrCreateRegion_ExistingRegionName_ShouldReturnExistingRegion()
		{
			// Arrange
			var region = new Region() { CountryId = "1", Name = "Canada" };
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(ApplicationAddressService), user);
			var service = helper.Create<ApplicationAddressService>();

			helper.MockDbSet(region);

			// Act
			var results = service.VerifyOrCreateRegion(region.Name, region.CountryId);

			// Assert
			results.Name.Should().Be(region.Name);
		}

		[TestMethod, TestCategory("Application Address"), TestCategory("Service")]
		public void VerifyOrCreateRegion_PassInNewRegion_ShouldAddData()
		{
			// Arrange
			var region = new Region() { CountryId = "1", Name = "Canada", Id = "CA" };
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(ApplicationAddressService), user);
			var service = helper.Create<ApplicationAddressService>();

			helper.MockDbSet(region);

			// Act
			var results = service.VerifyOrCreateRegion("America", "2");

			// Assert
			results.Name.Should().Be("America");
			helper.GetMock<DbSet<Region>>().Verify(x => x.Add(It.IsAny<Region>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Application Address"), TestCategory("Service")]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TryGenerateUniqueRegionId_WhenPassInNullRegionName_ThrowException()
		{
			// Arrange
			var aList = new List<string>();
			string regionID;

			// Act
			var results = ApplicationAddressService.TryGenerateUniqueRegionId(aList, null, out regionID);

			// Assert
			// Should throw exception with null country name as parameter
		}

		[TestMethod, TestCategory("Application Address"), TestCategory("Service")]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TryGenerateUniqueRegionId_WhenRegionNameIsLessthan2Chars_ThrowException()
		{
			// Arrange
			var aList = new List<string>();
			string regionID;

			// Act
			var results = ApplicationAddressService.TryGenerateUniqueRegionId(aList, "A", out regionID);

			// Assert
			// Should throw exception with null country name less than 2 chars
		}

		[TestMethod, TestCategory("Application Address"), TestCategory("Service")]
		public void TryGenerateUniqueRegionId_PassInRegionName_ShouldReturnTrue()
		{
			// Arrange
			var aList = new List<string>();
			string regionID;

			// Act
			var results = ApplicationAddressService.TryGenerateUniqueRegionId(aList, "Canada", out regionID);

			// Assert
			results.Should().Be(true);
		}
	}
}
