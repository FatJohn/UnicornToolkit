using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unicorn;
using Unicorn.Net;
using Unicorn.ServiceModel;

namespace DemoApp.Core
{
    class Program
    {
        static async Task Main(string[] args)
        {
            PlatformService.Log = new NullLogService();

            var parameter = new GoogleSearchParameter
            {
                q = "周杰倫",
            };
            var service = new GoogleSearchService();
            var result = await service.InvokeAsync(parameter);

            Console.WriteLine(result.Content);
            Console.ReadLine();
        }
    }
}
