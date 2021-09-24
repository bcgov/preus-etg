using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
	public class ApplicantDeclarationViewModel : BaseViewModel
	{
		#region Properties
		public string GrantProgramName { get; set; }
		public string GrantProgramCode { get; set; }
		public ProgramTypes ProgramType { get; set; }
		public int Steps { get; set; } = 2;
		public string Body { get; set; }
		public bool DeclarationConfirmed { get; set; }
		public string Applicant { get; set; }
		public string RowVersion { get; set; }
		public string PreviousStepUrl { get; set; }
		#endregion

		#region Constructors
		public ApplicantDeclarationViewModel()
		{

		}

		public ApplicantDeclarationViewModel(GrantApplication grantApplication, User applicant, IGrantProgramService grantProgramService)
		{
			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.Applicant = applicant.GetUserFullName();
			var program = grantApplication.GrantOpening.GrantStream.GrantProgram;
			this.GrantProgramCode = program.ProgramCode;
			this.ProgramType = program.ProgramTypeId;
			this.GrantProgramName = program.Name;

			var applicatDeclaration = grantProgramService.GenerateApplicantDeclarationBody(grantApplication);

			if (string.IsNullOrEmpty(applicatDeclaration)) throw new InvalidOperationException("Unable to find the application declaration page.");

			this.Body = applicatDeclaration;

			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
			{
				this.Steps += 1;

				this.Steps = this.Steps + grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && x.IsActive).Count();

				if (grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Any(x => x.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && x.IsActive))
				{
					this.Steps += 1;
				}

				this.Steps += 1;

			}

			if (grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner)
			{
				this.Steps += 1;
				this.PreviousStepUrl = string.Format("/Ext/Application/Review/Delivery/Partner/View/{0}", grantApplication.Id);
			}
			else
			{
				if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService)
				{

					this.PreviousStepUrl = string.Format("/Ext/Application/Review/Training/Cost/View/{0}", grantApplication.Id);
				}
				else
				{
					this.PreviousStepUrl = string.Format("/Ext/Application/Review/View/{0}", grantApplication.Id);
				}
			}

		}
		#endregion
	}
}
