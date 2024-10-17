using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Web.External.Helpers
{
    public static class RadioButtonListHelper
    {
        public static List<KeyValuePair<int, string>> GetEnumList<T>()
        {
            var list = new List<KeyValuePair<int, string>>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                var description = e.ToString();

                if (typeof(T).Equals(typeof(OrganizationTypeCodes)))
                {
                    description = ((int)e).ResolveEmployerTypeCode<OrganizationTypeCodes>();

                    if (description == "Default")
                    {
                        continue;
                    }
                }
                list.Add(new KeyValuePair<int, string>((int)e, description));
            }
            return list;
        }
    }
}