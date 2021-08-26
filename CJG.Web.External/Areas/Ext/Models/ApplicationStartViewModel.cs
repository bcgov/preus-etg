using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CJG.Core.Entities;
using System;
using CJG.Web.External.Models.Shared;
using CJG.Core.Interfaces.Service;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ApplicationStartViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get { return Id; } set { Id = value; } }

		public byte[] RowVersion { get; set; }

		public int GrantProgramId { get; set; }
		
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

		public bool? EligibilityConfirmed { get; set; }

		public IEnumerable<ApplicationGrantOpeningGroupModel> TrainingPeriods { get; set; }

		public IEnumerable<KeyValuePair<int, string>> PrioritySectors { get; set; }

		public StreamEligibilityViewModel GrantStream { get; set; } = new StreamEligibilityViewModel();

		public ProgramTypes ProgramType { get; set; }
		#endregion

		#region Constructors
		public ApplicationStartViewModel()
		{
		}

		public ApplicationStartViewModel(GrantApplication grantApplication, IGrantOpeningService grantOpeningService, IGrantProgramService grantProgramService, IStaticDataService staticDataService)
			: this(grantApplication.GrantOpening.GrantStream.GrantProgramId, grantOpeningService, grantProgramService, staticDataService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgramId;
			this.GrantOpeningId = grantApplication.GrantOpeningId;

			this.DeliveryStartDate = grantApplication.StartDate.ToLocalTime();
			this.DeliveryStartYear = this.DeliveryStartDate.Value.Year;
			this.DeliveryStartMonth = this.DeliveryStartDate.Value.Month;
			this.DeliveryStartDay = this.DeliveryStartDate.Value.Day;

			this.DeliveryEndDate = grantApplication.EndDate.ToLocalTime();
			this.DeliveryEndYear = this.DeliveryEndDate.Value.Year;
			this.DeliveryEndMonth = this.DeliveryEndDate.Value.Month;
			this.DeliveryEndDay = this.DeliveryEndDate.Value.Day;

			this.ReimbursementRate = grantApplication.ReimbursementRate;

			this.RowVersion = grantApplication.RowVersion;

			var stream = grantApplication.GrantOpening.GrantStream;
			this.ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			this.GrantStream = new StreamEligibilityViewModel()
			{
				Name = stream.Name,
				EligibilityEnabled = stream.EligibilityEnabled,
				EligibilityQuestion = stream.EligibilityQuestion,
				EligibilityRequired = stream.EligibilityRequired,
				EligibilityRequirements = stream.EligibilityRequirements
			};
			this.EligibilityConfirmed = grantApplication.EligibilityConfirmed;
		}

		public ApplicationStartViewModel(int grantProgramId, IGrantOpeningService grantOpeningService, IGrantProgramService grantProgramService, IStaticDataService staticDataService)
		{
			if (grantOpeningService == null) throw new ArgumentNullException(nameof(grantOpeningService));
			if (grantProgramService == null) throw new ArgumentNullException(nameof(grantProgramService));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));

			this.GrantProgramId = grantProgramId;
			var grantOpenings = grantOpeningService.GetGrantOpenings(AppDateTime.UtcNow, this.GrantProgramId).GroupBy(o => new { o.TrainingPeriod.StartDate, o.TrainingPeriod.EndDate });
			this.TrainingPeriods = grantOpenings.Select(
				o => new ApplicationGrantOpeningGroupModel(
					o.Key.StartDate,
					o.Key.EndDate,
					o.Select(g => new ApplicationGrantOpeningViewModel(g)).ToList())).ToList();
			this.PrioritySectors = staticDataService.GetPrioritySectors().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToList();

			var grantProgram = grantProgramService.Get(this.GrantProgramId);
			this.GrantProgramName = grantProgram.Name;
			this.ProgramType = grantProgram.ProgramTypeId;
		}
		#endregion
	}
}
