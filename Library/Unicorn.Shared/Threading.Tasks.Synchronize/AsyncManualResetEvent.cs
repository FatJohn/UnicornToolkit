using System.Threading;
using System.Threading.Tasks;

namespace Unicorn
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://blogs.msdn.com/b/pfxteam/archive/2012/02/11/10266920.aspx"/>
    public class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

        public Task WaitAsync()
        {
            return taskCompletionSource.Task;
        }

        public void Set()
        {
            taskCompletionSource.TrySetResult(true);
        }

        public void Reset()
        {
            while (true)
            {
                var tcs = taskCompletionSource;
                if (!tcs.Task.IsCompleted ||
                    Interlocked.CompareExchange(ref taskCompletionSource, new TaskCompletionSource<bool>(), tcs) == tcs)
                {
                    return;
                }
            }
        }
    }
}
