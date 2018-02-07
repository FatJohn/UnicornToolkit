using System;
using System.Collections.Generic;
using System.Text;

namespace Unicorn.ServiceModel
{
    public class HttpServiceParameterOption
    {
        public HttpServiceParameterUrlOption Url { get; private set; } = new HttpServiceParameterUrlOption();

        public HttpServiceParameterCacheOption Cache { get; private set; } = new HttpServiceParameterCacheOption();

        public HttpServcieParameterRetryOption Retry { get; private set; } = new HttpServcieParameterRetryOption();

        public void EnableCache(int minutes)
        {
            EnableCache(true, minutes);
        }

        public void EnableCache(bool isEanble, int minutes)
        {
            Cache.IsEnable = isEanble;
            Cache.Minutes = minutes;
        }
    }

    public class HttpServiceParameterUrlOption
    {
        /// <summary>
        /// 當 Service 發現此欄位有值時，就會使用此欄位的值當作發送的 URL
        /// </summary>
        public string CustomUrl { get; set; }

        /// <summary>
        /// true 則不會跑組合參數的流程, 預設是 false
        /// 當 CustomUrl 有值的時候才有作用
        /// </summary>
        public bool IsByPassAutoGenerateUrl { get; set; } = false;
    }

    public class HttpServiceParameterCacheOption
    {
        public bool IsEnable { get; set; } = false;

        /// <summary>
        /// Cache 的時間. 預設值是 10, 小於等於0即便把 IsEnableCache 啟動也是會重新抓資料
        /// </summary>
        public int Minutes { get; set; } = 10;
    }

    public class HttpServcieParameterRetryOption
    {
        public bool IsEnable => MaxRetryTimes > 0;

        /// <summary>
        /// 最多再次重試的次數，不包含原本那次
        /// </summary>
        public uint MaxRetryTimes { get; set; } = 0;

        /// <summary>
        /// 間隔時間 (豪秒)
        /// </summary>
        public int Interval { get; set; } = 15000;
    }
}
