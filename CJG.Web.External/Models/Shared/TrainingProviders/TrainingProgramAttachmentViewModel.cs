using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Models.Shared.TrainingProviders
{
	public class TrainingProgramAttachmentViewModel
	{
		public int Id { get; set; }
		public int GrantApplicationId { get; set; }
		public int TrainingProgramId { get; set; }
		public int? Index { get; set; }
		public string FileName { get; set; }
		public string Description { get; set; }
		public string RowVersion { get; set; }
		public string TrainingProgramRowVersion { get; set; }

		public TrainingProgramAttachmentViewModel() { }

		public TrainingProgramAttachmentViewModel(Attachment attachment, int trainingProgramId, string trainingProviderRowVersion)
		{
			TrainingProgramId = trainingProgramId;

			if (attachment == null)
				return;

			Id = attachment.Id;
			RowVersion = Convert.ToBase64String(attachment.RowVersion);
			FileName = attachment.FileName;
			Description = attachment.Description;
		}
		
		public TrainingProgramAttachmentViewModel(Attachment attachment, TrainingProgram trainingProgram)
		{
			if (trainingProgram != null)
			{
				TrainingProgramRowVersion = Convert.ToBase64String(trainingProgram.RowVersion);
			}

			if (attachment == null)
				return;

			Id = attachment.Id;
			RowVersion = Convert.ToBase64String(attachment.RowVersion);
			FileName = attachment.FileName;
			Description = attachment.Description;
		}
	}
}
