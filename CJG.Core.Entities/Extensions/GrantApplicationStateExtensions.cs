using System.Linq;

namespace CJG.Core.Entities.Extensions
{
	public static class GrantApplicationStateExtensions
	{
		public static bool HasBeenReturnedToDraft(this GrantApplication grantApplication)
		{
			if (grantApplication?.StateChanges == null || !grantApplication.StateChanges.Any())
				return false;

			if (grantApplication.StateChanges.Count == 1)
				return false;

			var latestState = grantApplication.StateChanges
				.OrderByDescending(sc => sc.ChangedDate)
				.FirstOrDefault();

			if (latestState == null)
				return false;

			if (latestState.ToState != ApplicationStateInternal.Draft)
				return false;

			return true;
		}

		public static bool HasBeenReturnedToNew(this GrantApplication grantApplication)
		{
			if (grantApplication?.StateChanges == null || !grantApplication.StateChanges.Any())
				return false;

			if (grantApplication.StateChanges.Count == 1)
				return false;

			var newStates = grantApplication.StateChanges
				.Where(sc => sc.ToState == ApplicationStateInternal.New)
				.OrderByDescending(sc => sc.ChangedDate)
				.ToList();

			if (grantApplication.StateChanges.Count <= 1)
				return false;

			return newStates.Any(state => state.FromState != ApplicationStateInternal.Draft);
		}
	}
}