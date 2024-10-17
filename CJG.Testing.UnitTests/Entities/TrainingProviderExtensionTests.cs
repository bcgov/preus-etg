using Microsoft.VisualStudio.TestTools.UnitTesting;
using CJG.Core.Entities;
using CJG.Testing.Core;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass]
	public class TrainingProviderExtensionTests
	{
		private TrainingProvider _originalTrainingProvider;
		private TrainingProvider _newTrainingProvider;

		[TestInitialize]
		public void Setup()
		{
			var application = EntityHelper.CreateGrantApplication();

			_originalTrainingProvider = EntityHelper.CreateTrainingProvider(application);
			_originalTrainingProvider.Id = 57;
			_originalTrainingProvider.TrainingProviderAddress = new ApplicationAddress(new Address("1224 St", null, "Victoria", "V9C0E4", new Region("BC", "British Columbia", new Country("CA", "Canada"))));

			_newTrainingProvider = EntityHelper.CreateTrainingProvider(application);
			_newTrainingProvider.OriginalTrainingProviderId = 58;
			_newTrainingProvider.OriginalTrainingProvider = _originalTrainingProvider;
			_newTrainingProvider.TrainingProviderAddress = new ApplicationAddress(new Address("1224 St", null, "Victoria", "V9C0E4", new Region("BC", "British Columbia", new Country("CA", "Canada"))));
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void No_ChangeRequestProvider_Empty()
		{
			_newTrainingProvider.OriginalTrainingProviderId = null;
			_newTrainingProvider.OriginalTrainingProvider = null;

			var differences = _newTrainingProvider.GetChangeRequestDifferences();

			Assert.AreEqual(0, differences.Count);
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void No_Differences_Found()
		{
			var differences = _newTrainingProvider.GetChangeRequestDifferences();
			Assert.AreEqual(0, differences.Count);
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void General_Differences_Found()
		{
			_newTrainingProvider.Name = "Fred Dog Co.";
			_newTrainingProvider.TrainingProviderType = new TrainingProviderType("Corporate Honcho");

			var differences = _newTrainingProvider.GetChangeRequestDifferences();

			Assert.AreEqual(2, differences.Count);

			Assert.AreEqual(true, differences.Contains("Training Provider Type: changed from 'Training Provider Type' to 'Corporate Honcho'"));
			Assert.AreEqual(true, differences.Contains("Name: changed from 'Training Provider' to 'Fred Dog Co.'"));
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void Contact_Differences_Found()
		{
			_newTrainingProvider.ContactFirstName = "Fred";
			_newTrainingProvider.ContactLastName = "Jones";
			_newTrainingProvider.ContactEmail = "fred@jones.com";
			_newTrainingProvider.ContactPhoneNumber = "(250) 555-JEDI";
			_newTrainingProvider.ContactPhoneExtension = "1234";

			var differences = _newTrainingProvider.GetChangeRequestDifferences();

			Assert.AreEqual(5, differences.Count);

			Assert.AreEqual(true, differences.Contains("ContactFirstName: changed from 'First Name' to 'Fred'"));
			Assert.AreEqual(true, differences.Contains("ContactLastName: changed from 'Last Name' to 'Jones'"));
			Assert.AreEqual(true, differences.Contains("ContactEmail: changed from 'contact@email.com' to 'fred@jones.com'"));
			Assert.AreEqual(true, differences.Contains("ContactPhoneNumber: changed from '(123) 123-1235' to '(250) 555-JEDI'"));
			Assert.AreEqual(true, differences.Contains("ContactPhoneExtension: changed from '' to '1234'"));
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void TrainingLocationAddress_Differences_Found()
		{
			_newTrainingProvider.TrainingAddress.AddressLine1 = "1 Smith St";
			_newTrainingProvider.TrainingAddress.AddressLine2 = "Suite 2";
			_newTrainingProvider.TrainingAddress.City = "Frogberg";
			_newTrainingProvider.TrainingAddress.PostalCode = "90210";
			_newTrainingProvider.TrainingAddress.Region.Name = "Hazel Town";
			_newTrainingProvider.TrainingAddress.Country.Name = "Iceville";

			var differences = _newTrainingProvider.GetChangeRequestDifferences();
			Assert.AreEqual(6, differences.Count);
			Assert.AreEqual(true, differences.Contains("AddressLine1: changed from '1224 St' to '1 Smith St'"));
			Assert.AreEqual(true, differences.Contains("AddressLine2: changed from '' to 'Suite 2'"));
			Assert.AreEqual(true, differences.Contains("City: changed from 'Victoria' to 'Frogberg'"));
			Assert.AreEqual(true, differences.Contains("PostalCode: changed from 'V9C0E4' to '90210'"));
			Assert.AreEqual(true, differences.Contains("Region: changed from 'British Columbia' to 'Hazel Town'"));
			Assert.AreEqual(true, differences.Contains("Country: changed from 'Canada' to 'Iceville'"));
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void TrainingProviderAddress_Differences_Found()
		{
			_newTrainingProvider.TrainingProviderAddress.AddressLine1 = "2 Smith St";
			_newTrainingProvider.TrainingProviderAddress.AddressLine2 = "Suite 3";
			_newTrainingProvider.TrainingProviderAddress.City = "Frogland";
			_newTrainingProvider.TrainingProviderAddress.PostalCode = "90211";
			_newTrainingProvider.TrainingProviderAddress.Region.Name = "Hazel Ville";
			_newTrainingProvider.TrainingProviderAddress.Country.Name = "Icetown";

			var differences = _newTrainingProvider.GetChangeRequestDifferences();
			Assert.AreEqual(6, differences.Count);
			Assert.AreEqual(true, differences.Contains("AddressLine1: changed from '1224 St' to '2 Smith St'"));
			Assert.AreEqual(true, differences.Contains("AddressLine2: changed from '' to 'Suite 3'"));
			Assert.AreEqual(true, differences.Contains("City: changed from 'Victoria' to 'Frogland'"));
			Assert.AreEqual(true, differences.Contains("PostalCode: changed from 'V9C0E4' to '90211'"));
			Assert.AreEqual(true, differences.Contains("Region: changed from 'British Columbia' to 'Hazel Ville'"));
			Assert.AreEqual(true, differences.Contains("Country: changed from 'Canada' to 'Icetown'"));
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void AddressBeing_Null_DoesNotExplode()
		{
			_newTrainingProvider.TrainingProviderAddress = null;
			_newTrainingProvider.TrainingAddress = null;

			var differences = _newTrainingProvider.GetChangeRequestDifferences();
			Assert.AreEqual(0, differences.Count);
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void BusinessCase_Attachment_Differences()
		{
			_originalTrainingProvider.BusinessCaseDocument = EntityHelper.CreateAttachment();

			_newTrainingProvider.BusinessCaseDocument = EntityHelper.CreateAttachment();
			_newTrainingProvider.BusinessCaseDocument.FileName = "fred";
			_newTrainingProvider.BusinessCaseDocument.FileExtension = ".txt";
			_newTrainingProvider.BusinessCaseDocument.Description = "This is a new document.";
			
			var differences = _newTrainingProvider.GetChangeRequestDifferences();

			Assert.AreEqual(3, differences.Count);
			
			Assert.AreEqual(true, differences.Contains("Business Case File Name: changed from 'File Name' to 'fred'"));
			Assert.AreEqual(true, differences.Contains("Business Case File Extension: changed from '.pdf' to '.txt'"));
			Assert.AreEqual(true, differences.Contains("Business Case File Description: changed from '' to 'This is a new document.'"));
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void CourseOutline_Attachment_Differences()
		{
			_originalTrainingProvider.CourseOutlineDocument = EntityHelper.CreateAttachment();

			_newTrainingProvider.CourseOutlineDocument = EntityHelper.CreateAttachment();
			_newTrainingProvider.CourseOutlineDocument.FileName = "fred";
			_newTrainingProvider.CourseOutlineDocument.FileExtension = ".txt";
			_newTrainingProvider.CourseOutlineDocument.Description = "This is a new document.";
			
			var differences = _newTrainingProvider.GetChangeRequestDifferences();

			Assert.AreEqual(3, differences.Count);
			
			Assert.AreEqual(true, differences.Contains("Course Outline File Name: changed from 'File Name' to 'fred'"));
			Assert.AreEqual(true, differences.Contains("Course Outline File Extension: changed from '.pdf' to '.txt'"));
			Assert.AreEqual(true, differences.Contains("Course Outline File Description: changed from '' to 'This is a new document.'"));
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void ProofOfQualificationsDocument_Attachment_Differences()
		{
			_originalTrainingProvider.ProofOfQualificationsDocument = EntityHelper.CreateAttachment();

			_newTrainingProvider.ProofOfQualificationsDocument = EntityHelper.CreateAttachment();
			_newTrainingProvider.ProofOfQualificationsDocument.FileName = "fred";
			_newTrainingProvider.ProofOfQualificationsDocument.FileExtension = ".txt";
			_newTrainingProvider.ProofOfQualificationsDocument.Description = "This is a new document.";
			
			var differences = _newTrainingProvider.GetChangeRequestDifferences();

			Assert.AreEqual(3, differences.Count);
			
			Assert.AreEqual(true, differences.Contains("Proof of Qualifications File Name: changed from 'File Name' to 'fred'"));
			Assert.AreEqual(true, differences.Contains("Proof of Qualifications File Extension: changed from '.pdf' to '.txt'"));
			Assert.AreEqual(true, differences.Contains("Proof of Qualifications File Description: changed from '' to 'This is a new document.'"));
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void Attachment_Added()
		{
			_originalTrainingProvider.ProofOfQualificationsDocument = null;

			_newTrainingProvider.ProofOfQualificationsDocument = EntityHelper.CreateAttachment();
			_newTrainingProvider.ProofOfQualificationsDocument.FileName = "fred";
			_newTrainingProvider.ProofOfQualificationsDocument.FileExtension = ".txt";
			_newTrainingProvider.ProofOfQualificationsDocument.Description = "This is a new document.";
			
			var differences = _newTrainingProvider.GetChangeRequestDifferences();

			Assert.AreEqual(3, differences.Count);
			
			Assert.AreEqual(true, differences.Contains("Proof of Qualifications File Name: changed from '' to 'fred'"));
			Assert.AreEqual(true, differences.Contains("Proof of Qualifications File Extension: changed from '' to '.txt'"));
			Assert.AreEqual(true, differences.Contains("Proof of Qualifications File Description: changed from '' to 'This is a new document.'"));
		}

		[TestMethod, TestCategory("Training Provider"), TestCategory("Extensions")]
		public void Attachment_Removed()
		{
			_originalTrainingProvider.ProofOfQualificationsDocument = EntityHelper.CreateAttachment();
			_originalTrainingProvider.ProofOfQualificationsDocument.Description = "Fred was here";

			_newTrainingProvider.ProofOfQualificationsDocument = null;
			
			var differences = _newTrainingProvider.GetChangeRequestDifferences();

			Assert.AreEqual(3, differences.Count);

			Assert.AreEqual(true, differences.Contains("Proof of Qualifications File Name: changed from 'File Name' to ''"));
			Assert.AreEqual(true, differences.Contains("Proof of Qualifications File Extension: changed from '.pdf' to ''"));
			Assert.AreEqual(true, differences.Contains("Proof of Qualifications File Description: changed from 'Fred was here' to ''"));
		}
	}
}