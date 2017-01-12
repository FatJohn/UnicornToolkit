// Copyright (c) 2016 Pou Lin

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
using Windows.ApplicationModel;
using Windows.System;

namespace Unicorn
{
    public class MarketHelper
    {
        public static string FamilyName
        {
            get { return Package.Current.Id.FamilyName; }
        }

        public const string SHOW_APP_URI_TEMPLATE = "ms-windows-store:PDP?PFN={0}";
        public const string REVIEW_APP_URI_TEMPLATE = "ms-windows-store:REVIEW?PFN={0}";

        /// <summary>
        /// 開啟市集，進入 KKBOX 介紹頁
        /// </summary>
        /// <returns></returns>
        public static async Task ShowMyAppPage()
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri(string.Format(SHOW_APP_URI_TEMPLATE, FamilyName)));
            }
            catch (Exception)
            {
                // 不因開啟市集失敗而 crash，避免體驗不佳
            }
        }

        /// <summary>
        /// 開啟市集，進到 KKBOX 評分頁
        /// </summary>
        /// <returns></returns>
        public static async Task ShowReviewPage()
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri(string.Format(REVIEW_APP_URI_TEMPLATE, FamilyName)));
            }
            catch (Exception)
            {
                // 不因開啟市集失敗而 crash，避免體驗不佳
            }
        }
    }
}