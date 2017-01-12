// -----------------------------------------------------------------------
//  <copyright file="ExceptionExtensions.cs" company="Henric Jungheim">
//  Copyright (c) 2012-2014.
//  <author>Henric Jungheim</author>
//  </copyright>
// -----------------------------------------------------------------------
// Copyright (c) 2012-2014 Henric Jungheim <software@henric.org>
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
using System.Text;

namespace Unicorn
{
    public static class ExceptionExtensions
    {
        public static string ExtendedMessage(this Exception ex)
        {
#if DEBUG
            if (null == ex)
            {
                throw new ArgumentNullException("ex");
            }
#endif

            if (null == ex.InnerException)
            {
                var aggregateException = ex as AggregateException;

                if (null == aggregateException)
                {
                    return string.IsNullOrEmpty(ex.Message) ? ex.GetType().FullName : ex.Message;
                }
            }

            var sb = new StringBuilder();

            DumpException(sb, 0, ex);

            return sb.ToString();
        }

        private static void DumpException(StringBuilder sb, int indent, AggregateException aggregateException)
        {
            AppendWithIndent(sb, indent, aggregateException);

            foreach (var ex in aggregateException.InnerExceptions)
            {
                DumpException(sb, indent + 1, ex);
            }
        }

        private static void DumpException(StringBuilder sb, int indent, Exception exception)
        {
            var aggregateException = exception as AggregateException;

            if (null != aggregateException)
            {
                DumpException(sb, indent, aggregateException);
            }
            else
            {
                AppendWithIndent(sb, indent, exception);
            }
        }

        private static void AppendWithIndent(StringBuilder sb, int indent, Exception exception)
        {
            sb.Append(' ', indent * 3);
            sb.AppendLine(exception.GetType().FullName + ": " + exception.Message);

            if (null != exception.InnerException)
            {
                DumpException(sb, indent + 1, exception.InnerException);
            }
        }
    }
}
