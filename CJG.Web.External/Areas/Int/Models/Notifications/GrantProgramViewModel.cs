using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using System;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class GrantProgramViewModel
	{
		#region Properties
		public int Id { get; set; }

		public string Name { get; set; }

		public int ApplicantsWithApplications { get; set; }

		public int SubscribedApplicants { get; set; }
		#endregion

		#region Constructors
		public GrantProgramViewModel()
		{

		}

		public GrantProgramViewModel(GrantProgram grantProgram, IGrantProgramService grantProgramService)
		{
			this.Id = grantProgram?.Id ?? throw new ArgumentNullException(nameof(grantProgram));
			this.Name = grantProgram.Name;
			this.ApplicantsWithApplications = grantProgramService?.CountApplicantsWithApplications(grantProgram.Id) ?? throw new ArgumentNullException(nameof(grantProgramService));
			this.SubscribedApplicants = grantProgramService.CountSubscribedApplicants(grantProgram.Id);
		}
		#endregion
	}
}