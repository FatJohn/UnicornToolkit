# UnicornToolkit

##### NuGet: https://www.nuget.org/packages/UnicornToolkit/

##### This library is mainly to provide a simple way connect http service.
> Other classes were create during my developing or take from other open source project.

OK~ Let me use an example to explain the idea.

If you are trying connect a http login service.
- API host at : http://myUrl.personal/v1/login
- one GET parameter : date (in format yyyy/MM/dd)
- two POST parameter : userid, passowrd
- result in JSON format as below

```json
{
  "status": 1,
  "memberType": "Premium",
  "message" : "Login Success!",
}
```


In my thoughts, a service is combined in three parts
1. Data 
2. Parameter
3. Parser. 

So I made BaseHttpService combined with theese three generic parts. Here're steps to create a client service.

#### 1 : Create a Data class
```csharp
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
```


#### 2 : Create a Parameter class
```csharp
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
    [HttpProperty("date", HttpPropertyFor.GET, typeof(DateTimeToLoginDateServiceConverter)]
    public DateTime Date { get; set; }

    [HttpProperty("userid", HttpPropertyFor.POST)]
    public string UserId { get; set; }

    [HttpProperty("password", HttpPropertyFor.POST)]
    public string Password { get; set; }
}
```

#### 3 : Create Service class
```csharp
public class LoginService : BaseHttpService<LoginResult, LoginParameter, JsonParser<LoginResult>>
{
    protected override string CreateRequestUri(LoginParameter parameter)
    {
        return "http://myUrl.personal/v1/login";
    }
}
```

#### 4 : Invoke Service
```csharp
var parameter = new LoginParameter
{
    Date = DateTime.Now,
    UserId = "myUserId",
    Password = "myPassword",
};

var service = new LoginService();
var result = await service.InvokeAsync(parameter);
```

And its done. Now LoginService will pack in correct GET url and POST content to send a request.
Then you get the result you declared.

***

#### Custom Parser
Of course you can create your custom Parser since there're lots of situation. Just like our built in JsonParser.
```csharp
public class CustomParser : BaseParser<LoginResult, HttpResponseMessage>
{
    public override Task<ParseResult<LoginResult>> Parse(HttpResponseMessage source)
    {
        // implement your parse logic here
    }
}
```

Because we're all http api. so the second generic parameter of BaseParser should be HttpResponseMessage.

*which namespace of HttpResponseMessage depends on which platform you are developing. 
The simplest way to decide UWP(WinRT): Windows.Web.Http, Other : System.Net.Http*

****

#### Log
If you want to log what servcie sent. You can implement your own logger and set it to PlatformService.Logger at the begging of your application.
```csharp
// YourPlatformLogger must inherit from ILogService
PlatformService.Logger = YourPlatformLogger;
```

***

#### PreFlow
If you look at BaseHttpService, you'll find it has a PreProcessFlow property. It's a collection. These preflow will Pre-Process your parameter in the order you add your preflow classes to the collection. After preflow then parameter will be packed and send.

Preflow class must inherit from IHttpPreFlow
```csharp
public interface IHttpPreFlow<TParameter>
    where TParameter : HttpServiceParameter
{
    Task Process(TParameter parameter);
}
```