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
        /// 當 Service 發現此欄位有值時，就會使用此欄位的值當作發送的 URL
        /// </summary>
        [JsonIgnore]
        [HttpIgnore]
        public string RequestUrl { get; set; }

        /// <summary>
        /// 是否為完整的 RequestUrl, true 則不會跑組合參數的流程, 預設是 false
        /// 當 RequestUrl 有值的時候才有作用
        /// </summary>
        [JsonIgnore]
        [HttpIgnore]
        public bool IsCompletedRequestUrl { get; set; }

        [JsonIgnore]
        [HttpIgnore]
        public bool IsEnableCache { get; set; } = false;

        /// <summary>
        /// Cache 的時間. 預設值是 0, 小於等於0即便把 IsEnableCache 啟動也是會重新抓資料
        /// </summary>
        [JsonIgnore]
        [HttpIgnore]
        public int CacheMinutes { get; set; } = 0;

        /// <summary>
        /// 可以指定這次 Parameter 要用的 Http Method, 預設是不指定自動在 Get / Post 判斷
        /// </summary>
        [JsonIgnore]
        [HttpIgnore]
        public HttpParameterMethod HttpMethod { get; set; } = HttpParameterMethod.NotSpecific;
    }
}
