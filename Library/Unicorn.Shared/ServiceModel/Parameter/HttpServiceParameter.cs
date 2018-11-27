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

using Newtonsoft.Json;
using System;

namespace Unicorn.ServiceModel
{
    public enum HttpParameterMethod
    {
        NotSpecific,
        Delete,
        Get,
        Head,
        Options,
        Patch,
        Post,
        Put,
    }

    public class HttpServiceParameter
    {
        private readonly static HttpServiceParameter emptyParameter = new HttpServiceParameter();
        [JsonIgnore]
        [HttpIgnore]
        public static HttpServiceParameter Empty
        {
            get { return emptyParameter; }
        }

        /// <summary>
        /// 可以指定這次 Parameter 要用的 Http Method, 預設是不指定自動在 Get / Post 判斷
        /// </summary>
        [JsonIgnore]
        [HttpIgnore]
        public HttpParameterMethod HttpMethod { get; set; } = HttpParameterMethod.NotSpecific;

        /// <summary>
        /// 指定 Http content-type
        /// </summary>
        [JsonIgnore]
        [HttpIgnore]
        public string HttpContentType { get; set; }

        [JsonIgnore]
        [HttpIgnore]
        public HttpServiceParameterOption Options { get; private set; } = new HttpServiceParameterOption();

        /// <summary>
        /// 代表這個 Request 會 Timeout 的時間
        /// 預設值是 100 秒
        /// </summary>
        /// <remarks>建議不要小於 300 豪秒</remarks>
        [JsonIgnore]
        [HttpIgnore]
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
    }
}
