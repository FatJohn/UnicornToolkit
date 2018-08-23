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
using System.Security.Cryptography;
using System.Text;

namespace Unicorn
{
    public class CryptographyService : ICryptographyService
    {
        public byte[] HashMD5(byte[] source)
        {
            if (source == null)
            {
                return null;
            }

            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
            return hashProvider.ComputeHash(source);
        }

        public string HashMD5(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
            var encryptedBuffer = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(source));
            return BitConverter.ToString(encryptedBuffer);
        }

        public string HashSHA1(string source)
        {
            SHA1 hashProvider = new SHA1CryptoServiceProvider();
            byte[] encryptedBuffer = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(source));//進行SHA1加密
            return BitConverter.ToString(encryptedBuffer);
        }
    }
}
