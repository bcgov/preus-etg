using System.IO;
using CJG.Web.External.Areas.Ext.Models;
using Newtonsoft.Json;

namespace CJG.Web.External.Helpers
{
    public static class JsonHelper
    {
        public static dynamic ConvertAddressToJsonObject(AddressViewModel addressModel)
        {
            if (addressModel == null) return null;

            return new
            {
                addr1 = addressModel.AddressLine1,
                addr2 = addressModel.AddressLine2,
                city = addressModel.City,
                postalcode = addressModel.PostalCode,
                region = addressModel.RegionId,
                regionName = addressModel.Region,
                country = addressModel.CountryId,
                countryName = addressModel.Country
            };
        }

        public static string SerializeForJavascript(object obj, bool quoteName = false, bool replaceUnderscoreWithDash = false)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new JsonTextWriter(stringWriter) { QuoteName = quoteName, QuoteChar = '\'' })
                {
                    new JsonSerializer().Serialize(writer, obj);
                }
                
                var str = stringWriter.ToString();

                if (replaceUnderscoreWithDash)
                {
                    str = str.Replace("_", "-");
                }

                return str;

            }
        }
    }
}