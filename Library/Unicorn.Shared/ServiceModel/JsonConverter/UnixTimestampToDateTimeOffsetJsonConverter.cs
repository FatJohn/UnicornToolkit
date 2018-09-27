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
using Newtonsoft.Json;

namespace Unicorn.ServiceModel
{
    public class UnixTimestampToDateTimeOffsetJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int) || objectType == typeof(long) || objectType == typeof(float) || objectType == typeof(double);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return DateTimeOffset.MinValue;
            }

            var seconds = Convert.ToInt64(reader.Value);

            // 目前用的 dotnet 版本為 4.5.2 不支援 FromUnixTimeSeconds，若日後升級到 4.6 以上就可以把此 #if 拔掉
#if WINDOWS_UWP
            return DateTimeOffset.FromUnixTimeSeconds(seconds).ToLocalTime();
#else
            return new DateTimeOffset(UnixDateTimeConverter.SecondsToDateTime(seconds)).ToLocalTime();
#endif
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType() != typeof(DateTimeOffset))
            {
                writer.WriteValue(0);
                return;
            }

            var dateTime = (DateTimeOffset)value;
            
            // 目前用的 dotnet 版本為 4.5.2 不支援 ToUnixTimeSeconds，若日後升級到 4.6 以上就可以把此 #if 拔掉
#if WINDOWS_UWP
            writer.WriteValue(dateTime.ToUnixTimeSeconds());
#else
            writer.WriteValue(UnixDateTimeConverter.ToSeconds(dateTime.DateTime));
#endif
        }
    }
}
