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
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace Unicorn
{
    public static class NetworkHelper
    {
        public static event EventHandler<bool> NetworkAvailabilityChange;
        private static void OnNetworkAvailabilityChange(bool value)
        {
            SafeRaise.Raise(NetworkAvailabilityChange, null, value);
        }

        private static bool isNetworkAvailable;
        public static bool IsNetworkAvailable
        {
            get
            {
                return isNetworkAvailable;
            }
            private set
            {
                if (isNetworkAvailable == value)
                {
                    return;
                }

                isNetworkAvailable = value;
                OnNetworkAvailabilityChange(isNetworkAvailable);
            }
        }

        static NetworkHelper()
        {
            try
            {
                NetworkInformation.NetworkStatusChanged += NetworkInformationOnNetworkStatusChanged;
                IsNetworkAvailable = CheckInternetAvailable();
                PlatformService.Log?.Trace("Initial Network Status : " + IsNetworkAvailable);
            }
            catch (Exception ex)
            {
                PlatformService.Log?.Error(ex);
            }
        }

        private static void NetworkInformationOnNetworkStatusChanged(object sender)
        {
            IsNetworkAvailable = CheckInternetAvailable();

            PlatformService.Log?.Trace("Network Status changed : " + IsNetworkAvailable);
        }

        /// <summary>
        /// 檢查網路連線是否可用
        /// </summary>
        /// <returns></returns>
        public static bool CheckInternetAvailable()
        {
            bool available = NetworkInterface.GetIsNetworkAvailable();
            return available;
        }

        /// <summary>
        /// 取得目前正在連線的名稱
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetCurrentConnectionNames()
        {
            var internetConnectionProfiles = NetworkInformation.GetConnectionProfiles();

            if (internetConnectionProfiles != null)
            {
                foreach (var connectionProfile in internetConnectionProfiles)
                {
                    if (connectionProfile.IsWwanConnectionProfile)
                    {
                        try
                        {
                            var networkNames = connectionProfile.GetNetworkNames();
                            return networkNames.ToList();
                        }
                        catch (Exception)
                        {
                            return new List<string>();
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 檢查 targetName 是否正在連線
        /// </summary>
        /// <param name="targetName">目標名稱</param>
        /// <returns></returns>
        public static bool CheckConnectedName(string targetName)
        {
            var connectionNames = GetCurrentConnectionNames();
            if (connectionNames != null && connectionNames.Count > 0)
            {
                foreach (var name in connectionNames)
                {
                    if (string.Compare(name, targetName, true) == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 檢查目前的連線中的連線有非 3G/4G 的網路的
        /// 理論上 windows 會使用非 WWan 連線優先當上網的
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckIsNoneWwanConnected()
        {
            // 找到連線了，但不是 WWAN 的 profile
            ConnectionProfileFilter filter = new ConnectionProfileFilter
            {
                IsConnected = true,
                IsWwanConnectionProfile = false,
            };
            var result = await NetworkInformation.FindConnectionProfilesAsync(filter);
            return result.Count > 0;
        }

        /// <summary>
        /// 取得目前設備使用的 Mobiler operator。(operator, id, data class).
        /// ref: https://en.wikipedia.org/wiki/Mobile_country_code
        /// </summary>
        /// <returns></returns>
        public static Tuple<MobileOperator, string, WwanDataClass> GetMobileOperator()
        {
            MobileOperator mobileOperator = default(MobileOperator);
            WwanDataClass wwanDataClass = default(WwanDataClass);
            string providerId = string.Empty;

            var connectionProfiles = NetworkInformation.GetConnectionProfiles().Where(x => x.IsWwanConnectionProfile);
            if (connectionProfiles != null)
            {
                foreach (var profile in connectionProfiles)
                {
                    providerId = profile.WwanConnectionProfileDetails.HomeProviderId;
                    switch (providerId)
                    {
                        case "45500":
                        case "45506":
                        case "45406":
                        case "45415":
                        case "45417":
                            mobileOperator = MobileOperator.SmarTone;
                            break;
                        case "46692":
                        case "466092":
                            mobileOperator = MobileOperator.Chunghwa;
                            break;
                        case "52001":
                            mobileOperator = MobileOperator.AIS;
                            break;
                        case "52018":
                            mobileOperator = MobileOperator.DTAC;
                            break;
                        default:
                            break;
                    }
                    wwanDataClass = profile.WwanConnectionProfileDetails.GetCurrentDataClass();
                }
            }
            return new Tuple<MobileOperator, string, WwanDataClass>(mobileOperator, providerId, wwanDataClass);
        }

        /// <summary>
        /// Get network type: wifi, 3G, 4G.
        /// </summary>
        public static async Task<string> GetNetworkType()
        {
            // 非 3G/4G 連線一律回傳 wifi
            if (await CheckIsNoneWwanConnected())
            {
                return "wifi";
            }

            var mobile = GetMobileOperator();
            var type = mobile.Item3;

            if (type == WwanDataClass.LteAdvanced)
            {
                return "4G";
            }

            return "3G";
        }
    }

    public enum MobileOperator
    {
        Unknow,
        SmarTone,
        Chunghwa,
        AIS,
        DTAC
    }
}