using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Models.Shared.TrainingProviders
{
	public class TrainingCostAttachmentViewModel
	{
		public int Id { get; set; }
		public int GrantApplicationId { get; set; }
		public int TrainingCostId { get; set; }
		public int? Index { get; set; }
		public string FileName { get; set; }
		public string Description { get; set; }
		public string RowVersion { get; set; }
		public string TrainingCostRowVersion { get; set; }

		public TrainingCostAttachmentViewModel() { }

		public TrainingCostAttachmentViewModel(Attachment attachment, int trainingCostId, string rowVersion)
		{
			TrainingCostId = trainingCostId;

			if (attachment == null)
				return;

			Id = attachment.Id;
			RowVersion = Convert.ToBase64String(attachment.RowVersion);
			FileName = attachment.FileName;
			Description = attachment.Description;
		}
	}
}