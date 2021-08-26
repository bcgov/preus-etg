using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.TrainingProviders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Constants = CJG.Core.Entities.Constants;

namespace CJG.Web.External.Areas.Ext.Models.TrainingProviders
{
	public class TrainingProviderViewModel : BaseViewModel
	{
		#region Properties
		public string GrantApplicationRowVersion { get; set; }
		public string RowVersion { get; set; }
		public int? EligibleCostId { get; set; }

		[Required(ErrorMessage = "Training Provider Name is required"), MaxLength(500)]
		public string Name { get; set; }
		public string ChangeRequestReason { get; set; }
		public int? GrantApplicationId { get; set; }
		public int? TrainingProgramId { get; set; }

		[Required(ErrorMessage = "Training Provider Type is required"), Range(1, int.MaxValue, ErrorMessage = "Training Provider Type is required")]
		public int? TrainingProviderTypeId { get; set; }

		public int TrainingAddressId { get; set; }
		public TrainingProviderStates TrainingProviderState { get; set; } = TrainingProviderStates.Incomplete;

		public int? TrainingProviderInventoryId { get; set; }
		public TrainingProviderInventory TrainingProviderInventory { get; set; }

		[Required(ErrorMessage = "Contact First Name is required")]
		[RegularExpression("^[a-zA-Z -]*$", ErrorMessage = "Invalid Format")]
		[MaxLength(128)]
		public string ContactFirstName { get; set; }

