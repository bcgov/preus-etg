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
		public void HaveYouEverBeenEmployedShouldBeRequired(int employmentStatus, bool required)
		{
			_model.EmploymentStatus = employmentStatus;

			var results = ValidateModel(_model);
			var errorToLookFor = "The Have you ever been employed field is required.";

			Assert.AreEqual(required, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		[DataTestMethod]
		[DataRow(false, false)]
		[DataRow(true, true)]
		public void HaveYouEverBeenEmployedAffectsSubQuestions(bool beenEmployed, bool required)
		{
			_model.HaveYouEverBeenEmployed = beenEmployed;

			_model.EmploymentStatus = 1;
			_model.PreviousEmploymentLastDayOfWork = null;

			var results = ValidateModel(_model);
			var errorToLookFor = "Previous employment last day of work is required.";

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
			_model.HaveYouEverBeenEmployed = true;

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
			_model.HaveYouEverBeenEmployed = true;

			var results = ValidateModel(_model);
			var errorToLookFor = "Previous employment last day of work cannot be greater than today.";

			Assert.AreEqual(hasError, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		[DataTestMethod]
		[DataRow(1, true)]
		[DataRow(2, false)]
		[DataRow(3, false)]
		[DataRow(4, true)]
		public void PreviousAverageWageShouldBeRequired(int employmentStatus, bool required)
		{
			_model.EmploymentStatus = employmentStatus;
			_model.PreviousHourlyWage = null;
			_model.HaveYouEverBeenEmployed = true;

			var results = ValidateModel(_model);
			var errorToLookFor = "The Previous Average Hour field is required.";

			Assert.AreEqual(required, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		[DataTestMethod]
		[DataRow(-5, true)]
		[DataRow(0, false)]
		[DataRow(50, false)]
		[DataRow(120, false)]
		[DataRow(170, true)]
		public void PreviousAverageWageShouldBeWithinRange(int rate, bool showError)
		{
			_model.EmploymentStatus = 1;
			_model.PreviousAvgHoursPerWeek = rate;

			var results = ValidateModel(_model);
			var errorToLookFor = "The Previous Average Hours per Week must be within 0 to 168.";

			Assert.AreEqual(showError, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		[DataTestMethod]
		[DataRow(1, true)]
		[DataRow(2, false)]
		[DataRow(3, false)]
		[DataRow(4, true)]
		[DataRow(5, false)]
		[DataRow(6, false)]
		public void LastPreviousEmployerNameShouldBeRequired(int employmentStatus, bool required)
		{
			_model.EmploymentStatus = employmentStatus;
			_model.PreviousEmployerFullName = null;
			_model.HaveYouEverBeenEmployed = true;

			var results = ValidateModel(_model);
			var errorToLookFor = "The Last Previous Employer field is required.";

			Assert.AreEqual(required, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		[DataTestMethod]
		[DataRow(1, true)]
		[DataRow(2, false)]
		[DataRow(3, false)]
		[DataRow(4, true)]
		[DataRow(5, false)]
		[DataRow(6, false)]
		public void PreviousEmploymentNocShouldBeRequired(int employmentStatus, bool required)
		{
			_model.EmploymentStatus = employmentStatus;
			_model.HaveYouEverBeenEmployed = true;

			_model.PreviousEmploymentNoc1Id = null;
			_model.PreviousEmploymentNoc2Id = null;
			_model.PreviousEmploymentNoc3Id = null;
			_model.PreviousEmploymentNoc4Id = null;
			_model.PreviousEmploymentNoc5Id = null;

			var results = ValidateModel(_model);
			var errorToLookFor = "Your National Occupation Classification (NOC) for previous employment is required.";

			Assert.AreEqual(required, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		[TestMethod]
		public void PreviousEmploymentNocIsSatisfied()
		{
			_model.EmploymentStatus = 1;
			_model.PreviousEmploymentNoc1Id = 1;
			_model.PreviousEmploymentNoc2Id = 2;
			_model.PreviousEmploymentNoc3Id = 3;
			_model.PreviousEmploymentNoc4Id = 4;
			_model.PreviousEmploymentNoc5Id = 5;

			var results = ValidateModel(_model);
			var errorToLookFor = "Your National Occupation Classification (NOC) for previous employment is required.";

			Assert.AreEqual(false, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		[DataTestMethod]
		[DataRow(1, true)]
		[DataRow(2, false)]
		[DataRow(3, false)]
		[DataRow(4, true)]
		[DataRow(5, false)]
		[DataRow(6, false)]
		public void PreviousEmploymentNaicsShouldBeRequired(int employmentStatus, bool required)
		{
			_model.EmploymentStatus = employmentStatus;
			_model.HaveYouEverBeenEmployed = true;

			_model.PreviousEmploymentNaics1Id = null;
			_model.PreviousEmploymentNaics2Id = null;
			_model.PreviousEmploymentNaics3Id = null;
			_model.PreviousEmploymentNaics4Id = null;
			_model.PreviousEmploymentNaics5Id = null;

			var results = ValidateModel(_model);
			var errorToLookFor = "Your North American Industry Classification System (NAICS) for previous employment is required.";

			Assert.AreEqual(required, results.Any(x => x.ErrorMessage == errorToLookFor));
		}

		[TestMethod]
		public void PreviousEmploymentNaicsIsSatisfied()
		{
			_model.EmploymentStatus = 1;
			_model.PreviousEmploymentNaics1Id = 1;
			_model.PreviousEmploymentNaics2Id = 2;
			_model.PreviousEmploymentNaics3Id = 3;
			_model.PreviousEmploymentNaics4Id = 4;
			_model.PreviousEmploymentNaics5Id = 5;

			var results = ValidateModel(_model);
			var errorToLookFor = "Your North American Industry Classification System (NAICS) for previous employment is required.";

			Assert.AreEqual(false, results.Any(x => x.ErrorMessage == errorToLookFor));
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