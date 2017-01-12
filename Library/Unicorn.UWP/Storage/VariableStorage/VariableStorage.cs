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

namespace Unicorn
{
    public class VariableStorage
    {
        private readonly IVariableStorageTarget storageTarget;

        public VariableStorage(IVariableStorageTarget storageTarget)
        {
            this.storageTarget = storageTarget;
        }

        public T Get<T>(string key, T defaultValue)
        {
            return storageTarget.Get(key, defaultValue);
        }

        public T Get<T>(string key)
        {
            return storageTarget.Get<T>(key);
        }

        public T Get<T>()
        {
            return storageTarget.Get<T>();
        }

        public void Set(object value)
        {
            storageTarget.Set(value);
        }

        public void Set(string key, object value)
        {
            storageTarget.Set(key, value);
        }

        public bool Remove(string key)
        {
            return storageTarget.Remove(key);
        }

        public bool Remove<T>()
        {
            return storageTarget.Remove<T>();
        }

        public void RemoveAll()
        {
            storageTarget.RemoveAll();
        }

        public bool ContainsKey(string key)
        {
            return storageTarget.ContainsKey(key);
        }
    }
}
