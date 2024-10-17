using CJG.Web.External.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CJG.Testing.UnitTests.Helpers
{
	[TestClass]
    public class StringExtenstionTests
    {
        [TestMethod, TestCategory("Unit")]
        public void FormatTextLinesToHtml_WithTwoLines_ReturnsTwoLineHtml()
        {
            var htmlLines = StringExtensions.FormatTextLinesToHtml($"line1{Environment.NewLine}line2", 3, 2);
            htmlLines.Should().Be("line1<br/>lin...");
        }

        [TestMethod, TestCategory("Unit")]
        public void FormatTextLinesToHtml_WithSingleLine_ReturnsOneLine()
        {
            var htmlLines = StringExtensions.FormatTextLinesToHtml("line1", 3, 1);
            htmlLines.Should().Be("lin...");
        }

        [TestMethod, TestCategory("Unit")]
        public void FormatTextLinesToHtml_WithTwoLinesMaxOneLine_ReturnsOneLine()
        {
            var htmlLines = StringExtensions.FormatTextLinesToHtml($"line1{Environment.NewLine}line2", 3, 1);
            htmlLines.Should().Be("lin...");
        }

        [TestMethod, TestCategory("Unit")]
        public void FormatTextLinesToHtml_WithTwoLinesMaxOneLine_ReturnsOneLineWithEclipsis()
        {
            var htmlLines = StringExtensions.FormatTextLinesToHtml($"line1{Environment.NewLine}line2", 6, 1);
            htmlLines.Should().Be("line1...");
        }

        [TestMethod, TestCategory("Unit")]
        public void FormatTextLinesToHtml_WithTwoLinesMaxThreeLines_ReturnsTwoLines()
        {
            var htmlLines = StringExtensions.FormatTextLinesToHtml($"line1{Environment.NewLine}line2", 5, 3);
            htmlLines.Should().Be("line1<br/>line2");
        }

        [TestMethod, TestCategory("Unit")]
        public void FormatCurrency_WithTwoDecimalPlaces_ReturnsDollarsAndCents()
        {
            var number = 1500.0M;
            var dollarCurrencyString = number.ToDollarCurrencyString();
            dollarCurrencyString.Should().Be("$1,500.00");
        }

        [TestMethod, TestCategory("Unit")]
        public void FormatCurrency_WithZeroDecimalPlaces_ReturnsDollarsOnly()
        {
            var number = 1500.0M;
            var dollarCurrencyString = number.ToDollarCurrencyString(0);
            dollarCurrencyString.Should().Be("$1,500");
        }

        [TestMethod, TestCategory("Unit")]
        public void ToPercentString_WithNegativeNumber0_9995AndDefaultFormatting_Returns100Percent()
        {
            0.9995M.ToPercentString(decimalPlaces: 1, defaultFormatWithRounding: true).Should().Be("100.0%");
        }

        [TestMethod, TestCategory("Unit")]
        public void ToPercentString_WithNegativeNumber100Percent_Returns100Percent()
        {
            (-1M).ToPercentString().Should().Be("(100.0)%");
        }

        [TestMethod, TestCategory("Unit")]
        public void ToPercentString_WithNegativeNumberCloseTo100Percent_Returns99_9()
        {
            (-0.9995M).ToPercentString().Should().Be("(99.9)%");
        }

        [TestMethod, TestCategory("Unit")]
        public void ToPercentString_WithPositiveNumberCloseTo100Percent_Returns99_9()
        {
            0.9995M.ToPercentString().Should().Be("99.9%");
        }

        [TestMethod, TestCategory("Unit")]
        public void ToPercentString_WithZero_Returns0_0()
        {
            0M.ToPercentString(2).Should().Be("0.00%");
        }

        [TestMethod, TestCategory("Unit")]
        public void ToPercentString_With0999_Returns99_900()
        {
            0.999M.ToPercentString(5).Should().Be("99.90000%");
        }

        [TestMethod, TestCategory("Unit")]
        public void ToPercentString_WithPositiveNumberCloseTo100PercentAnd5Digits_Returns99With5Digits()
        {
            0.99999959999M.ToPercentString(5).Should().Be("99.99995%");
        }
    }
}
