using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared.TrainingProviders;
using System;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CJG.Web.External.Models.Shared.SkillsTrainings
{
	public class ServiceProviderDetailsViewModel : BaseProviderViewModel
	{
		#region Properties
		public int EligibleExpenseTypeId { get; set; }
		public int? ParentProviderId { get; set; }
		public bool IsServiceProvider { get; set; }
		public string Caption { get; set; }
		public TrainingProviderPrivateSectorValidationTypes PrivateSectorValidationType { get; set; }
		public TrainingProviderAttachmentViewModel ProofOfQualificationsDocument { get; set; } = new TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes.ProofOfQualifications);
		public TrainingProviderAttachmentViewModel CourseOutlineDocument { get; set; } = new TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes.CourseOutline);
		public int ProofOfInstructorQualifications { get; set; }
		public int CourseOutline { get; set; }
		#endregion

		#region Constructors
		public ServiceProviderDetailsViewModel()
		{
		}

		public ServiceProviderDetailsViewModel(TrainingProvider trainingProvider, EligibleExpenseType eligibleExpenseType) : base(trainingProvider)
		{
			this.IsServiceProvider = false;
			this.EligibleExpenseTypeId = eligibleExpenseType.Id;
			this.Caption = eligibleExpenseType.Caption;
			this.ParentProviderId = trainingProvider.OriginalTrainingProviderId;
			this.ProofOfQualificationsDocument = new TrainingProviderAttachmentViewModel(trainingProvider.ProofOfQualificationsDocument, this.Id, TrainingProviderAttachmentTypes.ProofOfQualifications, this.RowVersion);
			this.CourseOutlineDocument = new TrainingProviderAttachmentViewModel(trainingProvider.CourseOutlineDocument, this.Id, TrainingProviderAttachmentTypes.CourseOutline, this.RowVersion);
			this.ProofOfInstructorQualifications = trainingProvider.TrainingProviderType.ProofOfInstructorQualifications;
			this.CourseOutline = trainingProvider.TrainingProviderType.CourseOutline;

		}

		public ServiceProviderDetailsViewModel(EligibleExpenseType eligibleExpenseType)
		{
			this.IsServiceProvider = false;
			this.EligibleExpenseTypeId = eligibleExpenseType.Id;
			this.Caption = eligibleExpenseType.Caption;
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
		/// <param name="eligibleCostService"></param>
		/// <param name="eligibleExpenseTypeService"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public TrainingProvider MapProperties(
			IGrantApplicationService grantApplicationService,
			ITrainingProgramService trainingProgramService,
			ITrainingProviderService trainingProviderService,
			IAttachmentService attachmentService,
			IEligibleCostService eligibleCostService,
			IEligibleExpenseTypeService eligibleExpenseTypeService,
			IStaticDataService staticDataService,
			IPrincipal user,
			HttpPostedFileBase[] files = null)
		{
			if (grantApplicationService == null) throw new ArgumentNullException(nameof(grantApplicationService));
			if (trainingProgramService == null) throw new ArgumentNullException(nameof(trainingProgramService));
			if (trainingProviderService == null) throw new ArgumentNullException(nameof(trainingProviderService));
			if (attachmentService == null) throw new ArgumentNullException(nameof(attachmentService));
			if (eligibleCostService == null) throw new ArgumentNullException(nameof(eligibleCostService));
			if (eligibleExpenseTypeService == null) throw new ArgumentNullException(nameof(eligibleExpenseTypeService));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));
			if (user == null) throw new ArgumentNullException(nameof(user));

			var grantApplication = grantApplicationService.Get(this.GrantApplicationId);
			TrainingProvider trainingProvider;

			if (this.TrainingProgramId > 0)
			{
				var trainingProgram = trainingProgramService.Get(this.TrainingProgramId.Value);
				trainingProvider = this.Id > 0 ? trainingProviderService.Get(this.Id) : new TrainingProvider(trainingProgram);
			}
			else
			{
				if (this.ParentProviderId > 0)
				{
					// This is a change request if it's a new service provider.
					var parentProvider = trainingProviderService.Get(this.ParentProviderId.Value);
					trainingProvider = this.Id > 0 ? trainingProviderService.Get(this.Id) : new TrainingProvider(parentProvider.OriginalTrainingProvider ?? parentProvider);
				}
				else
					trainingProvider = this.Id > 0 ? trainingProviderService.Get(this.Id) : new TrainingProvider(grantApplication);
			}

			if (this.Id == 0)
			{
				if (!user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.AddRemoveTrainingProvider))
					throw new NotAuthorizedException($"User does not have permission to add a training provider in Grant Application '{grantApplication?.Id}'.");
			}
			else
			{
				if (!user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.EditTrainingProvider))
					throw new NotAuthorizedException($"User does not have permission to edit the training provider in Grant Application '{grantApplication?.Id}'.");

				trainingProvider.RowVersion = System.Convert.FromBase64String(this.RowVersion);
			}

			trainingProvider.Name = this.Name;
			trainingProvider.ChangeRequestReason = this.ChangeRequestReason;
			trainingProvider.TrainingProviderTypeId = this.TrainingProviderTypeId.Value;

			if (trainingProvider.TrainingProviderState == TrainingProviderStates.Incomplete)
				trainingProvider.TrainingProviderState = this.ParentProviderId > 0 ? TrainingProviderStates.Requested : TrainingProviderStates.Complete;

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
			trainingProvider.TrainingAddress.RegionId = this.RegionId;

			trainingProvider.TrainingAddress.PostalCode = this.PostalCode;
			trainingProvider.TrainingAddress.CountryId = this.CountryId;

			// check Training Cost
			if (grantApplication.TrainingCost == null) grantApplication.TrainingCost = new TrainingCost(grantApplication, 1);
			// check Estimated Participants
			if (grantApplication.TrainingCost.EstimatedParticipants < 1) grantApplication.TrainingCost.EstimatedParticipants = 1;

			// check Eligible Cost
			EligibleCost eligibleCost = null;
			if (this.EligibleCostId.HasValue)
				eligibleCost = eligibleCostService.Get(this.EligibleCostId.Value);
			else if (this.EligibleExpenseTypeId > 0)
				eligibleCost = grantApplication.TrainingCost.EligibleCosts.FirstOrDefault(e => e.EligibleExpenseTypeId == this.EligibleExpenseTypeId);

			if (eligibleCost != null)
			{
				trainingProvider.EligibleCost = eligibleCost;
			}
			else
			{
				trainingProvider.EligibleCost = new EligibleCost(grantApplication.TrainingCost, eligibleExpenseTypeService.Get(this.EligibleExpenseTypeId), 0, grantApplication.TrainingCost.EstimatedParticipants);
				grantApplication.TrainingCost.EligibleCosts.Add(trainingProvider.EligibleCost);
			}

			trainingProvider.TrainingProviderType = staticDataService.GetTrainingProviderType(trainingProvider.TrainingProviderTypeId);
			//if (trainingProvider.TrainingProviderType.PrivateSectorValidationType == TrainingProviderPrivateSectorValidationTypes.Never)
			//{
			//	if (this.CourseOutlineDocument != null)
			//		this.CourseOutlineDocument.Id = 0;
			//	if (this.ProofOfQualificationsDocument != null)
			//		this.ProofOfQualificationsDocument.Id = 0;
			//}

			UpdateAttachment(trainingProvider, attachmentService, files);

			return trainingProvider;
		}

		private void UpdateAttachment(TrainingProvider trainingProvider, IAttachmentService attachmentService, HttpPostedFileBase[] files = null)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			if (attachmentService == null) throw new ArgumentNullException(nameof(attachmentService));

			if (files != null && files.Any())
			{
				var isPrivateSector = trainingProvider.IsPrivateSectorType(AppDateTime.UtcNow);
				if (this.CourseOutlineDocument != null && this.CourseOutlineDocument.Index.HasValue && files.Count() > this.CourseOutlineDocument.Index)
				{
					var attachment = files[this.CourseOutlineDocument.Index.Value].UploadFile(this.CourseOutlineDocument.Description, this.CourseOutlineDocument.FileName);
					attachment.Id = this.CourseOutlineDocument.Id;
					if (this.CourseOutlineDocument.Id == 0)
					{
						trainingProvider.CourseOutlineDocument = attachment;
						attachmentService.Add(attachment);
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
					}
					else
					{
						attachment.RowVersion = Convert.FromBase64String(this.ProofOfQualificationsDocument.RowVersion);
						attachmentService.Update(attachment);
					}
				}
			}

			if (trainingProvider.ProofOfQualificationsDocumentId != null && trainingProvider.ProofOfQualificationsDocumentId != this.ProofOfQualificationsDocument.Id)
			{
				attachmentService.Remove(trainingProvider.ProofOfQualificationsDocument);
				trainingProvider.ProofOfQualificationsDocumentId = null;
			}

			if (trainingProvider.CourseOutlineDocumentId != null && trainingProvider.CourseOutlineDocumentId != this.CourseOutlineDocument.Id)
			{
				attachmentService.Remove(trainingProvider.CourseOutlineDocument);
				trainingProvider.CourseOutlineDocumentId = null;
			}
		}
		#endregion
	}
}
