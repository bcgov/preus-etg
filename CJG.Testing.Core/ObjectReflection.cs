using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CJG.Testing.Core
{
	public static class ObjectReflection
	{
		/// <summary>
		/// Get Reflected Property Value
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static object GetReflectedProperty(this object obj, string propertyName)
		{
			if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

			var property = obj.GetType().GetProperty(propertyName);
			if (property == null) return null;
			return property.GetValue(obj, null);
		}

		/// <summary>
		/// Get Reflected Property Value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static T GetReflectedProperty<T>(this object obj, string propertyName)
		{
			if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

			var property = obj.GetType().GetProperty(propertyName);
			if (property == null) return default;
			return (T)property.GetValue(obj, null);
		}

		/// <summary>
		/// Get Reflected Property Value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static PT GetReflectedProperty<T, PT>(this T obj, Expression<Func<T, PT>> prop)
		{
			if (prop == null) throw new ArgumentNullException(nameof(prop));

			var type = obj.GetType();
			var member = prop.Body as MemberExpression;
			if (member == null) throw new ArgumentException($"Expression '{prop.ToString()}' must reference a property.", nameof(prop));

			var propInfo = member.Member as PropertyInfo;
			if (propInfo == null) throw new ArgumentException($"Expression '{prop.ToString()}' must reference a property.", nameof(prop));
			if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType)) throw new ArgumentException($"Expression '{prop.ToString()}' must reference an existing property.", nameof(prop));

			var propertyName = propInfo.Name;

			var property = obj.GetType().GetProperty(propertyName);
			if (property == null) return default;
			return (PT)property.GetValue(obj, null);
		}
	}
}
