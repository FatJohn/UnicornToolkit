using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unicorn
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx"/>
    public class AsyncLock
    {
        private readonly AsyncSemaphore asyncSemaphore;
        private readonly Task<Releaser> releaser;

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock lockToRelease;

            internal Releaser(AsyncLock toRelease)
            {
                lockToRelease = toRelease;
            }

            public void Dispose()
            {
                if (lockToRelease != null)
                {
                    lockToRelease.asyncSemaphore.Release();
                }
            }
        }

        public AsyncLock()
        {
            asyncSemaphore = new AsyncSemaphore(1);
            releaser = Task.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            var wait = asyncSemaphore.WaitAsync();
            return wait.IsCompleted ?
                releaser :
                wait.ContinueWith((_, state) => new Releaser((AsyncLock)state),
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
    }
}
