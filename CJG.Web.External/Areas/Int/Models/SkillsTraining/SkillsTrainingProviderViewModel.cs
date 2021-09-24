using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.SkillsTraining
{
	public class SkillsTrainingProviderViewModel : BaseViewModel
	{
		#region Properties
		public bool IsValidated { get; set; }
		public bool CanValidate { get; set; }
		public bool CanEdit { get; set; }
		public bool CanRemove { get; set; }
		public bool CanRecommendChangeRequest { get; set; }

		public string RowVersion { get; set; }
		public int? EligibleCostId { get; set; }

		public string Name { get; set; }
		public string ChangeRequestReason { get; set; }
		public int TrainingProgramId { get; set; }
		public int? TrainingProviderTypeId { get; set; }
		public TrainingProviderPrivateSectorValidationTypes PrivateSectorValidationType { get; set; }

		public TrainingProviderStates TrainingProviderState { get; set; }
		public int? TrainingProviderInventoryId { get; set; }

		public string ContactFirstName { get; set; }

		public string ContactLastName { get; set; }

		public string ContactEmail { get; set; }

		public string ContactPhone { get; set; }
		public string ContactPhoneAreaCode { get; set; }
		public string ContactPhoneExchange { get; set; }
		public string ContactPhoneNumber { get; set; }
		public string ContactPhoneExtension { get; set; }

		public int? TrainingAddressId { get; set; }
		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }
		public string City { get; set; }
		public string PostalCode { get; set; }
		public string ZipCode { get; set; }
		public string RegionId { get; set; }
		public string Region { get; set; }
		public string CountryId { get; set; }

		public int? TrainingProviderAddressId { get; set; }
		public string AddressLine1TrainingProvider { get; set; }
		public string AddressLine2TrainingProvider { get; set; }
		public string CityTrainingProvider { get; set; }
		public string PostalCodeTrainingProvider { get; set; }
		public string ZipCodeTrainingProvider { get; set; }
		public string RegionIdTrainingProvider { get; set; }
		public string RegionTrainingProvider { get; set; }
		public string CountryIdTrainingProvider { get; set; }

		public bool TrainingOutsideBC { get; set; }
		public string BusinessCase { get; set; }
		public int? BusinessCaseDocumentId { get; set; }
		public Attachments.AttachmentViewModel BusinessCaseDocument { get; set; }

		public int? CourseOutlineDocumentId { get; set; }
		public Attachments.AttachmentViewModel CourseOutlineDocument { get; set; }

		public int? ProofOfQualificationsDocumentId { get; set; }
		public Attachments.AttachmentViewModel ProofOfQualificationsDocument { get; set; }

		public int[] SelectedDeliveryMethodIds { get; set; }

		public int? ProofOfInstructorQualifications { get; set; }
		public int? CourseOutline { get; set; }
		#endregion

		#region Constructors
		public SkillsTrainingProviderViewModel() { }

		public SkillsTrainingProviderViewModel(TrainingProvider trainingProvider, IPrincipal user, IStaticDataService staticDataService)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = trainingProvider.Id;
			this.RowVersion = trainingProvider.RowVersion != null ? Convert.ToBase64String(trainingProvider.RowVersion) : null;

			this.IsValidated = trainingProvider.TrainingProviderInventoryId.HasValue;
			this.CanValidate = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.ValidateTrainingProvider);
			this.CanEdit = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.EditTrainingProvider);
			this.CanRemove = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.AddRemoveTrainingProvider);
			this.CanRecommendChangeRequest = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.EditTrainingProvider);

			this.EligibleCostId = trainingProvider.EligibleCostId;
			this.TrainingProviderInventoryId = trainingProvider.TrainingProviderInventoryId;
			this.TrainingProgramId = trainingProvider.TrainingProgram?.Id ?? throw new ArgumentException("A skills training component provider must be associated with a program.", nameof(trainingProvider));
			this.Name = trainingProvider.Name;
			this.TrainingProviderState = trainingProvider.TrainingProviderState;
			this.TrainingProviderTypeId = trainingProvider.TrainingProviderTypeId;
			this.PrivateSectorValidationType = trainingProvider.TrainingProviderType.PrivateSectorValidationType;
			this.ProofOfInstructorQualifications = trainingProvider.TrainingProviderType.ProofOfInstructorQualifications;
			this.CourseOutline = trainingProvider.TrainingProviderType.CourseOutline;

			this.ChangeRequestReason = trainingProvider.ChangeRequestReason;

			this.ContactEmail = trainingProvider.ContactEmail;
			this.ContactFirstName = trainingProvider.ContactFirstName;
			this.ContactLastName = trainingProvider.ContactLastName;
			this.ContactPhone = trainingProvider.ContactPhoneNumber;
			this.ContactPhoneAreaCode = trainingProvider.ContactPhoneNumber.GetPhoneAreaCode()?.ToString();
			this.ContactPhoneExchange = trainingProvider.ContactPhoneNumber.GetPhoneExchange()?.ToString();
			this.ContactPhoneNumber = trainingProvider.ContactPhoneNumber.GetPhoneNumber()?.ToString();
			this.ContactPhoneExtension = trainingProvider.ContactPhoneExtension;
			if (trainingProvider.TrainingAddress != null)
			{
				this.TrainingAddressId = trainingProvider.TrainingAddressId;
				this.AddressLine1 = trainingProvider.TrainingAddress.AddressLine1;
				this.AddressLine2 = trainingProvider.TrainingAddress.AddressLine2;
				this.City = trainingProvider.TrainingAddress.City;
				this.CountryId = trainingProvider.TrainingAddress.CountryId;
				this.RegionId = trainingProvider.TrainingAddress.RegionId;
				this.Region = staticDataService.GetRegion(this.CountryId, this.RegionId).Name;
				this.PostalCode = trainingProvider.TrainingAddress.PostalCode;
				this.ZipCode = trainingProvider.TrainingAddress.PostalCode;
			}
			if (trainingProvider.TrainingProviderAddress != null)
			{
				this.TrainingProviderAddressId = trainingProvider.TrainingProviderAddressId;
				this.AddressLine1TrainingProvider = trainingProvider.TrainingProviderAddress.AddressLine1;
				this.AddressLine2TrainingProvider = trainingProvider.TrainingProviderAddress.AddressLine2;
				this.CityTrainingProvider = trainingProvider.TrainingProviderAddress.City;
				this.CountryIdTrainingProvider = trainingProvider.TrainingProviderAddress.CountryId;
				this.RegionIdTrainingProvider = trainingProvider.TrainingProviderAddress.RegionId;
				this.RegionTrainingProvider = staticDataService.GetRegion(this.CountryIdTrainingProvider, this.RegionIdTrainingProvider).Name;
				this.PostalCodeTrainingProvider = trainingProvider.TrainingProviderAddress.PostalCode;
				this.ZipCodeTrainingProvider = trainingProvider.TrainingProviderAddress.PostalCode;
			}
			this.TrainingOutsideBC = trainingProvider.TrainingOutsideBC;
			this.BusinessCase = trainingProvider.BusinessCase;
			this.BusinessCaseDocumentId = trainingProvider.BusinessCaseDocumentId;
			this.BusinessCaseDocument = trainingProvider.BusinessCaseDocumentId.HasValue ? new Attachments.AttachmentViewModel(trainingProvider.BusinessCaseDocument) : null;

			this.CourseOutlineDocumentId = trainingProvider.CourseOutlineDocumentId;
			this.CourseOutlineDocument = trainingProvider.CourseOutlineDocumentId.HasValue ? new Attachments.AttachmentViewModel(trainingProvider.CourseOutlineDocument) : null;

			this.ProofOfQualificationsDocumentId = trainingProvider.ProofOfQualificationsDocumentId;
			this.ProofOfQualificationsDocument = trainingProvider.ProofOfQualificationsDocumentId.HasValue ? new Attachments.AttachmentViewModel(trainingProvider.ProofOfQualificationsDocument) : null;
			this.SelectedDeliveryMethodIds = trainingProvider.TrainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
		
		}
		#endregion
	}
}