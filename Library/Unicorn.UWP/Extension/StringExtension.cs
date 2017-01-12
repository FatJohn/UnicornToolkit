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
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.ApplicationModel.DataTransfer;

namespace Unicorn
{
    public static class StringExtension
    {
        /// <summary>
        /// 將來源字串做 UrlDecode
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string UrlDecode(this string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return string.Empty;
            }

            string parsedResult = WebUtility.UrlDecode(sourceString);
            return parsedResult;
        }

        public static Color ConvertStringToColor(this string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return Colors.Black;
            }

            //remove the # at the front
            hex = hex.Replace("#", string.Empty);

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes
            r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
            b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// 複製字串到系統剪貼簿
        /// </summary>
        /// <param name="targetString"></param>
        /// <returns>true: 複製成功</returns>
        public static bool CopyToClipboard(this string targetString)
        {
            try
            {
                if (targetString == null)
                {
                    targetString = string.Empty;
                }

                DataPackage copySourceData = new DataPackage()
                {
                    RequestedOperation = DataPackageOperation.Copy
                };
                copySourceData.SetText(targetString);

                Clipboard.SetContent(copySourceData);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Dictionary<string, string> ToKeyValueDictionary(this string param)
        {
            try
            {
                var splitArray = param.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                if (splitArray == null || splitArray.Length == 0)
                {
                    return new Dictionary<string, string>();
                }

                var splitArray2 = splitArray.Select(part => part.Split('='));

                Dictionary<string, string> returnData = new Dictionary<string, string>();

                foreach (var item in splitArray2)
                {
                    var key = item[0];
                    var value = item.Length > 1 ? item[1] : string.Empty;
                    returnData.Add(key, value);
                }

                return returnData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int[] ToMobileCountryCode(this string content)
        {
            Regex regex = new Regex("[^0-9.-]");
            int[] codes = new int[] { 0, 0 };
            if (string.IsNullOrEmpty(content) == false && content.Length >= 5 && regex.IsMatch(content))
            {
                codes[0] = int.Parse(content.Substring(0, 3));
                codes[1] = int.Parse(content.Substring(4));
            }
            return codes;
        }
    }
}