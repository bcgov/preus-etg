using System;
using System.Data.Entity;

namespace CJG.Application.Services.Helpers
{
	public static class DbFunctions
	{
		[DbFunction("Edm", "AddDays")]
		public static DateTime? AddDays(DateTime? dateValue, int? addValue)
		{
			if (!dateValue.HasValue || !addValue.HasValue) return null;

			return dateValue.Value.AddDays(addValue.Value);
		}
	}
}
