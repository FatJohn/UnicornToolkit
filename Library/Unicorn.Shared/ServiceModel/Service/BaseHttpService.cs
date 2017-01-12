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
#if WINDOWS_UWP
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http.Filters;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
#else
using System.Net.Http;
#endif

namespace Unicorn.ServiceModel
{
#if WINDOWS_UWP
    public delegate void HttpServiceProgressCallback(double? progress);
#endif

    public abstract class BaseHttpService<TResult, TParameter, TParser>
        where TParameter : HttpServiceParameter
        where TParser : BaseParser<TResult, HttpResponseMessage>, new()
    {
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// 發送 Request 前的前置處理步驟
        /// </summary>
        public List<IHttpPreFlow<TParameter>> PreProcessFlow { get; } = new List<IHttpPreFlow<TParameter>>();

#if WINDOWS_UWP
        private WeakReference<HttpServiceProgressCallback> sendProgressCallbackReference;
        public HttpServiceProgressCallback SendProgressCallback
        {
            private get
            {
                HttpServiceProgressCallback callback = null;
                sendProgressCallbackReference?.TryGetTarget(out callback);
                return callback;
            }
            set
            {
                if (sendProgressCallbackReference == null)
                {
                    sendProgressCallbackReference = new WeakReference<HttpServiceProgressCallback>(value);
                }
                else
                {
                    sendProgressCallbackReference.SetTarget(value);
                }
            }
        }

        private WeakReference<HttpServiceProgressCallback> receiveProgressCallbackReference;
        public HttpServiceProgressCallback ReceiveProgressCallback
        {
            private get
            {
                HttpServiceProgressCallback callback = null;
                receiveProgressCallbackReference?.TryGetTarget(out callback);
                return callback;
            }
            set
            {
                if (receiveProgressCallbackReference == null)
                {
                    receiveProgressCallbackReference = new WeakReference<HttpServiceProgressCallback>(value);
                }
                else
                {
                    receiveProgressCallbackReference.SetTarget(value);
                }
            }
        }
#endif

        public BaseHttpService()
        {
        }

        public virtual async Task<ParseResult<TResult>> InvokeAsync(TParameter parameter, CancellationTokenSource cancellationTokenSource = null)
        {
            this.cancellationTokenSource = cancellationTokenSource;
            var taskCompletionSource = new TaskCompletionSource<ParseResult<TResult>>();

            var invokeTask = Task.Run(async () =>
            {
                var result = await Invoke(parameter).ConfigureAwait(false);
                taskCompletionSource.TrySetResult(result);
            });

            return await taskCompletionSource.Task.ConfigureAwait(false);
        }

        protected virtual Task<ParseResult<TResult>> Invoke(TParameter parameter)
        {
            if (parameter.IsEnableCache && parameter.CacheMinutes > 0)
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

                var cacheResponse = await CreateCacheResponse(parameter, requestUrl);
                if (cacheResponse == null)
                {
                    return await RemoteInvoke(parameter).ConfigureAwait(false);
                }

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
#if WINDOWS_UWP
            var rootFolder = ApplicationData.Current.LocalCacheFolder;
            var storageItem = await rootFolder.TryGetItemAsync(cacheFileName);
            var cacheFile = storageItem as StorageFile;
            if (cacheFile == null)
            {
                return null;
            }

            var cacheFileStream = await cacheFile.OpenReadAsync();
#else
            var cacheFileStream = await PlatformService.File.OpenReadStreamAsync(cacheFileName).ConfigureAwait(false);
#endif
            if (cacheFileStream == null)
            {
                return null;
            }

            // 4. 讀取 cache 的時間
            var cacheContent = await ReadCacheContent(parameter, cacheFileStream);
            if (cacheContent == null)
            {
                return null;
            }

            return CreateCacheResponse(cacheContent);
        }

#if WINDOWS_UWP
        private async Task<IBuffer> ReadCacheContent(TParameter parameter, IRandomAccessStream cacheFileStream)
        {
            var cacheFileSize = Convert.ToUInt32(cacheFileStream.Size);
            var readNativeBuffer = new Windows.Storage.Streams.Buffer(cacheFileSize);
            await cacheFileStream.ReadAsync(readNativeBuffer, readNativeBuffer.Capacity, InputStreamOptions.None);

            cacheFileStream.Dispose();

            const int cacheTimeTicksLength = sizeof(long);
            var readStartIndex = cacheFileSize - cacheTimeTicksLength;
            var ticksBytes = new byte[cacheTimeTicksLength];
            readNativeBuffer.CopyTo(readStartIndex, ticksBytes, 0, ticksBytes.Length);

            var cacheTimeTicks = BitConverter.ToInt64(ticksBytes, 0);
            if (!IsValidCacheTime(parameter, cacheTimeTicks))
            {
                return null;
            }

            readNativeBuffer.Length = readStartIndex;

            return readNativeBuffer;
        }

        private HttpResponseMessage CreateCacheResponse(IBuffer cacheContent)
        {
            var httpResponse = new HttpResponseMessage(Windows.Web.Http.HttpStatusCode.Ok);
            httpResponse.Content = new HttpBufferContent(cacheContent);
            return httpResponse;
        }
#else
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
#endif

        private bool IsValidCacheTime(TParameter parameter, long cacheTimeTicks)
        {
            var cacheTime = new DateTime(cacheTimeTicks);
            return (DateTime.Now.Subtract(cacheTime).TotalMinutes < parameter.CacheMinutes);
        }

        #endregion

        #region 正常發送遠端的 Request 的 Method

        protected virtual async Task<ParseResult<TResult>> RemoteInvoke(TParameter parameter)
        {
            try
            {
                await PreProcess(parameter).ConfigureAwait(false);

                var requestUrl = CreateRequestUri(parameter);

                // 1. pack HttpPackResult
                var packResult = HttpParameterPacker.CreatePackedParameterResult(parameter);

                // 2. create HttpRequestMessage
                var httpRequest = PackParameterToHttpReqeustMessage(parameter, requestUrl, packResult);

                // 3. Send & Get response
                var httpResponse = await SendRequest(httpRequest).ConfigureAwait(false);

                if (httpResponse == null)
                {
                    return new ParseResult<TResult>(new ParseError(true));
                }

                // 4. Save cache
                if (parameter.IsEnableCache && parameter.CacheMinutes > 0)
                {
                    await SaveCache(parameter, requestUrl, packResult, httpResponse).ConfigureAwait(false);
                }

                if (cancellationTokenSource?.IsCancellationRequested == true)
                {
                    httpResponse.Content.Dispose();
                    httpResponse.Dispose();

                    return new ParseResult<TResult>(new ParseError(true));
                }

                var parseResult = await ParseResponse(httpResponse).ConfigureAwait(false);

                // 5. free resource
                if (httpResponse != null)
                {
                    httpResponse.Content.Dispose();
                    httpResponse.Dispose();
                }

                if (cancellationTokenSource?.IsCancellationRequested == true)
                {
                    parseResult = new ParseResult<TResult>(new ParseError(true));
                }

                return parseResult;
            }
            catch (Exception ex)
            {
                return new ParseResult<TResult>(new ParseError(ex.Message));
            }
        }

        protected abstract string CreateRequestUri(TParameter parameter);

        protected virtual HttpRequestMessage PackParameterToHttpReqeustMessage(TParameter parameter, string requestUrl, HttpParameterPackResult packResult)
        {
            return HttpRequestCreator.Create(parameter, requestUrl, packResult);
        }

        protected virtual async Task<HttpResponseMessage> SendRequest(HttpRequestMessage httpRequestMessage)
        {
            var httpClient = HttpClientContainer.Get();

#if WINDOWS_UWP
            var asyncOperationWithProgress = httpClient.SendRequestAsync(httpRequestMessage);
            asyncOperationWithProgress.Progress = HttpClientProgressCallback;

            try
            {
                cancellationTokenSource?.Token.Register(() =>
                {
                    asyncOperationWithProgress.Cancel();
                });
            }
            catch (ObjectDisposedException)
            {
                asyncOperationWithProgress.Cancel();
                return null;
            }

            try
            {
                return await asyncOperationWithProgress;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
#else
            try
            {
                var cancellationToken = cancellationTokenSource == null ? CancellationToken.None : cancellationTokenSource.Token;
                return await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
#endif
        }

#if WINDOWS_UWP
        private void HttpClientProgressCallback(IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> result, HttpProgress progress)
        {
            double? progressValue = null;

            switch (progress.Stage)
            {
                case HttpProgressStage.SendingContent:
                    if (progress.TotalBytesToSend.HasValue)
                    {
                        progressValue = (double)progress.BytesSent / (double)progress.TotalBytesToSend.Value;
                    }
                    SendProgressCallback?.Invoke(progressValue);
                    break;
                case HttpProgressStage.ReceivingContent:
                    if (progress.TotalBytesToReceive.HasValue)
                    {
                        progressValue = (double)progress.BytesReceived / (double)progress.TotalBytesToReceive.Value;
                    }
                    ReceiveProgressCallback?.Invoke(progressValue);
                    break;
            }
        }

        protected async Task<HttpResponseMessage> SendRedirectableRequest(HttpRequestMessage httpRequestMessage)
        {
            var httpFilter = new HttpBaseProtocolFilter();
            httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            httpFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
            httpFilter.AllowAutoRedirect = false;
            var client = new HttpClient(httpFilter);

            var response = await SendRedirectableRequestRecursively(client, httpRequestMessage).ConfigureAwait(false);

            client.Dispose();
            httpFilter.Dispose();

            return response;
        }

        private async Task<HttpResponseMessage> SendRedirectableRequestRecursively(HttpClient client, HttpRequestMessage httpRequestMessage)
        {
            try
            {
                var response = await client.SendRequestAsync(httpRequestMessage);
                if (response.StatusCode != Windows.Web.Http.HttpStatusCode.Found)
                {
                    return response;
                }

                var cloneHttpRequest = new HttpRequestMessage(httpRequestMessage.Method, response.Headers.Location);
                if (httpRequestMessage.Content != null)
                {
                    var buffer = await httpRequestMessage.Content.ReadAsBufferAsync();
                    cloneHttpRequest.Content = new HttpBufferContent(buffer);

                    // copy headers
                    if (httpRequestMessage.Content.Headers != null)
                    {
                        foreach (var header in httpRequestMessage.Headers)
                        {
                            cloneHttpRequest.Content.Headers.Add(header);
                        }
                    }
                }

                foreach (var httpProperty in httpRequestMessage.Properties)
                {
                    cloneHttpRequest.Properties.Add(httpProperty);
                }

                foreach (var header in httpRequestMessage.Headers)
                {
                    cloneHttpRequest.Headers.Add(header);
                }

                return await SendRedirectableRequestRecursively(client, cloneHttpRequest).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }
#endif

        private async Task SaveCache(TParameter parameter, string baseRequestUrl, HttpParameterPackResult packResult, HttpResponseMessage httpResponse)
        {
            var cacheFileName = CreateCacheFileName(baseRequestUrl, packResult);
#if WINDOWS_UWP
            var rootFolder = ApplicationData.Current.LocalCacheFolder;
            IRandomAccessStream cacheFileStream = null;
#else
            Stream cacheFileStream = null;
#endif

            try
            {
#if WINDOWS_UWP
                var cacheFile = await rootFolder.CreateFileAsync(cacheFileName, CreationCollisionOption.ReplaceExisting);
                cacheFileStream = await cacheFile.OpenAsync(FileAccessMode.ReadWrite);
                var content = await httpResponse.Content.ReadAsBufferAsync();
                await cacheFileStream.WriteAsync(content);
#else
                cacheFileStream = await PlatformService.File.OpenWriteStreamAsync(cacheFileName);
                var content = await httpResponse.Content.ReadAsByteArrayAsync();
                await cacheFileStream.WriteAsync(content, 0, content.Length);
#endif
                var cacheTimeTicks = DateTime.Now.Ticks;
                var cacheTimeTicksBytes = BitConverter.GetBytes(cacheTimeTicks);
#if WINDOWS_UWP
                await cacheFileStream.WriteAsync(cacheTimeTicksBytes.AsBuffer());
#else
                await cacheFileStream.WriteAsync(cacheTimeTicksBytes, 0, cacheTimeTicksBytes.Length);
#endif
                await cacheFileStream.FlushAsync();
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
                    cacheFileStream = null;
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
