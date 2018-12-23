using System;

namespace PriorityQueueLib
{
    public interface IPriorityQueue<TKey>
    {
        void Add(TKey key, double val);
        void Delete(TKey key);
        void Update(TKey key, double val);
        (TKey key, double val) ExtractMin();
        bool TryGetValue(TKey key, out double val);
    }

    public static class PriorityQueueExt
    {
        public static bool UpdateOrAdd<TKey>(this IPriorityQueue<TKey> queue, TKey key, double newValue)
        {
            var nodeInQueue = queue.TryGetValue(key, out var oldPrice);
            if (nodeInQueue && !(oldPrice > newValue)) return false;
            queue.Update(key, newValue);
            return true;
        }
    }
}
