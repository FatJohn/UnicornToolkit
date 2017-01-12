// Copyright (c) 2016 John Shu

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Unicorn
{
    public static class CollectionExtensions
    {
        #region Dictionary
        public static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                return;
            }

            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            value = default(TValue);

            if (dictionary == null)
            {
                return false;
            }

            if (!dictionary.ContainsKey(key))
            {
                return false;
            }

            value = dictionary[key];
            return true;
        }

        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key)
        {
            if (dictionary == null)
            {
                return default(T);
            }

            return (T)dictionary[key];
        }

        public static bool TryGetValue(this IDictionary<string, object> dictionary, string key, out object value)
        {
            value = null;

            if (dictionary == null)
            {
                return false;
            }

            if (!dictionary.ContainsKey(key))
            {
                return false;
            }

            value = dictionary[key];
            return true;
        }

        public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            value = default(T);

            if (dictionary == null)
            {
                return false;
            }

            if (!dictionary.ContainsKey(key))
            {
                return false;
            }

            value = dictionary.GetValue<T>(key);
            return true;
        }

        public static bool TryGetValueAndRemove<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            value = default(T);
            if (dictionary == null)
            {
                return false;
            }

            if (!dictionary.ContainsKey(key))
            {
                return false;
            }

            value = dictionary.GetValue<T>(key);
            dictionary.Remove(key);
            return true;
        }
        #endregion

        #region List<T>

        public static bool Contains<T>(this List<T> list, T item)
        {
            if (list == null || list.Count == 0)
            {
                return false;
            }

            return (list.IndexOf(item) != -1);
        }

        public static bool TryGetValue<T>(this IList<T> source, int idx, out T value)
        {
            value = default(T);

            if (source == null)
            {
                return false;
            }

            if (source.Count <= idx)
            {
                return false;
            }

            value = source[idx];
            return true;
        }

        public static T GetValue<T>(this IList<T> source, int idx, T defaultValue = default(T))
        {
            if (source.Count > idx)
            {
                return source[idx];
            }

            return defaultValue;
        }

        public static T Peek<T>(this IList<T> source)
        {
            T result = default(T);

            if (source == null)
            {
                return result;
            }

            if (source.Count > 0)
            {
                result = source[0];
            }

            return result;
        }

        public static T Pop<T>(this IList<T> source)
        {
            T result = default(T);
            if (source == null)
            {
                return result;
            }

            if (source.Count > 0)
            {
                result = source[0];
                source.RemoveAt(0);
            }

            return result;
        }

        public static void Push<T>(this IList<T> source, T item)
        {
            if (source != null)
            {
                source.Insert(0, item);
            }
        }
        #endregion

        #region IEnmerable

        public static void ForEach<T>(this IEnumerable items, Action<T> action)
        {
            if (items == null)
            {
#if DEBUG
                throw new ArgumentNullException("items");
#else
                return;
#endif
            }

            if (action == null)
            {
#if DEBUG
                throw new ArgumentNullException("action");
#else
                return;
#endif
            }

            foreach (object obj in items)
            {
                action((T)obj);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
            {
#if DEBUG
                throw new ArgumentNullException("items");
#else
                return;
#endif
            }

            if (action == null)
            {
#if DEBUG
                throw new ArgumentNullException("action");
#else
                return;
#endif
            }

            foreach (T obj in items)
            {
                action(obj);
            }
        }

        #endregion
    }
}
