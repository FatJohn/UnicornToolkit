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
using System.Globalization;
using System.Text;

namespace Unicorn
{
    /// <summary>
    /// https://msdn.microsoft.com/zh-tw/library/zdtaw1bw(v=vs.110).aspx
    /// </summary>
    public static class PrimitiveTypeExtensions
    {
        public static string ToLongDateString(this DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString(DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Sunday, June 15, 2008
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToShortDateString(this DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString("D", DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// 9:15 PM
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToShortTimeString(this DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString("t", DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// 6/15/2008 9:15 PM
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToShortDateWithTimeString(this DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString("g", DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// 將字串轉換成指定小數點位數的字串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pointDigits"></param>
        /// <returns></returns>
        public static string ToString(this double source, int pointDigits)
        {
            string digitFormat = string.Format("{{0:F{0}}}", pointDigits);
            return string.Format(digitFormat, source);
        }

        public static string ToStringSafely(this byte[] input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            string result = Encoding.UTF8.GetString(input, 0, input.Length);
            return result;
        }
    }
}
