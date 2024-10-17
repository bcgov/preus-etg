using System;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="GrantStreamExtensions"/> static class, provides extension methods for grant streams.
	/// </summary>
	public static class GrantStreamExtensions
	{
		/// <summary>
		/// Indicates whether or not the grant stream has changed.
		/// </summary>
		/// <param name="originalGrantStream"></param>
		/// <param name="updatedGrantStream"></param>
		/// <returns>Returns a bool to indicate if any properties of the grant stream have changed.</returns>
		public static bool HasChanged(this GrantStream originalGrantStream, GrantStream updatedGrantStream)
		{
			return !originalGrantStream.MaxReimbursementAmt.IsApproximately(updatedGrantStream.MaxReimbursementAmt) ||
						!originalGrantStream.ReimbursementRate.IsApproximately(updatedGrantStream.ReimbursementRate) ||
						originalGrantStream.Objective != updatedGrantStream.Objective;
		}

		/// <summary>
		/// Indicates whether or not the stream properties have changed.
		/// </summary>
		/// <param name="originalGrantStream"></param>
		/// <param name="updatedGrantStream"></param>
		/// <returns>Returns a bool to indicate if any properties of the stream have changed.</returns>
		public static bool InstancePropertiesHaveChanged(this GrantStream originalGrantStream, GrantStream updatedGrantStream)
		{
			return !Math.Round(originalGrantStream.DefaultDeniedRate, 3).IsApproximately(Math.Round(updatedGrantStream.DefaultDeniedRate, 3)) ||
					!Math.Round(originalGrantStream.DefaultWithdrawnRate, 3).IsApproximately(Math.Round(updatedGrantStream.DefaultWithdrawnRate, 3)) ||
					!Math.Round(originalGrantStream.DefaultReductionRate, 3).IsApproximately(Math.Round(updatedGrantStream.DefaultReductionRate, 3)) ||
					!Math.Round(originalGrantStream.DefaultSlippageRate, 3).IsApproximately(Math.Round(updatedGrantStream.DefaultSlippageRate, 3)) ||
					!Math.Round(originalGrantStream.DefaultCancellationRate, 3).IsApproximately(Math.Round(updatedGrantStream.DefaultCancellationRate, 3));
		}

		/// <summary>
		/// Determine if eligiblity requirements are required.
		/// </summary>
		/// <param name="grantStream"></param>
		/// <returns></returns>
		public static bool EligibilityRequired(this GrantStream grantStream)
		{
			return grantStream.EligibilityEnabled && grantStream.EligibilityRequired;
		}
	}
}
