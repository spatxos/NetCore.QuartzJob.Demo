using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Microsoft.Extensions.Logging;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IApplicationLifetime;
using System;
using Quartz.Spi;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace QuartzJob
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory factory, IHostingEnvironment env)
        {
            EnvironmentName = env.EnvironmentName;
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfiguration Configuration { get; }
        public string EnvironmentName { get; }
        private void ConfigureQuartz(IServiceCollection services, params Type[] jobs)
        {
            services.AddSingleton<IJobFactory, JobFactory>();
            foreach (ServiceDescriptor serviceDescriptor in jobs.Select(jobType => new ServiceDescriptor(jobType, jobType, ServiceLifetime.Singleton)))
            {
                services.Add(serviceDescriptor);
            }
            services.AddSingleton(provider =>
            {
                var schedulerFactory = new StdSchedulerFactory();
                var scheduler = schedulerFactory.GetScheduler().Result;
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                scheduler.Start();
                return scheduler;
            });
        }
        protected void ConfigureJobsIoc(IServiceCollection services)
        {
            var types = new Type[] {
                typeof(TestJob)
            };
            ConfigureQuartz(services, types);
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            ConfigureJobsIoc(services);
            services.AddHttpContextAccessor();
        }
        protected void StartJobs(IApplicationBuilder app, IApplicationLifetime lifetime)
        {
            var scheduler = app.ApplicationServices.GetService<IScheduler>();
            //TODO: use some config
            QuartzServicesUtilities.StartJob<TestJob>(scheduler, TestJob.TriggerCron);
            lifetime.ApplicationStarted.Register(() => scheduler.Start());
            lifetime.ApplicationStopping.Register(() => scheduler.Shutdown());
        }
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {
            StartJobs(app, lifetime);
            app.Build();
        }
    }
}
