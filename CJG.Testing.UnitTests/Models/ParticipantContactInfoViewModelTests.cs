using CJG.Web.External.Areas.Int.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.Models
{
	[TestClass]
	public class ParticipantContactInfoViewModelTests
	{
		[TestMethod]
		public void GenderStateReportsCorrectly()
		{
			var model = new ParticipantContactInfoViewModel();

			Assert.AreEqual("Male", model.GenderState(1));
			Assert.AreEqual("Female", model.GenderState(2));
			Assert.AreEqual("Unspecified", model.GenderState(4));  // the 3/4 order here is correct
			Assert.AreEqual("Prefer not to answer", model.GenderState(3));  
		}

		[TestMethod]
		public void GenderStateReportsIncorrectValuesAsNull()
		{
			var model = new ParticipantContactInfoViewModel();

			Assert.AreEqual(null, model.GenderState(0));
			Assert.AreEqual(null, model.GenderState(5));
		}
	}
}