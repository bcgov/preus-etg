using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using CJG.Core.Entities.Attributes;

namespace CJG.Core.Entities
{
	/// <summary>
	/// TrainingProvider class, provides the ORM a way to manage training providers that are requested by the Application Administrators.
	/// </summary>
	public class TrainingProvider : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - Primary key identity.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The name of the training provider.
		/// </summary>
		[Required(ErrorMessage = "Provider name is required"), MaxLength(500), Index("IX_TrainingProviders", Order = 2)]
		public string Name { get; set; }

		/// <summary>
		/// get/set - Foreign key to the eligible cost this training provider is associated with.
		/// </summary>
		public int? EligibleCostId { get; set; }

		/// <summary>
		/// get/set - The eligible cost this training provider is associated with.
		/// </summary>
		[ForeignKey(nameof(EligibleCostId))]
		public virtual EligibleCost EligibleCost { get; set; }

		/// <summary>
		/// get/set - The foreign key to the grant application this training provider belongs to.
		/// </summary>
		[Index("IX_TrainingProviders", Order = 3)]
		public int? GrantApplicationId { get; set; }

		/// <summary>
		/// get/set - The grant application this training provider belongs to.
		/// </summary>
		[ForeignKey(nameof(GrantApplicationId))]
		public virtual GrantApplication GrantApplication { get; set; }

		/// <summary>
		/// get/set - Foreign key to the parent training provider, used for change requests.
		/// </summary>
		public int? OriginalTrainingProviderId { get; set; }

		/// <summary>
		/// get/set - The parent training provider, used for change requests.
		/// </summary>
		[ForeignKey(nameof(OriginalTrainingProviderId))]
		public virtual TrainingProvider OriginalTrainingProvider { get; set; }

		/// <summary>
		/// get/set - The foreign key to the type of training provider.
		/// </summary>
		[Range(1, int.MaxValue, ErrorMessage = "Training Provider Type is required."), Index("IX_TrainingProviders", Order = 5)]
		public int TrainingProviderTypeId { get; set; }

		/// <summary>
		/// get/set - The type of training provider.
		/// </summary>
		[ForeignKey(nameof(TrainingProviderTypeId))]
		public virtual TrainingProviderType TrainingProviderType { get; set; }

		/// <summary>
		/// get/set - The foreign key to the training provider's address.
		/// </summary>
		public int? TrainingAddressId { get; set; }

		/// <summary>
		/// get/set - The training provider's address.
		/// </summary>
		[ForeignKey(nameof(TrainingAddressId))]
		public virtual ApplicationAddress TrainingAddress { get; set; }

		public int? TrainingProviderAddressId { get; set; }

		/// <summary>
		/// get/set - The training provider's address.
		/// </summary>
		[ForeignKey(nameof(TrainingProviderAddressId))]
		public virtual ApplicationAddress TrainingProviderAddress { get; set; }
		/// <summary>
		/// get/set - The current state of this training provider.
		/// </summary>
		[DefaultValue(TrainingProviderStates.Incomplete), Index("IX_TrainingProviders", Order = 1)]
		public TrainingProviderStates TrainingProviderState { get; set; } = TrainingProviderStates.Incomplete;

		/// <summary>
		/// get/set - The foreign key to the training provider inventory.  This means it has been validated.
		/// </summary>
		public int? TrainingProviderInventoryId { get; set; }

		/// <summary>
		/// get/set - The training provider inventory.  This means it has been validated.
		/// </summary>
		[ForeignKey(nameof(TrainingProviderInventoryId))]
		public virtual TrainingProviderInventory TrainingProviderInventory { get; set; }

		/// <summary>
		/// get/set - Whether training will occur outside of BC.
		/// </summary>
		[Required(ErrorMessage = "Training outside BC choice required"), Index("IX_TrainingProviders", Order = 6)]
		public bool TrainingOutsideBC { get; set; }

		/// <summary>
		/// get/set - The business case for hosting training outside of BC.
		/// </summary>
		public string BusinessCase { get; set; }

		/// <summary>
		/// get/set - The foreign key to the business case document for hosting training outside of BC.
		/// </summary>
		public int? BusinessCaseDocumentId { get; set; }

		/// <summary>
		/// get/set - The business case document for hosting training outside of BC.
		/// </summary>
		[ForeignKey(nameof(BusinessCaseDocumentId))]
		public virtual Attachment BusinessCaseDocument { get; set; }

		/// <summary>
		/// get/set - The foreign key to the proof of qualifications document.
		/// </summary>
		public int? ProofOfQualificationsDocumentId { get; set; }

		/// <summary>
		/// get/set - The proof of qualifications document.
		/// </summary>
		[ForeignKey(nameof(ProofOfQualificationsDocumentId))]
		public virtual Attachment ProofOfQualificationsDocument { get; set; }

