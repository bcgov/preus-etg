using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }

		[Required(ErrorMessage = "Program code is required")]
		public string ProgramCode { get; set; }

		public GrantProgramStates State { get; set; }

		[Required(ErrorMessage = "Program name is required")]
		public string Name { get; set; }

		public string Message { get; set; }
		public string EligibilityDescription { get; set; }
		public string BatchRequestDescription { get; set; }
		public bool ShowMessage { get; set; }
		public string RequestedBy { get; set; }

		[StringLength(15, ErrorMessage = "The field must be a string with a maximum length of 15")]
		public string ProgramPhone { get; set; }
		public int? ExpenseAuthorityId { get; set; }
		public string DocumentPrefix { get; set; }

		[EnumDataType(typeof(ProgramTypes), ErrorMessage = "Program type is required")]
		public ProgramTypes ProgramTypeId { get; set; }

		public bool UseFIFOReservation { get; set; }
		public bool IncludeDeliveryPartner { get; set; }

		public AccountCodeViewModel AccountCode { get; set; }

		[Required(ErrorMessage = "Program configuration is required")]
		public int? ProgramConfigurationId { get; set; }

		public ProgramConfigurationViewModel ProgramConfiguration { get; set; }

		public DeliveryPartnerViewModel[] DeliveryPartners { get; set; }
		public DeliveryPartnerViewModel[] DeliveryPartnerServices { get; set; }

		#region Permissions
		public bool CanImplement
		{
			get
			{
				return !(string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(ProgramCode)
					 || string.IsNullOrEmpty(EligibilityDescription) || string.IsNullOrEmpty(BatchRequestDescription)
					 || string.IsNullOrEmpty(RequestedBy) || string.IsNullOrEmpty(ProgramPhone)
					 || string.IsNullOrEmpty(DocumentPrefix) || string.IsNullOrEmpty(AccountCode.GLClientNumber)
					 || string.IsNullOrEmpty(AccountCode.GLRESP) || string.IsNullOrEmpty(AccountCode.GLServiceLine)
					 || string.IsNullOrEmpty(AccountCode.GLSTOBNormal) || string.IsNullOrEmpty(AccountCode.GLSTOBAccrual)
					 || string.IsNullOrEmpty(AccountCode.GLProjectCode));
			}
		}
		public bool CanTerminate { get; set; }
		public bool CanDelete { get; set; }
		#endregion
		#endregion

		#region Constructors
		public GrantProgramViewModel()
		{
		}

		public GrantProgramViewModel(GrantProgram grantProgram, IGrantStreamService grantStreamService, IGrantOpeningService grantOpeningService, IDeliveryPartnerService deliveryPartnerService, IExpenseTypeService expenseTypeService)
		{
			this.Id = grantProgram?.Id ?? throw new ArgumentNullException(nameof(grantProgram));

			Utilities.MapProperties(grantProgram, this, o => new { o.ProgramConfiguration });

			var streams = grantStreamService.GetGrantStreamsForProgram(grantProgram.Id);
			var deliveryPartners = deliveryPartnerService.GetDeliveryPartners(grantProgram.Id);
			var deliveryPartnerServices = deliveryPartnerService.GetDeliveryPartnerServices(grantProgram.Id);

			this.CanTerminate = !(streams.Any(t => grantOpeningService.GetGrantOpenings(t).Any(o => o.State != GrantOpeningStates.Closed)) || streams.Any(s => s.IsActive)); // Can't terminate if there are active streams or any openings that are not closed.
			this.CanDelete = grantProgram.State == GrantProgramStates.NotImplemented && !streams.Any(t => grantOpeningService.GetGrantOpenings(t).Any());

			this.AccountCode = grantProgram.AccountCode != null ? new AccountCodeViewModel(grantProgram.AccountCode) : new AccountCodeViewModel();
			this.ProgramConfiguration = grantProgram.ProgramConfiguration != null ? new ProgramConfigurationViewModel(grantProgram.ProgramConfiguration) : new ProgramConfigurationViewModel();

			this.DeliveryPartners = deliveryPartners.Select(o => new DeliveryPartnerViewModel(o, deliveryPartnerService)).ToArray();
			this.DeliveryPartnerServices = deliveryPartnerServices.Select(o => new DeliveryPartnerViewModel(o, deliveryPartnerService)).ToArray();
		}
		#endregion

		#region Methods

		#endregion
	}
}
