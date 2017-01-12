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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Search;
using Windows.Storage.Streams;

namespace Unicorn
{
    public enum ApplicationDataLocation
    {
        Local,
        Roaming,
        Temp,
        Install,
        External,
    }

    public static class StorageHelper
    {
        #region Event, Property
        /// <summary>
        /// 目前處理檔案的進度。
        /// </summary>
        public static event EventHandler<double> ProgressChanged;

        public static bool IsOpeningPicker { get; set; } = false;
        #endregion

        public static StorageFolder GetApplicationDataFolder(ApplicationDataLocation location)
        {
            StorageFolder folder = null;
            switch (location)
            {
                case ApplicationDataLocation.Local:
                    folder = ApplicationData.Current.LocalFolder;
                    break;
                case ApplicationDataLocation.Roaming:
                    folder = ApplicationData.Current.RoamingFolder;
                    break;
                case ApplicationDataLocation.Temp:
                    folder = ApplicationData.Current.TemporaryFolder;
                    break;
                case ApplicationDataLocation.Install:
                    folder = Package.Current.InstalledLocation;
                    break;
            }

            return folder;
        }

        public static Task<bool> CopyFileFromInstalledLocation(string filePath, bool overwrite = true, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            return CopyFileFromInstalledLocation(filePath, filePath, overwrite, location);
        }

