using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="TrainingProviderService"/> class, provides a way to manage training providers.
	/// </summary>
	public class TrainingProviderService : Service, ITrainingProviderService
	{
		#region Variables
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantAgreementService _grantAgreementService;
		private readonly ITrainingProviderInventoryService _trainingProviderInventoryService;
		private readonly IApplicationAddressService _applicationAddressService;
		private readonly INoteService _noteService;
		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderService"/> class.
		/// </summary>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantAgreementService"></param>
		/// <param name="trainingProviderInventoryService"></param>
		/// <param name="applicationAddressService"></param>
		/// <param name="noteService"></param>
		/// <param name="dbContext"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public TrainingProviderService(
			IGrantApplicationService grantApplicationService,
			IGrantAgreementService grantAgreementService,
			ITrainingProviderInventoryService trainingProviderInventoryService,
			IApplicationAddressService applicationAddressService,
			INoteService noteService,
			IDataContext dbContext,
			HttpContextBase httpContext,
			ILogger logger) : base(dbContext, httpContext, logger)
		{
			_grantApplicationService = grantApplicationService;
			_grantAgreementService = grantAgreementService;
			_trainingProviderInventoryService = trainingProviderInventoryService;
			_applicationAddressService = applicationAddressService;
			_noteService = noteService;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the training provider for the specified 'id' from the datasource.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public TrainingProvider Get(int id)
		{
			var trainingProvider = Get<TrainingProvider>(id);
			if (trainingProvider == null)
				throw new NoContentException(nameof(trainingProvider));
			if (!_httpContext.User.CanPerformAction(trainingProvider.GetGrantApplication(), ApplicationWorkflowTrigger.ViewApplication))
				throw new NotAuthorizedException($"User does not have permission to access application {trainingProvider.GetGrantApplication().Id}.");

			return trainingProvider;
		}

		/// <summary>
		/// Update the specified training provider in the datasource.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <returns></returns>
		public TrainingProvider Update(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null)
				throw new ArgumentNullException(nameof(trainingProvider));

			if (!_httpContext.User.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.EditTrainingProvider))
				throw new NotAuthorizedException($"User does not have permission to edit application {trainingProvider.GetGrantApplication().Id}.");

			var grantApplication = trainingProvider.GetGrantApplication();

			// If it's an EmployerTraining grant then link the training provider to the program.
			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.EmployerGrant && trainingProvider.TrainingProgram == null)
			{
				var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
				if (trainingProgram != null)
				{
					trainingProgram.TrainingProviders.Add(trainingProvider);
				}
			}

			if (trainingProvider.GrantApplication != null && trainingProvider.TrainingProgram != null)
			{
				// It should only be one or the other.
				trainingProvider.GrantApplication = null;
				trainingProvider.GrantApplicationId = null;
				grantApplication.TrainingProviders.Remove(trainingProvider);
				_dbContext.Update(grantApplication);
			}

			var entry = _dbContext.Entry(trainingProvider);
			var businessCaseDocumentId = ((int?)entry.OriginalValues[nameof(trainingProvider.BusinessCaseDocumentId)]);
			if ((businessCaseDocumentId.HasValue || trainingProvider.BusinessCaseDocumentId.HasValue) && (!trainingProvider.TrainingOutsideBC || trainingProvider.BusinessCaseDocumentId == null))
			{
				_dbContext.Attachments.Remove(Get<Attachment>(businessCaseDocumentId ?? trainingProvider.BusinessCaseDocumentId));
			}
			var courseOutlineDocumentId = ((int?)entry.OriginalValues[nameof(trainingProvider.CourseOutlineDocumentId)]);
			if (trainingProvider.CourseOutlineDocumentId == null && courseOutlineDocumentId.HasValue)
			{
				_dbContext.Attachments.Remove(Get<Attachment>(courseOutlineDocumentId.Value));
			}
			var proofOfQualificationsDocumentId = ((int?)entry.OriginalValues[nameof(trainingProvider.ProofOfQualificationsDocumentId)]);
			if (trainingProvider.ProofOfQualificationsDocumentId == null && proofOfQualificationsDocumentId.HasValue)
			{
				_dbContext.Attachments.Remove(Get<Attachment>(proofOfQualificationsDocumentId.Value));
			}

			_dbContext.Update(trainingProvider);

			var accountType = _httpContext.User.GetAccountType();
			if (accountType == AccountTypes.Internal && _grantAgreementService.AgreementUpdateRequired(grantApplication))
			{
				_grantAgreementService.UpdateAgreement(grantApplication);
				_dbContext.Update(grantApplication);
			}

			_noteService.GenerateUpdateNote(grantApplication);
			_dbContext.CommitTransaction();

			return trainingProvider;
		}

		/// <summary>
		/// Add the specified training provider to the datasource.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <returns></returns>
		public TrainingProvider Add(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null)
				throw new ArgumentNullException(nameof(trainingProvider));

			if (!_httpContext.User.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.AddRemoveTrainingProvider))
				throw new NotAuthorizedException($"User does not have permission to edit application {trainingProvider.GetGrantApplication().Id}.");

			var grantApplication = trainingProvider.GetGrantApplication();

			// Service Providers must be associated to an eligible cost.
			if (grantApplication.GetProgramType() == ProgramTypes.WDAService && trainingProvider.GrantApplicationId.HasValue && trainingProvider.EligibleCost == null)
				throw new InvalidOperationException("The service provider must be associated with an eligible cost.");

			// If it's an EmployerTraining grant then link the training provider to the program.
			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.EmployerGrant && trainingProvider.TrainingProgram == null)
			{
				var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
				if (trainingProgram != null)
				{
					trainingProgram.TrainingProviders.Add(trainingProvider);
				}
			}

			if (trainingProvider.GrantApplication != null && trainingProvider.TrainingProgram != null)
			{
				// It should only be one or the other.
				trainingProvider.GrantApplication = null;
				trainingProvider.GrantApplicationId = null;
				grantApplication.TrainingProviders.Remove(trainingProvider);
				_dbContext.Update(grantApplication);
			}

			_dbContext.TrainingProviders.Add(trainingProvider);

			var accountType = _httpContext.User.GetAccountType();
			if (accountType == AccountTypes.Internal && _grantAgreementService.AgreementUpdateRequired(grantApplication))
			{
				_grantAgreementService.UpdateAgreement(grantApplication);
				_dbContext.Update(grantApplication);
			}

			_noteService.GenerateUpdateNote(grantApplication);
			_dbContext.CommitTransaction();

			return trainingProvider;
		}

		/// <summary>
		/// Get the default training provider type.
		/// </summary>
		/// <returns></returns>
		public TrainingProviderType GetDefaultTrainingProviderType()
		{
			return _dbContext.TrainingProviderTypes.FirstOrDefault(tpt => tpt.IsActive == true);
		}

		/// <summary>
		/// Validate the specified training provider with the specified inventory.
		/// The Assessor performs this action.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <param name="trainingProviderInventoryId"></param>
		/// <returns></returns>
		public TrainingProvider ValidateTrainingProvider(TrainingProvider trainingProvider, int trainingProviderInventoryId)
		{
			var grantApplication = trainingProvider.GetGrantApplication();
			var trainingProviderInventory = _trainingProviderInventoryService.Get(trainingProviderInventoryId);

			if (!_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ValidateTrainingProvider))
				throw new NotAuthorizedException($"Not allowed to validate the training provider when the application is in '{grantApplication.ApplicationStateInternal.GetDescription()}' state.");

			var old_name = trainingProvider.Name;
			trainingProvider.Name = trainingProviderInventory.Name;
			trainingProvider.TrainingProviderInventoryId = trainingProviderInventoryId;

			_grantApplicationService.Update(grantApplication, ApplicationWorkflowTrigger.ValidateTrainingProvider);

			return trainingProvider;
		}

		/// <summary>
		/// Delete the specified training provider.
		/// </summary>
		/// <param name="trainingProvider"></param>
		public void Delete(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null)
				throw new ArgumentNullException(nameof(trainingProvider));

			if (!_httpContext.User.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.AddRemoveTrainingProvider))
				throw new NotAuthorizedException($"User does not have permission to remove provider {trainingProvider.Id}.");

			var grantApplication = trainingProvider.GetGrantApplication();
			var ids = trainingProvider.TrainingPrograms.Select(x => x.Id).ToArray();
			trainingProvider.TrainingPrograms.Clear();
			foreach (var id in ids)
			{
				var trainingProgram = Get<TrainingProgram>(id);
				trainingProgram.DeliveryMethods.Clear();
				trainingProgram.UnderRepresentedGroups.Clear();
				_dbContext.TrainingPrograms.Remove(trainingProgram);
			}

			var attachments = new Attachment[] {
				trainingProvider.BusinessCaseDocument,
				trainingProvider.ProofOfQualificationsDocument,
				trainingProvider.CourseOutlineDocument
			};

			// remove associated attachment and versioned attachments
			foreach (var attachment in attachments.Where(a => a != null))
			{
				foreach (var v in attachment.Versions)
				{
					_dbContext.VersionedAttachments.Remove(v);
				}
				_dbContext.Attachments.Remove(attachment);
			}

			_dbContext.TrainingProviders.Remove(trainingProvider);

			var accountType = _httpContext.User.GetAccountType();
			if (accountType == AccountTypes.Internal && _grantAgreementService.AgreementUpdateRequired(grantApplication))
			{
				_grantAgreementService.UpdateAgreement(grantApplication);
				_dbContext.Update(grantApplication);
			}

			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Delete the specified training provider.
		/// </summary>
		/// <param name="trainingProvider"></param>
		public void DeleteRequestedTrainingProvider(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null)
				throw new ArgumentNullException(nameof(trainingProvider));

			if (!_httpContext.User.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.AddRemoveTrainingProvider))
				throw new NotAuthorizedException($"User does not have permission to remove provider {trainingProvider.Id}.");

			trainingProvider.TrainingPrograms.Clear();

			var attachments = new Attachment[] {
				trainingProvider.BusinessCaseDocument,
				trainingProvider.ProofOfQualificationsDocument,
				trainingProvider.CourseOutlineDocument
			};

			// remove associated attachment and versioned attachments
			foreach (var attachment in attachments.Where(a => a != null))
			{
				foreach (var v in attachment.Versions)
				{
					_dbContext.VersionedAttachments.Remove(v);
				}
				_dbContext.Attachments.Remove(attachment);
			}

			_dbContext.TrainingProviders.Remove(trainingProvider);
			CommitTransaction();
		}

		/// <summary>
		/// Add the specified attachment to the specified training provider.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <param name="attachment"></param>
		/// <param name="type"></param>
		public void AddAttachment(TrainingProvider trainingProvider, Attachment attachment, TrainingProviderAttachmentTypes type)
		{
			if (attachment == null) throw new ArgumentNullException(nameof(attachment));

			// When the training provider is null it means they have uploaded the attachment before saving the training provider. 
			// The Attachment.Id will be stored in Session and linked to the training provider when it is saved.
			// This can leave orphaned attachments however...
			if (trainingProvider == null)
			{
				_dbContext.Attachments.Add(attachment);
			}
			else
			{
				switch (type)
				{
					case TrainingProviderAttachmentTypes.BusinessCase:
						trainingProvider.BusinessCaseDocument = attachment;
						break;
					case TrainingProviderAttachmentTypes.CourseOutline:
						trainingProvider.CourseOutlineDocument = attachment;
						break;
					case TrainingProviderAttachmentTypes.ProofOfQualifications:
						trainingProvider.ProofOfQualificationsDocument = attachment;
						break;
				}
				_dbContext.Update<TrainingProvider>(trainingProvider);
			}
			CommitTransaction();
		}

		public IEnumerable<TrainingProviderType> GetTrainingProviderTypes()
		{
			return _dbContext.TrainingProviderTypes
				.AsNoTracking()
				.OrderBy(t => t.RowSequence)
				.ThenBy(t => t.Caption)
				.ThenBy(t => t.IsActive);
		}

		#endregion
	}
}
