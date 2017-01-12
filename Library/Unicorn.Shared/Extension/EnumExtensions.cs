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
using System.Reflection;

namespace Unicorn
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get key of EnumPropertyAttribute
        /// </summary>
        /// <param name="enumerationValue"></param>
        /// <returns></returns>
        public static string GetKey(this Enum enumerationValue)
        {
            var customAttributes = GetEnumPropertyAttributes(enumerationValue);

            if (customAttributes != null && customAttributes.Length > 0)
            {
                return customAttributes[0].Key;
            }

            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        /// <summary>
        /// Get keys of EnumPropertyAttribute
        /// </summary>
        /// <param name="enumerationValue"></param>
        /// <returns></returns>
        public static string[] GetKeys(this Enum enumerationValue)
        {
            var customAttributes = GetEnumPropertyAttributes(enumerationValue);

            if (customAttributes != null && customAttributes.Length > 0)
            {
                return customAttributes[0].Keys;
            }

            //If we have no description attribute, just return the ToString of the enum
            return new string[] { enumerationValue.ToString() };
        }

        private static EnumPropertyAttribute[] GetEnumPropertyAttributes(Enum enumerationValue)
        {
            // 取得type
            Type type = enumerationValue.GetType();

            // 取得TypeInfo後才能取得有沒有CustomAttribute
            FieldInfo fieldInfo = type.GetRuntimeField(enumerationValue.ToString());
            return fieldInfo.GetCustomAttributes(typeof(EnumPropertyAttribute), false) as EnumPropertyAttribute[];
        }
    }
}
