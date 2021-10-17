using System;
using System.Collections.Generic;
using System.Linq;

namespace Raid.Service
{
    public class NumericDictionary<K, V> : Dictionary<int, V> where K : Enum
    {
        public NumericDictionary() : base() { }
        public NumericDictionary(Dictionary<K, V> dict)
        : base(dict.ToDictionary(kvp => (int)(object)kvp.Key, kvp => kvp.Value))
        {
        }
        public static implicit operator NumericDictionary<K, V>(Dictionary<K, V> value)
        {
            if (value == null)
            {
                return null;
            }
            return new NumericDictionary<K, V>(value);
        }

        public bool TryGetValue(K key, out V value)
        {
            return TryGetValue((int)(object)key, out value);
        }
    }
}