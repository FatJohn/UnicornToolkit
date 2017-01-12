using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unicorn
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/building-async-coordination-primitives-part-7-asyncreaderwriterlock.aspx"/>
    public class AsyncReaderWriterLock
    {
        private readonly Task<Releaser> readerReleaser;
        private readonly Task<Releaser> writerReleaser;

        private readonly Queue<TaskCompletionSource<Releaser>> waitingWriters = new Queue<TaskCompletionSource<Releaser>>();
        private TaskCompletionSource<Releaser> waitingReader = new TaskCompletionSource<Releaser>();
        private int readersWaiting;

        private int status;

        public struct Releaser : IDisposable
        {
            private readonly AsyncReaderWriterLock toRelease;
            private readonly bool writer;

            internal Releaser(AsyncReaderWriterLock toRelease, bool writer)
            {
                this.toRelease = toRelease;
                this.writer = writer;
            }

            public void Dispose()
            {
                if (toRelease != null)
                {
                    if (writer)
                    {
                        toRelease.WriterRelease();
                    }
                    else
                    {
                        toRelease.ReaderRelease();
                    }
                }
            }
        }

        public AsyncReaderWriterLock()
        {
            readerReleaser = Task.FromResult(new Releaser(this, false));
            writerReleaser = Task.FromResult(new Releaser(this, true));
        }

        public Task<Releaser> ReaderLockAsync()
        {
            lock (waitingWriters)
            {
                if (status >= 0 && waitingWriters.Count == 0)
                {
                    ++status;
                    return readerReleaser;
                }
                else
                {
                    ++readersWaiting;
                    return waitingReader.Task.ContinueWith(t => t.Result);
                }
            }
        }

        public Task<Releaser> WriterLockAsync()
        {
            lock (waitingWriters)
            {
                if (status == 0)
                {
                    status = -1;
                    return writerReleaser;
                }
                else
                {
                    var waiter = new TaskCompletionSource<Releaser>();
                    waitingWriters.Enqueue(waiter);
                    return waiter.Task;
                }
            }
        }

        private void ReaderRelease()
        {
            TaskCompletionSource<Releaser> toWake = null;

            lock (waitingWriters)
            {
                --status;
                if (status == 0 && waitingWriters.Count > 0)
                {
                    status = -1;
                    toWake = waitingWriters.Dequeue();
                }
            }

            if (toWake != null)
            {
                toWake.SetResult(new Releaser(this, true));
            }
        }

        private void WriterRelease()
        {
            TaskCompletionSource<Releaser> toWake = null;
            bool toWakeIsWriter = false;

            lock (waitingWriters)
            {
                if (waitingWriters.Count > 0)
                {
                    toWake = waitingWriters.Dequeue();
                    toWakeIsWriter = true;
                }
                else if (readersWaiting > 0)
                {
                    toWake = waitingReader;
                    status = readersWaiting;
                    readersWaiting = 0;
                    waitingReader = new TaskCompletionSource<Releaser>();
                }
                else
                {
                    status = 0;
                }
            }

            if (toWake != null)
            {
                toWake.SetResult(new Releaser(this, toWakeIsWriter));
            }
        }
    }
}
