using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Part.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.Models
{
	[TestClass]
	public class ParticipantInfoStepModelValidatorTests
	{
		private ParticipantInfoStep4ViewModel _model;

		[TestInitialize]
		public void SetUp()
		{
			_model = new ParticipantInfoStep4ViewModel();
		}

		[DataTestMethod]
		[DataRow(1, false)]
		[DataRow(2, true)]
		[DataRow(3, true)]
		[DataRow(4, false)]
		[DataRow(5, false)]
		[DataRow(6, true)]
		public void MultipleEmploymentPositionsShouldBeRequired(int employmentStatus, bool required)
		{
			_model.EmploymentStatus = employmentStatus;

			var results = ValidateModel(_model);
			var errorToLookFor = "The Multiple Employment Positions field is required.";

			Assert.AreEqual(required, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		[DataTestMethod]
		[DataRow(1, false)]
		[DataRow(2, true)]
		[DataRow(3, true)]
		[DataRow(4, false)]
		[DataRow(5, false)]
		[DataRow(6, true)]
		public void EmployedByShouldBeRequired(int employmentStatus, bool required)
		{
			_model.EmploymentStatus = employmentStatus;
			_model.EmployedBySupportEmployer = null;

			var results = ValidateModel(_model);
			var errorToLookFor = "The Employed By field is required.";

			Assert.AreEqual(required, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		[DataTestMethod]
		[DataRow(1, true)]
		[DataRow(2, false)]
		[DataRow(3, false)]
		[DataRow(4, true)]
		[DataRow(5, false)]
		[DataRow(6, false)]
		public void LastWorkedDateShouldBeRequired(int employmentStatus, bool required)
		{
			_model.EmploymentStatus = employmentStatus;
			_model.PreviousEmploymentLastDayOfWork = null;

			var results = ValidateModel(_model);
			var errorToLookFor = "Previous employment last day of work is required.";

			Assert.AreEqual(required, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		[DataTestMethod]
		[DataRow(-5, false)]
		[DataRow(5, true)]
		public void LastWorkedCannotBeBeforeToday(int dayOffset, bool hasError)
		{
			var currentDate = AppDateTime.UtcNow;

			_model.EmploymentStatus = 1;
			_model.PreviousEmploymentLastDayOfWork = currentDate.AddDays(dayOffset);

			var results = ValidateModel(_model);
			var errorToLookFor = "Previous employment last day of work cannot be greater than today.";

			Assert.AreEqual(hasError, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		public IList<ValidationResult> ValidateModel(object model)
		{
			var results = new List<ValidationResult>();
			var validationContext = new ValidationContext(model, null, null);

			Validator.TryValidateObject(model, validationContext, results, true);

			if (model is IValidatableObject validatableModel)
				results.AddRange(validatableModel.Validate(validationContext));

			return results;
		}
	}
}