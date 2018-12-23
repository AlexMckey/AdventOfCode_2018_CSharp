using System;
using System.Collections.Generic;
using System.Linq;

namespace PriorityQueueLib
{
    public class DictionaryPriorityQueue<TKey>: IPriorityQueue<TKey>
    {
        readonly Dictionary<TKey, double> dict = new Dictionary<TKey, double>();

        public double TOLERANCE = 0.0001;

        public void Add(TKey key, double val)
        {
            dict.Add(key, val);
        }

        public void Delete(TKey key)
        {
            dict.Remove(key);
        }

        public void Update(TKey key, double val)
        {
            dict[key] = val;
        }

        public (TKey key, double val) ExtractMin()
        {
            if (dict.Count == 0) throw new AccessViolationException("Queue is Empty");
            var min = dict.Min(kvp => kvp.Value);
            var key = dict.First(kvp => Math.Abs(kvp.Value - min) < TOLERANCE).Key;
            dict.Remove(key);
            return (key, min);
        }

        public bool TryGetValue(TKey key, out double val)
        {
            return dict.TryGetValue(key, out val);
        }
    }
}