		[Required(ErrorMessage = "Contact Last Name is required")]
		[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Invalid Format")]
		[MaxLength(128)]
		public string ContactLastName { get; set; }

		[Required(ErrorMessage = "Email Address is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		[RegularExpression(@"^[\s*\d+\s*]*[a-zA-Z0-9_\.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}[\s*\d+\s*]*$", ErrorMessage = "Email Address is not valid")]
		public string ContactEmail { get; set; }

		[Required(ErrorMessage = "Contact phone number is required")]
		[RegularExpression("^\\D*(\\d\\D*){10}", ErrorMessage = "Contact phone number must be 10-digit number")]
		public string ContactPhone { get; set; }
		public string ContactPhoneAreaCode { get; set; }
		public string ContactPhoneExchange { get; set; }
		public string ContactPhoneNumber { get; set; }
		public string ContactPhoneExtension { get; set; }

		[Required(ErrorMessage = "Address Line 1 is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine1 { get; set; }

		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string AddressLine2 { get; set; }

		[Required(ErrorMessage = "City is required")]
		[RegularExpression("^([a-zA-Z\u0080-\u024F]+(?:. |-| |'))*[a-zA-Z\u0080-\u024F]*$", ErrorMessage = "Invalid Format")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		public string City { get; set; }

		private string _postalCode;
		[Required(ErrorMessage = "Postal Code is required")]
		[RegularExpression(CJG.Core.Entities.Constants.PostalCodeValidationRegEx, ErrorMessage = "Invalid Format")]
		public string PostalCode
		{
			get
			{
				var postalcode = _postalCode;
				if (!string.IsNullOrEmpty(postalcode))
				{
					postalcode = postalcode.ToUpper().Replace(" ", "");
					if (postalcode.Length == 6) postalcode = postalcode.Insert(postalcode.Length - 3, " ");
					else postalcode = _postalCode; //no change
				}
				return postalcode;
			}
			set
			{
				_postalCode = value;
			}
		}

		[StringLength(10, ErrorMessage = "The field must be a string with a maximum length of 10")]
		[Required(ErrorMessage = "Zip Code is required for international addresses")]
		[DisplayName("Zip Code")]
		public string OtherZipCode { get; set; }

		[Required(ErrorMessage = "Province is required")]
		public string RegionId { get; set; } = "BC";
		public string Region { get; set; } = "British Columbia";

		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		[Required(ErrorMessage = "State / Region is required for international addresses")]
		[RegularExpression("^[a-zA-Z -]*$", ErrorMessage = "Invalid Format")]
		[DisplayName("State / Region")]
		public string OtherRegion { get; set; }

		[Required(ErrorMessage = "Country is required for international addresses")]
		[DefaultValue(Constants.CanadaCountryId)]
		[DisplayName("Country")]
		public string CountryId { get; set; } = Constants.CanadaCountryId;
		public string Country { get; set; } = "Canada";

		public bool IsCanadianAddress { get; set; } = true;

		public int? ParentProviderId { get; set; }
		public bool IsServiceProvider { get; set; }

		[Required(ErrorMessage = "Please answer the question 'Does the training take place outside of BC?'")]
		public bool? TrainingOutsideBC { get; set; }
		public string BusinessCase { get; set; }
		public TrainingProviderPrivateSectorValidationTypes PrivateSectorValidationType { get; set; }
		public TrainingProviderAttachmentViewModel ProofOfQualificationsDocument { get; set; } = new TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes.ProofOfQualifications);
		public TrainingProviderAttachmentViewModel BusinessCaseDocument { get; set; } = new TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes.BusinessCase);
		public TrainingProviderAttachmentViewModel CourseOutlineDocument { get; set; } = new TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes.CourseOutline);
		#endregion

		#region Constructors
		public TrainingProviderViewModel() { }

		public TrainingProviderViewModel(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));

			this.Id = trainingProvider.Id;
			this.RowVersion = trainingProvider.RowVersion != null ? Convert.ToBase64String(trainingProvider.RowVersion) : null;
			this.GrantApplicationId = trainingProvider.GrantApplicationId;
			var grantApplication = trainingProvider.GetGrantApplication();
			this.GrantApplicationRowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.TrainingProgramId = trainingProvider.TrainingPrograms.FirstOrDefault()?.Id;

			this.EligibleCostId = trainingProvider.EligibleCostId;
			this.Name = trainingProvider.Name;
			this.ChangeRequestReason = trainingProvider.ChangeRequestReason;
			this.TrainingProviderState = trainingProvider.TrainingProviderState;
			this.TrainingProviderTypeId = trainingProvider.Id == 0 ? (int?)null : trainingProvider.TrainingProviderTypeId;
			this.ContactPhone = trainingProvider.ContactPhoneNumber;
			this.ContactPhoneAreaCode = trainingProvider.ContactPhoneNumber.GetPhoneAreaCode();
			this.ContactPhoneExchange = trainingProvider.ContactPhoneNumber.GetPhoneExchange();
			this.ContactPhoneNumber = trainingProvider.ContactPhoneNumber.GetPhoneNumber();
			this.ContactPhoneExtension = trainingProvider.ContactPhoneExtension;
			this.ContactEmail = trainingProvider.ContactEmail;
			this.ContactFirstName = trainingProvider.ContactFirstName;
			this.ContactLastName = trainingProvider.ContactLastName;
			this.TrainingProviderInventoryId = trainingProvider.TrainingProviderInventoryId;
			this.TrainingProviderInventory = trainingProvider.TrainingProviderInventory;
			this.TrainingAddressId = trainingProvider.TrainingAddressId;
			if (trainingProvider.TrainingAddress != null)
			{
				var address = trainingProvider.TrainingAddress;
				this.AddressLine1 = address.AddressLine1;
				this.AddressLine2 = address.AddressLine2;
				this.City = address.City;
				this.PostalCode = address.PostalCode;
				this.RegionId = address.RegionId;
				this.Region = address.Region?.Name;
				this.CountryId = address.CountryId;
				this.Country = address.Country?.Name;

				this.IsCanadianAddress = address.CountryId == Constants.CanadaCountryId;

				if (!IsCanadianAddress)
				{
					this.OtherRegion = Region;
					this.OtherZipCode = PostalCode;
				}
			}

			this.ParentProviderId = trainingProvider.OriginalTrainingProviderId;
			this.TrainingOutsideBC = trainingProvider.Id == 0 ? (bool?)null : trainingProvider.TrainingOutsideBC;
			this.BusinessCase = trainingProvider.BusinessCase;
			this.PrivateSectorValidationType = trainingProvider.TrainingProviderType != null ? trainingProvider.TrainingProviderType.PrivateSectorValidationType : TrainingProviderPrivateSectorValidationTypes.Never;
			this.BusinessCaseDocument = new TrainingProviderAttachmentViewModel(trainingProvider.BusinessCaseDocument, this.Id, TrainingProviderAttachmentTypes.BusinessCase, this.RowVersion);
			this.ProofOfQualificationsDocument = new TrainingProviderAttachmentViewModel(trainingProvider.ProofOfQualificationsDocument, this.Id, TrainingProviderAttachmentTypes.ProofOfQualifications, this.RowVersion);
			this.CourseOutlineDocument = new TrainingProviderAttachmentViewModel(trainingProvider.CourseOutlineDocument, this.Id, TrainingProviderAttachmentTypes.CourseOutline, this.RowVersion);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Map the model to the appropriate training provider.
		/// Also Add/Update the attachments associated with this training provider.
		/// </summary>
		/// <param name="grantApplicationService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="trainingProviderService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="applicationAddressService"></param>
		/// <param name="staticDataService"></param>
		/// <param name="user"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		public TrainingProvider MapProperties(
			IGrantApplicationService grantApplicationService,
			ITrainingProgramService trainingProgramService,
			ITrainingProviderService trainingProviderService,
			IAttachmentService attachmentService,
			IApplicationAddressService applicationAddressService,
			IStaticDataService staticDataService,
			IPrincipal user,
			HttpPostedFileBase[] files = null)
		{
			if (grantApplicationService == null) throw new ArgumentNullException(nameof(grantApplicationService));
			if (trainingProgramService == null) throw new ArgumentNullException(nameof(trainingProgramService));
			if (trainingProviderService == null) throw new ArgumentNullException(nameof(trainingProviderService));
			if (attachmentService == null) throw new ArgumentNullException(nameof(attachmentService));
			if (applicationAddressService == null) throw new ArgumentNullException(nameof(applicationAddressService));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));
			if (user == null) throw new ArgumentNullException(nameof(user));

