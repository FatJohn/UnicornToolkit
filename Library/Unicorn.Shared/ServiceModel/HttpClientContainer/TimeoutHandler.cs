#if !WINDOWS_UWP

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unicorn
{
    public class TimeoutHandler : DelegatingHandler
    {
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(100);

        public TimeoutHandler()
            : this(new HttpClientHandler())
        {
        }

        public TimeoutHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                try
                {
                    var timeout = request.GetTimeout() ?? DefaultTimeout;
                    var sendTask = base.SendAsync(request, cts?.Token ?? cancellationToken);
                    cts.CancelAfter(timeout);
                    return await sendTask;
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException();
                }
            }
        }
    }
}
#endif