using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models;

namespace CJG.Web.External.Helpers.Converters
{
    public class DeliveryPartnerServicesConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (typeof(IEnumerable<DeliveryPartnerService>).IsAssignableFrom(destinationType))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return null;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}