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
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace Unicorn
{
    public class ResourceManager
    {
        private static Lazy<ResourceLoader> resouceLoader = new Lazy<ResourceLoader>(() => new ResourceLoader());
        public static ResourceLoader Default
        {
            get { return resouceLoader.Value; }
        }

        public static string GetString(string id)
        {
            try
            {
                return Default?.GetString(id) ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static T GetApplicationResource<T>(string id)
        {
            return (T)Application.Current.Resources[id];
        }

        public string this[string key]
        {
            get
            {
                return GetString(key);
            }
        }
    }
}
