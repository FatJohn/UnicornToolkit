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
                CheckInternetAccess();
            }
            catch (Exception ex)
            {
                PlatformService.Log?.Error(ex);
            }
        }

        private static void CheckInternetAccess()
        {
            try
            {
                var connectionProfile = NetworkInformation.GetInternetConnectionProfile();

                IsNetworkAvailable = (connectionProfile != null &&
                                 connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);

                PlatformService.Log?.Trace("has network changed: " + IsNetworkAvailable);
            }
            catch (Exception ex)
            {
                PlatformService.Log?.Error(ex);
            }
        }

        private static void NetworkInformationOnNetworkStatusChanged(object sender)
        {
            CheckInternetAccess();
        }

        /// <summary>
        /// 檢查網路連線是否可用
        /// </summary>
        /// <returns></returns>
        public static bool CheckInternetAvailable()
        {
            // 在 Store 執行時，手動將網路卡停用時，在 GetNetworkConnectivityLevel() 會取得 LocalAccess
            // 在 Phone 執行時，開啟飛航模式時，在 NetworkInformation.GetInternetConnectionProfile() 就會取得 null
            // 在 Phone 上即便是連上一個無法對外的無線網路，在 GetNetworkConnectivityLevel() 仍然會取得 NetworkConnectivityLevel.InternetAccess
            // 但此狀況不在正常使用範圍
            // PS: 要測試 Phone 的狀態記得用實機，模擬器的狀態不準（即便你開啟飛航模式或用模擬器的網路模擬讓網路停止）
            bool result = false;
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            if (connectionProfile != null)
            {
                switch (connectionProfile.GetNetworkConnectivityLevel())
                {
                    case NetworkConnectivityLevel.None:
                    case NetworkConnectivityLevel.LocalAccess:
                        break;
                    case NetworkConnectivityLevel.ConstrainedInternetAccess:
                        result = true;
                        break;
                    case NetworkConnectivityLevel.InternetAccess:
                        result = true;
                        break;
                }
            }

            return result;
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
                    if (name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 正在用 3G/4G 上網
        /// </summary>
        public static bool GetCurrentConnectionIsWwan()
        {
            return NetworkInformation.GetInternetConnectionProfile().IsWwanConnectionProfile;
        }

        /// <summary>
        /// 正在用 wifi 連線
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsWiFiEnabled()
        {
            //Get the Internet connection profile
            string ssid = string.Empty;
            ConnectionProfileFilter filter = new ConnectionProfileFilter();
            filter.IsConnected = true;
            filter.IsWlanConnectionProfile = true;
            var result = await NetworkInformation.FindConnectionProfilesAsync(filter);
            if (result.Count > 0)
            {
                foreach (var profile in result)
                {
                    if (profile.IsWlanConnectionProfile)
                    {
                        ssid += profile.WlanConnectionProfileDetails.GetConnectedSsid();
                    }
                }
            }
            if (string.IsNullOrEmpty(ssid))
            {
                return false;
            }
            else
            {
                return true;
            }
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
            if (await IsWiFiEnabled())
            {
                return "wifi";
            }
            else
            {
                var mobile = GetMobileOperator();
                var type = mobile.Item3;
                mobile = null;
                if (type == WwanDataClass.Hsdpa)
                {
                    return "3G";
                }
                else
                {
                    return "4G";
                }
            }
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

    public enum WwanType
    {
        Unknow,
    }
}