namespace CJG.Web.External.Helpers
{
	/// <summary>
	/// <typeparamref name="BooleanExtensions"/> static class, provides extension methods for booleans.
	/// </summary>
	public static class BooleanExtensions
	{
		public static string ToStringValue(this bool value, string trueValue = "Yes", string falseValue = "No")
		{
			return value ? trueValue : falseValue;
		}

		public static string ToStringValue(this bool? value, string trueValue = "Yes", string falseValue = "No", string nullString = "--")
		{
			if (!value.HasValue)
				return nullString;

			return value.Value ? trueValue : falseValue;
		}
	}
}