			var create = this.Id == 0;
			var grantApplication = this.GrantApplicationId.HasValue ? grantApplicationService.Get(this.GrantApplicationId.Value) : null;
			TrainingProvider trainingProvider;

			if (create)
			{
				if (this.ParentProviderId > 0)
				{
					// This is a change request if it's a new service provider.
					var parentProvider = trainingProviderService.Get(this.ParentProviderId.Value);
					trainingProvider = new TrainingProvider(parentProvider.OriginalTrainingProvider ?? parentProvider);

					parentProvider.TrainingProgram.TrainingProviders.Add(trainingProvider);
				}
				else if (this.TrainingProgramId.HasValue)
				{
					var trainingProgram = trainingProgramService.Get(this.TrainingProgramId.Value);
					trainingProvider = new TrainingProvider(trainingProgram);
				}
				else if (grantApplication?.TrainingPrograms.Any() ?? false)
				{
					var trainingProgram = grantApplication.TrainingPrograms.First();
					trainingProvider = new TrainingProvider(trainingProgram);
				}
				else
				{
					trainingProvider = new TrainingProvider(grantApplication);
				}
			}
			else
			{
				trainingProvider = trainingProviderService.Get(this.Id);
				grantApplication = trainingProvider.GetGrantApplication();
			}

			if (!create)
				trainingProvider.RowVersion = Convert.FromBase64String(this.RowVersion);

			trainingProvider.Name = this.Name;
			trainingProvider.ChangeRequestReason = this.ChangeRequestReason;
			trainingProvider.TrainingProviderTypeId = this.TrainingProviderTypeId.Value;

