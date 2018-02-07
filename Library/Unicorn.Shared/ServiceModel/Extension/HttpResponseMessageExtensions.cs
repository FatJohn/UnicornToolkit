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
using System.Text;
using System.Threading.Tasks;
using System.Linq;
#if WINDOWS_UWP
using Windows.Web.Http;
using System.Runtime.InteropServices.WindowsRuntime;
#else
using System.Net.Http;
#endif

namespace Unicorn
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<string> ReadAsStringAsync(this HttpResponseMessage source)
        {
            if (source.Content == null)
            {
                return null;
            }

#if WINDOWS_UWP
            var buffer = await source.Content.ReadAsBufferAsync();
            if (buffer == null || buffer.Length == 0)
            {
                return null;
            }

            var contentBytes = new byte[buffer.Length];
            buffer.CopyTo(0, contentBytes, 0, contentBytes.Length);
#else
            var contentBytes = await source.Content.ReadAsByteArrayAsync();
#endif
            return Encoding.UTF8.GetString(contentBytes, 0, contentBytes.Length);
        }

        public static string ReadRequestId(this HttpResponseMessage source)
        {
#if WINDOWS_UWP
            return source.RequestMessage.Headers["X-Request-ID"];
#else
            return source.RequestMessage.Headers.GetValues("X-Request-ID").FirstOrDefault();
#endif
        }
    }
}
