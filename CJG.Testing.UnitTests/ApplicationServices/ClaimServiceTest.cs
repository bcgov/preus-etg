using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class ClaimServiceTest : ServiceUnitTestBase
	{
		#region Initialize
		[TestInitialize]
		public void Setup()
		{
		}
		#endregion

		#region Tests
		/// <summary>
		/// Submitting a valid claim should change the claim and application states.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void SubmitNewClaim_ShouldSubmitClaim()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var user = new User() { FirstName = "test", Id = 1, AccountType = AccountTypes.Internal };
			applicationAdministrator.AccountType = AccountTypes.External;
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var claim = EntityHelper.CreateClaim(grantApplication, 1, 1);
			claim.ClaimState = ClaimState.Complete;
			grantApplication.Claims.Add(claim);
			grantApplication.AddApplicationAdministrator(applicationAdministrator);
			var state = new GrantApplicationStateChange(grantApplication, ApplicationStateInternal.NewClaim, ApplicationStateInternal.New, user, "test");
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(claim);
			helper.MockDbSet(applicationAdministrator);
			helper.MockDbSet(grantApplication);
			helper.MockDbSet(state);
			helper.MockDbSet(user);
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<int>())).Returns(user);

			var httpContext = helper.GetMock<HttpContextBase>();
			httpContext.Setup(x => x.User).Returns(identity);
			var service = helper.Create<ClaimService>();

			// Act
			service.SubmitClaim(claim);

			// Assert
			claim.ClaimState.Should().Be(ClaimState.Unassessed);
			Assert.AreEqual(grantApplication.ApplicationStateExternal, ApplicationStateExternal.ClaimSubmitted);
			Assert.AreEqual(grantApplication.ApplicationStateInternal, ApplicationStateInternal.NewClaim);
		}

		/// <summary>
		/// Submitting a claim with a grant application in an invalid state should throw a NotAuthorizedException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void SubmitInvalidClaimShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.CancelledByMinistry);
			var claim = EntityHelper.CreateClaim(grantApplication, 1, 1);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet( new[] { claim });
			helper.MockDbSet( new[] { applicationAdministrator });
			helper.MockDbSet( new[] { grantApplication });
			helper.MockDbSet( new List<GrantApplicationStateChange>());
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<int>())).Returns(applicationAdministrator);

			var service = helper.Create<ClaimService>();

			// Act Could not load file or assembly 'NLog.Web' or one of its dependencies
			Action action =()=> service.SubmitClaim(claim);

			// Assert
			action.Should().Throw<NotAuthorizedException>();
		}

		/// <summary>
		/// Submitting a claim with an Unauthorized user should throw a NotAuthorizedException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void SubmitClaim_WithUnauthorizedUserShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var unAuthUser = EntityHelper.CreateExternalUser(2);
			var unAuthIdentity = HttpHelper.CreateIdentity(unAuthUser);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.Id = 1;
			grantApplication.TrainingCost.AgreedParticipants = 5;
			var claim = EntityHelper.CreateClaim(grantApplication, 1, 1);
			var trainingCost = new TrainingCost() { GrantApplication = grantApplication };
			var eligibleExpenseType = new EligibleExpenseType("Mandatory student fees", new ExpenseType(ExpenseTypes.ParticipantLimited));
			var eligibleCost = new EligibleCost() { Id = 1, EligibleExpenseTypeId = (int)ExpenseTypes.ParticipantLimited, TrainingCost = trainingCost };
			var claimEligibleCost = new ClaimEligibleCost() { Id = 1, Claim = claim, EligibleCostId = 1, EligibleCost = eligibleCost };
			var trainingProgram = new TrainingProgram() { Id = 2, GrantApplication = grantApplication };

			var helper = new ServiceHelper(typeof(ClaimService), unAuthIdentity);
			helper.MockContext();
			helper.MockDbSet<ParticipantCost>();
			helper.MockDbSet<TrainingProgram>(trainingProgram);
			helper.MockDbSet<EligibleExpenseType>(eligibleExpenseType);
			helper.MockDbSet(claimEligibleCost);
			helper.MockDbSet(new List<GrantApplicationStateChange>());
			helper.MockDbSet(claim);
			helper.MockDbSet(applicationAdministrator);
			helper.MockDbSet(grantApplication);

			var service = helper.Create<ClaimService>();
			var validationResults = service.Validate(claim).ToArray();

			// Act
			Action action = () => service.SubmitClaim(claim);

			// Assert
			action.Should().Throw<NotAuthorizedException>();
			Assert.AreEqual(grantApplication.ApplicationStateInternal, ApplicationStateInternal.AgreementAccepted);
			Assert.AreEqual(true, claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete));
			validationResults.Count().Should().Be(0);
		}

		/// <summary>
		/// Get claim attachment should return an attachment.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetAttachment_ShouldReturnAttachment()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var attachment = new Attachment() {
				Id = 2,
				FileName = "AttachmentFile",
				FileExtension = "Extension"
			};

			var claim = new Claim(1, 1, grantApplication) {
				Receipts = new List<Attachment>()
				{
					attachment
				}
			};

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { claim });
			helper.MockDbSet(new[] { attachment });
			var service = helper.Create<ClaimService>();

			// Act
			var result = service.GetAttachment(claim.Id, claim.ClaimVersion, attachment.Id);

			// Assert
			result.Id.Should().Be(attachment.Id);

		}

		/// <summary>
		/// Get claim attachments with no Id found should throw a NoContentException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetAttachmentNullShouldThrowNoContentException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var attachment = new Attachment() {
				Id = 2,
				FileName = "AttachmentFile",
				FileExtension = "Extension"
			};

			var claim = new Claim(1, 1, grantApplication) {
				Receipts = new List<Attachment>()
				{
					attachment
				}
			};

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { claim });
			helper.MockDbSet(new List<Attachment>());
			var service = helper.Create<ClaimService>();

			// Act
			Action action = () => service.GetAttachment(claim.Id, claim.ClaimVersion, attachment.Id);

			// Assert
			action.Should().Throw<NoContentException>();

		}

		/// <summary>
		/// Get claim attachments with unauthorized permissions should throw a NotAuthorizedException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetAttachmentInvalidUserShouldThrowNoContentException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var unAuthUser = EntityHelper.CreateExternalUser(2);
			var unAuthIdentity = HttpHelper.CreateIdentity(unAuthUser);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var attachment = new Attachment() {
				Id = 2,
				FileName = "AttachmentFile",
				FileExtension = "Extension"
			};

			var claim = new Claim(1, 1, grantApplication) {
				Receipts = new List<Attachment>()
				{
					attachment
				}
			};

			var helper = new ServiceHelper(typeof(ClaimService), unAuthIdentity);
			helper.MockDbSet(new[] { claim });
			helper.MockDbSet(new[] { attachment });
			var service = helper.Create<ClaimService>();

			// Act
			Action action = () => service.GetAttachment(claim.Id, claim.ClaimVersion, attachment.Id);

			// Assert
			action.Should().Throw<NotAuthorizedException>();

		}

		/// <summary>
		/// Add existing attachment to claim should update the claim and attachment.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void AddExistingAttachment_ShouldUpdateAttachment()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var existingAttachment = new Attachment() {
				Id = 1,
				FileName = "AttachmentFile",
				FileExtension = "Extension"
			};
			var updateAttachment = new Attachment() {
				Id = 1,
				FileName = "UpdatedFile",
				FileExtension = "Extension"
			};
			var claim = new Claim(1, 1, grantApplication) {
				Receipts = new List<Attachment> { existingAttachment }
			};
			List<Claim> claims = new List<Claim> { claim };

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(existingAttachment);
			helper.MockDbSet(claim);
			var service = helper.Create<ClaimService>();

			// Act
			service.AddReceipt(1, updateAttachment);

			// Assert
			Assert.AreEqual(claim.Receipts.Count(), 1);
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<Attachment>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<Claim>()), Times.Exactly(1));
		}

		/// <summary>
		/// Add attachment should add an attachment to the claim and update claim receipts(attachments).
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void AddAttachment_ShouldAddAttachmentToClaim()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var attachment = new Attachment() {
				Id = 2,
				FileName = "AttachmentFile",
				FileExtension = "Extension"
			};
			var claim = new Claim(1, 1, grantApplication) {
				Receipts = new List<Attachment>()
			};
			List<Claim> claims = new List<Claim> { claim };

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(attachment);
			helper.MockDbSet(claim);

			var service = helper.Create<ClaimService>();

			// Act
			service.AddReceipt(1, attachment);

			// Assert
			Assert.AreEqual(claim.Receipts.Count(), 1);
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<Claim>()), Times.Exactly(1));
		}

		/// <summary>
		/// Add attachment with no claim should throw a NoContentException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void AddAttachmentWithNoClaimShouldThrowNoContentException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var attachment = new Attachment() {
				Id = 2,
				FileName = "AttachmentFile",
				FileExtension = "Extension"
			};
			var claim = new Claim(1, 1, grantApplication) {
				Receipts = new List<Attachment>()
			};

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { attachment });
			helper.MockDbSet(new[] { claim });
			var service = helper.Create<ClaimService>();

			// Act
			Action action = () => service.AddReceipt(5, attachment);

			// Assert
			action.Should().Throw<NoContentException>();
		}

		/// <summary>
		/// Add attachment with no attachment should throw an ArgumentNullException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddAttachment_NullAttachmentShouldThrowArgumentNullException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			Attachment attachment = null;
			var claim = EntityHelper.CreateClaim(grantApplication);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { attachment });
			helper.MockDbSet(new[] { claim });
			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act & Assert (Handled by decorator)
			service.AddReceipt(1, attachment);
		}

		/// <summary>
		/// Add attachment with submitted claim should throw a NotAuthorizedException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void AddAttachment_WithSubmittedClaimShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.NewClaim);
			var attachment = EntityHelper.CreateAttachment();
			var claim = EntityHelper.CreateClaim(grantApplication);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { attachment });
			helper.MockDbSet(new[] { claim });
			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act & Assert (Handled by decorator)
			service.AddReceipt(1, attachment);
		}

		/// <summary>
		/// Add attachment with an unauthorized user should throw a NotAuthorizedException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void AddAttachmentUnauthorizedUserShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var unAuthUser = EntityHelper.CreateExternalUser(2);
			var unAuthIdentity = HttpHelper.CreateIdentity(unAuthUser);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var attachment = new Attachment() {
				Id = 2,
				FileName = "AttachmentFile",
				FileExtension = "Extension"
			};
			var claim = new Claim(1, 1, grantApplication) {
				Receipts = new List<Attachment>()
			};
			List<Claim> claims = new List<Claim> { claim };
			var helper = new ServiceHelper(typeof(ClaimService), unAuthIdentity);
			helper.MockDbSet(attachment);
			helper.MockDbSet(claim);

			var service = helper.Create<ClaimService>();

			// Act
			Action action = () => service.AddReceipt(1, attachment);

			// Assert
			action.Should().Throw<NotAuthorizedException>();
		}

		/// <summary>
		/// Add attachment should update an attachment from an older claim and update claim receipts(attachments).
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void AddAttachment_WithOldClaim_ShouldAddAttachmentToClaim()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var attachment = EntityHelper.CreateAttachment();
			var claimOne = EntityHelper.CreateClaim(grantApplication);
			claimOne.Receipts.Add(attachment);
			var claimTwo = EntityHelper.CreateClaim(grantApplication, 2, 2);
			var claims = new List<Claim> { claimOne, claimTwo };
			grantApplication.Claims = claims;

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(attachment);
			helper.MockDbSet(claims);
			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act
			service.AddReceipt(2, attachment);

			// Assert
			claimOne.Receipts.Should().HaveCount(1);
			claimTwo.Receipts.Should().HaveCount(1);
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<Claim>()), Times.Once);
		}

		/// <summary>
		/// Add attachment with no attachment should throw an ArgumentNullException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddAttachment_VersionedClaim_NullAttachmentShouldThrowArgumentNullException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			Attachment attachment = null;
			var claim = EntityHelper.CreateClaim(grantApplication);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(attachment);
			helper.MockDbSet(claim);
			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act & Assert (Handled by decorator)
			service.AddReceipt(1, 1, attachment);
		}

		/// <summary>
		/// Add attachment with submitted claim should throw a NotAuthorizedException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void AddAttachment_VersionedClaim_WithSubmittedClaimShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.NewClaim);
			var attachment = EntityHelper.CreateAttachment();
			var claim = EntityHelper.CreateClaim(grantApplication);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new List<Attachment>() { attachment });
			helper.MockDbSet(new[] { claim });
			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act & Assert (Handled by decorator)
			service.AddReceipt(1, 1, attachment);
		}

		/// <summary>
		/// Add attachment should add an attachment to the claim and update claim receipts(attachments).
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void AddAttachment_VersionedClaim_ShouldAddAttachmentToClaim()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var attachment = EntityHelper.CreateAttachment();
			var claim = EntityHelper.CreateClaim(grantApplication);
			var claims = new List<Claim> { claim };

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new List<Attachment>() { attachment });
			helper.MockDbSet(claims);

			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act
			var result = service.AddReceipt(1, 1, attachment);

			// Assert
			result.Should().NotBeNull().And.BeOfType<Attachment>();
			claim.Receipts.Should().HaveCount(1);
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<Claim>()), Times.Once);
		}

		/// <summary>
		/// Add attachment should update an attachment in the claim and update claim receipts(attachments).
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void AddAttachment_VersionedClaim_WithExistingAttachment_ShouldAddAttachmentToClaim()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var attachment = EntityHelper.CreateAttachment();
			var claim = EntityHelper.CreateClaim(grantApplication);
			claim.Receipts.Add(attachment);
			var claims = new List<Claim> { claim };

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(attachment);
			helper.MockDbSet(claims);
			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act
			var result = service.AddReceipt(1, 1, attachment);

			// Assert
			result.Should().NotBeNull().And.BeOfType<Attachment>();
			claim.Receipts.Should().HaveCount(1);
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<Attachment>()), Times.Once());
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<Claim>()), Times.Once);
		}

		/// <summary>
		/// Add attachment should update an attachment from an older claim and update claim receipts(attachments).
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void AddAttachment_VersionedClaim_WithOldClaim_ShouldAddAttachmentToClaim()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var attachment = EntityHelper.CreateAttachment();
			var claimOne = EntityHelper.CreateClaim(grantApplication);
			claimOne.Receipts.Add(attachment);
			var claimTwo = EntityHelper.CreateClaim(grantApplication, 2, 2);
			var claims = new List<Claim> { claimOne, claimTwo };
			grantApplication.Claims = claims;

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new List<Attachment>() { attachment });
			helper.MockDbSet(claims);

			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act
			var result = service.AddReceipt(2, 2, attachment);

			// Assert
			result.Should().NotBeNull().And.BeOfType<Attachment>();
			claimOne.Receipts.Should().HaveCount(1);
			claimTwo.Receipts.Should().HaveCount(1);
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<Claim>()), Times.Once);
		}

		/// <summary>
		/// Remove attachment should remove itself from the claim.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void RemoveAttachment_ShouldRemoveAttachment()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var attachment = EntityHelper.CreateAttachment();
			var claim = EntityHelper.CreateClaim(grantApplication);
			claim.Receipts.Add(attachment);
			var claims = new List<Claim> { claim };

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var dbSetAttachment = helper.MockDbSet(attachment);
			helper.MockDbSet(claims);
			helper.MockDbSet<VersionedAttachment>();
		
			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act
			service.RemoveReceipt(1, attachment);

			// Assert
			claim.Receipts.Should().BeEmpty();
			dbSetAttachment.Verify(x => x.Remove(It.IsAny<Attachment>()), Times.Once);
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<Claim>()), Times.Once);
		}

		/// <summary>
		/// Remove attachment with null attachment should throw ArgumentNullException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveAttachment_NullAttachmentShouldThrowArgumentNullException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			Attachment attachment = null;
			var claim = EntityHelper.CreateClaim(grantApplication);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { attachment });
			helper.MockDbSet(new[] { claim });
			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act & Assert (Handled by decorator)
			service.RemoveReceipt(1, attachment);
		}

		/// <summary>
		/// Remove attachment with submitted claim should throw a NotAuthorizedException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void RemoveAttachment_WithSubmittedClaimShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.NewClaim);
			var attachment = EntityHelper.CreateAttachment();
			var claim = EntityHelper.CreateClaim(grantApplication);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { attachment });
			helper.MockDbSet(new[] { claim });
			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act & Assert (Handled by decorator)
			service.RemoveReceipt(1, attachment);
		}

		/// <summary>
		/// Remove attachment with unauthorized user should throw a NotAuthorized exception.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void RemoveAttachmentNotAuthorizedShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var unAuthUser = EntityHelper.CreateExternalUser(2);
			var unAuthIdentity = HttpHelper.CreateIdentity(unAuthUser);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var attachment = new Attachment() {
				Id = 2,
				FileName = "AttachmentFile",
				FileExtension = "Extension"
			};
			var claim = new Claim(1, 1, grantApplication) {
				Receipts = new List<Attachment> { attachment }
			};
			List<Claim> claims = new List<Claim> { claim };

			var helper = new ServiceHelper(typeof(ClaimService), unAuthIdentity);
			helper.MockDbSet(attachment);
			helper.MockDbSet(claim);

			var service = helper.Create<ClaimService>();

			// Act
			Action action = () => service.RemoveReceipt(1, attachment);

			// Assert
			action.Should().Throw<NotAuthorizedException>();
		}

		/// <summary>
		/// Remove attachment with null claim should throw a NoContent exception.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void RemoveAttachmentWithNoClaimShouldThrowNoContentException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var unAuthUser = EntityHelper.CreateExternalUser(2);
			var unAuthIdentity = HttpHelper.CreateIdentity(unAuthUser);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var attachment = new Attachment() {
				Id = 2,
				FileName = "AttachmentFile",
				FileExtension = "Extension"
			};
			List<Claim> claims = new List<Claim> { };

			var helper = new ServiceHelper(typeof(ClaimService), unAuthIdentity);
			helper.MockDbSet(new[] { attachment });
			helper.MockDbSet(claims);

			var service = helper.Create<ClaimService>();

			// Act
			Action action = () => service.RemoveReceipt(1, attachment);

			// Assert
			action.Should().Throw<NoContentException>();
		}

		/// <summary>
		/// Remove attachment should remove itself from the claim.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void RemoveAttachment_VersionedClaim_ShouldRemoveAttachment()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var attachment = EntityHelper.CreateAttachment();
			var claim = EntityHelper.CreateClaim(grantApplication);
			claim.Receipts.Add(attachment);
			var claims = new List<Claim> { claim };

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var dbSetAttachment = helper.MockDbSet(attachment);
			helper.MockDbSet(claims);
			helper.MockDbSet<VersionedAttachment>();

			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act
			service.RemoveReceipt(1, 1, attachment);

			// Assert
			claim.Receipts.Should().BeEmpty();
			dbSetAttachment.Verify(x => x.Remove(It.IsAny<Attachment>()), Times.Once);
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<Claim>()), Times.Once);
		}

		/// <summary>
		/// Remove attachment with null attachment should throw ArgumentNullException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveAttachment_VersionedClaim_NullAttachmentShouldThrowArgumentNullException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			Attachment attachment = null;
			var claim = EntityHelper.CreateClaim(grantApplication);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { attachment });
			helper.MockDbSet(new[] { claim });
			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act & Assert (Handled by decorator)
			service.RemoveReceipt(1, 1, attachment);
		}

		/// <summary>
		/// Remove attachment with submitted claim should throw a NotAuthorizedException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void RemoveAttachment_VersionedClaim_WithSubmittedClaimShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.NewClaim);
			var attachment = EntityHelper.CreateAttachment();
			var claim = EntityHelper.CreateClaim(grantApplication);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { attachment });
			helper.MockDbSet(new[] { claim });
			var service = helper.Create<ClaimService>().As<IClaimService>();

			// Act & Assert (Handled by decorator)
			service.RemoveReceipt(1, 1, attachment);
		}

		/// <summary>
		/// Remove attachment with unauthorized user should throw a NotAuthorizedException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void RemoveAttachment_VersionedClaim_NotAuthorizedShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var unAuthUser = EntityHelper.CreateExternalUser(2);
			var unAuthIdentity = HttpHelper.CreateIdentity(unAuthUser);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var attachment = EntityHelper.CreateAttachment();
			var claim = EntityHelper.CreateClaim(grantApplication);

			var helper = new ServiceHelper(typeof(ClaimService), unAuthIdentity);
			helper.MockDbSet(new[] { attachment });
			helper.MockDbSet(new[] { claim });
			var service = helper.Create<ClaimService>();

			// Act & Assert (Handled by decorator)
			service.RemoveReceipt(1, 1, attachment);
		}

		/// <summary>
		/// Remove attachment with null claim should throw a NoContentException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NoContentException))]
		public void RemoveAttachment_VersionedClaim_WithNoClaimShouldThrowNoContentException()
		{
			// Arrange
			var unAuthUser = EntityHelper.CreateExternalUser(2);
			var unAuthIdentity = HttpHelper.CreateIdentity(unAuthUser);
			var attachment = EntityHelper.CreateAttachment();

			var helper = new ServiceHelper(typeof(ClaimService), unAuthIdentity);
			helper.MockDbSet(new[] { attachment });
			helper.MockDbSet(new List<Claim>());
			var service = helper.Create<ClaimService>();

			// Act & Assert (Handled by decorator)
			service.RemoveReceipt(1, 1, attachment);
		}

		/// <summary>
		/// Adding a new claim with a null grant application should throw an <typeparamref name="ArgumentNullException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddNewClaimNullShouldThrowArgumentNullException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			GrantApplication grantApplication = null;
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.Create<ClaimService>();

			// Act & Assert (handled by decorator)
			service.AddNewClaim(grantApplication);
		}

		/// <summary>
		/// Adding a new claim with a single amendable claim type should throw an <typeparamref name="InvalidOperationException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(InvalidOperationException))]
		public void AddNewClaim_WithSingleAmendableClaimShouldThrowInvalidOperationException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { grantApplication });
			var service = helper.Create<ClaimService>();

			// Act & Assert (handled by decorator)
			service.AddNewClaim(grantApplication);
		}

		/// <summary>
		/// Adding a new claim with an unauthorized user should throw a <typeparamref name="NotAuthorizedException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void AddNewClaim_WithUnauthorizedUserShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser(1, AccountTypes.Test);
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
			grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId =
				ClaimTypes.MultipleClaimsWithoutAmendments;
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { grantApplication });
			var service = helper.Create<ClaimService>();

			// Act & Assert (handled by decorator)
			service.AddNewClaim(grantApplication);
		}

		/// <summary>
		/// Adding a new claim should save data
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void AddNewClaim_WithClaim_ShouldSaveData()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var dbSetMock = helper.MockDbSet<ClaimId>(new ClaimId() { Id = 1});
			helper.MockDbSet(grantApplication);
			helper.MockDbSet<EligibleCost>();
			helper.GetMock<IDataContext>().Setup(m => m.Claims.Add(It.IsAny<Claim>()));
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(applicationAdministrator);
			var service = helper.Create<ClaimService>();

			dbSetMock.Setup(m => m.Add(It.IsAny<ClaimId>())).Callback<ClaimId>(c => c.Id = 1);

			
			// Act
			service.AddNewClaim(grantApplication);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.Claims.Add(It.IsAny<Claim>()));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Once);
		}

		/// <summary>
		/// Creating a new claim version with a null grant application should throw an <typeparamref name="ArgumentNullException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateNewClaimVersionNullShouldThrowArgumentNullException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			GrantApplication grantApplication = null;
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.Create<ClaimService>();

			// Act & Assert (handled by decorator)
			service.CreateNewClaimVersion(grantApplication);
		}

		/// <summary>
		/// Creating a new claim version with no existing claims should throw an <typeparamref name="InvalidOperationException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CreateNewClaimVersion_WithoutAnyClaimsShouldThrowInvalidOperationException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = new GrantApplication();
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.Create<ClaimService>();

			// Act & Assert (handled by decorator)
			service.CreateNewClaimVersion(grantApplication);
		}

		/// <summary>
		/// Creating a new claim version with an unauthorized user should throw an <typeparamref name="NotAuthorizedException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void CreateNewClaimVersion_WithUnauthorizedUserShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.Create<ClaimService>();

			// Act & Assert (handled by decorator)
			service.CreateNewClaimVersion(grantApplication);
		}

		/// <summary>
		/// Creating a new claim version with an incomplete claim should throw an <typeparamref name="InvalidOperationException"/>.
		/// TODO: Remove ignore if the code is actually reachable
		/// </summary>
		[Ignore, TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CreateNewClaimVersion_WithIncompleteClaimShouldThrowInvalidOperationException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator, "System Administrator");
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.ClaimDenied);
			var claim = new Claim(1, 1, grantApplication) {
				ClaimState = ClaimState.Incomplete
			};
			grantApplication.Claims.Add(claim);
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(claim);

			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			// Act
			service.Object.CreateNewClaimVersion(grantApplication);
		}

		/// <summary>
		/// Creating a new claim version with a claim should not save data.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void CreateNewClaimVersion_WithClaim_ShouldNotSaveData()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.ClaimDenied);
			grantApplication.ApplicationStateExternal = ApplicationStateExternal.AmendClaim;
			var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
			var claim = new Claim(1, 1, grantApplication) {
				ClaimState = ClaimState.ClaimDenied
			};
			grantApplication.Claims.Add(claim);
			grantApplication.Claims.Add(claim);
			trainingProgram.GrantApplication = grantApplication;
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var dbSetMock = helper.MockDbSet(claim);
			dbSetMock.Setup(x => x.Add(It.IsAny<Claim>()));
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			// Act
			service.Object.CreateNewClaimVersion(grantApplication);

			// Assert
			dbSetMock.Verify(x => x.Add(It.IsAny<Claim>()));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Never);
		}

		/// <summary>
		/// Clearing draft claims with a null grant application should throw an <typeparamref name="ArgumentNullException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ClearDraftClaimsNullShouldThrowArgumentNullException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			GrantApplication grantApplication = null;
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.Create<ClaimService>();

			// Act & Assert (handled by decorator)
			service.ClearDraftClaims(grantApplication);
		}

		/// <summary>
		/// Clearing draft claims with an unauthorized user should throw a <typeparamref name="NotAuthorizedException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void ClearDraftClaims_WithUnauthorizedUserShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser(1, AccountTypes.Test);
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var claim = EntityHelper.CreateClaim(grantApplication);
			grantApplication.Claims.Add(claim);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.Create<ClaimService>();

			// Act & Assert (handled by decorator)
			service.ClearDraftClaims(grantApplication);
		}

		/// <summary>
		/// Clearing draft claims should remove all incomplete Claims, Payments, PaymentRequests, ParticipantCosts, ClaimEligibleCosts and Attachments from the repository.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void ClearDraftClaims_ShouldRemoveAllIncompleteClaims()
		{
			// Arrange
			var internalUser = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(internalUser, "Financial Clerk");
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(internalUser, ApplicationStateInternal.AgreementAccepted);

			var approvedClaim = EntityHelper.CreateClaim(grantApplication);
			approvedClaim.ClaimState = ClaimState.ClaimApproved;
			grantApplication.Claims.Add(approvedClaim);

			var claim = EntityHelper.CreateClaim(grantApplication);
			claim.ClaimState = ClaimState.Incomplete;
			claim.PaymentRequests.Add(new PaymentRequest());
			claim.Receipts.Add(EntityHelper.CreateAttachment());
			grantApplication.Claims.Add(claim);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { grantApplication });
			helper.MockDbSet(new List<Claim>() { claim });
			helper.MockDbSet<PaymentRequest>();
			helper.MockDbSet<ParticipantCost>();
			helper.MockDbSet<ClaimEligibleCost>();
			helper.MockDbSet<Attachment>();
			helper.GetMock<IDataContext>().Setup(m => m.Claims.Add(It.IsAny<Claim>()));
			helper.GetMock<IDataContext>().Setup(m => m.PaymentRequests.Add(It.IsAny<PaymentRequest>()));
			helper.GetMock<IDataContext>().Setup(m => m.ParticipantCosts.Add(It.IsAny<ParticipantCost>()));
			helper.GetMock<IDataContext>().Setup(m => m.ClaimEligibleCosts.Add(It.IsAny<ClaimEligibleCost>()));
			helper.GetMock<IDataContext>().Setup(m => m.Attachments.Add(It.IsAny<Attachment>()));
			var service = helper.Create<ClaimService>();

			// Act
			service.ClearDraftClaims(grantApplication);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.Claims.Remove(It.IsAny<Claim>()), Times.Once);
			helper.GetMock<IDataContext>().Verify(x => x.ParticipantCosts.Remove(It.IsAny<ParticipantCost>()), Times.AtLeastOnce);
			helper.GetMock<IDataContext>().Verify(x => x.ClaimEligibleCosts.Remove(It.IsAny<ClaimEligibleCost>()), Times.Once);
			helper.GetMock<IDataContext>().Verify(x => x.Attachments.Remove(It.IsAny<Attachment>()), Times.Once);
		}

		/// <summary>
		/// Updating a claim with a null claim should throw a <typeparamref name="ArgumentNullException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void UpdateClaimNullShouldThrowArgumentNullException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();
			Claim claim = null;

			// Act & Assert (handled by decorator)
			service.Object.Update(claim);
		}

		/// <summary>
		/// Updating a claim with an unauthorized user should throw a <typeparamref name="NotAuthorizedException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void UpdateClaim_WithUnauthorizedUserShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser(1, AccountTypes.Test);
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var claim = EntityHelper.CreateClaim(grantApplication);
			grantApplication.Claims.Add(claim);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			// Act & Assert (handled by decorator)
			service.Object.Update(claim);
		}

		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void UpdateClaim_WithClaim_ShouldUpdateData()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var claim = new Claim(1, 1, grantApplication);
			grantApplication.Claims.Add(claim);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(claim);
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			// Act
			service.Object.Update(claim);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<Claim>()));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(1));
		}

		/// <summary>
		/// Get Claim with an id and version should return a <typeparamref name="Claim"/> object.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetClaim_WithIdAndVersion_ShouldReturnClaimData()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var claim = EntityHelper.CreateClaim(grantApplication);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(new[] { claim });
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			// Act
			var result = service.Object.Get(claim.Id, claim.ClaimVersion);

			// Assert
			result.Should().NotBeNull().And.BeOfType<Claim>();
			result.Id.Should().Be(claim.Id);
		}

		/// <summary>
		/// Get Claims with an id should return a list of <typeparamref name="Claim"/> objects.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetClaims_WithId_ShouldReturnListOfClaims()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var claims = new List<Claim>
			{
				EntityHelper.CreateClaim(grantApplication, 1, 1),
				EntityHelper.CreateClaim(grantApplication, 1, 2)
			};

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(claims);
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			// Act
			var results = service.Object.GetClaims(1);

			// Assert
			results.Should().NotBeNull().And.AllBeOfType<Claim>();
			results.Count().Should().Be(2);
		}

		/// <summary>
		/// Get Claims with an unauthorized user should throw a <typeparamref name="NotAuthorizedException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void GetClaims_WithUnauthorizedUserShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser(1, AccountTypes.Test);
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var claim = EntityHelper.CreateClaim(grantApplication);

			helper.MockDbSet(new[] { claim });

			// Act & Assert (Handled by decorator)
			service.Object.GetClaims(1);
		}

		/// <summary>
		/// Get Claims with a claim state and authorized user should return a list of <typeparamref name="Claims"/>
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetClaims_WithClaimStateAndAuthorizedUser_ShouldReturnListOfClaims()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var claims = new List<Claim>
			{
				EntityHelper.CreateClaim(grantApplication, 1, 1),
				EntityHelper.CreateClaim(grantApplication, 1, 2)
			};

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(claims);
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			// Act
			var results = service.Object.GetClaims(ClaimState.Incomplete);

			// Assert
			results.Should().NotBeNull().And.AllBeOfType<Claim>();
			results.Count().Should().Be(2);
		}

		/// <summary>
		/// Get Claims with a claim state and an unauthorized user should throw a <typeparamref name="NotAuthorizedException"/>.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void GetClaims_WithClaimStateAndUnauthorizedUserShouldThrowNotAuthorizedException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser(1, AccountTypes.Internal);
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var claim = EntityHelper.CreateClaim(grantApplication);

			helper.MockDbSet(new[] { claim });

			// Act & Assert (Handled by decorator)
			service.Object.GetClaims(ClaimState.Incomplete);
		}

		/// <summary>
		/// Getting ClaimEligibleCost without permission to view claim should throw a NotAuthorizedException.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		[ExpectedException(typeof(NotAuthorizedException))]
		public void GetClaimEligibleCost_WithoutViewClaimPermissionShouldThrowNotAuthorizedException()
		{
			// Arrange
			const int id = 1;

			var applicationAdministrator = EntityHelper.CreateExternalUser(1, AccountTypes.Internal);
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var claim = EntityHelper.CreateClaim(grantApplication);
			var claimEligibleCost = new ClaimEligibleCost(claim) {
				Id = id
			};

			helper.MockDbSet( new[] { claimEligibleCost });

			// Act & Assert (Handled by decorator)
			service.Object.GetClaimEligibleCost(id);
		}

		/// <summary>
		/// Getting ClaimEligibleCost with permission to view claim should return a ClaimEligibleCost.
		/// </summary>
		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetClaimEligibleCost_WithViewClaimPermission_ShouldReturnClaimEligibleCost()
		{
			// Arrange
			const int id = 1;

			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var claim = EntityHelper.CreateClaim(grantApplication);
			var claimEligibleCost = new ClaimEligibleCost(claim) {
				Id = id
			};

			helper.MockDbSet( new[] { claimEligibleCost });

			// Act
			var result = service.Object.GetClaimEligibleCost(id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ClaimEligibleCost>();
			result.Id.Should().Be(id);
		}

		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetClaimsForTrainingProgram_WithTrainingProgramID_ShouldReturnListOfClaims()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.Id = 3;
			var claims = new List<Claim>()
			{
				new Claim(1, 1, grantApplication),
				new Claim(1, 2, grantApplication)
			};

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			var dbSetMock = helper.MockDbSet<Claim>(claims);
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			// Act
			var results = service.Object.GetClaimsForGrantApplication(3);

			// Assert
			results.Count().Should().Be(2);
		}

		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void AddReceipt_WithIDANDVersionANDAttachment_ShouldNotAddWhenReciptExist()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var attachment = new Attachment() {
				Id = 1,
				FileName = "AttachmentFile",
				FileExtension = "Extension"
			};

			var claim = new Claim(1, 1, grantApplication) {
				GrantApplication = grantApplication,
				Receipts = new List<Attachment>()
				{
					attachment
				}
			};
			claim.GrantApplication.Claims.Add(claim);

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(claim);
			helper.MockDbSet(attachment);
			var dbContextMock = helper.GetMock<IDataContext>();

			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			// Act
			service.Object.AddReceipt(1, attachment);

			// Assert
			claim.Receipts.Count().Should().Be(1);
			dbContextMock.Verify(m => m.Update(It.IsAny<Attachment>()), Times.Once());
		}

		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void AddReceipt_WithIDANDVersionANDAttachment_ShouldAddWhenReciptNotExist()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			var newAttachment = new Attachment() { Id = 2, FileName = "AttachmentFile2", FileExtension = "Extension2" };
			var claim = new Claim(1, 1, grantApplication) {
				Receipts = new List<Attachment>() { new Attachment() { Id = 1, FileName = "AttachmentFile", FileExtension = "Extension" } }
			};

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet(claim);

			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			// Act
			service.Object.AddReceipt(1, newAttachment);

			// Assert
			claim.Receipts.Count().Should().Be(2);
			claim.Receipts.Contains(newAttachment);
		}

		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetCurrentClaimVersion_WithClaimID_ReturnLargestVersionNumber()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.AgreementAccepted);
			var claims = new List<Claim>()
			{
				new Claim(1, 1, grantApplication),
				new Claim(1, 2, grantApplication)
			};

			var helper = new ServiceHelper(typeof(ClaimService), identity);
			helper.MockDbSet<Claim>(claims);
			var service = helper.CreateMock<ClaimService>().As<IClaimService>();

			// Act
			var result = service.Object.GetCurrentClaimVersion(1);

			// Assert
			result.Should().Be(2);
		}

		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetAll_When_Claim_Configurations_Available_Returns_All()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(ProgramConfigurationService), identity);
			var service = helper.Create<ProgramConfigurationService>();

			var programConfigurations = new List<ProgramConfiguration>()
			{
				new ProgramConfiguration {Id = 1},
				new ProgramConfiguration {Id = 2 }
			};

			helper.MockDbSet(programConfigurations);

			//Act
			var result = service.GetAll();

			// Assert
			result.Count().Should().Be(2);
		}

		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetAll_When_Claim_Configurations_Available_And_Is_Active_Is_True_Returns_Only_Active_Claim_Configurations()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(ProgramConfigurationService), identity);
			var service = helper.Create<ProgramConfigurationService>();

			var programConfigurations = new List<ProgramConfiguration>()
			{
				new ProgramConfiguration {Id = 1, IsActive = true},
				new ProgramConfiguration {Id = 2, IsActive = false }
			};

			var dbSetMock = helper.MockDbSet(programConfigurations);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.ProgramConfigurations.AsNoTracking()).Returns(dbSetMock.Object);

			//Act
			var result = service.GetAll(true);

			// Assert
			result.Count().Should().Be(1);
		}

		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetProgramConfiguration_When_Claim_Configurations_Exist_Get_By_Id_Returns_Record()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(ProgramConfigurationService), identity);
			var service = helper.Create<ProgramConfigurationService>();

			var programConfigurations = new List<ProgramConfiguration>()
			{
				new ProgramConfiguration {Id = 1},
				new ProgramConfiguration {Id = 2}
			};

			helper.MockDbSet(programConfigurations);

			//Act
			var result = service.Get(1);

			// Assert 
			result.Should().NotBeNull();
		}

		[TestMethod, TestCategory("Claim"), TestCategory("Service")]
		public void GetProgramConfiguration_When_Claim_Configurations_Do_Not_Exist_Get_By_Id_Throws_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(ProgramConfigurationService), identity);
			var service = helper.Create<ProgramConfigurationService>();

			var programConfigurations = new List<ProgramConfiguration>()
			{
				new ProgramConfiguration {Id = 1},
				new ProgramConfiguration {Id = 2}
			};

			helper.MockDbSet(programConfigurations);

			//Act
			Action action = () => service.Get(999);

			// Assert
			action.Should().Throw<NoContentException>();
		}
		#endregion
	}
}
