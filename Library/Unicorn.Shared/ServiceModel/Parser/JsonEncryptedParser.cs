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

using System.Threading.Tasks;
using Newtonsoft.Json;
using Unicorn.Net;

#if WINDOWS_UWP
using Windows.Web.Http;
#else
using System.Net.Http;
#endif

namespace Unicorn.ServiceModel
{
    public class JsonEncryptedParser<TResult, TDecrypter> : BaseParser<TResult, HttpResponseMessage>
        where TDecrypter : IDecrypter<string, HttpResponseMessage>, new()
    {
        public override async Task<ParseResult<TResult>> Parse(HttpResponseMessage source)
        {
            var requestId = source.ReadRequestId();

            if (!source.IsSuccessStatusCode)
            {
                PlatformService.Log?.Trace($"[{requestId}] Response Status: ${source.StatusCode} | Url:{source.RequestMessage.RequestUri}");
                return new ParseResult<TResult>(new ParseError((int)source.StatusCode, source.StatusCode.ToString()));
            }

            var decrypter = new TDecrypter();
            var contentString = await decrypter.Decrypt(source).ConfigureAwait(false);
            if (contentString == null)
            {
                return new ParseResult<TResult>(new ParseError("decrypt fail"));
            }

            try
            {
                PlatformService.Log?.Trace($"[{requestId}] Response Json: {contentString}");

                var result = ServiceModelJsonConvert.DeserializeObject<TResult>(contentString);
                return new ParseResult<TResult>(result);
            }
            catch (JsonException ex)
            {
                return new ParseResult<TResult>(new ParseError(ex.Message));
            }
        }
    }
}
