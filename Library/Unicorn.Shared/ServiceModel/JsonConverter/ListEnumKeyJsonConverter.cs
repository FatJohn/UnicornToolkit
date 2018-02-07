// Copyright (c) 2017 Louis Wu

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
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Unicorn.ServiceModel
{
    public class ListEnumKeyJsonConverter<TEnum> : JsonConverter
        where TEnum : struct, IConvertible
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(TEnum).GetTypeInfo().IsEnum && objectType == typeof(List<TEnum>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new List<TEnum>();

            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray jArray = JArray.Load(reader);
                var enumValues = Enum.GetValues(typeof(TEnum));

                foreach (var jToken in jArray)
                {
                    var jValue = (jToken as JValue)?.Value as string;
                    if (jValue == null)
                    {
                        continue;
                    }

                    foreach (var item in enumValues)
                    {
                        var enumKey = ((Enum)item).GetKey();
                        if (enumKey == jValue)
                        {
                            result.Add((TEnum)item);
                        }
                    }
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var list = value as List<TEnum>;
            if (list == null)
            {
                return;
            }

            writer.WriteStartArray();

            foreach (var item in list)
            {
                var enumString = ((Enum)item.ToType(typeof(Enum), null)).GetKey();
                writer.WriteValue(enumString);
            }

            writer.WriteEndArray();
        }
    }
}
