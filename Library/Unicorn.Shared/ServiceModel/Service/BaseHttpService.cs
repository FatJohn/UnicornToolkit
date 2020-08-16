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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Unicorn.Net;
using System.IO;
using System.Net.Http;

namespace Unicorn.ServiceModel
{
    public abstract class BaseHttpService<TResult, TParameter, TParser>
        where TParameter : HttpServiceParameter
        where TParser : BaseParser<TResult, HttpResponseMessage>, new()
    {
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// 發送 Request 前的前置處理步驟
        /// </summary>
        public List<IHttpPreFlow> PreProcessFlow { get; } = new List<IHttpPreFlow>();

        protected BaseHttpService()
        {
        }

        public virtual Task<ParseResult<TResult>> InvokeAsync(TParameter parameter, CancellationTokenSource cancellationTokenSource = null)
        {
            this.cancellationTokenSource = cancellationTokenSource;

            if (this.cancellationTokenSource == null)
            {
                return Invoke(parameter);
            }

            if (this.cancellationTokenSource.IsCancellationRequested)
            {
                return Task.FromResult(new ParseResult<TResult>(new ParseError(true)));
            }

            return Invoke(parameter);
        }

        protected virtual Task<ParseResult<TResult>> Invoke(TParameter parameter)
        {
            if (parameter.Options.Cache.IsEnable && parameter.Options.Cache.Minutes > 0)
            {
                return CacheInvoke(parameter);
            }
            else
            {
                return RemoteInvoke(parameter);
            }
        }

        #region 讀取 Cache 的流程 Method

        protected virtual async Task<ParseResult<TResult>> CacheInvoke(TParameter parameter)
        {
            try
            {
                await PreProcess(parameter).ConfigureAwait(false);

                var requestUrl = CreateRequestUri(parameter);

                var cacheResponse = await CreateCacheResponse(parameter, requestUrl).ConfigureAwait(false);
                if (cacheResponse == null)
                {
                    return await RemoteInvoke(parameter).ConfigureAwait(false);
                }

                cacheResponse.RequestMessage = new HttpRequestMessage();
                var requestId = cacheResponse.RequestMessage.AddRequestId();

                PlatformService.Log?.Trace($"[{requestId}] RequestUrl | {requestUrl} | Cache");

                var result = await ParseResponse(cacheResponse).ConfigureAwait(false);

                cacheResponse.Content.Dispose();
                cacheResponse.Dispose();

                if (cancellationTokenSource?.IsCancellationRequested == true)
                {
                    result = new ParseResult<TResult>(new ParseError(true));
                }

                return result;
            }
            catch (Exception ex)
            {
                return new ParseResult<TResult>(new ParseError(ex.Message));
            }
        }

        private async Task<HttpResponseMessage> CreateCacheResponse(TParameter parameter, string requestUrl)
        {
            // 1. pack HttpPackResult
            var packResult = HttpParameterPacker.CreatePackedParameterResult(parameter);

            // 2. 取得 Cache 檔案名稱
            var cacheFileName = CreateCacheFileName(requestUrl, packResult);

            // 3. 檢查檔案是否存在
            var cacheFileStream = await PlatformService.File.OpenReadStreamAsync(cacheFileName).ConfigureAwait(false);
            if (cacheFileStream == null)
            {
                return null;
            }

            // 4. 讀取 cache 的時間
            var cacheContent = await ReadCacheContent(parameter, cacheFileStream).ConfigureAwait(false);
            if (cacheContent == null)
            {
                return null;
            }

            return CreateCacheResponse(cacheContent);
        }

        private Task<byte[]> ReadCacheContent(TParameter parameter, Stream cacheFileStream)
        {
            var br = new BinaryReader(cacheFileStream);

            const int cacheTimeTicksLength = sizeof(long);
            cacheFileStream.Seek(cacheTimeTicksLength, SeekOrigin.End);

            var cacheTimeTicks = br.ReadInt64();
            if (!IsValidCacheTime(parameter, cacheTimeTicks))
            {
                br.Dispose();
                cacheFileStream.Dispose();
                return null;
            }

            cacheFileStream.Seek(0, SeekOrigin.Begin);

            var contentLength = Convert.ToInt32(cacheFileStream.Length - cacheTimeTicksLength);
            var contentBytes = br.ReadBytes(contentLength);
            br.Dispose();
            cacheFileStream.Dispose();

            return Task.FromResult(contentBytes);
        }

        private HttpResponseMessage CreateCacheResponse(byte[] cacheContent)
        {
            var httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            httpResponse.Content = new ByteArrayContent(cacheContent);
            return httpResponse;
        }

        private bool IsValidCacheTime(TParameter parameter, long cacheTimeTicks)
        {
            var cacheTime = new DateTime(cacheTimeTicks);
            return (DateTime.Now.Subtract(cacheTime).TotalMinutes < parameter.Options.Cache.Minutes);
        }

        #endregion

        #region 正常發送遠端的 Request 的 Method

        protected virtual async Task<ParseResult<TResult>> RemoteInvoke(TParameter parameter)
        {
            HttpResponseMessage httpResponse = null;

            try
            {
                if (PlatformService.NetworkInformation?.IsNetworkAvailable == false)
                {
                    return new ParseResult<TResult>(new ParseError(true));
                }

                await PreProcess(parameter).ConfigureAwait(false);

                var requestUrl = CreateRequestUri(parameter);

                // 1. pack HttpPackResult
                var packResult = HttpParameterPacker.CreatePackedParameterResult(parameter);

                // 2. Send & Get response
                try
                {
                    httpResponse = await SendRequest(parameter, () => PackParameterToHttpReqeustMessage(parameter, requestUrl, packResult)).ConfigureAwait(false);
                }
                catch (ObjectDisposedException ex)
                {
                    // 還沒開始就被 cancel 了
                    return new ParseResult<TResult>(new ParseError(ex.HResult, "Operation already Canceled", true));
                }
                catch (OperationCanceledException ex)
                {
                    return new ParseResult<TResult>(new ParseError(ex.HResult, ex.Message, true));
                }
                catch (TimeoutException ex)
                {
                    return new ParseResult<TResult>(new ParseError(ex.HResult, ex.Message));
                }

                // 4. Save cache
                if (parameter.Options.Cache.IsEnable && parameter.Options.Cache.Minutes > 0)
                {
                    await SaveCache(parameter, requestUrl, packResult, httpResponse).ConfigureAwait(false);
                }

                if (cancellationTokenSource?.IsCancellationRequested == true)
                {
                    return new ParseResult<TResult>(new ParseError(true));
                }

                var parseResult = await ParseResponse(httpResponse).ConfigureAwait(false);

                if (cancellationTokenSource?.IsCancellationRequested == true)
                {
                    parseResult = new ParseResult<TResult>(new ParseError(true));
                }

                return parseResult;
            }
            catch (Exception ex)
            {
                return new ParseResult<TResult>(new ParseError(ex.HResult, ex.Message));
            }
            finally
            {
                if (httpResponse != null)
                {
                    httpResponse.Content?.Dispose();
                    httpResponse.Dispose();
                    httpResponse = null;
                }
            }
        }

        protected abstract string CreateRequestUri(TParameter parameter);

        protected virtual HttpRequestMessage PackParameterToHttpReqeustMessage(TParameter parameter, string requestUrl, HttpParameterPackResult packResult)
        {
            return HttpRequestCreator.Create(parameter, requestUrl, packResult);
        }

        protected virtual async Task<HttpResponseMessage> SendRequest(TParameter parameter, Func<HttpRequestMessage> httpRequestMessageDelegate)
        {
            var retryControl = parameter.Options.Retry;
            var retryRemain = retryControl.MaxRetryTimes;
            HttpResponseMessage httpResponse = null;
            Exception lastOccurException = null;

            do
            {
                lastOccurException = null;

                var httpRequestMessage = httpRequestMessageDelegate();
                if (httpRequestMessage == null)
                {
                    break;
                }

                httpRequestMessage.SetTimeout(parameter.Timeout);

                try
                {
                    httpResponse = await SendRequest(httpRequestMessage).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    lastOccurException = ex;
                }

                httpRequestMessage.Dispose();

                if (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
                {
                    // 被 cancel 了就不要有回傳的東西了
                    httpResponse?.Dispose();
                    httpResponse = null;
                    break;
                }

                if (httpResponse != null && httpResponse.IsSuccessStatusCode)
                {
                    break;
                }

                if (retryRemain <= 0)
                {
                    break;
                }

                await Task.Delay(retryControl.Interval).ConfigureAwait(false);
            }
            while (retryRemain-- > 0);

            if (lastOccurException != null)
            {
                throw lastOccurException;
            }

            return httpResponse;
        }

        protected virtual Task<HttpResponseMessage> SendRequest(HttpRequestMessage httpRequestMessage)
        {
            var httpClient = HttpClientContainer.Get();

            if (cancellationTokenSource == null)
            {
                return httpClient.SendAsync(httpRequestMessage);
            }
            else
            {
                return httpClient.SendAsync(httpRequestMessage, cancellationTokenSource.Token);
            }
        }

        private async Task SaveCache(TParameter parameter, string baseRequestUrl, HttpParameterPackResult packResult, HttpResponseMessage httpResponse)
        {
            var cacheFileName = CreateCacheFileName(baseRequestUrl, packResult);
            Stream cacheFileStream = null;

            try
            {

                cacheFileStream = await PlatformService.File.OpenWriteStreamAsync(cacheFileName).ConfigureAwait(false);
                var content = await httpResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                await cacheFileStream.WriteAsync(content, 0, content.Length).ConfigureAwait(false);

                var cacheTimeTicks = DateTime.Now.Ticks;
                var cacheTimeTicksBytes = BitConverter.GetBytes(cacheTimeTicks);
                await cacheFileStream.WriteAsync(cacheTimeTicksBytes, 0, cacheTimeTicksBytes.Length).ConfigureAwait(false);
                await cacheFileStream.FlushAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PlatformService.Log?.Error(ex);
            }
            finally
            {
                if (cacheFileStream != null)
                {
                    cacheFileStream.Dispose();
                }
            }
        }

        #endregion

        private string CreateCacheFileName(string baseRequestUrl, HttpParameterPackResult packResult)
        {
            var fileName = packResult.GetCombindedString;
            packResult.PostParameterMap.ForEach(p =>
            {
                fileName += string.Format("{0}{1}", p.Key, p.Value);
            });

            fileName += baseRequestUrl;
            return PlatformService.Cryptography.HashSHA1(fileName);
        }

        protected virtual async Task PreProcess(TParameter parameter)
        {
            foreach (var preFlow in PreProcessFlow)
            {
                await preFlow.Process(parameter).ConfigureAwait(false);
            }
        }

        protected virtual async Task<ParseResult<TResult>> ParseResponse(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage == null)
            {
                return new ParseResult<TResult>(new ParseError("Request fail"));
            }

            var parser = new TParser();
            return await parser.Parse(httpResponseMessage).ConfigureAwait(false);
        }
    }
}
