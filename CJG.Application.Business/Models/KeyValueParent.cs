using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJG.Application.Business.Models
{
    public class KeyValueParent<TKey, TValue, TParent>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public TParent Parent { get; set; }

        public KeyValueParent()
        {
        }

        public KeyValueParent(TKey key, TValue value, TParent parent)
        {
            Key = key;
            Value = value;
            Parent = parent;
        }
    }
}
