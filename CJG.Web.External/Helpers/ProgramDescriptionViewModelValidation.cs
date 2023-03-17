using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;

namespace CJG.Web.External.Helpers
{
	public class ProgramDescriptionViewModelValidation
	{
		public static ValidationResult ValidateCommunities(List<int> CommunityIds, ValidationContext context)
		{
			if (CommunityIds?.Count() == 0)
				return new ValidationResult("Community Names are required.");

			return ValidationResult.Success;
		}

		public static ValidationResult ValidateNAICS(int? NaicsLevelId, ValidationContext context)
		{
			if (context.ObjectInstance is Areas.Ext.Models.ProgramDescriptions.ProgramDescriptionViewModel extModel)
			{
				switch (context.MemberName)
				{
					case "Naics1Id":
						if (NaicsLevelId == null) return new ValidationResult("NAICS is required to three levels.");
						break;
					case "Naics2Id":
						if (NaicsLevelId == null && extModel.Naics1Id != null) return new ValidationResult("NAICS is required to three levels.");
						break;
					case "Naics3Id":
						if (NaicsLevelId == null && extModel.Naics2Id != null) return new ValidationResult("NAICS is required to three levels.");
						break;
				}
			}
			else if (context.ObjectInstance is Areas.Int.Models.ProgramDescriptions.ProgramDescriptionViewModel intModel)
			{
				switch (context.MemberName)
				{
					case "Naics1Id":
						if (NaicsLevelId == null) return new ValidationResult("NAICS is required to three levels.");
						break;
					case "Naics2Id":
						if (NaicsLevelId == null && intModel.Naics1Id != null) return new ValidationResult("NAICS is required to three levels.");
						break;
					case "Naics3Id":
						if (NaicsLevelId == null && intModel.Naics2Id != null) return new ValidationResult("NAICS is required to three levels.");
						break;
				}
			}
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateNOC(int? NOCLevelId, ValidationContext context)
		{
			var configNocSetting = ConfigurationManager.AppSettings["NocVersion"];

			if (configNocSetting != "2016" && configNocSetting != "2021")
				configNocSetting = "2016";

			var nocIsRequiredToXLevels = "NOC is required to four levels.";
			if (configNocSetting == "2021")
				nocIsRequiredToXLevels = "NOC is required to five levels.";

			if (context.ObjectInstance is Areas.Ext.Models.ProgramDescriptions.ProgramDescriptionViewModel extModel)
			{
				switch (context.MemberName)
				{
					case "Noc1Id":
						if (NOCLevelId == null)
							return new ValidationResult(nocIsRequiredToXLevels);
						break;
					case "Noc2Id":
						if (NOCLevelId == null && extModel.Noc1Id != null)
							return new ValidationResult(nocIsRequiredToXLevels);
						break;
					case "Noc3Id":
						if (NOCLevelId == null && extModel.Noc2Id != null)
							return new ValidationResult(nocIsRequiredToXLevels);
						break;
					case "Noc4Id":
						if (NOCLevelId == null && extModel.Noc3Id != null)
							return new ValidationResult(nocIsRequiredToXLevels);
						break;
					case "Noc5Id":
						if (configNocSetting == "2016")
							break;
						if (NOCLevelId == null && extModel.Noc4Id != null)
							return new ValidationResult(nocIsRequiredToXLevels);

						break;
				}
			}
			else if (context.ObjectInstance is Areas.Int.Models.ProgramDescriptions.ProgramDescriptionViewModel intModel)
			{
				switch (context.MemberName)
				{
					case "Noc1Id":
						if (NOCLevelId == null)
							return new ValidationResult(nocIsRequiredToXLevels);
						break;
					case "Noc2Id":
						if (NOCLevelId == null && intModel.Noc1Id != null)
							return new ValidationResult(nocIsRequiredToXLevels);
						break;
					case "Noc3Id":
						if (NOCLevelId == null && intModel.Noc2Id != null)
							return new ValidationResult(nocIsRequiredToXLevels);
						break;
					case "Noc4Id":
						if (NOCLevelId == null && intModel.Noc3Id != null)
							return new ValidationResult(nocIsRequiredToXLevels);
						break;
					case "Noc5Id":
						if (configNocSetting == "2016")
							break;
						if (NOCLevelId == null && intModel.Noc4Id != null)
							return new ValidationResult(nocIsRequiredToXLevels);
						break;
				}
			}
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateCIPS(int? CipsCodeId, ValidationContext context)
		{
			if (context.ObjectInstance is Areas.Int.Models.TrainingPrograms.TrainingProgramViewModel ETGTrainingProgramModel)
			{
				switch (context.MemberName)
				{
					case "CipsCode1Id":
						if (CipsCodeId == null) return new ValidationResult("CIPS is required to three levels.");
						break;
					case "CipsCode2Id":
						if (CipsCodeId == null && ETGTrainingProgramModel.CipsCode1Id != null) return new ValidationResult("CIPS is required to three levels.");
						break;
					case "CipsCode3Id":
						if (CipsCodeId == null && ETGTrainingProgramModel.CipsCode2Id != null) return new ValidationResult("CIPS is required to three levels.");
						break;
				}
			}
			else if (context.ObjectInstance is Areas.Int.Models.SkillsTraining.SkillsTrainingProgramViewModel WDASkillTrainingProgramModel)
			{
				switch (context.MemberName)
				{
					case "CipsCode1Id":
						if (CipsCodeId == null) return new ValidationResult("CIPS is required to three levels.");
						break;
					case "CipsCode2Id":
						if (CipsCodeId == null && WDASkillTrainingProgramModel.CipsCode1Id != null) return new ValidationResult("CIPS is required to three levels.");
						break;
					case "CipsCode3Id":
						if (CipsCodeId == null && WDASkillTrainingProgramModel.CipsCode2Id != null) return new ValidationResult("CIPS is required to three levels.");
						break;
				}
			}
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateParticipantEmploymentStatuses(List<int> Ids, ValidationContext context)
		{
			if (Ids?.Count() == 0)
				return new ValidationResult("You must select at least one participant employment status.");

			return ValidationResult.Success;
		}
	}
}