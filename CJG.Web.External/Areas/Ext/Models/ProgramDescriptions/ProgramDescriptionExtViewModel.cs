using System.Collections.Generic;
using System.Linq;
using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;

namespace CJG.Web.External.Areas.Ext.Models.ProgramDescriptions
{
	// TODO: Redo viewmodel as it's pulling way too much info.
	public class ProgramDescriptionExtViewModel : ProgramDescriptionViewModel
	{
		public IEnumerable<KeyValuePair<int, string>> ParticipantEmploymentStatuses { get; set; }
		public IEnumerable<KeyValuePair<int, string>> ApplicantOrganizationTypes { get; set; }
		public IEnumerable<KeyValuePair<int, string>> VulnerableGroups { get; set; }
		public IEnumerable<KeyValuePair<int, string>> UnderRepresentedPopulations { get; set; }
		public IEnumerable<KeyValuePair<int, string>> Communities { get; set; }
		public IEnumerable<KeyValuePair<int, string>> Naics1Codes { get; set; } = new List<KeyValuePair<int, string>>();
		public IEnumerable<KeyValueParent<int, string, int>> Naics2Codes { get; set; } = new List<KeyValueParent<int, string, int>>();
		public IEnumerable<KeyValueParent<int, string, int>> Naics3Codes { get; set; } = new List<KeyValueParent<int, string, int>>();
		public IEnumerable<KeyValueParent<int, string, int>> Naics4Codes { get; set; } = new List<KeyValueParent<int, string, int>>();
		public IEnumerable<KeyValueParent<int, string, int>> Naics5Codes { get; set; } = new List<KeyValueParent<int, string, int>>();
		public IEnumerable<KeyValuePair<int, string>> Noc1Codes { get; set; } = new List<KeyValuePair<int, string>>();
		public IEnumerable<KeyValueParent<int, string, int>> Noc2Codes { get; set; } = new List<KeyValueParent<int, string, int>>();
		public IEnumerable<KeyValueParent<int, string, int>> Noc3Codes { get; set; } = new List<KeyValueParent<int, string, int>>();
		public IEnumerable<KeyValueParent<int, string, int>> Noc4Codes { get; set; } = new List<KeyValueParent<int, string, int>>();
		public IEnumerable<KeyValueParent<int, string, int>> Noc5Codes { get; set; } = new List<KeyValueParent<int, string, int>>();

		public ProgramDescriptionExtViewModel()
		{
		}

		public ProgramDescriptionExtViewModel(
			ProgramDescription programDescription,
			IStaticDataService staticDataService,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService, 
			INationalOccupationalClassificationService nationalOccupationalClassificationService, 
			ICommunityService communityService) : base(programDescription, naIndustryClassificationSystemService, nationalOccupationalClassificationService)
		{
			PopulateListItems(staticDataService, naIndustryClassificationSystemService, nationalOccupationalClassificationService, communityService);
		}

		private void PopulateListItems(IStaticDataService staticDataService, INaIndustryClassificationSystemService naIndustryClassificationSystemService, INationalOccupationalClassificationService nationalOccupationalClassificationService, ICommunityService communityService)
		{
			ParticipantEmploymentStatuses = staticDataService.GetParticipantEmploymentStatuses().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
			ApplicantOrganizationTypes = staticDataService.GetApplicantOrganizationTypes().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
			Communities = communityService.GetAll().Where(t => t.IsActive).Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
			VulnerableGroups = staticDataService.GetVulnerableGroups().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
			UnderRepresentedPopulations = staticDataService.GetUnderRepresentedPopulations().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();

			// NAICS has maximum of 5 levels
			for (int i = 1; i <= 5; i++)
			{
				var property = GetType().GetProperty($"Naics{i}Codes");
				if (i == 1)  // first level do not have parent
					property?.SetValue(this, naIndustryClassificationSystemService
							 .GetNaIndustryClassificationSystemLevel(i)
							 .Select(n => new KeyValuePair<int, string>(n.Id, $"{n.Code} | {n.Description}")).ToArray());
				else
					property?.SetValue(this, naIndustryClassificationSystemService
							 .GetNaIndustryClassificationSystemLevel(i)
							 .Select(n => new KeyValueParent<int, string, int>(n.Id, $"{n.Code} | {n.Description}", n.ParentId ?? 0)).ToArray());
			}
			// NOC has maximum of 5 levels
			for (int i = 1; i <= 5; i++)
			{
				var property = GetType().GetProperty($"Noc{i}Codes");
				if (i == 1) // first level do not have parent
					property?.SetValue(this, nationalOccupationalClassificationService
							 .GetNationalOccupationalClassificationLevel(i)
							 .Select(n => new KeyValuePair<int, string>(n.Id, $"{n.Code} | {n.Description}")).ToArray());
				else
					property?.SetValue(this, nationalOccupationalClassificationService
							 .GetNationalOccupationalClassificationLevel(i)
							 .Select(n => new KeyValueParent<int, string, int>(n.Id, $"{n.Code} | {n.Description}", n.ParentId ?? 0)).ToArray());
			}
		}
	}
}
