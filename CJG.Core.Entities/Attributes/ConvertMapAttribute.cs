using System;
using System.ComponentModel;
using System.Linq;

namespace CJG.Core.Entities.Attributes
{

    /// <summary>
    /// <typeparamref name="ConvertMapAttribute"/> attribute class, provides a way to control how to convert a property from one class into another.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ConvertMapAttribute : Attribute
    {
        private readonly TypeConverter _propertyConverter;

        /// <summary>
        /// get - The name of the property in the destination object that this property will be converted to.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// get - An array of property names from the source object that will be converted into a single value in the destination object.
        /// </summary>
        public string[] Sources { get; }

        /// <summary>
        /// get - A string format that the destination value will be formatted with.
        /// </summary>
        public string PropertyFormat { get; }

        #region Constructors
        /// <summary>
        /// Creates a new instance ofa <typeparamref name="ConvertMapAttribute"/>.
        /// </summary>
        /// <param name="propertyName"></param>
        public ConvertMapAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// Creates a new instance ofa <typeparamref name="ConvertMapAttribute"/>.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="format"></param>
        public ConvertMapAttribute(string propertyName, string format) : this(propertyName, null, format)
        {

        }

        /// <summary>
        /// Creates a new instance ofa <typeparamref name="ConvertMapAttribute"/>.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="typeConverter"></param>
        public ConvertMapAttribute(string propertyName, Type typeConverter) : this(propertyName, null, typeConverter)
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="ConvertMapAttribute"/>.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="sources"></param>
        /// <param name="format"></param>
        public ConvertMapAttribute(string propertyName, string[] sources = null, string format = null)
        {
            this.PropertyName = propertyName;
            this.Sources = sources;
            this.PropertyFormat = format;
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="ConvertMapAttribute"/>.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="sources"></param>
        /// <param name="typeConverter"></param>
        public ConvertMapAttribute(string propertyName, string[] sources, Type typeConverter)
        {
            this.PropertyName = propertyName;
            this.Sources = sources;

            if (!typeof(TypeConverter).IsAssignableFrom(typeConverter))
                throw new ArgumentException($"Argument '{nameof(typeConverter)}' must be assignable from type '{nameof(TypeConverter)}'.");

            _propertyConverter = (TypeConverter)Activator.CreateInstance(typeConverter);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert the value into another value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Convert(object value)
        {
            if (_propertyConverter == null)
                return value;

            return _propertyConverter.ConvertTo(value, typeof(object));
        }

        public object ConvertTo(object value, Type convertToType)
        {
            if (_propertyConverter == null)
            {
                if (convertToType == typeof(string) && !String.IsNullOrEmpty(this.PropertyFormat))
                {
                    if (value.GetType().IsArray)
                        return Format((object[])value);
                    return Format(value);
                }

                return System.Convert.ChangeType(value, convertToType);
            }

            if (_propertyConverter.CanConvertTo(convertToType))
                return _propertyConverter.ConvertTo(value, convertToType);

            return value;
        }

        public TResult ConvertTo<TResult>(object value)
        {
            var type = typeof(TResult);
            return (TResult)ConvertTo(value, type);
        }

        /// <summary>
        /// Format the source values into a single string.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public string Format(object[] values)
        {
            if (string.IsNullOrEmpty(this.PropertyFormat))
                return values.Aggregate((a, b) => $"{a}{b}").ToString();

            return String.Format(this.PropertyFormat, values);
        }

        /// <summary>
        /// Format the source value into a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Format(object value)
        {
            if (string.IsNullOrEmpty(this.PropertyFormat))
                return value.ToString();

            return String.Format(this.PropertyFormat, value);
        }
        #endregion
    }
}

