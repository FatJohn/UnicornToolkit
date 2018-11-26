using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicorn;

namespace DemoApp.DotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            PlatformService.Log = new NullLogService();

            var resutl = Task.Run(async () =>
            {
                var parameter = new GoogleSearchParameter
                {
                    Timeout = TimeSpan.FromMilliseconds(300),
                    q = "周杰倫",
                };
                var service = new GoogleSearchService();
                var result = await service.InvokeAsync(parameter);

                return result;
            }).Result;

            Console.WriteLine(resutl.Content);
            Console.ReadLine();
        }
    }
}
