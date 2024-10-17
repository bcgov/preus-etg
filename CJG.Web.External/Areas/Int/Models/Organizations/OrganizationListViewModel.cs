using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using CJG.Core.Interfaces;

namespace CJG.Web.External.Areas.Int.Models.Organizations
{
	public class OrganizationListViewModel : BaseViewModel
	{
		#region Properties
		public int OrgId { get; set; }
		public bool RiskFlag { get; set; }
		public string LegalName { get; set; }
		public string DoingBusinessAs { get; set; }
		public decimal YTDRequested { get; set; } = 0;
		public decimal YTDApproved { get; set; } = 0;
		public decimal YTDPaid { get; set; } = 0;
		public string Notes { get; set; }
		public string RowVersion { get; set; }
		public int? YearEstablished { get; set; }
		public int? NumberOfEmployeesWorldwide { get; set; }
		public int? NumberOfEmployeesInBC { get; set; }
		public decimal? AnnualTrainingBudget { get; set; }
		public int? AnnualEmployeesTrained { get; set; }
		public int? OrganizationTypeId { get; set; }
		public int? LegalStructureId { get; set; }
		public int? Naics1Id { get; set; }
		public int? Naics2Id { get; set; }
		public int? Naics3Id { get; set; }
		public int? Naics4Id { get; set; }
		public int? Naics5Id { get; set; }
		public int? NaicsId { get { return Naics5Id ?? Naics4Id ?? Naics3Id ?? Naics2Id ?? Naics1Id; } }

		public int? HeadOfficeAddressId { get; set; }

		public AddressViewModel HeadOfficeAddress { get; set; }
		#endregion

		#region Constructors
		public OrganizationListViewModel() { }

		public OrganizationListViewModel(Organization organization, IOrganizationService organizationService, INaIndustryClassificationSystemService naIndustryClassificationSystemService)
		{
			if (organization == null) throw new ArgumentNullException(nameof(organization));

			Utilities.MapProperties(organization, this);

			this.OrgId = organization.Id;

			var ytdData = organizationService.GetOrganizationYTD(organization.Id);

			this.YTDRequested = ytdData.TotalRequested;
			this.YTDApproved = ytdData.TotalApproved;
			this.YTDPaid = ytdData.TotalPaid;

			var naics = naIndustryClassificationSystemService.GetNaIndustryClassificationSystems(organization.NaicsId);
			naics.ForEach(item =>
			{
				var property = this.GetType().GetProperty($"Naics{item.Level}Id");
				property?.SetValue(this,item.Id);
			});

			this.HeadOfficeAddress = organization.HeadOfficeAddress != null ? new AddressViewModel(organization.HeadOfficeAddress) : new AddressViewModel();
		}
		#endregion
	}
}
