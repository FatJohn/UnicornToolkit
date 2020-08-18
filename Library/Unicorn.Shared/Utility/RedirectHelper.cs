// Copyright (c) 2018 Pou Lin

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
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace Unicorn
{
    public class RedirectHelper
    {
        public static async Task<string> GetOriginalUrl(string url)
        {
            var result = url;

            Uri.TryCreate(url, UriKind.Absolute, out Uri uri);
            if (uri == null || !(uri.Scheme == "http" || uri.Scheme == "https"))
            {
                return url;
            }

            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
            };

            try
            {
                using (var client = new HttpClient(httpClientHandler))
                {
                    var httpResponse = await client.GetAsync(uri);
                    if (httpResponse.StatusCode == HttpStatusCode.Redirect || httpResponse.StatusCode == HttpStatusCode.Moved)
                    {
                        result = await GetOriginalUrl(httpResponse.Headers.Location.AbsoluteUri);
                    }
                    httpResponse.Dispose();
                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }
        }
    }
}
