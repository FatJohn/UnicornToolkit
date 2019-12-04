using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Unicorn
{
    public static class StorageFolderExtension
    {
        public static async Task<IReadOnlyList<IStorageItem>> GetSafeItemsAsync(this IStorageFolder folder)
        {
            try
            {
                return await folder.GetItemsAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new List<IStorageItem>();
            }
        }
    }
}
