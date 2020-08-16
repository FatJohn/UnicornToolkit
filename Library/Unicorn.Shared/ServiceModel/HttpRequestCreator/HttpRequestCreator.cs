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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Unicorn.ServiceModel
{
    public static class HttpRequestCreator
    {
        public static HttpRequestMessage Create<TParameter>(TParameter parameter, string baseRequestUrl, HttpParameterPackResult packResult)
            where TParameter : HttpServiceParameter
        {
            var httpMethod = GetHttpMethod(parameter, packResult);
            // 取得的 Url
            var requestUrl = GetRequestUrl(parameter, baseRequestUrl, packResult.GetCombindedString);
            if (Uri.TryCreate(requestUrl, UriKind.RelativeOrAbsolute, out var requestUri) == false)
            {
                return null;
            }
            // 依照 requestUrl 產生 Request Url
            var httpRequest = new HttpRequestMessage(httpMethod, requestUri);
            // 方便辨識送出的 request
            var requestId = httpRequest.AddRequestId();
            // 加入 Header
            packResult.HeaderParameterMap.ForEach((keyValue) => httpRequest.Headers.Add(keyValue.Key, keyValue.Value));
            // 加入 Body content
            CreateHttpContent(httpRequest, packResult);

            if (parameter.HttpContentType != null && httpRequest.Content?.Headers != null)
            {
                httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(parameter.HttpContentType);
            }

            LogParameterPackResult(httpRequest, packResult, requestId);

            return httpRequest;
        }

        private static void LogParameterPackResult(HttpRequestMessage httpRequest, HttpParameterPackResult packResult, string requestId)
        {
            var logger = PlatformService.Log;

            if (logger == null)
            {
                return;
            }

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
                case HttpParameterMethod.Patch:
                    result = new HttpMethod("PATCH");
                    break;
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
                var multiPartContent = new MultipartFormDataContent($"---------------{DateTime.Now.Ticks.ToString("x")}");
                foreach (var kv in packResult.MutliPartParameterMap)
                {
                    var multiPartPackItem = kv.Value;
                    var bufferContent = new ByteArrayContent(multiPartPackItem.Content);
                    bufferContent.Headers.ContentType = new MediaTypeHeaderValue(multiPartPackItem.ContentType);
                    
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
                httpRequest.Content = new StringContent(contentString);
            }
            else if (packResult.PostRawData != null)
            {
                httpRequest.Content = new ByteArrayContent(packResult.PostRawData);
                httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
            else if (packResult.PostParameterMap.Count > 0)
            {
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
