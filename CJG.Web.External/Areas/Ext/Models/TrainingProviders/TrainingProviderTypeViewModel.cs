using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.TrainingProviders
{
	public class TrainingProviderTypeViewModel
	{
		public int Id { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public int RowSequence { get; set; }
		public TrainingProviderPrivateSectorValidationTypes PrivateSectorValidationType { get; set; }
		public int ProofOfInstructorQualifications { get; set; }
		public int CourseOutline { get; set; }

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
			Id = trainingProviderType.Id;
			Caption = trainingProviderType.Caption;
			RowSequence = trainingProviderType.RowSequence;
			PrivateSectorValidationType = trainingProviderType.PrivateSectorValidationType;
			ProofOfInstructorQualifications = trainingProviderType.ProofOfInstructorQualifications;
			CourseOutline = trainingProviderType.CourseOutline;
		}
	}
}
