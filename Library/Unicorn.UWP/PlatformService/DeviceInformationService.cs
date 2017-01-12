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
        private readonly EasClientDeviceInformation deviceInformation = null;

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
            get { return deviceInformation.OperatingSystem; }
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

        public string SystemManufacturer
        {
            get { return deviceInformation.SystemManufacturer; }
        }

        public string Name
        {
            get { return deviceInformation.FriendlyName; }
        }

        public string ModelName
        {
            get { return deviceInformation.SystemProductName; }
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

        public DeviceInformationService()
        {
            deviceInformation = new EasClientDeviceInformation();
        }
    }
}