		/// <summary>
		/// get/set - The foreign key to the course outline document.
		/// </summary>
		public int? CourseOutlineDocumentId { get; set; }

		/// <summary>
		/// get/set - The course outline document.
		/// </summary>
		[ForeignKey(nameof(CourseOutlineDocumentId))]
		public virtual Attachment CourseOutlineDocument { get; set; }

		/// <summary>
		/// get/set - The training provider contact first name.
		/// </summary>
		[NameValidation]
		[Required(ErrorMessage = "Contact first name is required"), MaxLength(128), Index("IX_TrainingProviders", Order = 9)]
		public string ContactFirstName { get; set; }

		/// <summary>
		/// get/set - The training provider contact last name.
		/// </summary>
		[NameValidation]
		[Required(ErrorMessage = "Contact last name is required"), MaxLength(128), Index("IX_TrainingProviders", Order = 8)]
		public string ContactLastName { get; set; }

		/// <summary>
		/// get/set - The training provider contact email.
		/// </summary>
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		[Required(ErrorMessage = "Contact email is required"), MaxLength(500)]
		public string ContactEmail { get; set; }

		/// <summary>
		/// get/set - The training provider contact phone number.
		/// </summary>
		[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone Number")]
		[Required(ErrorMessage = "Contact phone number is required"), MaxLength(20)]
		public string ContactPhoneNumber { get; set; }

		/// <summary>
		/// get/set - The training provider contact phone number extension.
		/// </summary>
		[RegularExpression(@"^[0-9]*$", ErrorMessage = "Value must be numeric.")]
		[MaxLength(20)]
		public string ContactPhoneExtension { get; set; }

		/// <summary>
		/// get/set - The change request reason.
		/// </summary>
		[MaxLength(1000)]
		public string ChangeRequestReason { get; set; }

		/// <summary>
		/// Applicant-supplied HTML content about Alternative Training Options
		/// </summary>
		public string AlternativeTrainingOptions { get; set; }

		/// <summary>
		/// Applicant-supplied HTML content about Alternative Training Options
		/// </summary>
		public string ChoiceOfTrainerOrProgram { get; set; }

		/// <summary>
		/// Justification for supplying a Training Provider that isn't in B.C.
		/// </summary>
		public string OutOfProvinceLocationRationale { get; set; }

		/// <summary>
		/// get - All the associated training programs.
		/// The relationship is many-to-many, but in practice it's one training program with many training providers.
		/// </summary>
		public virtual ICollection<TrainingProgram> TrainingPrograms { get; set; } = new List<TrainingProgram>();

		/// <summary>
		/// get - The training program associated with this training provider.  It will traverse the parent training providers to find it.
		/// </summary>
		[NotMapped]
		public TrainingProgram TrainingProgram
		{
			get
			{
				// We only support one-to-many regardless of the fact the DB supports many-to-many.
				return this.TrainingPrograms.FirstOrDefault() ?? this.OriginalTrainingProvider?.TrainingProgram;
			}
		}

		/// <summary>
		/// get - all the associated requested service providers
		/// </summary>
		public virtual ICollection<TrainingProvider> RequestedTrainingProviders { get; set; } = new List<TrainingProvider>();

		/// <summary>
		/// get - The currently approved training provider.
		/// </summary>
		[NotMapped]
		public TrainingProvider ApprovedTrainingProvider
		{
			get
			{
				// If this is the original requested training provider find the currently approved training provider.
				// Otherwise go back to the original and find the currently approved training provider.
				if (this.OriginalTrainingProviderId == null)
				{
					// Get the currently validated training provider.
					var validated = this.RequestedTrainingProviders.OrderByDescending(tp => tp.DateAdded).FirstOrDefault(tp => tp.TrainingProviderInventoryId != null && tp.TrainingProviderState == TrainingProviderStates.Complete)?.ApprovedTrainingProvider;
					// If there is not a validated training provider, get the most recent requested one.
					return validated ?? this.RequestedTrainingProviders.OrderByDescending(tp => tp.DateAdded).FirstOrDefault(tp => tp.TrainingProviderInventoryId == null && tp.TrainingProviderState == TrainingProviderStates.Complete)?.ApprovedTrainingProvider ?? this;
				}
				else
				{
					// Get the currently validated training provider.
					var validated = this.OriginalTrainingProvider.RequestedTrainingProviders.OrderByDescending(tp => tp.DateAdded).FirstOrDefault(tp => tp.TrainingProviderInventoryId != null && tp.TrainingProviderState == TrainingProviderStates.Complete);
					// If there is not a validated training provider, get the most recent requested one.
					return validated ?? this.OriginalTrainingProvider.RequestedTrainingProviders.OrderByDescending(tp => tp.DateAdded).FirstOrDefault(tp => tp.TrainingProviderInventoryId == null && tp.TrainingProviderState == TrainingProviderStates.Complete) ?? this;
				}
			}
		}

		/// <summary>
		/// get - The currently requested service provider for this training provider.  This will only contain a value if a Change Request has been submitted.
		/// </summary>
		[NotMapped]
		public TrainingProvider RequestedTrainingProvider
		{
			get
			{
				if (this.OriginalTrainingProviderId == null)
				{
					var requested = this.RequestedTrainingProviders.OrderByDescending(tp => tp.DateAdded).ThenByDescending(tp => tp.Id).FirstOrDefault(tp => tp.TrainingProviderState.In(TrainingProviderStates.Requested, TrainingProviderStates.RequestApproved, TrainingProviderStates.RequestDenied) && tp.DateAdded >= this.DateAdded);
					return requested?.RequestedTrainingProvider ?? requested;
				}
				else
				{
					var requested = this.OriginalTrainingProvider.RequestedTrainingProviders.OrderByDescending(tp => tp.DateAdded).ThenByDescending(tp => tp.Id).FirstOrDefault(tp => tp.TrainingProviderState.In(TrainingProviderStates.Requested, TrainingProviderStates.RequestApproved, TrainingProviderStates.RequestDenied) && tp.DateAdded >= this.DateAdded);
					if (requested?.Id == this.Id) return null;
					return requested?.RequestedTrainingProvider ?? requested;
				}
			}
		}

		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a TrainingProvider object.
		/// </summary>
		public TrainingProvider()
		{

		}

		/// <summary>
		/// Creates a new instance of a TrainingProvider object and initializes with the specified property values.
		/// Use this to add a new training provider to the grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		public TrainingProvider(GrantApplication grantApplication)
		{
			this.GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
			this.GrantApplicationId = grantApplication.Id;
		}

		/// <summary>
		/// Creates a new instance of a TrainingProvider object and initializes with the specified property values.
		/// Use this to add a new change request for the specified training provider.
		/// </summary>
		/// <param name="trainingProvider"></param>
		public TrainingProvider(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));

