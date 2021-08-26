using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass()]
	public class DocumentTemplateValidationTests
	{
		ServiceHelper helper { get; set; }

		[TestInitialize]
		public void Setup()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			helper = new ServiceHelper(typeof(ClaimService), user);
			helper.MockContext();
		}

		[TestMethod, TestCategory("DocumentTemplate"), TestCategory("Validate")]
		public void Validate_When_DocumentTemplate_IsRequired_Properties()
		{
			// Arrange
			var documentTemplate = new DocumentTemplate();

			helper.MockDbSet<DocumentTemplate>(new[] { documentTemplate });

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(documentTemplate).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The Title field is required."));
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The Body field is required."));
		}

		[TestMethod, TestCategory("DocumentTemplate"), TestCategory("Validate")]
		public void Validate_When_DocumentTemplate_AssociatedGrantProgram_Document_Not_Set_Inactive()
		{
			// Arrange
			var documentTemplate = new DocumentTemplate() { Title = "Default Applicant Cover Letter Template", Body = "Some texts", IsActive = false, DocumentType = DocumentTypes.GrantAgreementCoverLetter, Id = 1 };
			var grantProgram = new GrantProgram { Id = 1, State = GrantProgramStates.Implemented, ApplicantCoverLetterTemplateId = 1 };

			helper.MockDbSet<DocumentTemplate>(new[] { documentTemplate });
			helper.MockDbSet<GrantProgram>(new[] { grantProgram });

			var service = helper.Create<ClaimService>();

			// Act
			var validationResults = service.Validate(documentTemplate).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "Document cannot set inactive due to there is at least one Grant Program associated to it."));
		}
	}
}