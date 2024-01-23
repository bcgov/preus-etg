using System.Collections.Generic;
using CJG.Core.Entities;
using CJG.Core.Entities.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.Entities.Extensions
{
	[TestClass]
	public class GrantApplicationStateExtensionTests
	{
		private GrantApplication _grantApplication;

		[TestInitialize]
		public void Setup()
		{
			_grantApplication = new GrantApplication
			{
				ApplicationStateInternal = ApplicationStateInternal.Draft,
				StateChanges = new List<GrantApplicationStateChange>(),
			};
		}

		[TestMethod]
		public void DraftApplicationIsNotReturnedToDraft()
		{
			Assert.IsFalse(_grantApplication.HasBeenReturnedToDraft());
		}

		[TestMethod]
		public void SubmittedApplicationIsNotReturnedToDraft()
		{
			AddStateChange(_grantApplication, ApplicationStateInternal.Draft, ApplicationStateInternal.New);

			Assert.IsFalse(_grantApplication.HasBeenReturnedToDraft());
		}

		[TestMethod]
		public void UnderAssessmentApplicationIsNotReturnedToDraft()
		{
			AddStateChange(_grantApplication, ApplicationStateInternal.Draft, ApplicationStateInternal.New);
			AddStateChange(_grantApplication, ApplicationStateInternal.New, ApplicationStateInternal.PendingAssessment);
			AddStateChange(_grantApplication, ApplicationStateInternal.PendingAssessment, ApplicationStateInternal.UnderAssessment);

			Assert.IsFalse(_grantApplication.HasBeenReturnedToDraft());
		}

		[TestMethod]
		public void ReturnedToDraftApplicationIsReturnedToDraft()
		{
			AddStateChange(_grantApplication, ApplicationStateInternal.Draft, ApplicationStateInternal.New);
			AddStateChange(_grantApplication, ApplicationStateInternal.New, ApplicationStateInternal.PendingAssessment);
			AddStateChange(_grantApplication, ApplicationStateInternal.PendingAssessment, ApplicationStateInternal.UnderAssessment);
			AddStateChange(_grantApplication, ApplicationStateInternal.UnderAssessment, ApplicationStateInternal.Draft);

			Assert.IsTrue(_grantApplication.HasBeenReturnedToDraft());
		}

		private static void AddStateChange(GrantApplication grantApplication, ApplicationStateInternal fromState, ApplicationStateInternal toState)
		{
			// Offset the add date to make sure they space out properly in the test run.
			var currentStateCount = grantApplication.StateChanges.Count;
			var addedDate = AppDateTime.UtcNow.AddHours(currentStateCount + 1);

			grantApplication.StateChanges.Add(new GrantApplicationStateChange
			{
				FromState = fromState,
				ToState = toState,
				DateAdded = addedDate,
				ChangedDate = addedDate
			});
			grantApplication.ApplicationStateInternal = toState;
		}
	}
}