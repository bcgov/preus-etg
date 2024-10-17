using System;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace CJG.Core.Entities.Helpers
{
	public struct PropertyChange
	{
		public string Name { get; }
		public object OldValue { get; }
		public object NewValue { get; }
		public EntityState State { get; }

		/// <summary>
		/// Creates a new instance of a PropertyChange object and initializes it.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		/// <param name="state"></param>
		public PropertyChange(string name, object oldValue, object newValue, EntityState state = EntityState.Modified)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Argument 'name' is required and cannot be null, empty or whitespace.");

			if (oldValue?.GetType() == typeof(DateTime) || oldValue?.GetType() == typeof(DateTime?))
				oldValue = ((DateTime)oldValue).ToLocalTime().ToString("yyyy-MM-dd");

			if (newValue?.GetType() == typeof(DateTime) || newValue?.GetType() == typeof(DateTime?))
				newValue = ((DateTime)newValue).ToLocalTime().ToString("yyyy-MM-dd");

			Name = SplitCamelCase(name);
			OldValue = oldValue;
			NewValue = newValue;
			State = state;
		}

		public override string ToString()
		{
			var oldValue = this.OldValue == null ? "null" : $"\"{this.OldValue.ToString().Replace("\"", "\\\"").Replace("\n", "").Replace("\r", "")}\"";
			var newValue = this.NewValue == null ? "null" : $"\"{this.NewValue.ToString().Replace("\"", "\\\"").Replace("\n", "").Replace("\r", "")}\"";
			return $"{{ \"name\": \"{this.Name}\", \"oldValue\": {oldValue}, \"newValue\": {newValue}, \"state\": \"{this.State.ToString("g")}\" }}";
		}

		private static string SplitCamelCase(string value)
		{
			return Regex.Replace(value, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
		}
	}
}
