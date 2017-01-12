using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unicorn
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://blogs.msdn.com/b/pfxteam/archive/2012/02/11/10266930.aspx"/>
    public class AsyncCountdownEvent
    {
        private readonly AsyncManualResetEvent asyncManualResetEvent = new AsyncManualResetEvent();
        private int count;

        public AsyncCountdownEvent(int initialCount)
        {
            if (initialCount <= 0)
            {
                throw new ArgumentOutOfRangeException("initialCount");
            }
            count = initialCount;
        }

        public Task WaitAsync()
        {
            return asyncManualResetEvent.WaitAsync();
        }

        public void Signal()
        {
            if (count <= 0)
            {
                throw new InvalidOperationException();
            }

            int newCount = Interlocked.Decrement(ref count);
            if (newCount == 0)
            {
                asyncManualResetEvent.Set();
            }
            else if (newCount < 0)
            {
                throw new InvalidOperationException();
            }
        }

        public Task SignalAndWait()
        {
            Signal();
            return WaitAsync();
        }
    }
}
