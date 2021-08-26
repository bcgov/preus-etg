using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Models.Shared
{
    public class KeyValueViewModel<T>
    {
        public T Key { get; set; }
        public string Value { get; set; }

        public KeyValueViewModel()
        {

        }

        public KeyValueViewModel(T key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}