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

namespace Unicorn.ServiceModel
{
    public class EnumKeyJsonKeywordConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetTypeInfo().IsEnum;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumValues = Enum.GetValues(objectType);
            foreach (var item in enumValues)
            {
                var enumKeys = ((Enum)item).GetKeys();
                string target = (string)reader.Value;
                foreach (var key in enumKeys)
                {
                    if (target.IndexOf(key) >= 0)
                    {
                        return item;
                    }
                }
            }

            return (Enum)enumValues.GetValue(0);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumString = ((Enum)value).GetKey();
            writer.WriteValue(enumString);
        }
    }
}
