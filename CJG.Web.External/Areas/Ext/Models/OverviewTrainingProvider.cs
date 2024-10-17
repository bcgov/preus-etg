using CJG.Application.Services;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class OverviewTrainingProvider
	{
		public int Id { get; set; }
		public TrainingProviderStates TrainingProviderState { get; set; }
		public string Name { get; set; }
		public TrainingProviderType TrainingProviderType { get; set; }
		public Attachment CourseOutlineDocument { get; set; }
		public Attachment ProofOfQualificationsDocument { get; set; }
		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }
		public string City { get; set; }
		public string Region { get; set; }
		public string PostalCode { get; set; }
		public string Country { get; set; }
		public string AddressLine1TrainingProvider { get; set; }
		public string AddressLine2TrainingProvider { get; set; }
		public string CityTrainingProvider { get; set; }
		public string RegionTrainingProvider { get; set; }
		public string PostalCodeTrainingProvider { get; set; }
		public string CountryTrainingProvider { get; set; }
		public bool TrainingOutsideBC { get; set; }
		public Attachment BusinessCaseDocument { get; set; }
		public string ContactFirstName { get; set; }
		public string ContactLastName { get; set; }
		public string ContactEmail { get; set; }
		public string ContactPhoneNumber { get; set; }
		public string ContactPhoneExtension { get; set; }
		public string AlternativeTrainingOptions { get; set; }
		public string ChoiceOfTrainerOrProgram { get; set; }
		public string RowVersion { get; set; }

		public OverviewTrainingProvider()
		{
		}

		public OverviewTrainingProvider(TrainingProvider trainingProvider)
		{
			Utilities.MapProperties(trainingProvider, this);

			TrainingProviderState = trainingProvider.TrainingProviderState;

			TrainingProviderType = trainingProvider.TrainingProviderType;

			CourseOutlineDocument = trainingProvider.CourseOutlineDocument;
			BusinessCaseDocument = trainingProvider.BusinessCaseDocument;
			ProofOfQualificationsDocument = trainingProvider.ProofOfQualificationsDocument;

			if (trainingProvider.TrainingAddress != null)
			{
				AddressLine1 = trainingProvider.TrainingAddress.AddressLine1;
				AddressLine2 = trainingProvider.TrainingAddress.AddressLine2;
				City = trainingProvider.TrainingAddress.City;
				Region = trainingProvider.TrainingAddress.Region.Name;
				PostalCode = trainingProvider.TrainingAddress.PostalCode;
				Country = trainingProvider.TrainingAddress.Country.Name;
			}

			if (trainingProvider.TrainingProviderAddress != null)
			{
				AddressLine1TrainingProvider = trainingProvider.TrainingProviderAddress.AddressLine1;
				AddressLine2TrainingProvider = trainingProvider.TrainingProviderAddress.AddressLine2;
				CityTrainingProvider = trainingProvider.TrainingProviderAddress.City;
				RegionTrainingProvider = trainingProvider.TrainingProviderAddress.Region.Name;
				PostalCodeTrainingProvider = trainingProvider.TrainingProviderAddress.PostalCode;
				CountryTrainingProvider = trainingProvider.TrainingProviderAddress.Country.Name;
			}
		}
	}
}