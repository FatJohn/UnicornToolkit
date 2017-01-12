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

using System.Text;

namespace Unicorn
{
    /// <summary>
    /// 有例外處理的Convert class
    /// </summary>
    public static class Convert2
    {
        /// <summary>
        /// 將字串轉換成Double（使用TryParse避免Exception）
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static double ToDouble(string src)
        {
            double result = 0;
            double.TryParse(src, out result);
            return result;
        }

        /// <summary>
        /// 將字串轉換成Int（使用TryParse避免Exception）
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static int ToInt(string src)
        {
            int result = 0;
            int.TryParse(src, out result);
            return result;
        }

        /// <summary>
        /// 將字串轉換成short（使用TryParse避免Exception）
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static short ToShort(string src)
        {
            short result = 0;
            short.TryParse(src, out result);
            return result;
        }

        /// <summary>
        /// 將字串轉換成byte（使用TryParse避免Exception）
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static byte ToByte(string src)
        {
            byte result = 0;
            byte.TryParse(src, out result);
            return result;
        }

        public static byte[] ToByteArray(string src)
        {
            Encoding enc_default = Encoding.UTF8;
            byte[] byteArray = enc_default.GetBytes(src);
            return byteArray;
        }

        public static bool ToBoolean(string src)
        {
            bool result;
            bool.TryParse(src, out result);
            return result;
        }
    }
}
