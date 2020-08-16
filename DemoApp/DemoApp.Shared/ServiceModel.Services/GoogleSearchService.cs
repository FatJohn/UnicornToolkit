using System;
using System.Collections.Generic;
using System.Net.Http;
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
            var byteArray = await source.Content.ReadAsByteArrayAsync();
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
