using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class FiscalYearValidationTests
    {
        ServiceHelper helper { get; set; }

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            helper = new ServiceHelper(typeof(FiscalYearService), user);
            helper.MockContext();
            helper.MockDbSet<FiscalYear>();
        }
    }
}