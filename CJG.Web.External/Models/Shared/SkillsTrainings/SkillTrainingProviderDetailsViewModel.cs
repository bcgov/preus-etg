using CJG.Core.Entities;

namespace CJG.Web.External.Models.Shared.SkillsTrainings
{
	public class SkillTrainingProviderDetailsViewModel : TrainingProviderDetailsViewModel
	{
		#region Properties
		public bool IsApproved { get; set; }
		public int EligibleCostBreakdownId { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="SkillTrainingProviderModel"/> object.
		/// </summary>
		public SkillTrainingProviderDetailsViewModel()
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="SkillTrainingProviderModel"/> object and initializes it.
		/// </summary>
		/// <param name="trainingProvider"></param>
		public SkillTrainingProviderDetailsViewModel(TrainingProvider trainingProvider) : base(trainingProvider)
		{
		}
		#endregion
	}
}
