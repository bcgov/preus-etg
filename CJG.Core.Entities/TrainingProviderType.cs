namespace CJG.Core.Entities
{
	/// <summary>
	/// TrainingProviderType class, provides a way to manage a list of valid training provider type.
	/// </summary>
	/// <example>
	///     Public Post-Secondary Institution
	///     BC School District Training Organization
	///     Registered Trade/Technical School
	///     Union Hall
	///     Industry Recognized Safety Trainer
	///     BC Private Post-Secondary Institution - Registered w PCTIA
	///     Private Training Provider - Registered w PCTIA
	///     Private Training Provider - Not Registered w PCTIA
	/// </example>
	public class TrainingProviderType : LookupTable<int>
	{
		#region Properties
		/// <summary>
		/// get/set - If the training provider is a private sector type.
		/// </summary>
		public TrainingProviderPrivateSectorValidationTypes PrivateSectorValidationType { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderType"/> object.
		/// </summary>
		public TrainingProviderType() : base()
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderType"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="rowSequence"></param>
		public TrainingProviderType(string caption, int rowSequence = 0) : base(caption, rowSequence)
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderType"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="validationType"></param>
		/// <param name="rowSequence"></param>
		public TrainingProviderType(string caption, TrainingProviderPrivateSectorValidationTypes validationType, int rowSequence = 0) : this(caption, rowSequence)
		{
			this.PrivateSectorValidationType = validationType;
		}
		#endregion
	}
}
