using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Unicorn;
using Unicorn.Net;
using Unicorn.ServiceModel;

namespace DemoApp.UWP
{
    public enum LoginStatus
    {
        Fail,
        Success,
    }

    public enum MemberType
    {
        [EnumProperty("premium")]
        Premium,
        [EnumProperty("normal")]
        Normal,
    }

    public class LoginResult
    {
        [JsonProperty("status")]
        [JsonConverter(typeof(IntToEnumJsonConverter))]
        public LoginStatus IsSuccess { get; set; }

        [JsonProperty("memberType")]
        [JsonConverter(typeof(EnumKeyJsonConverter))]
        public MemberType MemberType { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class DateTimeToLoginDateServiceConverter : IServiceParameterConveter
    {
        public string Convert(object value)
        {
            var date = (DateTime)value;
            return date.ToString("yyyy/MM/dd");
        }
    }

    public class LoginParameter : HttpServiceParameter
    {
        [HttpProperty("date", HttpPropertyFor.GET, typeof(DateTimeToLoginDateServiceConverter))]
        public DateTime Date { get; set; }

        [HttpProperty("userid", HttpPropertyFor.POST)]
        public string UserId { get; set; }

        [HttpProperty("password", HttpPropertyFor.POST)]
        public string Password { get; set; }
    }

    public class LoginService : BaseHttpService<LoginResult, LoginParameter, JsonParser<LoginResult>>
    {
        protected override string CreateRequestUri(LoginParameter parameter)
        {
            return "http://myUrl.personal/v1/login";
        }
    }

    public class CustomParser : BaseParser<LoginResult, HttpResponseMessage>
    {
        public override Task<ParseResult<LoginResult>> Parse(HttpResponseMessage source)
        {
            throw new NotImplementedException();
        }
    }
}
