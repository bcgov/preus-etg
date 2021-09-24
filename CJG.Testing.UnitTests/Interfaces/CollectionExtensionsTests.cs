using CJG.Core.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Testing.UnitTests.Interfaces
{
	[TestClass]
    public class CollectionExtensionsTests
    {
        [TestMethod]
        public void Pivot_WithOneKeyDictionary_ReturnsDictionary()
        {
            var target = new Dictionary<string, IEnumerable<string>> { { "a1", new[] {"b1", "b2"}}}.Pivot();
            target.Should().HaveCount(2);
            target["b1"].Should().HaveCount(1);
            target["b1"].ToArray()[0].Should().Be("a1");
            target["b2"].Should().HaveCount(1);
            target["b2"].ToArray()[0].Should().Be("a1");
        }

        [TestMethod]
        public void Pivot_WithMultiKeyDictionary_ReturnsDictionary()
        {
            var target = new Dictionary<string, IEnumerable<string>>
            {
                { "a1", new[] {"b1", "b2"}},
                { "a2", new[] {"b1", "b2", "b3"}},
                { "a3", new[] {"b4"}},
            }.Pivot();
            target.Should().HaveCount(4);
            target["b1"].Should().HaveCount(2);
            target["b1"].ToArray()[0].Should().Be("a1");
            target["b1"].ToArray()[1].Should().Be("a2");
            target["b2"].Should().HaveCount(2);
            target["b2"].ToArray()[0].Should().Be("a1");
            target["b2"].ToArray()[1].Should().Be("a2");
            target["b3"].Should().HaveCount(1);
            target["b3"].ToArray()[0].Should().Be("a2");
            target["b4"].Should().HaveCount(1);
            target["b4"].ToArray()[0].Should().Be("a3");
            
        }

        [TestMethod]
        public void Pivot_WithEmptyDictionary_ReturnsEmptyDictionary()
        {
            var target = new Dictionary<string, IEnumerable<string>>().Pivot();
            target.Should().HaveCount(0);
        }

        [TestMethod]
        public void Pivot_WithNullDictionary_ThrowsArgumentNullException()
        {
            Action action = () => CollectionExtensions.Pivot(null);
            action.Should().Throw<ArgumentNullException>();
        }
    }
}
