using CJG.Core.Entities;
using CJG.Core.Entities.Attributes;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class TrainingProviderViewModel : BaseViewModel
	{
		#region Properties
		public int EligibleExpenseTypeId { get; set; }

		public string ProviderQualificationsHiddenClass { get; set; }

		public IEnumerable<TrainingProviderType> TrainingProviderTypes { get; set; }

		public bool AllowDocumentReplacement { get; set; }

		public string BusinessCaseHiddenClass { get; set; }

		#region Entity
		public int Id { get; set; }

		[Required(ErrorMessage = "Training Provider Name is required"), MaxLength(500)]
		public string Name { get; set; }

		public int GrantApplicationId { get; set; }

		public int? TrainingProgramId { get; set; }

		[Required(ErrorMessage = "Please select a value")]
		public int TrainingProviderTypeId { get; set; }

		public int? TrainingAddressId { get; set; }

		[Required]
		//[ConvertMap(nameof(TrainingAddress), typeof(ApplicationAddressTypeConverter))]
		public ApplicationAddressViewModel TrainingAddress { get; set; }

		[NotMapped]
		public TrainingProviderStates TrainingProviderState { get; set; } = TrainingProviderStates.Incomplete;

		public int? TrainingProviderInventoryId { get; set; }

		[NotMapped]
		public TrainingProviderInventory TrainingProviderInventory { get; set; }

		[Required(ErrorMessage = "Please answer the question 'Does the training take place outside of BC?'")]
		public bool? TrainingOutsideBC { get; set; }

		public string BusinessCase { get; set; }

		[NotMapped]
		public int? BusinessCaseDocumentId { get; set; }

		[NotMapped]
		public Attachment BusinessCaseDocument { get; set; }

		[NotMapped]
		public int? ProofOfQualificationsDocumentId { get; set; }

		[NotMapped]
		public Attachment ProofOfQualificationsDocument { get; set; }

		[NotMapped]
		public int? CourseOutlineDocumentId { get; set; }

		[NotMapped]
		public Attachment CourseOutlineDocument { get; set; }

		[Required(ErrorMessage = "Contact First Name is required")]
		[RegularExpression("^[a-zA-Z -]*$", ErrorMessage = "Contact First Name Invalid Format")]
		[MaxLength(128)]
		public string ContactFirstName { get; set; }

		[Required(ErrorMessage = "Contact Last Name is required")]
		[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Contact Last Name Invalid Format")]
		[MaxLength(128)]
		public string ContactLastName { get; set; }

		[Required(ErrorMessage = "Contact Email is required")]
		[RegularExpression(@"^[\s*\d+\s*]*[a-zA-Z0-9_\.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}[\s*\d+\s*]*$", ErrorMessage = "Email Address is not valid")]
		[MaxLength(500)]
		public string ContactEmail { get; set; }

		[NotMapped]
		[Required(ErrorMessage = "Area code and phone number are required.")]
		[RegularExpression("\\d{10}", ErrorMessage = "Contact phone number must be a 10-digit number")]
		public String ContactPhone { get; set; }


		[NotMapped]
		//[RegularExpression("\\d{3}", ErrorMessage = "Area Code must be a 3-digit number")]
		public string ContactPhoneAreaCode { get; set; }


		[NotMapped]
		//[RegularExpression("\\d{3}", ErrorMessage = "Exchange must be a 3-digit number")]
		public string ContactPhoneExchange { get; set; }


		[ConvertMap(nameof(ContactPhoneNumber), new[] { nameof(ContactPhoneAreaCode), nameof(ContactPhoneExchange), nameof(ContactPhoneNumber) }, "({0}) {1}-{2}")]
		//[RegularExpression("\\d{4}$", ErrorMessage = "Phone number must be a 4-digit number")]
		public string ContactPhoneNumber { get; set; }

		[RegularExpression("^[0-9]*$", ErrorMessage = "Value must be numeric")]
		[Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
		public string ContactPhoneExtension { get; set; }

		// should move this to BaseViewModel
		private byte[] rowVersion=null;
		public byte[] RowVersion
		{
			get
			{
				if (rowVersion != null)
					return rowVersion;
				else if (!string.IsNullOrEmpty(RowVersionString))
					return Convert.FromBase64String(RowVersionString);
				else
					return null;
			}
			set
			{
				if(value!=null)
				{
					rowVersion = value;
					RowVersionString = Convert.ToBase64String(value);
				}
				
			}
		}

		[NotMapped]
		public string RowVersionString { get; set; }
		
		[NotMapped]
		public string AllowFileAttachmentExtensions { get; set; }

		#endregion
		#endregion

		#region Constructors
		public TrainingProviderViewModel()
		{
			ProviderQualificationsHiddenClass = "form__section--hidden";
			BusinessCaseHiddenClass = "form__section--hidden";
			AllowDocumentReplacement = false;
		}

		public TrainingProviderViewModel(TrainingProvider trainingProvider, string allowFileAttachmentExtensions) : this()
		{
			if (trainingProvider == null)
				throw new ArgumentNullException(nameof(trainingProvider));

			this.Id = trainingProvider.Id;
			this.Name = trainingProvider.Name;
			this.GrantApplicationId = trainingProvider.GrantApplicationId ?? trainingProvider.TrainingPrograms.FirstOrDefault().GrantApplicationId;
			this.TrainingProgramId = trainingProvider.TrainingPrograms.FirstOrDefault()?.Id;
			this.TrainingProviderTypeId = trainingProvider.TrainingProviderTypeId;
			this.TrainingAddressId = trainingProvider.TrainingAddressId;
			this.TrainingAddress = trainingProvider.TrainingAddress != null ? new ApplicationAddressViewModel(trainingProvider.TrainingAddress) : new ApplicationAddressViewModel() { RegionId = "BC", CountryId = "CA", IsCanadianAddress = true };
			this.TrainingProviderState = trainingProvider.Id == 0 ? TrainingProviderStates.Incomplete : trainingProvider.TrainingProviderState;
			this.TrainingProviderInventoryId = trainingProvider.TrainingProviderInventoryId;
			this.TrainingProviderInventory = trainingProvider.TrainingProviderInventory;
			this.TrainingOutsideBC = trainingProvider.Id == 0 ? null : (bool?)trainingProvider.TrainingOutsideBC;
			this.BusinessCase = trainingProvider.BusinessCase;
			this.BusinessCaseDocumentId = trainingProvider.BusinessCaseDocumentId;
			this.BusinessCaseDocument = trainingProvider.BusinessCaseDocument;
			this.ProofOfQualificationsDocumentId = trainingProvider.ProofOfQualificationsDocumentId;
			this.ProofOfQualificationsDocument = trainingProvider.ProofOfQualificationsDocument;
			this.CourseOutlineDocumentId = trainingProvider.CourseOutlineDocumentId;
			this.CourseOutlineDocument = trainingProvider.CourseOutlineDocument;
			this.ContactFirstName = trainingProvider.ContactFirstName;
			this.ContactLastName = trainingProvider.ContactLastName;
			this.ContactEmail = trainingProvider.ContactEmail;
			this.ContactPhoneAreaCode = trainingProvider.ContactPhoneNumber?.GetPhoneAreaCode()?.ToString();
			this.ContactPhoneExchange = trainingProvider.ContactPhoneNumber?.GetPhoneExchange()?.ToString();
			this.ContactPhoneNumber = trainingProvider.ContactPhoneNumber?.GetPhoneNumber()?.ToString();
			this.ContactPhoneExtension = trainingProvider.ContactPhoneExtension;
			this.AllowFileAttachmentExtensions = allowFileAttachmentExtensions;
			this.ContactPhone = string.Format("({0}) {1}-{2}", this.ContactPhoneAreaCode, this.ContactPhoneExchange, this.ContactPhoneNumber);
			RowVersion = trainingProvider.RowVersion;
		}
		#endregion

		#region Methods
		public static implicit operator TrainingProvider(TrainingProviderViewModel model)
		{
			if (model == null)
				return null;

			var trainingProvider = new TrainingProvider();
			trainingProvider.Id = model.Id;
			trainingProvider.Name = model.Name;
			trainingProvider.GrantApplicationId = model.GrantApplicationId;
			trainingProvider.TrainingProviderTypeId = model.TrainingProviderTypeId;
			trainingProvider.TrainingAddressId = model.TrainingAddressId;
			trainingProvider.TrainingAddress = model.TrainingAddress;
			trainingProvider.TrainingProviderState = model.TrainingProviderState;
			trainingProvider.TrainingProviderInventoryId = model.TrainingProviderInventoryId;
			trainingProvider.TrainingProviderInventory = model.TrainingProviderInventory;
			trainingProvider.TrainingOutsideBC = model.TrainingOutsideBC.Value;
			trainingProvider.BusinessCase = model.BusinessCase;
			trainingProvider.BusinessCaseDocumentId = model.BusinessCaseDocumentId;
			trainingProvider.BusinessCaseDocument = model.BusinessCaseDocument;
			trainingProvider.ProofOfQualificationsDocumentId = model.ProofOfQualificationsDocumentId;
			trainingProvider.ProofOfQualificationsDocument = model.ProofOfQualificationsDocument;
			trainingProvider.CourseOutlineDocumentId = model.CourseOutlineDocumentId;
			trainingProvider.CourseOutlineDocument = model.CourseOutlineDocument;
			trainingProvider.ContactFirstName = model.ContactFirstName;
			trainingProvider.ContactLastName = model.ContactLastName;
			trainingProvider.ContactEmail = model.ContactEmail.Trim();
			trainingProvider.ContactPhoneNumber = $"{model.ContactPhoneAreaCode}{model.ContactPhoneExchange}{model.ContactPhoneNumber}".FormatPhoneNumber();
			trainingProvider.ContactPhoneExtension = model.ContactPhoneExtension;
			trainingProvider.RowVersion = model.RowVersion;
			return trainingProvider;
		}
		#endregion
	}
}
