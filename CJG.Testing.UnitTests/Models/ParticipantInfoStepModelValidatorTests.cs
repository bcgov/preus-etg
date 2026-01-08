using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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