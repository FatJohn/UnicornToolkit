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
using Windows.Foundation.Metadata;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System;
using Windows.System.Profile;

namespace Unicorn
{
    public class DeviceInformationService : IDeviceInformationService
    {
        private string deviceUniqeId;
        public string UniqueId
        {
            get
            {
                if (deviceUniqeId != null)
                {
                    return deviceUniqeId;
                }

                try
                {
                    if (ApiInformation.IsTypePresent("Windows.System.Profile.HardwareIdentification"))
                    {
                        var packageSpecificToken = HardwareIdentification.GetPackageSpecificToken(null);
                        var hardwareId = packageSpecificToken.Id;

                        var hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
                        var hashedHardwareId = hasher.HashData(hardwareId);

                        deviceUniqeId = CryptographicBuffer.EncodeToHexString(hashedHardwareId);
                        return deviceUniqeId;
                    }
                }
                catch (Exception)
                {
                    // XBOX 目前會取失敗
                }

                // support IoT Device
                var networkProfiles = Windows.Networking.Connectivity.NetworkInformation.GetConnectionProfiles();
                var adapter = networkProfiles[0].NetworkAdapter;
                string networkAdapterId = adapter.NetworkAdapterId.ToString();
                deviceUniqeId = networkAdapterId.Replace("-", string.Empty);

                return deviceUniqeId;
            }
        }

        public string OperatingSystem
        {
            get;
            private set;
        }

        private static string operatingSystemVersion;
        public string OperationSystemVersion
        {
            get
            {
                if (!string.IsNullOrEmpty(operatingSystemVersion))
                {
                    return operatingSystemVersion;
                }

                var analyticsInfoUtility = new AnalyticsInfoUtility();
                var deviceFamilyVersion = analyticsInfoUtility.GetDeviceFamilyVersion();

                operatingSystemVersion = $"{deviceFamilyVersion.Major}.{deviceFamilyVersion.Minor}.{deviceFamilyVersion.Build}";

                return operatingSystemVersion;
            }
        }

        private ulong operationSystemBuild = 0;
        public ulong OperationSystemBuild
        {
            get
            {
                if (operationSystemBuild > 0)
                {
                    return operationSystemBuild;
                }

                var analyticsInfoUtility = new AnalyticsInfoUtility();
                operationSystemBuild = analyticsInfoUtility.GetDeviceFamilyVersion().Build;
                return operationSystemBuild;
            }
        }

        public string SystemManufacturer
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string ModelName
        {
            get;
            private set;
        }

        public string DeviceFamily
        {
            get { return AnalyticsInfo.VersionInfo.DeviceFamily; }
        }

        public bool IsMobile
        {
            get { return DeviceFamily?.IndexOf("mobile", StringComparison.OrdinalIgnoreCase) >= 0; }
        }

        public bool IsDesktop
        {
            get { return DeviceFamily?.IndexOf("desktop", StringComparison.OrdinalIgnoreCase) >= 0; }
        }

        public bool IsXBOX
        {
            get { return DeviceFamily?.IndexOf("xbox", StringComparison.OrdinalIgnoreCase) >= 0; }
        }

        public ulong MemoryLimit
        {
            get
            {
                try
                {
                    var limit = MemoryManager.AppMemoryUsageLimit / 1024 / 1024;
                    return limit;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public ulong MemoryUsage
        {
            get
            {
                try
                {
                    var usage = MemoryManager.AppMemoryUsage / 1024 / 1024;
                    return usage;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public ulong MemoryUsageRatio
        {
            get
            {
                if (MemoryLimit == 0)
                {
                    return 0;
                }

                return MemoryUsage / MemoryLimit;
            }
        }

        public DeviceInformationService()
        {
            var deviceInformation = new EasClientDeviceInformation();
            OperatingSystem = deviceInformation.OperatingSystem;
            SystemManufacturer = deviceInformation.SystemManufacturer;
            Name = deviceInformation.FriendlyName;
            ModelName = deviceInformation.SystemProductName;
        }
    }
}