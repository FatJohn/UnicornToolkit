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
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Unicorn
{
    public class CustomDispatcherService : IDispatcherService
    {
        private CoreDispatcher CoreDispatcher { get; set; }

        public CustomDispatcherService(CoreDispatcher dispatcher)
        {
            CoreDispatcher = dispatcher;
        }

        public bool IsRunOnUIThread
        {
            get
            {
                try
                {
                    return CoreDispatcher.HasThreadAccess;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

#pragma warning disable 4014
        public void InvokeAsync(Action action)
        {
            try
            {
                if (action == null)
                {
                    return;
                }

                CoreDispatcher.TryRunAsync(CoreDispatcherPriority.Normal, () => action());
            }
            catch (Exception)
            {
            }
        }

        public void InvokeAsync(Func<Task> asyncAction)
        {
            try
            {
                if (asyncAction == null)
                {
                    return;
                }

                CoreDispatcher.TryRunAsync(CoreDispatcherPriority.Normal, async () => await asyncAction());
            }
            catch (Exception)
            {
            }
        }
#pragma warning restore 4014

        public async Task InvokeAwaitableAsync(Action action)
        {
            try
            {
                if (action == null)
                {
                    return;
                }

                await CoreDispatcher.TryRunAsync(CoreDispatcherPriority.Normal, () => action());
            }
            catch (Exception)
            {
            }
        }

        public async Task InvokeAwaitableAsync(Func<Task> asyncAction)
        {
            try
            {
                if (asyncAction == null)
                {
                    return;
                }

                TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();
                await CoreDispatcher.TryRunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await asyncAction();
                    completionSource.TrySetResult(true);
                });
                await completionSource.Task;
            }
            catch (Exception)
            {
            }
        }

        public async Task<T> InvokeAwaitableAsync<T>(Func<T> func)
        {
            try
            {
                if (func == null)
                {
                    return await Task.FromResult(default(T));
                }

                TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
                await CoreDispatcher.TryRunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var result = func();
                    completionSource.TrySetResult(result);
                });
                return await completionSource.Task;
            }
            catch (Exception)
            {
                return await Task.FromResult(default(T));
            }
        }

        public async Task<T> InvokeAwaitableAsync<T>(Func<Task<T>> func)
        {
            try
            {
                if (func == null)
                {
                    return await Task.FromResult(default(T));
                }

                TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
                await CoreDispatcher.TryRunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var result = await func();
                    completionSource.TrySetResult(result);
                });
                return await completionSource.Task;
            }
            catch (Exception)
            {
                return await Task.FromResult(default(T));
            }
        }
    }
}
