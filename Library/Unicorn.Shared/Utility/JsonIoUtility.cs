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

using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Unicorn
{
    public static class JsonIoUtility
    {
        public static async Task SerializeAsync(object obj, string filePath)
        {
            var stream = await PlatformService.File.OpenWriteStreamAsync(filePath);
            await SerializeAsync(obj, stream);
        }

        public static async Task SerializeAsync(object obj, Stream stream)
        {
            if (stream != null)
            {
                var jsonString = JsonConvert.SerializeObject(obj);
                var sw = new StreamWriter(stream);
                await sw.WriteAsync(jsonString);
                sw.Dispose();
            }
        }

        public static void Serialize(object obj, Stream stream)
        {
            if (stream != null)
            {
                var jsonString = JsonConvert.SerializeObject(obj);
                var sw = new StreamWriter(stream);
                sw.Write(jsonString);
                sw.Dispose();
            }
        }

        public static async Task<T> DeserializeAsync<T>(string filePath)
        {
            var stream = await PlatformService.File.OpenReadStreamAsync(filePath);
            return await DeserializeAsync<T>(stream);
        }

        public static async Task<T> DeserializeAsync<T>(Stream stream)
        {
            if (stream != null)
            {
                var sr = new StreamReader(stream);
                var jsonString = await sr.ReadToEndAsync();
                sr.Dispose();
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            else
            {
                return default(T);
            }
        }

        public static T Deserialize<T>(Stream stream)
        {
            var sr = new StreamReader(stream);
            var jsonString = sr.ReadToEnd();
            sr.Dispose();

            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static async Task<object> DeserializeAsync(string filePath)
        {
            var stream = await PlatformService.File.OpenReadStreamAsync(filePath);
            return await DeserializeAsync(stream);
        }

        public static async Task<object> DeserializeAsync(Stream stream)
        {
            if (stream != null)
            {
                var sr = new StreamReader(stream);
                var jsonString = await sr.ReadToEndAsync();
                sr.Dispose();

                return JsonConvert.DeserializeObject(jsonString);
            }
            else
            {
                return null;
            }
        }

        public static object Deserialize(Stream stream)
        {
            if (stream != null)
            {
                var sr = new StreamReader(stream);
                var jsonString = sr.ReadToEnd();
                sr.Dispose();

                return JsonConvert.DeserializeObject(jsonString);
            }
            else
            {
                return null;
            }
        }
    }
}