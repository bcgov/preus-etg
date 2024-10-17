using System;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests
{
	internal static class JsonResultTestHelper
	{
		public static T GetJsonValue<T>(this JsonResult jsonResult, string propertyName)
		{
			var property = jsonResult.Data.GetType()
				.GetProperties()
				.FirstOrDefault(p => string.Compare(p.Name, propertyName) == 0);

			if (null == property)
				throw new ArgumentException("propertyName not found", "propertyName");

			return (T)property.GetValue(jsonResult.Data, null);
		}
	}
}
