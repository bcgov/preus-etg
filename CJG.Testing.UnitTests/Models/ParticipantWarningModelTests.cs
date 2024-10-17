using CJG.Web.External.Areas.Ext.Models.ParticipantReporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.Models
{
	[TestClass]
	public class ParticipantWarningModelTests
	{
		private ParticipantWarningModel _model;

		[TestInitialize]
		public void SetUp()
		{
			_model = new ParticipantWarningModel
			{
				FiscalYearLimit = 10000M,
				ParticipantName = "Fred Smith"
			};
		}

		[TestMethod]
		public void IsOverLimit()
		{
			_model.CurrentClaims = 12000;
			var result = _model.IsOverLimit();
			Assert.AreEqual(true, result);
		}

		[TestMethod]
		public void IsNotOverLimit()
		{
			_model.CurrentClaims = 8000;
			var result = _model.IsOverLimit();
			Assert.AreEqual(false, result);
		}

		[TestMethod]
		public void IsNearingLimit_WhenNear()
		{
			_model.CurrentClaims = 9500;
			var result = _model.IsNearingLimit();
			Assert.AreEqual(true, result);
		}

		[TestMethod]
		public void IsNearingLimit_WhenAtLimit()
		{
			_model.CurrentClaims = 9000;
			var result = _model.IsNearingLimit();
			Assert.AreEqual(true, result);
		}

		[TestMethod]
		public void IsNearingLimit_WhenUnder()
		{
			_model.CurrentClaims = 5000;
			var result = _model.IsNearingLimit();
			Assert.AreEqual(false, result);
		}

		[TestMethod]
		public void IsNearingLimit_WhenOver()
		{
			_model.CurrentClaims = 15000;
			var result = _model.IsNearingLimit();
			Assert.AreEqual(false, result);
		}
	}
}