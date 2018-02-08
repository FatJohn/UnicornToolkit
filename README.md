# UnicornToolkit  [![NuGet Version](https://img.shields.io/nuget/v/UnicornToolkit.svg?style=flat)](https://www.nuget.org/packages/UnicornToolkit/)

Install from NuGet
> PM> Install-Package UnicornToolkit

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

There are 4 types of attributes that you can add on your property.
- **HttpIgnoreAttribute** : When sending http request won't treat this property as any part of query string or body. This is can be used in places which you want user to fill something is part of your dynamic request uri. And you can use it in the override **CreateRequestUri** method in your service.
- **HttpMultiPartPropertyAttribute** : see this property as part of http Multi-Part content.
- **HttpRawByteArrayPropertyAttribute** : this is used for your own custom byte content.
- **HttpRawStringPropertyAttribute** : this is used for you own string content.

You can set HttpMethod, Content-Type by setting **HttpMethod** and **HttpContentType** property.
  > Default value of HttpMethod is NotSpecific. My generator will check your attribute to determine whether it should use GET or POST. If you change **HttpMethod** property to other value, generator will use your value.

Also there's a **Options** property which you can set like if enable cache, retry, even by pass auto generate request url.

There're some built-in ServiceParameterParser. Such as **DateTimeToUnixSecondsParameterConverter, EnumKeyParameterConverter, EnumToIntParameterConverter, TimeSpanParameterConverter**.

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
// Or you can use built-in Logger
PlatformService.Logger = new NullLogService
```

> **NullLogService** ouput log to output window by using **System.Diagnostics.Debug.WriteLine** method.
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

#### Other Useful classes
- UnixDateTimeConverter
- EnumHelper
- Convert2
- AsyncLock, AsyncAutoResetEvent, AsyncBarrier, AsyncCountdownEvent, AsyncReaderWriterLock, AsyncSemaphore