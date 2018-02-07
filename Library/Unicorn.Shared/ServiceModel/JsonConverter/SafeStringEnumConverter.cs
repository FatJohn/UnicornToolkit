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
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Unicorn.ServiceModel
{
    public class SafeStringEnumConverter : JsonConverter
    {
        private readonly StringEnumConverter internalConverter;

        public SafeStringEnumConverter()
        {
            internalConverter = new StringEnumConverter()
            {
                AllowIntegerValues = true,
            };
        }

        public override bool CanConvert(Type objectType)
        {
            return internalConverter.CanConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return internalConverter.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (JsonSerializationException)
            {
                var typeInfo = objectType.GetTypeInfo();
                bool isNullable = typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
                if (isNullable)
                {
                    return null;
                }

                return Enum.GetValues(objectType).GetValue(0);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                internalConverter.WriteJson(writer, value, serializer);
            }
            catch (JsonSerializationException)
            {
                // 看原始碼是在 WriteJson 會跳 JsonSerializationException 的時機是
                // 來源是 "數字" 但是 AllowIntegerValues 是 false 的時候，所以就直接把自己的值寫入
                writer.WriteValue(value.ToString());
            }
        }
    }
}
