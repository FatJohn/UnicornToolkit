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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Linq;
#if WINDOWS_UWP
using Windows.Web.Http;
using Windows.Web.Http.Headers;
#else
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
#endif

namespace Unicorn.ServiceModel
{
    public static class HttpRequestCreator
    {
        private static readonly string requestIdHeaderKey = "X-Request-ID";

        public static HttpRequestMessage Create<TParameter>(TParameter parameter, string baseRequestUrl, HttpParameterPackResult packResult)
            where TParameter : HttpServiceParameter
        {
            var httpMethod = GetHttpMethod(parameter, packResult);
            // 取得的 Url
            var requestUrl = GetRequestUrl(parameter, baseRequestUrl, packResult.GetCombindedString);
            // 依照 requestUrl 產生 Request Url
            var httpRequest = new HttpRequestMessage(httpMethod, new Uri(requestUrl));
            // 方便辨識送出的 request
            httpRequest.Headers.Add(requestIdHeaderKey, Guid.NewGuid().ToString("N"));
            // 加入 Header
            packResult.HeaderParameterMap.ForEach((keyValue) => httpRequest.Headers.Add(keyValue.Key, keyValue.Value));
            // 加入 Body content
            CreateHttpContent(httpRequest, packResult);

            if (parameter.HttpContentType != null && httpRequest.Content?.Headers != null)
            {
#if WINDOWS_UWP
                httpRequest.Content.Headers.ContentType = new HttpMediaTypeHeaderValue(parameter.HttpContentType);
#else
                httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(parameter.HttpContentType);
#endif
            }

            LogParameterPackResult(httpRequest, packResult);

            return httpRequest;
        }

        private static void LogParameterPackResult(HttpRequestMessage httpRequest, HttpParameterPackResult packResult)
        {
            var logger = PlatformService.Log;

            if (logger == null)
            {
                return;
            }
#if WINDOWS_UWP
            var requestId = httpRequest.Headers[requestIdHeaderKey];
#else
            var requestId = httpRequest.Headers.GetValues(requestIdHeaderKey).FirstOrDefault();
#endif
            logger.Trace($"[{requestId}] RequestUrl | {httpRequest.RequestUri}");
            packResult.HeaderParameterMap.ForEach((keyValue) =>
            {
                logger.Trace($"[{requestId}] Header Parameters | {keyValue.Key}:{keyValue.Value}");
            });

            if (packResult.PostParameterMap.Count > 0)
            {
                string postContet = string.Empty;
                packResult.PostParameterMap.ForEach((keyValue) =>
                {
                    postContet += $"{keyValue.Key}={keyValue.Value}&";
                });
                logger.Trace($"[{requestId}] Content | {postContet.Remove(postContet.Length - 1)}");
            }

            if (packResult.PostRawStringList.Count > 0)
            {
                StringBuilder rawStringBuilder = new StringBuilder();
                packResult.PostRawStringList.ForEach((rawString) =>
                {
                    rawStringBuilder.AppendLine(rawString);
                });
                logger.Trace($"[{requestId}] Content | {rawStringBuilder.ToString()}");
            }
        }

        private static HttpMethod GetHttpMethod<TParameter>(TParameter parameter, HttpParameterPackResult packResult)
            where TParameter : HttpServiceParameter
        {
            if (parameter.HttpMethod == HttpParameterMethod.NotSpecific)
            {
                // 依照 PackResult 中的 PostParameterMap 的數量來決定這次的 HttpMethod
                return ((packResult.PostParameterMap.Count > 0 || packResult.MutliPartParameterMap.Count > 0 || packResult.PostRawStringList.Count > 0 || packResult.PostRawData != null) ? HttpMethod.Post : HttpMethod.Get);
            }

            HttpMethod result = HttpMethod.Get;

            switch (parameter.HttpMethod)
            {
                case HttpParameterMethod.Delete:
                    result = HttpMethod.Delete;
                    break;
                case HttpParameterMethod.Get:
                    result = HttpMethod.Get;
                    break;
                case HttpParameterMethod.Head:
                    result = HttpMethod.Head;
                    break;
                case HttpParameterMethod.Options:
                    result = HttpMethod.Options;
                    break;
#if WINDOWS_UWP
                case HttpParameterMethod.Patch:
                    result = HttpMethod.Patch;
                    break;
#endif
                case HttpParameterMethod.Post:
                    result = HttpMethod.Post;
                    break;
                case HttpParameterMethod.Put:
                    result = HttpMethod.Put;
                    break;
            }

            return result;
        }