			if (!trainingProvider.TrainingPrograms.Any() && trainingProvider.TrainingProviderState == TrainingProviderStates.Incomplete)
			{
				trainingProvider.TrainingProviderState = TrainingProviderStates.Complete;
			}
			if (trainingProvider.TrainingPrograms.Any() && trainingProvider.TrainingProviderState == TrainingProviderStates.Incomplete)
			{
				var associatedTrainingProgram = trainingProvider.TrainingPrograms.First();
				trainingProvider.TrainingProviderState = associatedTrainingProgram.TrainingProvider?.Id == trainingProvider.Id ? TrainingProviderStates.Complete : TrainingProviderStates.Requested;
			}

			trainingProvider.ContactPhoneNumber = this.ContactPhone;
			trainingProvider.ContactPhoneExtension = this.ContactPhoneExtension;
			trainingProvider.ContactEmail = this.ContactEmail;
			trainingProvider.ContactFirstName = this.ContactFirstName;
			trainingProvider.ContactLastName = this.ContactLastName;
			if (trainingProvider.TrainingAddress == null)
				trainingProvider.TrainingAddress = new ApplicationAddress();

			trainingProvider.TrainingAddress.AddressLine1 = this.AddressLine1;
			trainingProvider.TrainingAddress.AddressLine2 = this.AddressLine2;
			trainingProvider.TrainingAddress.City = this.City;
			if (this.IsCanadianAddress)
			{
				trainingProvider.TrainingAddress.RegionId = this.RegionId;
			}
			else
			{
				var region = applicationAddressService.VerifyOrCreateRegion(this.OtherRegion, this.CountryId);
				trainingProvider.TrainingAddress.RegionId = region.Id;
			}

			trainingProvider.TrainingAddress.PostalCode = this.IsCanadianAddress ? this.PostalCode : this.OtherZipCode;
			trainingProvider.TrainingAddress.CountryId = this.CountryId;

			trainingProvider.TrainingOutsideBC = this.TrainingOutsideBC.Value;

			if (!trainingProvider.TrainingOutsideBC)
			{
				this.BusinessCaseDocument.Id = 0;
				this.BusinessCase = "";
			}

			var trainingProviderType = staticDataService.GetTrainingProviderType(trainingProvider.TrainingProviderTypeId);
			if (trainingProviderType.PrivateSectorValidationType == TrainingProviderPrivateSectorValidationTypes.Never)
			{
				this.CourseOutlineDocument.Id = 0;
				this.ProofOfQualificationsDocument.Id = 0;
			}

			trainingProvider.BusinessCase = this.BusinessCase;

			UpdateAttachments(trainingProvider, attachmentService, files);

			if ((grantApplication.ApplicationStateExternal == ApplicationStateExternal.ApplicationWithdrawn
				&& grantApplication.ApplicationStateInternal == ApplicationStateInternal.ApplicationWithdrawn)
				|| grantApplication.ApplicationStateExternal == ApplicationStateExternal.NotAccepted)
			{
				grantApplication.RowVersion = Convert.FromBase64String(this.GrantApplicationRowVersion);

				// This GrantApplication was withdrawn or returned as not accepted, and can now be editted for resubmission
				grantApplication.ApplicationStateInternal = ApplicationStateInternal.Draft;
				grantApplication.ApplicationStateExternal = ApplicationStateExternal.Incomplete;

				// remove the filenumber
				grantApplication.FileNumber = string.Empty;

				// also remove the assigned assessor
				grantApplication.Assessor = null;
			}

			return trainingProvider;
		}

		/// <summary>
		/// Add/update/remove attachments associated with the specific properties of the training provider.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <param name="attachmentService"></param>
		/// <param name="files"></param>
		private void UpdateAttachments(TrainingProvider trainingProvider, IAttachmentService attachmentService, HttpPostedFileBase[] files = null)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			if (attachmentService == null) throw new ArgumentNullException(nameof(attachmentService));

