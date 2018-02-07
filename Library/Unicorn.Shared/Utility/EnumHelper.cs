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
using System.Linq;

namespace Unicorn
{
    public static class EnumHelper
    {
        public static T Parse<T>(string value)
            where T : struct
        {
            var result = default(T);
            Enum.TryParse<T>(value, out result);
            return result;
        }

        public static T ParseByAttribute<T>(string value)
            where T : struct
        {
            var enumValues = Enum.GetValues(typeof(T));
            foreach (var item in enumValues)
            {
                var enumKeys = ((Enum)item).GetKeys();
                if (enumKeys.Contains(value))
                {
                    return (T)item;
                }
            }

            return (T)enumValues.GetValue(0);
        }

        /// <summary>
        /// Get keys and values of the Enum.
        /// </summary>
        /// <param name="enumerationValue">Enum type</param>
        /// <returns></returns>
        public static Dictionary<string, Enum> ToDictionary(Type enumType)
        {
            var dictionary = new Dictionary<string, Enum>();
            var enumValues = Enum.GetValues(enumType);

            foreach (var item in enumValues)
            {
                var enumItem = ((Enum)item);
                var enumKeys = enumItem.GetKeys();

                foreach (var key in enumKeys)
                {
                    dictionary.Add(key, enumItem);
                }
            }

            return dictionary;
        }
    }
}
