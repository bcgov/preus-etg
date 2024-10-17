using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
	public class ApplicationDeliveryViewModel : BaseViewModel
	{
		#region Properties
		public string NextStepUrl { get; set; }
		public string PreviousStepUrl { get; set; }
		public int CurrentStep { get; set; }
		public int Steps { get; set; } = 2;

		[Required(ErrorMessage = "An answer is required")]
		public bool? UsedDeliveryPartner { get; set; }
		public int? DeliveryPartnerId { get; set; }
		public IEnumerable<int> SelectedDeliveryPartnerServices { get; set; } = new List<int>();
		public IEnumerable<CollectionItemModel> DeliveryPartners { get; set; }
		public IEnumerable<CollectionItemModel> DeliveryPartnerServices { get; set; }
		public string RowVersion { get; set; }
		#endregion

		#region Constructors
		public ApplicationDeliveryViewModel()
		{
		}

		public ApplicationDeliveryViewModel(GrantApplication grantApplication)
		{
			var grantStream = grantApplication.GrantOpening.GrantStream;
			var grantProgram = grantStream.GrantProgram;

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			if (grantApplication.DeliveryPartnerServices != null)
			{
				this.DeliveryPartnerId = grantApplication.DeliveryPartnerId;
				this.SelectedDeliveryPartnerServices = grantApplication.DeliveryPartnerServices.Select(x => x.Id).ToArray();
			}
			this.UsedDeliveryPartner = DeliveryPartnerId.HasValue; 
			
			this.DeliveryPartners = grantProgram.DeliveryPartners.Where(x => x.IsActive).Select(x => new CollectionItemModel(x, "", false));
			this.DeliveryPartnerServices = grantProgram.DeliveryPartnerServices.Where(x => x.IsActive).Select(x => new CollectionItemModel(x, "", false));

			if (grantProgram.ProgramTypeId == ProgramTypes.WDAService)
			{
				this.Steps += 1;

				this.Steps = this.Steps + grantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).Count();

				if (grantStream.ProgramConfiguration.EligibleExpenseTypes.Any(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && x.IsActive))
				{
					this.Steps += 1;
				}

				this.Steps += 1;
			}

			if (grantStream.IncludeDeliveryPartner)
			{
				this.Steps += 1;
			}

			this.CurrentStep = this.Steps - 1;

			this.NextStepUrl = string.Format("/Ext/Application/Review/Applicant/Declaration/View/{0}", grantApplication.Id);
			if (grantProgram.ProgramTypeId == ProgramTypes.WDAService)
			{
				this.PreviousStepUrl = string.Format("/Ext/Application/Review/Training/Cost/View/{0}", grantApplication.Id);
			}
			else
			{
				this.PreviousStepUrl = string.Format("/Ext/Application/Review/View/{0}", grantApplication.Id);
			}
		}
		#endregion
	}
}
