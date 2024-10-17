using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramDocumentTemplatesViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }

		public int ApplicantDeclarationTemplateId { get; set; }
		public string ApplicantDeclarationTemplateRowVersion { get; set; }
		public string ApplicantDeclarationTemplate { get; set; }


		public int ApplicantCoverLetterTemplateId { get; set; }
		public string ApplicantCoverLetterTemplateRowVersion { get; set; }
		public string ApplicantCoverLetterTemplate { get; set; }


		public int ApplicantScheduleATemplateId { get; set; }
		public string ApplicantScheduleATemplateRowVersion { get; set; }
		public string ApplicantScheduleATemplate { get; set; }


		public int ApplicantScheduleBTemplateId { get; set; }
		public string ApplicantScheduleBTemplateRowVersion { get; set; }
		public string ApplicantScheduleBTemplate { get; set; }


		public int ParticipantConsentTemplateId { get; set; }
		public string ParticipantConsentTemplateRowVersion { get; set; }
		public string ParticipantConsentTemplate { get; set; }
		#endregion

		#region Constructors
		public GrantProgramDocumentTemplatesViewModel() { }

		public GrantProgramDocumentTemplatesViewModel(GrantProgram grantProgram)
		{
			if (grantProgram == null) throw new ArgumentNullException(nameof(grantProgram));

			this.Id = grantProgram.Id;
			this.RowVersion = Convert.ToBase64String(grantProgram.RowVersion);

			this.ApplicantDeclarationTemplateId = grantProgram.ApplicantDeclarationTemplateId;
			this.ApplicantDeclarationTemplateRowVersion = Convert.ToBase64String(grantProgram.ApplicantDeclarationTemplate.RowVersion);
			this.ApplicantDeclarationTemplate = grantProgram.ApplicantDeclarationTemplate.Body;

			this.ApplicantCoverLetterTemplateId = grantProgram.ApplicantCoverLetterTemplateId;
			this.ApplicantCoverLetterTemplateRowVersion = Convert.ToBase64String(grantProgram.ApplicantCoverLetterTemplate.RowVersion);
			this.ApplicantCoverLetterTemplate = grantProgram.ApplicantCoverLetterTemplate.Body;

			this.ApplicantScheduleATemplateId = grantProgram.ApplicantScheduleATemplateId;
			this.ApplicantScheduleATemplateRowVersion = Convert.ToBase64String(grantProgram.ApplicantScheduleATemplate.RowVersion);
			this.ApplicantScheduleATemplate = grantProgram.ApplicantScheduleATemplate.Body;

			this.ApplicantScheduleBTemplateId = grantProgram.ApplicantScheduleBTemplateId;
			this.ApplicantScheduleBTemplateRowVersion = Convert.ToBase64String(grantProgram.ApplicantScheduleBTemplate.RowVersion);
			this.ApplicantScheduleBTemplate = grantProgram.ApplicantScheduleBTemplate.Body;

			this.ParticipantConsentTemplateId = grantProgram.ParticipantConsentTemplateId;
			this.ParticipantConsentTemplateRowVersion = Convert.ToBase64String(grantProgram.ParticipantConsentTemplate.RowVersion);
			this.ParticipantConsentTemplate = grantProgram.ParticipantConsentTemplate.Body;

		}
		#endregion
	}
}