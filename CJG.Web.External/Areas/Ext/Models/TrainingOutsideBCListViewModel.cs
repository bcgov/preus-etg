using CJG.Core.Entities;
using CJG.Web.External.Areas.Int.Models.Attachments;
using System;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class TrainingOutsideBCListViewModel
	{
		public int ApplicationId { get; set; }
		public int TrainingProviderId { get; set; }
		public bool? TrainingOutsideBC { get; set; }
		public string BusinessCase { get; set; }
		public AttachmentViewModel BusinessCaseDocument { get; set; }

		public TrainingOutsideBCListViewModel()
		{
		}

		public TrainingOutsideBCListViewModel(TrainingProvider trainingProvider)
		{
			this.ApplicationId = trainingProvider.GrantApplicationId ?? trainingProvider.TrainingProgram?.GrantApplicationId ?? throw new ArgumentException("Training provider doesn't reference a grant application.");
			this.TrainingProviderId = trainingProvider.Id;
			this.TrainingOutsideBC = trainingProvider.Id == 0 ? null : (bool?)(trainingProvider.TrainingOutsideBC ? true : false);
			this.BusinessCase = trainingProvider.BusinessCase;

			if (trainingProvider.BusinessCaseDocument != null)
			{
				var doc = trainingProvider.BusinessCaseDocument;
				this.BusinessCaseDocument = new AttachmentViewModel
				{
					Id = doc.Id,
					FileName = doc.FileName,
					FileExtension = doc.FileExtension
				};
			}
		}
	}
}
