using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CJG.Core.Entities
{
	public static class EntityExtensions
	{
		private readonly static ConcurrentDictionary<KeyValuePair<Type, string>, MethodInfo> _methods = new ConcurrentDictionary<KeyValuePair<Type, string>, MethodInfo>();

		private static Dictionary<object,
		Dictionary<string, object>>
		PropertyValues = new Dictionary<
			object, Dictionary<string, object>>();

		/// <summary>
		/// Cast the items within the enumerable object to the specified type and return a new collection.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="obj"></param>
		/// <returns>A new instance of <typeparamref name="List[TEntity]"/>.</returns>
		private static IEnumerable<TEntity> CastTo<TEntity>(this IEnumerable obj)
		{
			var result = new List<TEntity>();
			foreach (var item in obj)
			{
				result.Add((TEntity)item);
			}
			return result;
		}
		
		/// <summary>
		/// Copy the property values from source to the destination only for specified properties.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="source">Source instance</param>
		/// <param name="target">Destination instance</param>
		/// <param name="properties">List of properties that has to be copied</param>
		public static bool CopyPropertiesTo<TEntity>(this TEntity source, TEntity target, Func<TEntity, object> properties) where TEntity : class
		{
			if (ReferenceEquals(source, target) || properties == null)
				return false;

			var hasDifferentValues = false;
			var includedProperties = properties.Invoke(source).GetType().GetProperties().Select(o => o.Name).ToArray();
			var sourcePropertities = typeof(TEntity).GetProperties().Where(x => x.CanRead && (includedProperties.Contains(x.Name))).ToArray();

			foreach (var prop in sourcePropertities)
			{
				var targetValue = prop.GetValue(target);
				var sourceValue = prop.GetValue(source);
				if (Equals(targetValue, sourceValue)) continue;
				prop.SetValue(target, sourceValue);
				hasDifferentValues = true;
			}

			return hasDifferentValues;
		}

		/// <summary>
		/// Get the Key properties of the specified entity.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static PropertyInfo[] GetKeys<TEntity>(this TEntity entity) where TEntity : class
		{
			var props = entity.GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
			var keys = (from p in props
						where p.GetCustomAttribute<KeyAttribute>() != null
							|| p.Name == "Id"
						orderby p.GetCustomAttribute<ColumnAttribute>()?.Order ascending
						select p).ToArray();
			return keys;
		}

		/// <summary>
		/// Get the Key property values of the specified entity.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static object[] GetKeyValues<TEntity>(this TEntity entity) where TEntity : class
		{
			var values = entity.GetKeys().Select(p => p.GetValue(entity)).ToArray();
			return values;
		}

		/// <summary>
		/// Get the description of the enum value, or just return the name of the enum value.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public static string GetDescription(this ApplicationStateExternal state)
		{
			return state.GetDescription<ApplicationStateExternal>();
		}

		/// <summary>
		/// Get the description of the enum value, or just return the name of the enum value.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public static string GetDescription(this ApplicationStateInternal state)
		{
			return state.GetDescription<ApplicationStateInternal>();
		}

		/// <summary>
		/// Get the description of the enum value, or just return the name of the enum value.
		/// </summary>
		/// <param name="typeCode"></param>
		/// <returns></returns>
		public static string GetDescription(this OrganizationTypeCodes typeCode)
		{
			return typeCode.GetDescription<OrganizationTypeCodes>();
		}

		/// <summary>
		/// Get the description of the enum value, or just return the name of the enum value.
		/// </summary>
		/// <typeparam name="TEnum"></typeparam>
		/// <param name="state"></param>
		/// <returns></returns>
		public static string GetDescription<TEnum>(this TEnum state)
		{
			var description = Enum.GetName(typeof(TEnum), state);

			try
			{
				var type = typeof(TEnum);
				var memInfo = type.GetMember(state.ToString());
				var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

				if (attributes.Length > 0)
				{
					description = ((DescriptionAttribute)attributes[0]).Description;
				}
			}
			catch { }

			return description;
		}

		/// <summary>
		/// Provides a way to do a Contains function with the specified values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="val"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static bool In<T>(this T val, params T[] values) where T : struct
		{
			return values.Contains(val);
		}

		/// <summary>
		/// Because floating point values shouldn't be compared for equality/inequality you must instead compare within an acceptable range.
		/// </summary>
		/// <param name="self">The value to compare with.</param>
		/// <param name="value">The value to compare to.</param>
		/// <param name="within">The acceptable range.</param>
		/// <returns>True if the values are within the specified range.</returns>
		public static bool IsApproximately(this decimal self, decimal value, decimal within = 0.001m)
		{
			return Math.Abs(self - value) <= within;
		}

		/// <summary>
		/// Because floating point values shouldn't be compared for equality/inequality you must instead compare within an acceptable range.
		/// </summary>
		/// <param name="self">The value to compare with.</param>
		/// <param name="value">The value to compare to.</param>
		/// <param name="within">The acceptable range.</param>
		/// <returns>True if the values are within the specified range.</returns>
		public static bool IsApproximately(this double self, double value, double within = 0.001d)
		{
			return Math.Abs(self - value) <= within;
		}

		/// <summary>
		/// Because floating point values shouldn't be compared for equality/inequality you must instead compare within an acceptable range.
		/// </summary>
		/// <param name="self">The value to compare with.</param>
		/// <param name="value">The value to compare to.</param>
		/// <param name="within">The acceptable range.</param>
		/// <returns>True if the values are within the specified range.</returns>
		public static bool IsApproximately(this float self, float value, float within = 0.001f)
		{
			return Math.Abs(self - value) <= within;
		}

		public static string ResolveEmployerTypeCode<T>(this int enumValue)
		{
			var description = enumValue.ToString();

			if (typeof(T).Equals(typeof(OrganizationTypeCodes)))
			{
				var enumKey = (OrganizationTypeCodes)Enum.Parse(typeof(OrganizationTypeCodes), (enumValue).ToString());
				description = enumKey.GetDescription();
			}

			return description;
		}

		/// <summary>
		/// Get the claim state description for the specified claim.
		/// The assessment process is hidden from the external user, they will only ever see "Claim Submitted".
		/// Essentially only Deny and Approve states should be shown for previous claims.
		/// </summary>
		/// <param name="claim">The claim to get the state description for.</param>
		/// <returns>The claim state description.</returns>
		public static string GetClaimStateDescription(this Claim claim)
		{
			if (claim.GrantApplication == null || claim.GrantApplication == null)
				throw new ArgumentException("The Claim.TrainingProgram.GrantApplication cannot be null.", nameof(claim));

			// Only the current claim can ever be in an assessment process.
			// We hide the progress of the assessment from external users.
			if (claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
			{
				if ((claim.GrantApplication.Claims.Count() <= 1 || claim.ClaimVersion == claim.GrantApplication.Claims.Max(c => c.ClaimVersion))
					&& claim.GrantApplication.ApplicationStateExternal == ApplicationStateExternal.ClaimSubmitted)
				{
					return claim.GrantApplication.ApplicationStateExternal.GetDescription();
				}
			}
			else
			{
				if (claim.ClaimState == ClaimState.Unassessed && claim.GrantApplication.ApplicationStateExternal == ApplicationStateExternal.ClaimSubmitted)
				{
					return claim.GrantApplication.ApplicationStateExternal.GetDescription();
				}
			}
			// Essentially only Deny and Approve should get through here.
			return claim.ClaimState.GetDescription();
		}

		/// <summary>
		/// set the value of a temporary property
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="item"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public static void SetTemporaryValue<TEntity>(this TEntity item, string name, object value) where TEntity : class
		{
			// If we don't have a dictionary for this item yet,
			// make one.
			if (!PropertyValues.ContainsKey(item))
				PropertyValues[item] = new Dictionary<string, object>();

			// Set the value in the item's dictionary.
			PropertyValues[item][name] = value;
		}

		/// <summary>
		/// get the value of a temporary property
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="item"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		public static object GetTemporaryValue<TEntity>(this TEntity item, string name, object defaultValue)
		{
			// If we don't have a dictionary for
			// this item yet, return the default value.
			if (!PropertyValues.ContainsKey(item))
				return defaultValue;

			// If the value isn't in the dictionary,
			// return the default value.
			if (!PropertyValues[item].ContainsKey(name))
				return defaultValue;

			// Return the saved value.
			return PropertyValues[item][name];
		}

		/// <summary>
		/// Returns a key value pair array of the properties of the given class
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="item"></param>
		/// <param name="excludedProperties"></param>
		/// <returns></returns>
		public static KeyValuePair<string, string>[] GetPropertiesAsKeyValuePairs<TEntity>(this TEntity item, string[] excludedProperties = null) where TEntity : Type
		{
			var properties = item.GetProperties()
				.Select(p => p.Name)
				.Where(p => excludedProperties != null ? excludedProperties.All(xp => xp != p) : true)
				.ToArray();

			return properties.Zip(
				properties.Select(p => Regex.Replace(p, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1")).ToArray(),
				(key, value) => new KeyValuePair<string, string>(key, value)
			).ToArray();
		}
	}
}