			// Attaching to original reduces the depth that can occur with multiple change requests.
			var original = trainingProvider.OriginalTrainingProvider ?? trainingProvider;
			this.OriginalTrainingProviderId = original.Id;
			this.OriginalTrainingProvider = original;
			this.TrainingProviderState = TrainingProviderStates.Requested;
			original.RequestedTrainingProviders.Add(this);
			this.EligibleCostId = original.EligibleCostId;
			this.EligibleCost = original.EligibleCost;
			this.GrantApplication = original.GrantApplication;
			this.GrantApplicationId = original.GrantApplicationId;
		}

		/// <summary>
		/// Creates a new instance of a TrainingProvider object and initializes with the specified property values.
		/// Use this to add a new training provider to a training program.
		/// If the training program already has a training provider, this will create a change request.
		/// </summary>
		/// <param name="trainingProgram"></param>
		public TrainingProvider(TrainingProgram trainingProgram)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));

			// If the training program already has a training provider, this means we're making a change request.
			if (trainingProgram.TrainingProviders.Any())
			{
				// Attaching to original reduces the depth that can occur with multiple change requests.
				var original = trainingProgram.TrainingProvider?.OriginalTrainingProvider ?? trainingProgram.TrainingProvider;
				this.OriginalTrainingProviderId = original?.Id;
				this.OriginalTrainingProvider = original;
				this.TrainingProviderState = TrainingProviderStates.Requested;
				original?.RequestedTrainingProviders.Add(this);
				this.EligibleCostId = original.EligibleCostId;
				this.EligibleCost = original.EligibleCost;
			}
			else
			{
				this.TrainingPrograms.Add(trainingProgram);
				trainingProgram.TrainingProviders.Add(this);
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the TrainingProvider property values.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var context = validationContext.GetDbContext();
			var entry = validationContext.GetDbEntityEntry();
			var httpContext = validationContext.GetHttpContext();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null)
				yield break;

			var programs = context.Set<TrainingProgram>().Where(x => x.TrainingProviders.Any(y => y.Id == this.Id)).ToArray();
			if (this.GrantApplicationId.HasValue && this.GrantApplication == null)
				context.Set<GrantApplication>().Include(nameof(this.TrainingPrograms)).First(ga => ga.Id == this.GrantApplicationId);

			// This is ugly, but currently ensures that the appropriate grant application is loaded into memory.
			var originalTrainingProvider = context.Set<TrainingProvider>().Include(tp => tp.TrainingPrograms).FirstOrDefault(tp => tp.Id == this.OriginalTrainingProviderId);
			var grantApplicationId = this.GrantApplicationId ?? this.TrainingPrograms.FirstOrDefault()?.GrantApplicationId ?? (programs.Any() ? programs.First().GrantApplicationId : originalTrainingProvider?.GetGrantApplication()?.Id ?? 0);
			var grantApplication = (this.GrantApplication ?? this.TrainingPrograms.FirstOrDefault()?.GrantApplication) ?? context.Set<GrantApplication>().First(x => x.Id == grantApplicationId);

			// If TrainingOutsideBC there must be a BusinessCaseDocument.
			if (this.TrainingProviderState != TrainingProviderStates.Incomplete && this.TrainingOutsideBC && String.IsNullOrEmpty(this.BusinessCase) && (this.BusinessCaseDocumentId == null && this.BusinessCaseDocument == null))
				yield return new ValidationResult("If training is outside of BC you must provide a business case and/or document.", new[] { nameof(BusinessCaseDocument) });

			var checkPrivateSectorsOn = context.Set<Setting>().FirstOrDefault(o => o.Key.Equals("CheckPrivateSectorsOn"))?.GetValue() as DateTime?;
			var trainingProviderType = this.TrainingProviderType ?? context.Set<TrainingProviderType>().FirstOrDefault(x => x.Id == this.TrainingProviderTypeId);

			// for certain TrainingProviderTypes, there must be a proof of qualifications document and course outline
			var validatingTrainingProvider = entry.State == EntityState.Modified && (int?)entry.OriginalValues[nameof(TrainingProviderInventoryId)] != this.TrainingProviderInventoryId;
			if (!validatingTrainingProvider
				&& this.TrainingProviderState != TrainingProviderStates.Incomplete
				&& (
					trainingProviderType.PrivateSectorValidationType == TrainingProviderPrivateSectorValidationTypes.Always
					|| trainingProviderType.PrivateSectorValidationType == TrainingProviderPrivateSectorValidationTypes.ByDateSetting
					&& (grantApplication.DateSubmitted == null || grantApplication.DateSubmitted.Value.ToLocalTime().Date >= checkPrivateSectorsOn?.ToLocalTime().Date)
				))
			{
				if (this.ProofOfQualificationsDocument == null && this.ProofOfQualificationsDocumentId == null && trainingProviderType.ProofOfInstructorQualifications == 1)
					yield return new ValidationResult("You must provide proof of qualifications.", new[] { nameof(ProofOfQualificationsDocument) });

				if (this.CourseOutlineDocument == null && this.CourseOutlineDocumentId == null && trainingProviderType.CourseOutline == 1)
					yield return new ValidationResult("You must provide a course outline document.", new[] { nameof(CourseOutlineDocument) });
			}
			
			int[] selectedDeliveryMethodIds = TrainingProgram?.DeliveryMethods.Select(dm => dm.Id).ToArray();
			if (selectedDeliveryMethodIds != null
			    && (selectedDeliveryMethodIds.Contains(Constants.Delivery_Workplace) || selectedDeliveryMethodIds.Contains(Constants.Delivery_Classroom)))
			{
				if (this.TrainingAddress == null && this.TrainingAddressId == 0)
					yield return new ValidationResult("You must enter the training location address information.", new[] { nameof(TrainingAddress) });
			}

			if (this.TrainingProviderAddress == null && this.TrainingProviderAddressId == 0)
				yield return new ValidationResult("You must enter the training provider address information.", new[] { nameof(TrainingProviderAddress) });

			if (entry.State == EntityState.Modified)
			{
				// If a service category becomes disabled, the training provider must throw a validation error.
				if (httpContext.User.IsExternalUser() && this.EligibleCostId.HasValue && this.GrantApplication.ApplicationStateInternal.In(ApplicationStateInternal.Draft, ApplicationStateInternal.ApplicationWithdrawn))
				{
					var eligibleCost = context.Set<EligibleCost>().Include(ec => ec.EligibleExpenseType).FirstOrDefault(ec => ec.Id == this.EligibleCostId.Value);
					if (!eligibleCost?.EligibleExpenseType?.IsActive ?? false)
					{
						yield return new ValidationResult($"The service category '{eligibleCost.EligibleExpenseType.Caption}' is no longer available, please select another.", new[] { nameof(this.EligibleCostId) });
					}
				}
			}

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}

		public void Clone(TrainingProvider tp)
		{
			Name = tp.Name;
			TrainingProviderTypeId = tp.TrainingProviderTypeId;
			TrainingProviderState = tp.TrainingProviderState;
			TrainingOutsideBC = tp.TrainingOutsideBC;
			BusinessCase = tp.BusinessCase;

			ContactFirstName = tp.ContactFirstName;
			ContactLastName = tp.ContactLastName;
			ContactEmail = tp.ContactEmail;
			ContactPhoneNumber = tp.ContactPhoneNumber;
			ContactPhoneExtension = tp.ContactPhoneExtension;

			ChangeRequestReason = tp.ChangeRequestReason;
		}
		#endregion
	}
}
