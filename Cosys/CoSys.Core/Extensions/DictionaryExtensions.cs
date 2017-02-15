using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoSys.Core
{
    public static class DictionaryExtensions
    {
        public static TValue GetValue<Tkey, TValue>(this Dictionary<Tkey, TValue> source, Tkey key)
        {
            TValue model = default(TValue);
            if (source != null && key != null)
            {
                source.TryGetValue(key, out model);
            }
            return model;
        }
    }
}
