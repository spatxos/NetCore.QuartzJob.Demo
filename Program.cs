using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;
using System;

namespace QuartzJob
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
               .UseUrls("http://locahost:8080")
               .UseStartup<Startup>()
               .ConfigureLogging((hostContext, configLogging) =>
               {
                   configLogging.AddConsole();
               })
               .Build();
        }
    }
}
