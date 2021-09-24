using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJG.Application.Business.Models
{
	public class GrantProgramApplicantModel
	{
		public int GrantProgramId { get; set; }
		public string GrantProgramName { get; set; }
		public int NumberOfSubscribers { get; set; }
		public int NumberOfApplicants { get; set; }

		public GrantProgramApplicantModel()
		{
		}

		public GrantProgramApplicantModel(GrantProgram grantProgram, int numberOfApplicants)
		{
			if (grantProgram == null) throw new ArgumentNullException(nameof(grantProgram));

			this.GrantProgramId = grantProgram.Id;
			this.GrantProgramName = grantProgram.Name;
			this.NumberOfSubscribers = grantProgram.UserGrantProgramPreferences.Count(p => p.IsSelected);
			this.NumberOfApplicants = numberOfApplicants;
		}
	}
}
