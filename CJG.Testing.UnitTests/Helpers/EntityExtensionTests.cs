using CJG.Core.Entities;
using CJG.Testing.Core;
using CJG.Web.External.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.Helpers
{
	[TestClass]
    public class EntityExtensionTests
    {
        [TestMethod, TestCategory("Unit")]
        public void GetFileName_WithCourseTitle_ReturnsCourseTitle()
        {
            var user = EntityHelper.CreateExternalUser();
            var app = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.Draft);
            app.TrainingPrograms.Add(new TrainingProgram() { CourseTitle = "test" });
            var fileName = app.GetFileName();
            fileName.Should().EndWith("test");
        }

	}
}
