﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// Setting class, provides the ORM a way to manage application settings.
	/// </summary>
	public class Setting : EntityBase
	{
		/// <summary>
		/// get/set - Unique key to identify the setting.
		/// </summary>
		[Key, MaxLength(50), DatabaseGenerated(DatabaseGeneratedOption.None)]
		public string Key { get; set; }

		/// <summary>
		/// get/set - The value of the key.
		/// </summary>
		[MaxLength(2000)]
		public string Value { get; set; }

		/// <summary>
		/// get/set - The value type so that a new instance can be created when extracted from the datasource.
		/// </summary>
		[MaxLength(500), Required]
		public string ValueType { get; set; }

		/// <summary>
		/// get/set - The foreign key to the user who owns this setting.
		/// </summary>
		public int? InternalUserId { get; set; }

		/// <summary>
		/// get/set - The user who owns this setting.
		/// </summary>
		[ForeignKey(nameof(InternalUserId))]
		public InternalUser Owner { get; set; }

		/// <summary>
		/// Creates a new instance of a Setting object.
		/// </summary>
		public Setting()
		{
		}

		/// <summary>
		/// Creates a new instance of a Setting object.
		/// </summary>
		/// <param name="key">Unique key to identify the setting.</param>
		/// <param name="value">The value of the key.</param>
		/// <param name="valueType">The value type so that a new instance can be created when extracted from the datasource.</param>
		public Setting(string key, string value, Type valueType)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			if (valueType == null)
				throw new ArgumentNullException(nameof(valueType));

			Key = key;
			Value = value;
			ValueType = valueType.FullName;
		}

		/// <summary>
		/// Creates a new instance of a Setting object.
		/// </summary>
		/// <param name="key">Unique key to identify the setting.</param>
		/// <param name="value">The value of the key.</param>
		public Setting(string key, object value)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			if (value == null)
				throw new ArgumentNullException(nameof(value));

			Key = key;
			Value = typeof(DateTime) == value.GetType()
				? ((DateTime)value).ToString("yyyy-MM-dd h:mm:ss tt")
				: value.ToString();
			ValueType = value.GetType().FullName;
		}

		/// <summary>
		/// Creates a new instance of a Setting object.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public Setting(InternalUser owner, string key, object value) : this(key, value)
		{
			InternalUserId = owner?.Id;
			Owner = owner;
		}

		/// <summary>
		/// If the Setting Type is valid it will attempt to convert the Value to the Type.
		/// </summary>
		/// <returns></returns>
		public object GetValue()
		{
			var type = Type.GetType(ValueType);

			if (type == null)
				throw new InvalidOperationException($"Unable to find the specified type '{ValueType}'.");

			if (type == typeof(string))
				return Value;

			return Convert.ChangeType(Value, type);
		}

		/// <summary>
		/// If the specified <typeparamref name="T"/> is the same as the Setting.ValueType it will return the value.
		/// Otherwise it attempt to convert the Value to Type <typeparamref name="T"/>.
		/// </summary>
		/// <returns></returns>
		public T GetValue<T>()
		{
			var value = GetValue();
			if (typeof(T) == value.GetType())
				return (T)value;

			return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
		}

		/// <summary>
		/// If the specified <typeparamref name="T"/> is the same as the Setting.ValueType it will return the value.
		/// Otherwise it attempt to convert the Value to Type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public bool TryGetValue<T>(out T value, T defaultValue = default)
		{
			try
			{
				value = GetValue<T>();
				return true;
			}
			catch
			{
				value = defaultValue;
				return false;
			}
		}

		/// <summary>
		/// Converts the value and sets the Value property if the conversion succeeds.
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(string value)
		{
			var type = Type.GetType(ValueType);

			if (type == null)
				throw new InvalidOperationException($"Unable to find the specified type '{ValueType}'.");

			var result = Convert.ChangeType(value, type);

			if (type == typeof(DateTime) || type == typeof(DateTime?))
				Value = $"{result:yyyy/MM/dd hh:mm:ss tt}";
			else
				Value = value;
		}

		/// <summary>
		/// Attempts to set the convert the value and set the property.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public bool TrySetValue(string value, out object result)
		{
			var type = Type.GetType(ValueType);

			if (type == null)
				throw new InvalidOperationException($"Unable to find the specified type '{ValueType}'.");

			try
			{
				result = Convert.ChangeType(value, type);

				if (type == typeof(DateTime) || type == typeof(DateTime?))
				{
					Value = $"{result:yyyy/MM/dd hh:mm:ss tt}";
				}
				else
					Value = value;

				return true;
			}
			catch (Exception)
			{
				result = null;
				return false;
			}
		}

		/// <summary>
		/// Attempts to set the value and the type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		public void SetValue<T>(T value)
		{
			ValueType = typeof(T).FullName;

			if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
			{
				Value = $"{value:yyyy/MM/dd hh:mm:ss tt}";
			}
			else
				Value = $"{value}";
		}
	}
}
