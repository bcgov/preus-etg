using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared.TrainingProviders;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CJG.Web.External.Models.Shared.SkillsTrainings
{
	public class TrainingProviderDetailsViewModel : BaseProviderViewModel
	{
		#region Properties
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
		public TrainingProviderDetailsViewModel()
		{
		}

		public TrainingProviderDetailsViewModel(TrainingProvider trainingProvider) : base(trainingProvider)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));

			this.ParentProviderId = trainingProvider.OriginalTrainingProviderId;
			this.TrainingProgramId = trainingProvider.TrainingProgram?.Id;
			this.TrainingOutsideBC = trainingProvider.TrainingOutsideBC;
			this.BusinessCase = trainingProvider.BusinessCase;
			this.PrivateSectorValidationType = trainingProvider.TrainingProviderType != null ?
				trainingProvider.TrainingProviderType.PrivateSectorValidationType :
				TrainingProviderPrivateSectorValidationTypes.Never;
			this.BusinessCaseDocument = new TrainingProviderAttachmentViewModel(trainingProvider.BusinessCaseDocument, this.Id, TrainingProviderAttachmentTypes.BusinessCase, this.RowVersion);
			this.ProofOfQualificationsDocument = new TrainingProviderAttachmentViewModel(trainingProvider.ProofOfQualificationsDocument, this.Id, TrainingProviderAttachmentTypes.ProofOfQualifications, this.RowVersion);
			this.CourseOutlineDocument = new TrainingProviderAttachmentViewModel(trainingProvider.CourseOutlineDocument, this.Id, TrainingProviderAttachmentTypes.CourseOutline, this.RowVersion);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Add/Update the specified training provider in the datasource.
		/// Also Add/Update the attachments associated with this training provider.
		/// </summary>
		/// <param name="_grantApplicationService"></param>
		/// <param name="_trainingProgramService"></param>
		/// <param name="_trainingProviderService"></param>
		/// <param name="_attachmentService"></param>
		/// <param name="_applicationAddressService"></param>
		/// <param name="_staticDataService"></param>
		/// <param name="user"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		public TrainingProvider UpdateTrainingProvider(IGrantApplicationService _grantApplicationService,
											 ITrainingProgramService _trainingProgramService,
											 ITrainingProviderService _trainingProviderService,
											 IAttachmentService _attachmentService,
											 IApplicationAddressService _applicationAddressService,
											 IStaticDataService _staticDataService,
											 IPrincipal user,
											 HttpPostedFileBase[] files = null)
		{
			if (_grantApplicationService == null) throw new ArgumentNullException(nameof(_grantApplicationService));
			if (_trainingProgramService == null) throw new ArgumentNullException(nameof(_trainingProgramService));
			if (_trainingProviderService == null) throw new ArgumentNullException(nameof(_trainingProviderService));
			if (_attachmentService == null) throw new ArgumentNullException(nameof(_attachmentService));
			if (_applicationAddressService == null) throw new ArgumentNullException(nameof(_applicationAddressService));
			if (_staticDataService == null) throw new ArgumentNullException(nameof(_staticDataService));
			if (user == null) throw new ArgumentNullException(nameof(user));

			var grantApplication = _grantApplicationService.Get(this.GrantApplicationId);
			var create = this.Id == 0;
			TrainingProvider trainingProvider;

			if (create)
			{
				if (this.ParentProviderId > 0)
				{
					// This is a change request if it's a new service provider.
					var parentProvider = _trainingProviderService.Get(this.ParentProviderId.Value);
					trainingProvider = new TrainingProvider(parentProvider.OriginalTrainingProvider ?? parentProvider);

					parentProvider.TrainingProgram.TrainingProviders.Add(trainingProvider);
				}
				else if (this.TrainingProgramId > 0)
				{
					var trainingProgram = _trainingProgramService.Get(this.TrainingProgramId.Value);
					trainingProvider = new TrainingProvider(trainingProgram);
				}
				else if (grantApplication.TrainingPrograms.Any())
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
				trainingProvider = _trainingProviderService.Get(this.Id);
			}

			if (!create)
				trainingProvider.RowVersion = Convert.FromBase64String(this.RowVersion);

			trainingProvider.Name = this.Name;
			trainingProvider.ChangeRequestReason = this.ChangeRequestReason;
			trainingProvider.TrainingProviderTypeId = this.TrainingProviderTypeId.Value;
			if (this.TrainingProgramId > 0)
			{
				var associatedTrainingProgram = _trainingProgramService.Get(this.TrainingProgramId.Value);
				if (trainingProvider.TrainingProviderState == TrainingProviderStates.Incomplete)
					trainingProvider.TrainingProviderState = associatedTrainingProgram.TrainingProvider?.Id == trainingProvider.Id ? TrainingProviderStates.Complete : TrainingProviderStates.Requested;
			}
			else
			{
				if (!trainingProvider.TrainingPrograms.Any() && trainingProvider.TrainingProviderState == TrainingProviderStates.Incomplete)
				{
					trainingProvider.TrainingProviderState = TrainingProviderStates.Complete;
				}
				if (trainingProvider.TrainingPrograms.Any() && trainingProvider.TrainingProviderState == TrainingProviderStates.Incomplete)
				{
					var associatedTrainingProgram = trainingProvider.TrainingPrograms.First();
					trainingProvider.TrainingProviderState = associatedTrainingProgram.TrainingProvider?.Id == trainingProvider.Id ? TrainingProviderStates.Complete : TrainingProviderStates.Requested;
				}
			}

			if (create)
			{
				if (!user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.AddRemoveTrainingProvider))
					throw new NotAuthorizedException($"User does not have permission add the training provider in Grant Application '{grantApplication?.Id}'.");
			}
			else
			{
				if (!user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.EditTrainingProvider))
					throw new NotAuthorizedException($"User does not have permission edit the training provider in Grant Application '{grantApplication?.Id}'.");
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
				var region = _applicationAddressService.VerifyOrCreateRegion(this.OtherRegion, this.CountryId);
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

			trainingProvider.TrainingProviderType = _staticDataService.GetTrainingProviderType(trainingProvider.TrainingProviderTypeId);
			if (trainingProvider.TrainingProviderType.PrivateSectorValidationType == TrainingProviderPrivateSectorValidationTypes.Never)
			{
				this.CourseOutlineDocument.Id = 0;
				this.ProofOfQualificationsDocument.Id = 0;
			}

			trainingProvider.BusinessCase = this.BusinessCase;

			UpdateAttachment(trainingProvider, _attachmentService, files);

			if ((grantApplication.ApplicationStateExternal == ApplicationStateExternal.ApplicationWithdrawn
				&& grantApplication.ApplicationStateInternal == ApplicationStateInternal.ApplicationWithdrawn)
				|| grantApplication.ApplicationStateExternal == ApplicationStateExternal.NotAccepted)
			{
				// This GrantApplication was withdrawn or returned as not accepted, and can now be editted for resubmission
				grantApplication.ApplicationStateInternal = ApplicationStateInternal.Draft;
				grantApplication.ApplicationStateExternal = ApplicationStateExternal.Incomplete;

				// remove the filenumber
				grantApplication.FileNumber = string.Empty;

				// also remove the assigned assessor
				grantApplication.Assessor = null;
				_grantApplicationService.Update(grantApplication);
			}
			else if (create)
				_trainingProviderService.Add(trainingProvider);
			else
				_trainingProviderService.Update(trainingProvider);

			return trainingProvider;
		}

		public void UpdateAttachment(TrainingProvider trainingProvider, IAttachmentService _attachmentService, HttpPostedFileBase[] files = null)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			if (_attachmentService == null) throw new ArgumentNullException(nameof(_attachmentService));

			if (files != null && files.Any())
			{
				var isPrivateSector = trainingProvider.IsPrivateSectorType(AppDateTime.UtcNow);
				if (this.CourseOutlineDocument != null && this.CourseOutlineDocument.Index.HasValue && files.Count() > this.CourseOutlineDocument.Index && isPrivateSector)
				{
					var attachment = files[this.CourseOutlineDocument.Index.Value].UploadFile(this.CourseOutlineDocument.Description, this.CourseOutlineDocument.FileName);
					attachment.Id = this.CourseOutlineDocument.Id;
					if (this.CourseOutlineDocument.Id == 0)
					{
						trainingProvider.CourseOutlineDocument = attachment;
						_attachmentService.Add(attachment);
					}
					else
					{
						attachment.RowVersion = Convert.FromBase64String(this.CourseOutlineDocument.RowVersion);
						_attachmentService.Update(attachment);
					}
				}
				if (this.ProofOfQualificationsDocument != null && this.ProofOfQualificationsDocument.Index.HasValue && files.Count() > this.ProofOfQualificationsDocument.Index && isPrivateSector)
				{
					var attachment = files[this.ProofOfQualificationsDocument.Index.Value].UploadFile(this.ProofOfQualificationsDocument.Description, this.ProofOfQualificationsDocument.FileName);
					attachment.Id = this.ProofOfQualificationsDocument.Id;
					if (this.ProofOfQualificationsDocument.Id == 0)
					{
						trainingProvider.ProofOfQualificationsDocument = attachment;
						_attachmentService.Add(attachment);
					}
					else
					{
						attachment.RowVersion = Convert.FromBase64String(this.ProofOfQualificationsDocument.RowVersion);
						_attachmentService.Update(attachment);
					}
				}
				if (this.BusinessCaseDocument != null && this.BusinessCaseDocument.Index.HasValue && files.Count() > this.BusinessCaseDocument.Index && trainingProvider.TrainingOutsideBC)
				{
					var attachment = files[this.BusinessCaseDocument.Index.Value].UploadFile(this.BusinessCaseDocument.Description, this.BusinessCaseDocument.FileName);
					attachment.Id = this.BusinessCaseDocument.Id;
					if (this.BusinessCaseDocument.Id == 0)
					{
						trainingProvider.BusinessCaseDocument = attachment;
						_attachmentService.Add(attachment);
					}
					else
					{
						attachment.RowVersion = Convert.FromBase64String(this.BusinessCaseDocument.RowVersion);
						_attachmentService.Update(attachment);
					}
				}
			}

			if (trainingProvider.BusinessCaseDocumentId != null && trainingProvider.BusinessCaseDocumentId != trainingProvider.BusinessCaseDocument.Id)
			{
				_attachmentService.Remove(trainingProvider.BusinessCaseDocument);
				trainingProvider.BusinessCaseDocumentId = null;
			}

			if (trainingProvider.ProofOfQualificationsDocumentId != null && trainingProvider.ProofOfQualificationsDocumentId != trainingProvider.ProofOfQualificationsDocument.Id)
			{
				_attachmentService.Remove(trainingProvider.ProofOfQualificationsDocument);
				trainingProvider.ProofOfQualificationsDocument = null;
			}

			if (trainingProvider.CourseOutlineDocumentId != null && trainingProvider.CourseOutlineDocumentId != trainingProvider.CourseOutlineDocument.Id)
			{
				_attachmentService.Remove(trainingProvider.CourseOutlineDocument);
				trainingProvider.CourseOutlineDocument = null;
			}
		}
		#endregion
	}
}
