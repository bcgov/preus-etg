using CJG.Core.Entities;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CJG.Web.External.Helpers.Validation
{
	/// <summary>
	/// Validates the property and ensures that at least one item/value has been selected in the enumerable/array.
	/// </summary>
	public class RequiredEnumerableAttribute : ValidationAttribute
	{
		#region Methods
		public override bool IsValid(object value)
		{
			if (value == null || !value.GetType().IsEnumerable()) return false;
			IEnumerable<object> values = ((IEnumerable)value).Cast<object>();
			return values != null ? values.Count() > 0 : false;
		}
		#endregion
	}
}