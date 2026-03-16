namespace CJG.Core.Entities
{
	// This is a partial enum to use instead of magic numbers. Add values when needed.
	public enum TrainingObjectives
	{
		ApprenticeshipTraining = 3
	}

	public class TrainingObjective : LookupTable<int>
	{
		public TrainingObjective()
		{
			
		}

		public TrainingObjective(string caption, int rowSequence = 0) : base(caption, rowSequence)
		{

		}
	}
}