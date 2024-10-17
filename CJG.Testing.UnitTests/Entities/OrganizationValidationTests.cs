using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CJG.Testing.Core;
using System;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class OrganizationValidationTests
    {
        ServiceHelper helper { get; set; }

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            helper = new ServiceHelper(typeof(OrganizationService), user);
            helper.MockContext();
            helper.MockDbSet<Organization>();
        }

        [TestMethod, TestCategory("Organization"), TestCategory("Validate")]
        public void Validate_When_Organization_Is_Required_Properties_Error()
        {
            var organization = new Organization();

            var service = helper.Create<OrganizationService>();

            // Act
            var validationResults = service.Validate(organization).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The LegalName field is required."));
        }

        [TestMethod, TestCategory("Organization"), TestCategory("Validate")]
        public void Validate_When_Organization_Not_Associated_With_Organization_Type()
        {
            var organization = new Organization()
            {
                LegalName = "Avocette (New Westminster)",
                OrganizationTypeId = 0
            };

            var service = helper.Create<OrganizationService>();

            // Act
            var validationResults = service.Validate(organization).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The organization must be associated to an organization type."));
        }

        [TestMethod, TestCategory("Organization"), TestCategory("Validate")]
        public void Validate_When_Organization_BCeID_Is_Missing()
        {
            var organization = new Organization()
            {
                LegalName = "Avocette (New Westminster)",
                BCeIDGuid = new Guid("00000000-0000-0000-0000-000000000000")
            };

            var service = helper.Create<OrganizationService>();

            // Act
            var validationResults = service.Validate(organization).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "BCeID field is required."));
        }

        [Ignore, TestMethod, TestCategory("Organization"), TestCategory("Validate")]
        public void Validate_When_Organization_Not_Associated_To_A_Legal_Structure()
        {
            var organization = new Organization()
            {
                LegalName = "Avocette (New Westminster)",
                LegalStructureId = 0
            };

            var service = helper.Create<OrganizationService>();

            // Act
            var validationResults = service.Validate(organization).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The organization must be associated to a legal structure."));
        }

        [TestMethod, TestCategory("Organization"), TestCategory("Validate")]
        public void Validate_When_Organization_Year_Established_Not_Between_250_Years_And_1_Year_In_Future()
        {
            var organization = new Organization()
            {
                LegalName = "Avocette (New Westminster)",
                YearEstablished = AppDateTime.UtcNow.AddYears(2).Year
            };

            var service = helper.Create<OrganizationService>();

            // check if established year is more than 1 year in future
            // Act
            var validationResults = service.Validate(organization).ToArray();

            string validationMsg = "The year established must be between '" + AppDateTime.UtcNow.AddYears(-250).Year + "' and '" + AppDateTime.UtcNow.AddYears(1).Year + "'.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
        }

        [TestMethod, TestCategory("Organization"), TestCategory("Validate")]
        public void Validate_When_Organization_Employees_Worldwide_Number_Not_Equal_Or_Morethan_Employees_Number_In_BC()
        {
            var organization = new Organization()
            {
                LegalName = "Avocette (New Westminster)",
                NumberOfEmployeesInBC = 150,
                NumberOfEmployeesWorldwide = 100
            };

            var service = helper.Create<OrganizationService>();

            // Act
            var validationResults = service.Validate(organization).ToArray();

            string validationMsg = "The number of employees worldwide must be equal to or more than the number of employees in BC which is " + organization.NumberOfEmployeesInBC + ".";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
        }

        [TestMethod, TestCategory("Organization"), TestCategory("Validate")]
        public void Validate_When_Organization_Not_AnnualEmployeesTrained_Lessthan_Or_Equal_To_NumberOfEmployeesWorldwide()
        {
            var organization = new Organization()
            {
                LegalName = "Avocette (New Westminster)",
                NumberOfEmployeesWorldwide = 100,
                AnnualEmployeesTrained = 150
            };

            var service = helper.Create<OrganizationService>();

            // Act
            var validationResults = service.Validate(organization).ToArray();

            string validationMsg = "The number of employees trained annually must be greater than or equal to 0, and not greater than the number of employees worldwide.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
         }
    }
}