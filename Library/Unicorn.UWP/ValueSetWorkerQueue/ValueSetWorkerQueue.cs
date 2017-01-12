// -----------------------------------------------------------------------
//  <copyright file="ValueSetWorkerQueue.cs" company="Henric Jungheim">
//  Copyright (c) 2012-2015.
//  <author>Henric Jungheim</author>
//  </copyright>
// -----------------------------------------------------------------------
// Copyright (c) 2012-2015 Henric Jungheim <software@henric.org>
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace Unicorn
{
    public sealed class ValueSetWorkerQueue
    {
        private readonly Queue<ValueSet> queue = new Queue<ValueSet>();
        private readonly SignalTask signalTask;

        public ValueSetWorkerQueue(Func<ValueSet, Task> handler)
        {
            signalTask = new SignalTask(async () =>
            {
                for (;;)
                {
                    ValueSet valueSet;

                    lock (queue)
                    {
                        if (queue.Count < 1)
                        {
                            break;
                        }

                        valueSet = queue.Dequeue();
                    }

                    try
                    {
                        await handler(valueSet).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        PlatformService.Log?.Error(ex);
                    }
                }
            });
        }

        #region IDisposable Members

        public void Dispose()
        {
            signalTask.Dispose();
        }

        #endregion

        public void Submit(ValueSet valueSet)
        {
            int count;

            lock (queue)
            {
                count = queue.Count;

                queue.Enqueue(valueSet);
            }

            if (0 == count)
            {
                signalTask.Fire();
            }
        }
    }
}
