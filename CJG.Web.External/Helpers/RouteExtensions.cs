using System;
using System.Web.Routing;

namespace CJG.Web.External.Helpers
{
    public static class RouteExtensions
    {

        public const string IsRefreshedName = "IsRefreshed";

        public static bool IsExternal(this RouteData value)
        {
            return String.Compare(value.DataTokens["area"].ToString(), "ext", true) == 0;
        }

        public static bool IsExternal(this System.Web.Mvc.ViewDataDictionary values)
        {
            if (values["IsExternal"] != null)
            {
                bool isExternal = false;
                if (bool.TryParse(values["IsExternal"].ToString(), out isExternal))
                {
                    return isExternal;
                }
            }
            return false;
        }

        public static bool IsRefreshed(this RouteData data)
        {
            return (bool?) data.Values[IsRefreshedName] == true;
        }

        public static int? ToInt(this object value)
        {
            if (value == null)
                return null;

            var type = value.GetType();

            if (type == typeof(int))
                return (int)value;

            if (type == typeof(string))
            {
                int result;

                if (int.TryParse((string)value, out result))
                    return result;

                return null;
            }

            return Convert.ToInt32(value);
        }
    }

}