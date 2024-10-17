using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Int.Models.SkillsTraining
{
	public class UpdateSkillsTrainingProviderViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public int? EligibleCostId { get; set; }

		public string Name { get; set; }
		public string ChangeRequestReason { get; set; }
		public int TrainingProgramId { get; set; }

		[Required(ErrorMessage = "Provider type is required")]
		public int? TrainingProviderTypeId { get; set; }

		public TrainingProviderStates TrainingProviderState { get; set; }
		public int? TrainingProviderInventoryId { get; set; }

		[Required(ErrorMessage = "The Contact First Name is required")]
		[RegularExpression("^[a-zA-Z -]*$", ErrorMessage = "Contact First Name Invalid Format")]
		public string ContactFirstName { get; set; }

		[Required(ErrorMessage = "The Contact Last Name is required")]
		[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Contact Last Name Invalid Format")]
		public string ContactLastName { get; set; }

		[Required(ErrorMessage = "The Contact email is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		[RegularExpression(@"^[\s*\d+\s*]*[a-zA-Z0-9_\.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}[\s*\d+\s*]*$", ErrorMessage = "Email Address is not valid")]
		public string ContactEmail { get; set; }

		[Required(ErrorMessage = "The Contact phone number is required")]
		[RegularExpression("^\\D*(\\d\\D*){10}", ErrorMessage = "Contact phone number must be 10-digit number")]
		public string ContactPhone { get; set; }
		public string ContactPhoneAreaCode { get; set; }
		public string ContactPhoneExchange { get; set; }
		public string ContactPhoneNumber { get; set; }
		public string ContactPhoneExtension { get; set; }

		public int? TrainingAddressId { get; set; }

		public int? TrainingProviderAddressId { get; set; }

		[Required(ErrorMessage = "Address Line 1 is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine1 { get; set; }

		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine2 { get; set; }

		[Required(ErrorMessage = "City is required")]
		[RegularExpression(Constants.CityNameValidationRegEx, ErrorMessage = "City must be entered in English without accents or special characters")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string City { get; set; }

		[Required(ErrorMessage = "Address Line 1 is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine1TrainingProvider { get; set; }

		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine2TrainingProvider { get; set; }

		[Required(ErrorMessage = "Training Provider City is required")]
		[RegularExpression(Constants.CityNameValidationRegEx, ErrorMessage = "Training Provider City must be entered in English without accents or special characters")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string CityTrainingProvider { get; set; }

		[Required(ErrorMessage = "Postal Code is required")]
		[RegularExpression(Constants.PostalCodeValidationRegEx, ErrorMessage = "PostalCode Invalid Format")]
		public string PostalCode { get; set; }

		[StringLength(10, ErrorMessage = "The field must be a string with a maximum length of 10")]
		[Required(ErrorMessage = "Zip Code is required for international addresses")]
		public string ZipCode { get; set; }

		[Required(ErrorMessage = "Province is required")]
		public string RegionId { get; set; }

		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		[Required(ErrorMessage = "State / Region is required for international addresses")]
		[RegularExpression(Constants.ProvinceNameValidationRegEx, ErrorMessage = "State / Region must be entered in English without accents or special characters")]
		public string Region { get; set; }

		[Required(ErrorMessage = "Country is required for international addresses")]
		public string CountryId { get; set; }

		public string PostalCodeTrainingProvider { get; set; }

		[StringLength(10, ErrorMessage = "The field must be a string with a maximum length of 10")]
		[Required(ErrorMessage = "Zip Code of the Training Provider is required for international addresses")]
		public string ZipCodeTrainingProvider { get; set; }

		[Required(ErrorMessage = "Province of the Training Provider is required")]
		public string RegionIdTrainingProvider { get; set; }

		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		[Required(ErrorMessage = "State / Region of the Training Provider is required for international addresses")]
		[RegularExpression(Constants.ProvinceNameValidationRegEx, ErrorMessage = "Training Provider State / Region must be entered in English without accents or special characters")]
		public string RegionTrainingProvider { get; set; }

		[Required(ErrorMessage = "Country of the Training Provider is required for international addresses")]
		public string CountryIdTrainingProvider { get; set; }

		public bool TrainingOutsideBC { get; set; }
		public string BusinessCase { get; set; }
		public int? BusinessCaseDocumentId { get; set; }
		public Attachments.UpdateAttachmentViewModel BusinessCaseDocument { get; set; }

		public int? CourseOutlineDocumentId { get; set; }
		public Attachments.UpdateAttachmentViewModel CourseOutlineDocument { get; set; }

		public int? ProofOfQualificationsDocumentId { get; set; }
		public Attachments.UpdateAttachmentViewModel ProofOfQualificationsDocument { get; set; }
		public int ProofOfInstructorQualifications { get; set; }
		public int CourseOutline { get; set; }
		#endregion

		#region Constructors
		public UpdateSkillsTrainingProviderViewModel() { }
		
		#endregion
	}
}