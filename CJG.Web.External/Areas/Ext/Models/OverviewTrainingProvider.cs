using CJG.Application.Services;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class OverviewTrainingProvider
	{
		#region Properties
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
		public string RowVersion { get; set; }
		#endregion

		#region Constructors
		public OverviewTrainingProvider()
		{

		}

		public OverviewTrainingProvider(TrainingProvider trainingProvider)
		{
			Utilities.MapProperties(trainingProvider, this);

			this.TrainingProviderState = trainingProvider.TrainingProviderState;

			this.TrainingProviderType = trainingProvider.TrainingProviderType;

			this.CourseOutlineDocument = trainingProvider.CourseOutlineDocument;
			this.BusinessCaseDocument = trainingProvider.BusinessCaseDocument;
			this.ProofOfQualificationsDocument = trainingProvider.ProofOfQualificationsDocument;
			if (trainingProvider.TrainingAddress != null)
			{
				this.AddressLine1 = trainingProvider.TrainingAddress.AddressLine1;
				this.AddressLine2 = trainingProvider.TrainingAddress.AddressLine2;
				this.City = trainingProvider.TrainingAddress.City;
				this.Region = trainingProvider.TrainingAddress.Region.Name;
				this.PostalCode = trainingProvider.TrainingAddress.PostalCode;
				this.Country = trainingProvider.TrainingAddress.Country.Name;
			}
			if (trainingProvider.TrainingProviderAddress != null)
			{
				this.AddressLine1TrainingProvider = trainingProvider.TrainingProviderAddress.AddressLine1;
				this.AddressLine2TrainingProvider = trainingProvider.TrainingProviderAddress.AddressLine2;
				this.CityTrainingProvider = trainingProvider.TrainingProviderAddress.City;
				this.RegionTrainingProvider = trainingProvider.TrainingProviderAddress.Region.Name;
				this.PostalCodeTrainingProvider = trainingProvider.TrainingProviderAddress.PostalCode;
				this.CountryTrainingProvider = trainingProvider.TrainingProviderAddress.Country.Name;
			}
		}
		#endregion
	}
}