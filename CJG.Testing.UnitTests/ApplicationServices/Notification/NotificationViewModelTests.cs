using System;
using CJG.Application.Services.Notifications;
using CJG.Core.Entities;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.ApplicationServices.Notification
{
	[TestClass]
	public class NotificationViewModelTests
	{
		private InternalUser _assessor;
		private GrantApplication _grantApplication;
		private User _externalUser;

		[TestInitialize]
		public void Setup()
		{
			_assessor = EntityHelper.CreateInternalUser();
			_grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(_assessor, ApplicationStateInternal.AgreementAccepted);
			_externalUser = EntityHelper.CreateExternalUser();
		}

		[TestMethod, TestCategory("NotificationViewModel"), TestCategory("Model")]
		public void TestGrantTrainingPeriod_StartAndEndDates_ReturnUnknown()
		{
			_grantApplication.GrantOpening.TrainingPeriod = null;

			var model = new NotificationViewModel(_grantApplication, _externalUser);

			Assert.AreEqual("<Unknown>", model.TrainingPeriodStartDate);
			Assert.AreEqual("<Unknown>", model.TrainingPeriodEndDate);
		}

		[TestMethod, TestCategory("NotificationViewModel"), TestCategory("Model")]
		public void TestGrantTrainingPeriod_StartAndEndDates_FormatProperly()
		{
			var startDate = new DateTime(2021, 5, 1);
			var endDate = new DateTime(2021, 8, 15);

			_grantApplication.GrantOpening.TrainingPeriod.StartDate = startDate;
			_grantApplication.GrantOpening.TrainingPeriod.EndDate = endDate;

			var model = new NotificationViewModel(_grantApplication, _externalUser);

			Assert.AreEqual(FormatDate(startDate), model.TrainingPeriodStartDate);
			Assert.AreEqual(FormatDate(endDate), model.TrainingPeriodEndDate);
		}

		// Mimic NotificationViewModel.FormatDate rather than making it public or internal
		private static string FormatDate(DateTime? dateValue)
		{
			return dateValue?.ToLocalTime().ToString("yyyy-MM-dd") ?? "<Unknown>";
		}
	}
}