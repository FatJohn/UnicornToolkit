using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Unicorn
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://blogs.msdn.com/b/pfxteam/archive/2012/02/11/10266932.aspx"/>
    public class AsyncBarrier
    {
        private readonly int participantCount;
        private int remainingParticipants;
        private ConcurrentStack<TaskCompletionSource<bool>> waiters;

        public AsyncBarrier(int participantCount)
        {
            if (participantCount <= 0)
            {
                throw new ArgumentOutOfRangeException("participantCount");
            }

            remainingParticipants = this.participantCount = participantCount;
            waiters = new ConcurrentStack<TaskCompletionSource<bool>>();
        }

        public Task SignalAndWait()
        {
            var tcs = new TaskCompletionSource<bool>();
            waiters.Push(tcs);
            if (Interlocked.Decrement(ref remainingParticipants) == 0)
            {
                remainingParticipants = participantCount;
                var waiters = this.waiters;
                this.waiters = new ConcurrentStack<TaskCompletionSource<bool>>();
                Parallel.ForEach(waiters, w => w.SetResult(true));
            }
            return tcs.Task;
        }
    }
}
