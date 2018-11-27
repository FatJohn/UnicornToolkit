#if WINDOWS_UWP

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Unicorn
{
    public class TimeoutFilter : IHttpFilter
    {
        private readonly IHttpFilter _innerFilter;

        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(100);

        public TimeoutFilter()
            : this(new HttpBaseProtocolFilter())
        {
        }

        public TimeoutFilter(IHttpFilter innerFilter)
        {
            _innerFilter = innerFilter ?? throw new ArgumentException("innerFilter cannot be null.");
        }

        public IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> SendRequestAsync(HttpRequestMessage request)
        {
            return AsyncInfo.Run<HttpResponseMessage, HttpProgress>(async (cancellationToken, progress) =>
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    try
                    {
                        var timeout = request.GetTimeout() ?? DefaultTimeout;
                        var sendTask = _innerFilter.SendRequestAsync(request).AsTask(cts.Token, progress);
                        cts.CancelAfter(timeout);
                        return await sendTask;
                    }
                    catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                    {
                        // 這邊雖然是 throw TimeoutException ，但是外界攔到的會是 Exception，猜測是因為這段 code 是跑在 WinRT 裡面的關係
                        throw new TimeoutException();
                    }
                }
            });
        }

        public void Dispose()
        {
            _innerFilter.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

#endif