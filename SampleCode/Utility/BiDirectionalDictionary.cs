using System;
using System.Collections.Generic;

namespace Portfolio.SampleCode.Utility
{
    /// <summary>
    /// Stores a one-to-one mapping and supports lookups from either side.
    ///
    /// Adapted from:
    /// 2024/VRFingFing/ETC/BiDirectionalDictionary.cs
    ///
    /// Set removes both sides of any displaced mapping before inserting a new pair,
    /// so the forward and reverse dictionaries cannot retain stale entries.
    /// </summary>
    public sealed class BiDirectionalDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> forward;
        private readonly Dictionary<TValue, TKey> reverse;

        public int Count => forward.Count;

        public TValue this[TKey key]
        {
            get => forward[key];
            set => Set(key, value);
        }

        public BiDirectionalDictionary(
            IEqualityComparer<TKey> keyComparer = null,
            IEqualityComparer<TValue> valueComparer = null)
        {
            forward = new Dictionary<TKey, TValue>(keyComparer);
            reverse = new Dictionary<TValue, TKey>(valueComparer);
        }

        public void Add(TKey key, TValue value)
        {
            if (forward.ContainsKey(key))
            {
                throw new ArgumentException("The key already has a mapping.", nameof(key));
            }

            if (reverse.ContainsKey(value))
            {
                throw new ArgumentException("The value already has a mapping.", nameof(value));
            }

            forward.Add(key, value);
            reverse.Add(value, key);
        }

        /// <summary>
        /// Adds or replaces a pair while preserving the one-to-one invariant.
        /// Existing pairs that use either side are removed first.
        /// </summary>
        public void Set(TKey key, TValue value)
        {
            TValue currentValue;
            if (forward.TryGetValue(key, out currentValue) &&
                reverse.Comparer.Equals(currentValue, value))
            {
                return;
            }

            RemoveByKey(key);
            RemoveByValue(value);

            forward.Add(key, value);
            reverse.Add(value, key);
        }

        public bool ContainsKey(TKey key)
        {
            return forward.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return reverse.ContainsKey(value);
        }

        public bool TryGetByKey(TKey key, out TValue value)
        {
            return forward.TryGetValue(key, out value);
        }

        public bool TryGetByValue(TValue value, out TKey key)
        {
            return reverse.TryGetValue(value, out key);
        }

        public bool RemoveByKey(TKey key)
        {
            TValue value;
            if (!forward.TryGetValue(key, out value))
            {
                return false;
            }

            forward.Remove(key);
            reverse.Remove(value);
            return true;
        }

        public bool RemoveByValue(TValue value)
        {
            TKey key;
            if (!reverse.TryGetValue(value, out key))
            {
                return false;
            }

            reverse.Remove(value);
            forward.Remove(key);
            return true;
        }

        public void Clear()
        {
            forward.Clear();
            reverse.Clear();
        }
    }
}
