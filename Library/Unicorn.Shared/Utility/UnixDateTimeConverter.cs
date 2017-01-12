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

namespace Unicorn
{
    public static class UnixDateTimeConverter
    {
        /// <summary>
        /// UNIX TimeStamp 是採用 EPOC, EPOC 起使時間是 1970/1/1 開始
        /// </summary>
        /// <remarks>621355968000000000 ticks</remarks>
        private static readonly DateTime epocStartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime EpocStartDateTime
        {
            get { return epocStartDateTime; }
        }

        public static DateTime SecondsToDateTime(double timestamp)
        {
            if (timestamp <= 0)
            {
                return DateTime.MinValue;
            }

            return epocStartDateTime.AddSeconds(timestamp);
        }

        public static DateTime MilliSecondsToDateTime(double timestamp)
        {
            if (timestamp <= 0)
            {
                return DateTime.MinValue;
            }

            return epocStartDateTime.AddMilliseconds(timestamp);
        }

        public static double ToSeconds(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return 0;
            }

            var timeSpan = dateTime.ToUniversalTime() - epocStartDateTime;

            if (timeSpan.TotalSeconds < 0)
            {
                return 0;
            }

            return Math.Floor(timeSpan.TotalSeconds);
        }

        public static double ToMilliSeconds(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return 0;
            }

            var timeSpan = dateTime.ToUniversalTime() - epocStartDateTime;

            if (timeSpan.TotalMilliseconds < 0)
            {
                return 0;
            }

            return Math.Floor(timeSpan.TotalMilliseconds);
        }

        public static int ToEpoch(long time)
        {
            if (time <= 0)
            {
                return 0;
            }

            return decimal.ToInt32(decimal.Divide(time - epocStartDateTime.Ticks, 10000000));
        }
    }
}