        private static void CreateHttpContent(HttpRequestMessage httpRequest, HttpParameterPackResult packResult)
        {
            // 三種 POST 的狀況只會執行其中一種，是互斥的
            if (packResult.MutliPartParameterMap.Count > 0)
            {
#if WINDOWS_UWP
                var multiPartContent = new HttpMultipartFormDataContent($"---------------{DateTime.Now.Ticks.ToString("x")}");
#else
                var multiPartContent = new MultipartFormDataContent($"---------------{DateTime.Now.Ticks.ToString("x")}");
#endif
                foreach (var kv in packResult.MutliPartParameterMap)
                {
                    var multiPartPackItem = kv.Value;
#if WINDOWS_UWP
                    var bufferContent = new HttpBufferContent(multiPartPackItem.Content.AsBuffer());
                    bufferContent.Headers.ContentType = new HttpMediaTypeHeaderValue(multiPartPackItem.ContentType);
#else
                    var bufferContent = new ByteArrayContent(multiPartPackItem.Content);
                    bufferContent.Headers.ContentType = new MediaTypeHeaderValue(multiPartPackItem.ContentType);
#endif
                    if (!string.IsNullOrEmpty(multiPartPackItem.FileName))
                    {
                        multiPartContent.Add(bufferContent, kv.Key, multiPartPackItem.FileName);
                    }
                    else
                    {
                        multiPartContent.Add(bufferContent, kv.Key);
                    }
                }

                httpRequest.Content = multiPartContent;
            }
            else if (packResult.PostRawStringList.Count > 0)
            {
                var contentString = string.Concat(packResult.PostRawStringList);
#if WINDOWS_UWP
                httpRequest.Content = new HttpStringContent(contentString);
#else
                httpRequest.Content = new StringContent(contentString);
#endif
            }
            else if (packResult.PostRawData != null)
            {
#if WINDOWS_UWP
                httpRequest.Content = new HttpBufferContent(packResult.PostRawData.AsBuffer());
                httpRequest.Content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/octet-stream");
#else
                httpRequest.Content = new ByteArrayContent(packResult.PostRawData);
                httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
#endif
            }
            else if (packResult.PostParameterMap.Count > 0)
            {
#if WINDOWS_UWP
                httpRequest.Content = new HttpFormUrlEncodedContent(packResult.PostParameterMap);
#else
                // FormUrlEncodedContent has limit length so we have to do ourselves
                var sb = new StringBuilder();
                foreach (var pair in packResult.PostParameterMap)
                {
                    sb.Append($"&{pair.Key}={WebUtility.UrlEncode(pair.Value)}");
                }

                if (sb.Length > 0)
                {
                    sb.Remove(0, 1);
                }

                httpRequest.Content = new StringContent(sb.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
#endif
            }
        }

        private static string GetRequestUrl<TParameter>(TParameter parameter, string baseRequestUrl, string getParameterString)
            where TParameter : HttpServiceParameter
        {
            if (string.IsNullOrEmpty(parameter.Options.Url.CustomUrl))
            {
                return HttpRequestUrlHelper.GetRequestUrl(baseRequestUrl, getParameterString);
            }

            if (parameter.Options.Url.IsByPassAutoGenerateUrl == false)
            {
                return HttpRequestUrlHelper.GetRequestUrl(parameter.Options.Url.CustomUrl, getParameterString);
            }

            return parameter.Options.Url.CustomUrl;
        }
    }
}
