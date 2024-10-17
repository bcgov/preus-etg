using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace CJG.Web.External.Helpers.Converters
{
    /// <summary>
    /// <typeparamref name="DateTimeTypeConverter"/> class, provides a way to convert DateTime values.
    /// Provides a way to convert an array of int into a DateTime [year, month, day].
    /// </summary>
    public class DateTimeTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (typeof(IEnumerable<int>).IsAssignableFrom(sourceType))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (typeof(DateTime).IsAssignableFrom(destinationType))
                return true;

            if (typeof(DateTime?).IsAssignableFrom(destinationType))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
            {
                if (Nullable.GetUnderlyingType(destinationType) != null)
                    return null;
                else
                    return DateTime.MinValue;
            }

            if ((typeof(DateTime?).IsAssignableFrom(destinationType) || typeof(DateTime).IsAssignableFrom(destinationType))
                && typeof(IEnumerable<object>).IsAssignableFrom(value.GetType()))
            {
                var source = (object[])value;
                return (DateTime)new AppDateTime((int)source[0], (int)source[1], (int)source[2]);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}