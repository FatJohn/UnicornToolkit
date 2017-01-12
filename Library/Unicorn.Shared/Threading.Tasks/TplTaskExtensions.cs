// -----------------------------------------------------------------------
//  <copyright file="TplTaskExtensions.cs" company="Henric Jungheim">
//  Copyright (c) 2012-2016.
//  <author>Henric Jungheim</author>
//  </copyright>
// -----------------------------------------------------------------------
// Copyright (c) 2012-2016 Henric Jungheim <software@henric.org>
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
using System.Threading;
using System.Threading.Tasks;

namespace Unicorn
{
    public static class TplTaskExtensions
    {
        public static readonly Task CompletedTask;
        public static readonly Task NeverCompletedTask = new TaskCompletionSource<object>().Task;
        public static readonly Task<bool> TrueTask = Task.FromResult(true);
        public static readonly Task<bool> FalseTask = Task.FromResult(false);

        static TplTaskExtensions()
        {
            CompletedTask = TrueTask;
        }

#if WINDOWS_PHONE8
    /// <summary>
    /// http://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
        public static async Task<T> WithCancellation<T>(
            this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs, false))
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    throw new OperationCanceledException(cancellationToken);

            return await task.ConfigureAwait(false);
        }

        public static async Task WithCancellation(
            this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs, false))
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    throw new OperationCanceledException(cancellationToken);

            await task.ConfigureAwait(false);
        }
#else
        /// <summary>
        ///     http://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T> WithCancellation<T>(
            this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            // The only difference is the Task.WhenAny (instead of Task.WhenAny).
            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs, false))
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    throw new OperationCanceledException(cancellationToken);

            return await task.ConfigureAwait(false);
        }

        public static async Task WithCancellation(
            this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs, false))
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    throw new OperationCanceledException(cancellationToken);

            await task.ConfigureAwait(false);
        }
#endif
    }
}
