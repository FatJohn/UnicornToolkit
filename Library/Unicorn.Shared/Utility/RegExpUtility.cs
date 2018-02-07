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
using System.Text.RegularExpressions;

namespace Unicorn
{
    public class RegExpUtility
    {
        private enum RegExValideType
        {
            Cellphone,
            Password,
            Email,
        }

        public static bool ValidEmail(string sourceString)
        {
            return ValidateValueByRegex(RegExValideType.Email, sourceString);
        }

        public static bool ValidPassword(string sourceString)
        {
            return ValidateValueByRegex(RegExValideType.Password, sourceString);
        }

        private static bool ValidateValueByRegex(RegExValideType validateType, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            Regex regex = null;

            switch (validateType)
            {
                case RegExValideType.Cellphone:
                    regex = new Regex("^[0-9]{4}[0-9]{6}$");
                    break;
                case RegExValideType.Password:
                    regex = new Regex(@"^[_a-zA-Z0-9-]");
                    break;
                case RegExValideType.Email:
                    regex = new Regex("^[_a-z0-9-]+([.][_a-z0-9-]+)*@[a-z0-9-]+([.][a-z0-9-]+)*$");
                    break;
                default:
                    regex = null;
                    break;
            }

            if (regex != null)
            {
                return regex.IsMatch(value);
            }
            else
            {
                return false;
            }
        }

        public static string GetWebLink(string content)
        {
            try
            {
                return Regex.Match(content, @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)").Value;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GetEmail(string content)
        {
            try
            {
                return Regex.Match(content, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase).Value;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GetNumber(string input)
        {
            const string pattern = "\\d+";
            try
            {
                return Regex.Match(input, pattern).Value;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}