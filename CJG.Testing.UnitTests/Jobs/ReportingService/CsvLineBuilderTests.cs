using CJG.Infrastructure.ReportingService;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace CJG.Testing.UnitTests.Jobs.ReportingService
{
	[TestClass]
    public class CsvLineBuilderTests
    {
        [TestMethod]
        public void AppendColumn_WithTwoEmptyColumns_ReturnsSingleSeparator()
        {
            var lineBuilder = new CsvLineBuilder(new StringBuilder());

            lineBuilder.AppendColumn("");
            lineBuilder.AppendColumn("");

            var result = lineBuilder.ToString();

            result.Should().Be(",");
        }

        [TestMethod]
        public void AppendColumn_WithTwoNonEmptyColumns_ReturnsSingleSeparator()
        {
            var lineBuilder = new CsvLineBuilder(new StringBuilder());

            lineBuilder.AppendColumn("A");
            lineBuilder.AppendColumn("B");

            var result = lineBuilder.ToString();

            result.Should().Be("A,B");
        }
    }
}
