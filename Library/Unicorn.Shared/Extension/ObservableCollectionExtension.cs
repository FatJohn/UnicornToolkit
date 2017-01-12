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
// SOFTWARE

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Unicorn
{
    public static class ObservableCollectionExtension
    {
        public static void AddRange<T>(this ObservableCollection<T> t, IEnumerable<T> source)
        {
            if (t == null)
            {
                return;
            }

            foreach (var i in source)
            {
                t.Add(i);
            }
        }

        public static void RemoveAfter<T>(this ObservableCollection<T> t, int startIndex)
        {
            if (t == null)
            {
                return;
            }

            var itemCounts = t.Count;

            if (itemCounts > startIndex)
            {
                var removeTargetList = new List<T>();

                for (int i = startIndex; i < itemCounts; i++)
                {
                    removeTargetList.Add(t[i]);
                }

                foreach (var removeTargetItem in removeTargetList)
                {
                    t.Remove(removeTargetItem);
                }

                removeTargetList.Clear();
            }
        }

        public static void RemoveRange<T>(this ObservableCollection<T> t, int startIndex, int count)
        {
            if (t == null)
            {
                return;
            }

            var itemCounts = t.Count;

            if (count > 0 && itemCounts > startIndex)
            {
                var removeTargetList = new List<T>();
                var endIndex = Math.Min(startIndex + count, t.Count);

                for (int i = startIndex; i < endIndex; i++)
                {
                    removeTargetList.Add(t[i]);
                }

                foreach (var removeTargetItem in removeTargetList)
                {
                    t.Remove(removeTargetItem);
                }

                removeTargetList.Clear();
            }
        }
    }
}
