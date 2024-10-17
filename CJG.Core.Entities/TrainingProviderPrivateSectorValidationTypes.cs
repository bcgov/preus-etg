namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="TrainingProviderPrivateSectorValidationTypes"/> enum, provides all the private sector validation types for training providers.
	/// </summary>
	public enum TrainingProviderPrivateSectorValidationTypes
	{
		/// <summary>
		/// Course outline and proof of qualifications not required
		/// </summary>
		Never = 0,
		/// <summary>
		/// Course outline and proof of qualifications required
		/// </summary>
		Always = 1,
		/// <summary>
		/// Course outline and proof of qualifications required as of setting date
		/// </summary>
		ByDateSetting = 2
	}
}
