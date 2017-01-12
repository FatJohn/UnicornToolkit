using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unicorn
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://blogs.msdn.com/b/pfxteam/archive/2012/02/11/10266923.aspx"/>
    public class AsyncAutoResetEvent
    {
        private readonly static Task completedTask = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> watiTaskCompletionQueue = new Queue<TaskCompletionSource<bool>>();
        private bool signaled;

        public Task WaitAsync()
        {
            lock (watiTaskCompletionQueue)
            {
                if (signaled)
                {
                    signaled = false;
                    return completedTask;
                }
                else
                {
                    var tcs = new TaskCompletionSource<bool>();
                    watiTaskCompletionQueue.Enqueue(tcs);
                    return tcs.Task;
                }
            }
        }

        public void Set()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (watiTaskCompletionQueue)
            {
                if (watiTaskCompletionQueue.Count > 0)
                {
                    toRelease = watiTaskCompletionQueue.Dequeue();
                }
                else if (!signaled)
                {
                    signaled = true;
                }
            }
            if (toRelease != null)
            {
                toRelease.SetResult(true);
            }
        }
    }
}
