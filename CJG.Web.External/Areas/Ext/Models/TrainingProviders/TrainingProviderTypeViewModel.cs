using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.TrainingProviders
{
	public class TrainingProviderTypeViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public int RowSequence { get; set; }
		public TrainingProviderPrivateSectorValidationTypes PrivateSectorValidationType { get; set; }
		public int ProofOfInstructorQualifications { get; set; }
		public int CourseOutline { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderTypeViewModel"/> object.
		/// </summary>
		public TrainingProviderTypeViewModel()
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderTypeViewModel"/> object and initalizes it.
		/// </summary>
		/// <param name="trainingProviderType"></param>
		public TrainingProviderTypeViewModel(TrainingProviderType trainingProviderType)
		{
			this.Id = trainingProviderType.Id;
			this.Caption = trainingProviderType.Caption;
			this.RowSequence = trainingProviderType.RowSequence;
			this.PrivateSectorValidationType = trainingProviderType.PrivateSectorValidationType;
			this.ProofOfInstructorQualifications = trainingProviderType.ProofOfInstructorQualifications;
			this.CourseOutline = trainingProviderType.CourseOutline;
		}
		#endregion
	}
}
