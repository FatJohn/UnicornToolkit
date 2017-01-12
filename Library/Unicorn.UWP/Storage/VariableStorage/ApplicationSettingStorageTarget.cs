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
using Windows.Storage;

namespace Unicorn
{
    public class ApplicationSettingStorageTarget : IVariableStorageTarget
    {
        private readonly ApplicationDataContainer applicationDataContainer;

        private JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
        };

        public ApplicationSettingStorageTarget(ApplicationDataContainer applicationDataContainer)
        {
            this.applicationDataContainer = applicationDataContainer;
        }

        public T Get<T>()
        {
            var key = typeof(T).FullName;
            return Get<T>(key);
        }

        public T Get<T>(string key)
        {
            return Get(key, default(T));
        }

        public T Get<T>(string key, T defaultValue)
        {
            string jsonString = null;
            bool isSuccess = applicationDataContainer.Values.TryGetValue(key, out jsonString);

            if (isSuccess == false)
            {
                return defaultValue;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString, jsonSerializerSetting);
            }
            catch (Exception)
            {
                // got exceptions
                // 改用預設值
                return defaultValue;
            }
        }

        public bool Remove(string key)
        {
            return applicationDataContainer.Values.Remove(key);
        }

        public bool Remove<T>()
        {
            var key = typeof(T).FullName;
            return Remove(key);
        }

        public void RemoveAll()
        {
            applicationDataContainer.Values.Clear();
        }

        public void Set(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Set(value.GetType().FullName, value);
        }

        public void Set(string key, object value)
        {
            applicationDataContainer.Values[key] = JsonConvert.SerializeObject(value, jsonSerializerSetting);
        }

        public bool ContainsKey(string key)
        {
            return applicationDataContainer.Values.ContainsKey(key);
        }
    }
}
