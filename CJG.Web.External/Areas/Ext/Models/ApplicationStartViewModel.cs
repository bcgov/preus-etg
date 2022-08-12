using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models.GrantStreams;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ApplicationStartViewModel : BaseViewModel
	{
		#region Properties

		public int GrantApplicationId { get { return Id; } set { Id = value; } }

		public byte[] RowVersion { get; set; }

		public int GrantProgramId { get; set; }
		public int SeedGrantApplicationId { get; set; }

		public string GrantProgramName { get; set; }

		[Required(ErrorMessage = "Start Date is required")]
		public DateTime? DeliveryStartDate { get; set; }

		public int DeliveryStartYear { get; set; }
		public int DeliveryStartMonth { get; set; }
		public int DeliveryStartDay { get; set; }

		[Required(ErrorMessage = "End Date is required")]
		public DateTime? DeliveryEndDate { get; set; }

		public int DeliveryEndYear { get; set; }
		public int DeliveryEndMonth { get; set; }
		public int DeliveryEndDay { get; set; }

		[Required(ErrorMessage = "Grant opening is required")]
		public int? GrantOpeningId { get; set; }

		public double? ReimbursementRate { get; set; }

		public IEnumerable<ApplicationGrantOpeningGroupModel> TrainingPeriods { get; set; }

		public IEnumerable<KeyValuePair<int, string>> PrioritySectors { get; set; }

		public StreamEligibilityViewModel GrantStream { get; set; } = new StreamEligibilityViewModel();

		public ProgramTypes ProgramType { get; set; }

		public bool? HasRequestedAdditionalFunding { get; set; }

		[Required(ErrorMessage = "You must describe the funding received or requested for this training.")]
		public string DescriptionOfFundingRequested { get; set; }

		public bool? IsAlternateContact { get; set; }

		//[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Invalid Format")]
		[CustomValidation(typeof(ApplicationStartViewModelVmValidation), "ValidateAlternateJobTitle")]
		public string AlternateJobTitle { get; set; }

		//[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Invalid Format")]
		[CustomValidation(typeof(ApplicationStartViewModelVmValidation), "ValidateAlternateLastName")]
		public string AlternateLastName { get; set; }

		//[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Invalid Format")]
		[CustomValidation(typeof(ApplicationStartViewModelVmValidation), "ValidateAlternateFirstName")]
		public string AlternateFirstName { get; set; }

		public string AlternateSalutation { get; set; }

		//[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
		[CustomValidation(typeof(ApplicationStartViewModelVmValidation), "ValidateAlternateEmail")]
		public string AlternateEmail { get; set; }

		//public PhoneViewModel AlternatePhoneViewModel { get; set; }
				
		//[RegularExpression("^\\D*(\\d\\D*){10}?$", ErrorMessage = "Contact phone number must be a 10-digit number")]
		[CustomValidation(typeof(ApplicationStartViewModelVmValidation), "ValidateAlternatePhone")]
		public string AlternatePhone
		{
			get
			{
				return $"({AlternatePhoneAreaCode}) {AlternatePhoneExchange}-{AlternatePhoneNumber}";
			}
		}
		public string AlternatePhoneAreaCode { get; set; }
		public string AlternatePhoneExchange { get; set; }
		public string AlternatePhoneNumber { get; set; }
		public string AlternatePhoneExtension { get; set; }

		#endregion

		#region Constructors
		public ApplicationStartViewModel()
		{
		}

		public ApplicationStartViewModel(GrantApplication grantApplication, IGrantOpeningService grantOpeningService, IGrantProgramService grantProgramService, IStaticDataService staticDataService, IGrantStreamService grantStreamService)
			: this(grantApplication.GrantOpening.GrantStream.GrantProgramId, 0, grantOpeningService, grantProgramService, staticDataService, grantStreamService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			//var provinces = staticDataService.GetProvinces();

			Id = grantApplication.Id;
			GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgramId;
			GrantOpeningId = grantApplication.GrantOpeningId;

			DeliveryStartDate = grantApplication.StartDate.ToLocalTime();
			DeliveryStartYear = DeliveryStartDate.Value.Year;
			DeliveryStartMonth = DeliveryStartDate.Value.Month;
			DeliveryStartDay = DeliveryStartDate.Value.Day;

			DeliveryEndDate = grantApplication.EndDate.ToLocalTime();
			DeliveryEndYear = DeliveryEndDate.Value.Year;
			DeliveryEndMonth = DeliveryEndDate.Value.Month;
			DeliveryEndDay = DeliveryEndDate.Value.Day;

			ReimbursementRate = grantApplication.ReimbursementRate;

			RowVersion = grantApplication.RowVersion;

			var stream = grantApplication.GrantOpening.GrantStream;
			ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			GrantStream = new StreamEligibilityViewModel
			{
				Name = stream.Name,
				EligibilityEnabled = stream.EligibilityEnabled,
				EligibilityQuestion = stream.EligibilityQuestion,
				EligibilityRequired = stream.EligibilityRequired,
				EligibilityRequirements = stream.EligibilityRequirements,
				StreamEligibilityQuestions = grantStreamService.GetGrantStreamQuestions(stream.Id)
					.Where(l => l.IsActive)
					.Select(n => new GrantStreamQuestionViewModel(n)).ToList()
			};

			// Add answers to the questions. Note that the actual questions could change (add new, or Inactive an old one).
			foreach (var answer in grantApplication.GrantStreamEligibilityAnswers)
			{
				foreach (var streamQ in GrantStream.StreamEligibilityQuestions)
				{
					if (answer.GrantStreamEligibilityQuestionId == streamQ.Id)
					{
						streamQ.EligibilityAnswer = answer.EligibilityAnswer;

					}
				}
			}

			ApplicationStartViewModel applicationStartViewModel = this;
			applicationStartViewModel.IsAlternateContact = grantApplication.IsAlternateContact;
			applicationStartViewModel.HasRequestedAdditionalFunding = grantApplication.HasRequestedAdditionalFunding;
			applicationStartViewModel.DescriptionOfFundingRequested = grantApplication.DescriptionOfFundingRequested;
			applicationStartViewModel.AlternateEmail = grantApplication.AlternateEmail;
			applicationStartViewModel.AlternateFirstName = grantApplication.AlternateFirstName;
			applicationStartViewModel.AlternateJobTitle = grantApplication.AlternateJobTitle;
			applicationStartViewModel.AlternateLastName = grantApplication.AlternateLastName;

			applicationStartViewModel.AlternatePhoneAreaCode = grantApplication.AlternatePhoneNumber.GetPhoneAreaCode();
			applicationStartViewModel.AlternatePhoneExchange = grantApplication.AlternatePhoneNumber.GetPhoneExchange();
			applicationStartViewModel.AlternatePhoneNumber = grantApplication.AlternatePhoneNumber.GetPhoneNumber();
			applicationStartViewModel.AlternatePhoneExtension = grantApplication.AlternatePhoneExtension;
		}

		public ApplicationStartViewModel(int grantProgramId, int seedGrantApplicationId, IGrantOpeningService grantOpeningService, IGrantProgramService grantProgramService, IStaticDataService staticDataService, IGrantStreamService grantStreamService)
		{
			if (grantOpeningService == null) throw new ArgumentNullException(nameof(grantOpeningService));
			if (grantProgramService == null) throw new ArgumentNullException(nameof(grantProgramService));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));

			GrantProgramId = grantProgramId;
			var grantProgram = grantProgramService.Get(GrantProgramId);
			GrantProgramName = grantProgram.Name;
			ProgramType = grantProgram.ProgramTypeId;
			SeedGrantApplicationId = seedGrantApplicationId;

			var grantOpenings = grantOpeningService.GetGrantOpenings(AppDateTime.UtcNow, GrantProgramId).GroupBy(o => new { o.TrainingPeriod.StartDate, o.TrainingPeriod.EndDate });

			TrainingPeriods = grantOpenings.Select(
				o => new ApplicationGrantOpeningGroupModel(
					o.Key.StartDate,
					o.Key.EndDate,
					ProgramType,
					o.Select(g => new ApplicationGrantOpeningViewModel(g)).ToList())).ToList();
			PrioritySectors = staticDataService.GetPrioritySectors().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToList();
		}
		#endregion
	}
}
