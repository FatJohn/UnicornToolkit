using System;
using System.Collections.Generic;
#if  WINDOWS_UWP
using Windows.Web.Http;
using System.Runtime.InteropServices.WindowsRuntime;
#else
using System.Net.Http;
#endif
using System.Text;
using System.Threading.Tasks;
using Unicorn.Net;
using Unicorn.ServiceModel;

namespace DemoApp
{
    public class GoogleSearchParameter : HttpServiceParameter
    {
        public string q { get; set; }

        public string oe { get; set; } = "utf - 8";
    }
    
    public class RawStringParser : BaseParser<string, HttpResponseMessage>
    {
        public override async Task<ParseResult<string>> Parse(HttpResponseMessage source)
        {
#if WINDOWS_UWP
            var buffer = await source.Content.ReadAsBufferAsync();
            var byteArray = buffer.ToArray();
#else
            var byteArray = await source.Content.ReadAsByteArrayAsync();
#endif
            var result = Encoding.UTF8.GetString(byteArray);
            return new ParseResult<string>(result);
        }
    }

    public class GoogleSearchService : BaseHttpService<string, GoogleSearchParameter, RawStringParser>
    {
        protected override string CreateRequestUri(GoogleSearchParameter parameter)
        {
            return "https://www.google.com.tw/";
        }
    }
}
