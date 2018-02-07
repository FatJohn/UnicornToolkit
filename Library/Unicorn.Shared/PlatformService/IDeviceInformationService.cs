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
    public interface IDeviceInformationService
    {
        /// <summary>
        /// 裝置唯一識別
        /// </summary>
        string UniqueId { get; }

        /// <summary>
        /// 作業系統
        /// </summary>
        string OperatingSystem { get; }

        /// <summary>
        /// 作業系統版本
        /// </summary>
        string OperationSystemVersion { get; }

        /// <summary>
        /// 作業系統組建
        /// </summary>
        ulong OperationSystemBuild { get; }

        /// <summary>
        /// 系統製造商
        /// </summary>
        string SystemManufacturer { get; }

        /// <summary>
        /// 裝置名稱
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 裝置模組名稱
        /// </summary>
        string ModelName { get; }

        /// <summary>
        /// OS 類型
        /// </summary>
        string DeviceFamily { get; }

        /// <summary>
        /// 是否為 Mobile
        /// </summary>
        bool IsMobile { get; }

        /// <summary>
        /// 是否為桌機
        /// </summary>
        bool IsDesktop { get; }

        /// <summary>
        /// 是否為 XBOX
        /// </summary>
        bool IsXBOX { get; }

        /// <summary>
        /// 記憶體限制 (MB)
        /// </summary>
        ulong MemoryLimit { get; }

        /// <summary>
        /// 記憶體使用中用量 (MB)
        /// </summary>
        ulong MemoryUsage { get; }

        /// <summary>
        /// 記憶體使用中比例 (%)
        /// </summary>
        ulong MemoryUsageRatio { get; }
    }
}