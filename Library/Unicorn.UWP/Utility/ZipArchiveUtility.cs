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

using System.IO.Compression;
using System.Threading.Tasks;

namespace Unicorn
{
    public static class ZipArchiveUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationFolder"></param>
        /// <returns></returns>
        public static async Task ExtractAsync(string sourceFilePath, string destinationFolder)
        {
            var file = PlatformService.File;
            var zipFileStream = await file.OpenReadStreamAsync(sourceFilePath);
            var zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read);

            foreach (var zipEntry in zipArchive.Entries)
            {
                var zipEntryFileStream = zipEntry.Open();
                var fileName = string.Format(@"{0}\{1}", destinationFolder, zipEntry.FullName);
                var decompressStream = await file.OpenWriteStreamAsync(fileName);
                await zipEntryFileStream.CopyToAsync(decompressStream);
                await decompressStream.FlushAsync();

                decompressStream.Dispose();
                zipEntryFileStream.Dispose();
            }

            zipFileStream.Dispose();
        }
    }
}
