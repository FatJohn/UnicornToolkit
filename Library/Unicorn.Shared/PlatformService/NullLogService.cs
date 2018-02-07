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
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Unicorn
{
    public class NullLogService : ILogService
    {
        public void Debug(string content = "", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            System.Diagnostics.Debug.WriteLine(GetGeneralString("DEBUG", content, callerMemberName, sourceFilePath, sourceLineNumber));
        }

        public void Error(Exception exception = null, string content = "", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            System.Diagnostics.Debug.WriteLine(GetGeneralString("ERROR", string.Empty, callerMemberName, sourceFilePath, sourceLineNumber), exception);
        }

        public void Trace(string content = "", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            System.Diagnostics.Debug.WriteLine(GetGeneralString("TRACE", content, callerMemberName, sourceFilePath, sourceLineNumber));
        }

        private string GetGeneralString(string level, string content, string callerMemberName, string filePath, int lineNumber, Exception ex = null)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(DateTime.Now.ToLocalTime().ToString("HH:mm:ss:fff"));
            builder.Append(" | ");
            builder.Append(level);
            builder.Append(" | ");
            builder.Append($"{Path.GetFileName(filePath)} | {callerMemberName} | {lineNumber}");

            if (string.IsNullOrEmpty(content) == false)
            {
                builder.Append(" | ");
                builder.Append(content);
            }

            if (ex != null)
            {
                builder.AppendLine("=== Exception ===");
                builder.AppendLine($"HResult : {ex.HResult}");
                builder.AppendLine($"Source : {ex.Source}");
                builder.AppendLine($"Message : {ex.Message}");
                builder.AppendLine("=== Exception ===");
            }

            return builder.ToString();
        }
    }
}