			// If files were provided add/update the attachments for the specified properties.
			if (files != null && files.Any())
			{
				if (this.CourseOutlineDocument != null && this.CourseOutlineDocument.Index.HasValue && files.Count() > this.CourseOutlineDocument.Index)
				{
					var attachment = files[this.CourseOutlineDocument.Index.Value].UploadFile(this.CourseOutlineDocument.Description, this.CourseOutlineDocument.FileName);
					attachment.Id = this.CourseOutlineDocument.Id;
					if (this.CourseOutlineDocument.Id == 0)
					{
						trainingProvider.CourseOutlineDocument = attachment;
						attachmentService.Add(attachment);
						this.CourseOutlineDocument.Id = attachment.Id;
					}
					else
					{
						attachment.RowVersion = Convert.FromBase64String(this.CourseOutlineDocument.RowVersion);
						attachmentService.Update(attachment);
					}
				}
				if (this.ProofOfQualificationsDocument != null && this.ProofOfQualificationsDocument.Index.HasValue && files.Count() > this.ProofOfQualificationsDocument.Index)
				{
					var attachment = files[this.ProofOfQualificationsDocument.Index.Value].UploadFile(this.ProofOfQualificationsDocument.Description, this.ProofOfQualificationsDocument.FileName);
					attachment.Id = this.ProofOfQualificationsDocument.Id;
					if (this.ProofOfQualificationsDocument.Id == 0)
					{
						trainingProvider.ProofOfQualificationsDocument = attachment;
						attachmentService.Add(attachment);
						this.ProofOfQualificationsDocument.Id = attachment.Id;
					}
					else
					{
						attachment.RowVersion = Convert.FromBase64String(this.ProofOfQualificationsDocument.RowVersion);
						attachmentService.Update(attachment);
					}
				}
				if (this.BusinessCaseDocument != null && this.BusinessCaseDocument.Index.HasValue && files.Count() > this.BusinessCaseDocument.Index)
				{
					var attachment = files[this.BusinessCaseDocument.Index.Value].UploadFile(this.BusinessCaseDocument.Description, this.BusinessCaseDocument.FileName);
					attachment.Id = this.BusinessCaseDocument.Id;
					if (this.BusinessCaseDocument.Id == 0)
					{
						trainingProvider.BusinessCaseDocument = attachment;
						attachmentService.Add(attachment);
						this.BusinessCaseDocument.Id = attachment.Id;
					}
					else
					{
						attachment.RowVersion = Convert.FromBase64String(this.BusinessCaseDocument.RowVersion);
						attachmentService.Update(attachment);
					}
				}
			}

			if (trainingProvider.BusinessCaseDocumentId.HasValue && trainingProvider.BusinessCaseDocumentId != this.BusinessCaseDocument?.Id)
			{
				// Remove the prior attachment because it has been replaced.
				var attachment = attachmentService.Get(trainingProvider.BusinessCaseDocumentId.Value);
				trainingProvider.BusinessCaseDocumentId = null;
				attachmentService.Remove(attachment);
			}

			if (trainingProvider.ProofOfQualificationsDocumentId.HasValue && trainingProvider.ProofOfQualificationsDocumentId != this.ProofOfQualificationsDocument?.Id)
			{
				// Remove the prior attachment because it has been replaced.
				var attachment = attachmentService.Get(trainingProvider.ProofOfQualificationsDocumentId.Value);
				trainingProvider.ProofOfQualificationsDocumentId = null;
				attachmentService.Remove(attachment);
			}

			if (trainingProvider.CourseOutlineDocumentId.HasValue && trainingProvider.CourseOutlineDocumentId != this.CourseOutlineDocument?.Id)
			{
				// Remove the prior attachment because it has been replaced.
				var attachment = attachmentService.Get(trainingProvider.CourseOutlineDocumentId.Value);
				trainingProvider.CourseOutlineDocumentId = null;
				attachmentService.Remove(attachment);
			}

			trainingProvider.BusinessCaseDocumentId = trainingProvider.BusinessCaseDocument?.Id;
			trainingProvider.ProofOfQualificationsDocumentId = trainingProvider.ProofOfQualificationsDocument?.Id;
			trainingProvider.CourseOutlineDocumentId = trainingProvider.CourseOutlineDocument?.Id;
		}
		#endregion
	}
}