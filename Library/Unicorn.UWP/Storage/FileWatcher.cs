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
using System.Threading.Tasks;
using Windows.Storage.Search;

namespace Unicorn
{
    public class FileWatcher
    {
        public event Func<StorageFileQueryResult, Task> ContentsChanged;
        private async Task OnContentsChanged(StorageFileQueryResult e)
        {
            var task = ContentsChanged?.Invoke(e);
            await task.GetSafeAwaiter();
        }

        private readonly AsyncLock asyncLock;

        private string watchedFolderPath;
        private StorageFileQueryResult queryResult;

        public FileWatcher()
        {
            asyncLock = new AsyncLock();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileTypeFilter">Please follow the rule of QueryOption</param>
        /// <returns></returns>
        public async Task StartWatch(string folderPath, params string[] fileTypeFilter)
        {
            using (var releaser = await asyncLock.LockAsync())
            {
                // stop previous whatcher first
                StopWatch();

                if (folderPath == null)
                {
                    return;
                }

                if (watchedFolderPath == folderPath)
                {
                    return;
                }

                var folder = await StorageHelper.GetFolder(folderPath);
                if (folder == null)
                {
                    return;
                }

                watchedFolderPath = folderPath;

                var options = new QueryOptions(CommonFileQuery.OrderByDate, fileTypeFilter);
                queryResult = folder.CreateFileQueryWithOptions(options);
                queryResult.ContentsChanged += QueryResult_ContentsChanged;

                // this line is necessary to make this query affect and to receive ContentsChanged event
                var files = await queryResult.GetFilesAsync();
            }
        }

        public void StopWatch()
        {
            watchedFolderPath = null;

            if (queryResult == null)
            {
                return;
            }

            queryResult.ContentsChanged -= QueryResult_ContentsChanged;
            queryResult = null;
        }

        private async void QueryResult_ContentsChanged(IStorageQueryResultBase sender, object args)
        {
            // args is always null. and there's no way to tell if which file is added or deleted
            // "sender" should be the same as "queryResult"
            await OnContentsChanged(queryResult).ConfigureAwait(false);
        }
    }
}
