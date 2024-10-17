using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Models.Shared.TrainingProviders
{
	public class TrainingProviderAttachmentViewModel
	{
		#region Properties
		public int Id { get; set; }
		public int GrantApplicationId { get; set; }
		public int TrainingProviderId { get; set; }
		public TrainingProviderAttachmentTypes Type { get; set; }
		public int? Index { get; set; }
		public string FileName { get; set; }
		public string Description { get; set; }
		public string RowVersion { get; set; }
		public string TrainingProgramRowVersion { get; set; }
		public string TrainingProviderRowVersion { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderAttachmentModel"/> object.
		/// </summary>
		public TrainingProviderAttachmentViewModel() { }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderAttachmentModel"/> object and initializes it.
		/// </summary>
		/// <param name="type"></param>
		public TrainingProviderAttachmentViewModel(TrainingProviderAttachmentTypes type)
		{
			this.Type = type;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderAttachmentModel"/> object and initializes it.
		/// </summary>
		/// <param name="attachment"></param>
		/// <param name="trainingProviderId"></param>
		/// <param name="type"></param>
		/// <param name="trainingProviderRowVersion"></param>
		public TrainingProviderAttachmentViewModel(Attachment attachment, int trainingProviderId, TrainingProviderAttachmentTypes type, string trainingProviderRowVersion) : this(type)
		{
			this.TrainingProviderId = trainingProviderId;
			this.TrainingProviderRowVersion = trainingProviderRowVersion;

			if (attachment != null)
			{
				this.Id = attachment.Id;
				this.RowVersion = Convert.ToBase64String(attachment.RowVersion);
				this.FileName = attachment.FileName;
				this.Description = attachment.Description;
			}
		}
		
		public TrainingProviderAttachmentViewModel(Attachment attachment, TrainingProvider trainingProvider, TrainingProviderAttachmentTypes type) : this(type)
		{
			if (trainingProvider != null)
			{
				this.TrainingProviderId = trainingProvider.Id;
				this.TrainingProviderRowVersion = Convert.ToBase64String(trainingProvider.RowVersion);
				this.TrainingProgramRowVersion = Convert.ToBase64String(trainingProvider.TrainingProgram.RowVersion);
			}

			if (attachment != null)
			{
				this.Id = attachment.Id;
				this.RowVersion = Convert.ToBase64String(attachment.RowVersion);
				this.FileName = attachment.FileName;
				this.Description = attachment.Description;
			}
		}
		#endregion
	}
}