        /// <summary>
        /// 將檔案從安裝目錄複製到ApplicationData的Local目錄下
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public static async Task<bool> CopyFileFromInstalledLocation(string sourcePath, string destinationPath, bool overwrite = true, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            try
            {
                var sourceStorage = Package.Current.InstalledLocation;
                var sourceFile = await sourceStorage.CreateFileAsync(sourcePath, CreationCollisionOption.OpenIfExists);

                StorageFolder destinationStorage = GetApplicationDataFolder(location);
                destinationStorage = await destinationStorage.CreateFolderLazy(destinationPath);
                var fileName = Path.GetFileName(destinationPath);

                StorageFile destinationFile = null;
                if (!overwrite)
                {
                    destinationFile = await sourceFile.CopyAsync(destinationStorage, fileName, NameCollisionOption.FailIfExists);
                }
                else
                {
                    destinationFile = await sourceFile.CopyAsync(destinationStorage, fileName, NameCollisionOption.ReplaceExisting);
                }

                return (destinationFile != null);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 負治安裝目錄的資料夾到目的地
        /// </summary>
        /// <param name="path"></param>
        /// <param name="location"></param>
        /// <param name="overwrite"></param>
        /// <param name="copyCallback"></param>
        /// <returns></returns>
        public static async Task CopyFolderFromInstalledLocation(string path, bool overwrite = true, ApplicationDataLocation location = ApplicationDataLocation.Local, Action<IStorageItem> copyCallback = null)
        {
            IStorageFolder destination = GetApplicationDataFolder(location);
            IStorageFolder root = Package.Current.InstalledLocation;

            if (!await ExistsFolder(path))
            {
                await destination.CreateFolderAsync(path);
            }

            destination = await destination.GetFolderAsync(path);
            root = await root.GetFolderAsync(path);

            IReadOnlyList<IStorageItem> items = await root.GetItemsAsync();
            foreach (IStorageItem item in items)
            {
                if (item.IsOfType(StorageItemTypes.File))
                {
                    IStorageFile presFile = await root.CreateFileAsync(item.Name, CreationCollisionOption.OpenIfExists);

                    copyCallback?.Invoke(presFile);

                    if (!overwrite)
                    {
                        await presFile.CopyAsync(destination, presFile.Name, NameCollisionOption.FailIfExists);
                    }
                    else
                    {
                        await presFile.CopyAsync(destination, presFile.Name, NameCollisionOption.ReplaceExisting);
                    }
                }
                else
                {
                    // If folder doesn't exist, than create new one on destination folder
                    if (!await ExistsFolder(path + "\\" + item.Name))
                    {
                        await destination.CreateFolderAsync(item.Name);
                    }

                    // Do recursive copy for every items inside
                    await CopyFolderFromInstalledLocation(path + "\\" + item.Name, overwrite, location, copyCallback);
                }
            }
        }

        /// <summary>
        /// 開啟ApplicationData Folder下的檔案
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<IInputStream> OpenSequentialRead(string filePath, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            try
            {
                StorageFolder folder = GetApplicationDataFolder(location);
                var file = await folder.CreateFileAsync(filePath, CreationCollisionOption.OpenIfExists);
                if (file == null)
                {
                    return null;
                }

                return await file.OpenSequentialReadAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 開啟讀取串流，可能會回傳 null，外面要加 Null check
        /// </summary>
        /// <param name="filePath">使用相對路徑</param>
        /// <param name="forceCreate">是否強制產生檔案</param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<Stream> OpenReadStream(string filePath, bool forceCreate, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            StorageFolder folder = GetApplicationDataFolder(location);

            try
            {
                StorageFile file = null;
                if (forceCreate)
                {
                    file = await folder.CreateFileAsync(filePath, CreationCollisionOption.OpenIfExists);
                }
                else
                {
                    file = await folder.GetFileAsync(filePath);
                }

                if (file == null)
                {
                    return null;
                }

                return await file.OpenStreamForReadAsync();
            }
#if DEBUG
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
#else
            catch (Exception)
            {
                return null;
            }
#endif
        }

        /// <summary>
        /// 開啟寫入串流，可能會回傳 null，外面要加 Null check
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="overwrite"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<Stream> OpenWriteStream(string filePath, bool overwrite = true, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            StorageFolder folder = GetApplicationDataFolder(location);
            folder = await folder.CreateFolderLazy(filePath);
            var fileName = Path.GetFileName(filePath);
            CreationCollisionOption createOption = (overwrite ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.OpenIfExists);

            try
            {
                var file = await folder.CreateFileAsync(fileName, createOption);
                if (file == null)
                {
                    return null;
                }

                return await file.OpenStreamForWriteAsync();
            }
#if DEBUG
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
#else
            catch (Exception)
            {
                return null;
            }
#endif
        }

        public static async Task<StorageFile> CreateFile(string filePath, bool overwrite = true, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            try
            {
                StorageFolder folder = null;

                if (filePath.IsAbsolutePath())
                {
                    string folderPath = Path.GetDirectoryName(filePath);
                    folder = await GetFolderFromPath(folderPath, null);
                }
                else
                {
                    folder = GetApplicationDataFolder(location);
                    folder = await folder.CreateFolderLazy(filePath);
                }

                if (folder == null)
                {
                    return null;
                }

                var fileName = Path.GetFileName(filePath);
                CreationCollisionOption createOption = (overwrite ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.OpenIfExists);

                return await folder.CreateFileAsync(fileName, createOption);
            }
#if DEBUG
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
#else
            catch (Exception)
            {
                return null;
            }
#endif
        }

        /// <summary>
        /// 建立完整的子路徑
        /// </summary>
        /// <param name="storage">要建立路徑的跟目錄, EX: ApplicationData.LocalFolder</param>
        /// <param name="filePath">完整檔案名稱</param>
        /// <returns>最後一個建立的路徑的Handle</returns>
        public static async Task<StorageFolder> CreateFolderLazy(this StorageFolder storage, string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(directoryName))
            {
                return storage;
            }

            const char directorySeparatorChar = '\\';
            string[] subDirectoies = directoryName.Split(new char[] { directorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            string path = string.Empty;
            StorageFolder parentFolder = storage;
            StorageFolder childFolder = null;

            for (int i = 0; i < subDirectoies.Length; i++)
            {
                path = subDirectoies[i];

                var storageItem = await parentFolder.TryGetItemAsync(path);
                childFolder = storageItem as StorageFolder;

                if (null == childFolder)
                {
                    parentFolder = await parentFolder.CreateFolderAsync(path, CreationCollisionOption.OpenIfExists);
                }
                else
                {
                    parentFolder = childFolder;
                }
            }

            return parentFolder;
        }

        public static async Task DeleteAllFiles(StorageFolder folder)
        {
            try
            {
                if (folder == null)
                {
                    return;
                }

                var files = await folder.GetFilesAsync();

                foreach (var item in files)
                {
                    await item.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
#if DEBUG
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
#else
            catch (Exception)
            {
            }
#endif
        }

        public static async Task DeleteFolder(string folderName, bool needKeepFodler = false, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            var appDataFolder = GetApplicationDataFolder(location);

            try
            {
                var folder = await appDataFolder.TryGetItemAsync(folderName) as StorageFolder;
                if (folder == null)
                {
                    return;
                }

                await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                if (needKeepFodler)
                {
                    await CreateFolderLazy(appDataFolder, folderName);
                }
            }
#if DEBUG
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
#else
            catch (Exception)
            {
            }
#endif
        }

        /// <summary>
        /// Delete a file asynchronously
        /// </summary>
        /// <param name="fileName">相對路徑之檔案名稱</param>
        /// <param name="location">AppData的位置</param>
        /// <returns></returns>
        public static async Task<bool> DeleteFile(string fileName, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            var file = await GetFile(fileName, location);
            if (file == null)
            {
                return false;
            }

            try
            {
                await file.DeleteAsync();
                return true;
            }
#if DEBUG
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
#else
            catch (Exception)
            {
                return false;
            }
#endif
        }

        /// <summary>
        /// 取得檔案在ApplicationData Folder下的完整路徑
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static async Task<string> GetFilePath(string filePath, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            try
            {
                var storage = GetApplicationDataFolder(location);
                var file = await storage.CreateFileAsync(filePath, CreationCollisionOption.OpenIfExists);
                return file.Path;
            }
#if DEBUG
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
#else
            catch (Exception)
            {
                return null;
            }
#endif
        }

        /// <summary>
        /// 檢查目錄是否存在
        /// </summary>
        /// <param name="foldername"></param>
        /// <returns></returns>
        public static async Task<bool> ExistsFolder(string foldername, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            var folder = await GetFolder(foldername, location);
            return (folder != null);
        }

        /// <summary>
        /// 檢查檔案是否存在
        /// </summary>
        /// <param name="fileName">相對路徑之檔案名稱</param>
        /// <param name="location">AppData的位置</param>
        /// <returns></returns>
        public static async Task<bool> ExistsFile(string fileName, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            var file = await GetFile(fileName, location);
            return (file != null);
        }

        /// <summary>
        /// At the moment the only way to check if a file exists to catch an exception... :/
        /// </summary>
        /// <param name="fileName">相對路徑之檔案名稱</param>
        /// <param name="location">AppData的位置</param>
        /// <returns></returns>
        public static async Task<StorageFile> GetFile(string fileName, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            try
            {
                StorageFolder storageFolder = null;

                if (fileName.IsAbsolutePath())
                {
                    string folderPath = Path.GetDirectoryName(fileName);
                    fileName = Path.GetFileName(fileName);
                    storageFolder = await GetFolderFromPath(folderPath, null);
                }
                else
                {
                    storageFolder = GetApplicationDataFolder(location);
                }

                if (storageFolder == null)
                {
                    return null;
                }

                var item = await storageFolder.TryGetItemAsync(fileName);
                if (item != null && item.IsOfType(StorageItemTypes.File))
                {
                    return item as StorageFile;
                }
                else
                {
                    return null;
                }
            }
#if DEBUG
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
#else
            catch (Exception)
            {
                return null;
            }
#endif
        }

        public static async Task<IReadOnlyList<StorageFile>> GetFilesInFolder(List<string> fileTypeFilter, StorageFolder folder)
        {
            if (folder == null || fileTypeFilter == null || fileTypeFilter.Count == 0)
            {
                return null;
            }

            try
            {
                QueryOptions queryOptions = new QueryOptions();
                foreach (var item in fileTypeFilter)
                {
                    if (item.IndexOf(".") == 0)
                    {
                        queryOptions.FileTypeFilter.Add(item);
                    }
                    else
                    {
                        continue;
                    }
                }
                StorageFileQueryResult results = folder.CreateFileQueryWithOptions(queryOptions);
                return await results.GetFilesAsync();
            }
#if DEBUG
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
#else
            catch (Exception)
            {
                return null;
            }
#endif
        }

        public static async Task<StorageFile> GetFilesInFolder(string fileName, StorageFolder root)
        {
            if (root == null)
            {
                return null;
            }

            StorageFile file = null;
            StorageFolder folder = root;

            var items = await folder.GetItemsAsync();

            foreach (var item in items)
            {
                if (item.IsOfType(StorageItemTypes.File))
                {
                    if (item.Name == fileName)
                    {
                        file = item as StorageFile;
                        break;
                    }
                }
                else
                {
                    file = await GetFilesInFolder(fileName, item as StorageFolder);
                }
            }

            return file;
        }

        public static async Task<StorageFolder> GetFolder(string folderName, ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            try
            {
                IStorageItem item = null;

                if (folderName.IsAbsolutePath())
                {
                    item = await GetFolderFromPath(folderName, null);
                }
                else
                {
                    var storageFolder = GetApplicationDataFolder(location);

                    // make sure Empty FolderName will get root folder not null
                    if (string.IsNullOrEmpty(folderName))
                    {
                        item = storageFolder;
                    }
                    else
                    {
                        item = await storageFolder.TryGetItemAsync(folderName);
                    }
                }

                if (item != null && item.IsOfType(StorageItemTypes.Folder))
                {
                    return item as StorageFolder;
                }
                else
                {
                    return null;
                }
            }
#if DEBUG
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
#else
            catch (Exception)
            {
                return null;
            }
#endif
        }

        /// <summary>
        /// 利用絕對路徑轉換 StorageFolder。
        /// </summary>
        public static async Task<StorageFolder> GetFolderFromPath(string folderPath, string token)
        {
            StorageFolder folder = null;

            try
            {
                folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            }
#if DEBUG
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (ArgumentException)
            {
            }
#else
            catch (Exception)
            {
            }
#endif

            if (folder != null)
            {
                return folder;
            }

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var cacheItem = await GetFolderByPermissionToken(token);
            if (cacheItem?.Path == folderPath)
            {
                folder = cacheItem as StorageFolder;
            }

            return null;
        }

        /// <summary>
        /// 取得 FutureAccessList 中的 IStorageItem。
        /// </summary>
        /// <param name="token">token</param>
        /// <returns></returns>
        public static async Task<IStorageItem> GetFolderByPermissionToken(string token)
        {
            try
            {
                var storageItem = await StorageApplicationPermissions.FutureAccessList.GetItemAsync(token);
                return storageItem;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetFolderFromPath | FutureAccessList | {token} | {ex.Message} | {ex.StackTrace}");
                return null;
            }
        }

        public static async Task<bool> CopyFolder(StorageFolder sourceFolder, StorageFolder targetFolder)
        {
            if (sourceFolder == null || targetFolder == null)
            {
                return false;
            }

            try
            {
                var sourceItmes = await sourceFolder.GetItemsAsync();

                foreach (var sourceItem in sourceItmes)
                {
                    if (sourceItem.IsOfType(StorageItemTypes.File))
                    {
                        var file = sourceItem as StorageFile;
                        await file.CopyAsync(targetFolder, file.Name, NameCollisionOption.ReplaceExisting);
                    }
                    else if (sourceItem.IsOfType(StorageItemTypes.Folder))
                    {
                        var folder = sourceItem as StorageFolder;
                        var destinationSubFolder = await targetFolder.CreateFolderAsync(folder.Name, CreationCollisionOption.OpenIfExists);
                        await CopyFolder(folder, destinationSubFolder);
                    }
                }

                return true;
            }
#if DEBUG
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
#else
            catch (Exception)
            {
                return false;
            }
#endif
        }

        public static async Task<bool> WriteBufferAsync(StorageFile targetFile, IBuffer fileContent)
        {
            try
            {
                CachedFileManager.DeferUpdates(targetFile);
                await FileIO.WriteBufferAsync(targetFile, fileContent);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(targetFile);
                return status == FileUpdateStatus.Complete;
            }
#if DEBUG
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
#else
            catch (Exception)
            {
                return false;
            }
#endif
        }

        /// <summary>
        /// 移動 來源目錄 中 特定副檔名的檔案 到 目的目錄。
        /// </summary>
        /// <param name="source">來源目錄</param>
        /// <param name="dest">目的目錄</param>
        /// <param name="fileType">特定副檔名</param>
        /// <returns></returns>
        public static async Task<bool> MoveFiles(StorageFolder source, StorageFolder dest, string fileType)
        {
            var sourceFiles = await GetFilesInFolder(new List<string> { fileType }, source);
            if (sourceFiles == null)
            {
                return false;
            }

            double i = 0;
            double count = sourceFiles.Count;

            foreach (var file in sourceFiles)
            {
                try
                {
                    await file.CopyAsync(dest, file.Name, NameCollisionOption.ReplaceExisting);
                }
#if DEBUG
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
#else
                catch (Exception ex)
                {
                }
#endif
                i++;
                ProgressChanged?.Invoke(null, i / count);
            }

            return true;
        }

        /// <summary>
        /// 取得指令目錄路徑的 可用空間 與 全部空間，單位： MB.
        /// </summary>
        public static async Task<Tuple<long, long>> GetFolderSpaceInfo(string path)
        {
            StorageFolder folder = await GetFolder(path);

            if (folder == null)
            {
                return Tuple.Create<long, long>(0, 0);
            }

            return await GetFolderSpaceInfo(folder);
        }

        /// <summary>
        /// 取得目錄的 可用空間 與 全部空間，單位： MB.
        /// </summary>
        public static async Task<Tuple<long, long>> GetFolderSpaceInfo(ApplicationDataLocation location = ApplicationDataLocation.Local)
        {
            try
            {
                StorageFolder storageFolder = GetApplicationDataFolder(location);

                if (location == ApplicationDataLocation.External)
                {
                    var folders = await storageFolder.GetFoldersAsync();
                    if (folders.Count < 0)
                    {
                        return null;
                    }
                    else
                    {
                        storageFolder = folders.FirstOrDefault();
                    }
                }

                return await GetFolderSpaceInfo(storageFolder);
            }
#if DEBUG
            catch (IOException)
            {
                return Tuple.Create<long, long>(0, 0);
            }
            catch (UnauthorizedAccessException)
            {
                return Tuple.Create<long, long>(0, 0);
            }
#else
            catch (Exception)
            {
                return Tuple.Create<long, long>(0, 0);
            }
#endif
        }

        /// <summary>
        /// 取得指令目錄路徑的 可用空間 與 全部空間，單位： MB.
        /// </summary>
        private static async Task<Tuple<long, long>> GetFolderSpaceInfo(StorageFolder storageFolder)
        {
            try
            {
                var retrivedProperties = await storageFolder.Properties.RetrievePropertiesAsync(new string[] { "System.FreeSpace", "System.Capacity" });
                long free = Convert.ToInt64(retrivedProperties["System.FreeSpace"]);
                free = free / 1048576;
                long capacity = Convert.ToInt64(retrivedProperties["System.Capacity"]);
                capacity = capacity / 1048576;
                return new Tuple<long, long>(free, capacity);
            }
#if DEBUG
            catch (IOException)
            {
                return new Tuple<long, long>(0, 0);
            }
            catch (UnauthorizedAccessException)
            {
                return new Tuple<long, long>(0, 0);
            }
#else
            catch (Exception)
            {
                return new Tuple<long, long>(0, 0);
            }
#endif
        }

        public static async Task<StorageFolder> GetFolderFromPicker()
        {
            try
            {
                IsOpeningPicker = true;

                // 最大上限 1000, 要自己手動管理
                RemoveOldestFolderOfAccessList();

                var folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                folderPicker.FileTypeFilter.Add("*");
                return await folderPicker.PickSingleFolderAsync();
            }
#if DEBUG
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
#else
            catch (Exception)
            {
                return null;
            }
#endif
            finally
            {
                IsOpeningPicker = false;
            }
        }

        /// <summary>
        /// 注冊目錄或檔案到可以被存取的清單。
        /// </summary>
        public static void RegistStorageItemToAccessList(IStorageItem storageItem, string token, string metadata = "")
        {
            if (string.IsNullOrEmpty(token) || storageItem == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(metadata))
            {
                metadata = DateTime.UtcNow.Ticks.ToString();
            }

            // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
            // 相同目錄拿到的 token 是一樣的
            StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, storageItem, metadata);
        }

        private static void RemoveOldestFolderOfAccessList(int removeCount = 2)
        {
            // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh972344(v=win.10)
            if (StorageApplicationPermissions.FutureAccessList.Entries.Count >= StorageApplicationPermissions.FutureAccessList.MaximumItemsAllowed)
            {
                var removeItems = StorageApplicationPermissions.FutureAccessList.Entries.Select(x =>
                {
                    long ticks = DateTime.UtcNow.Ticks;
                    long.TryParse(x.Metadata, out ticks);
                    return new Tuple<string, long>(x.Token, ticks);
                }).OrderBy(x => x).Take(removeCount);

                foreach (var item in removeItems)
                {
                    StorageApplicationPermissions.FutureAccessList.Remove(item.Item1);
                }
            }
        }

        public static bool IsAbsolutePath(this string path)
        {
            Uri result;
            return Uri.TryCreate(path, UriKind.Absolute, out result);
        }
    }
}