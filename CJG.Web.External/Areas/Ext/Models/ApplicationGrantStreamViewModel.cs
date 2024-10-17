using CJG.Application.Services;
using CJG.Core.Entities;
using System.Web;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ApplicationGrantStreamViewModel
	{
		#region Properties
		public int Id { get; set; }
		public int GrantProgramId { get; set; }
		public int? AccountCodeId { get; set; }
		public string Name { get; set; }
		public string FullName { get; set; }
		public string Criteria { get; set; }
		public string Objective { get; set; }
		public decimal MaxReimbursementAmt { get; set; }
		public double ReimbursementRate { get; set; }
		public double DefaultDeniedRate { get; set; }
		public double DefaultWithdrawnRate { get; set; }
		public double DefaultReductionRate { get; set; }
		public double DefaultSlippageRate { get; set; }
		public double DefaultCancellationRate { get; set; }
		public bool IsActive { get; set; }
		public bool IncludeDeliveryPartner { get; set; }
		public bool CanApplicantReportParticipants { get; set; }
		public ProgramTypes ProgramTypeId { get; set; }
		#endregion

		#region Constructors
		public ApplicationGrantStreamViewModel()
		{
		}

		public ApplicationGrantStreamViewModel(GrantStream grantStream)
		{
			Utilities.MapProperties(grantStream, this);
			Objective = HttpUtility.UrlDecode(Objective);
			this.ProgramTypeId = grantStream.GrantProgram.ProgramTypeId;
		}
		#endregion
	}
}
