using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuartzJob
{
    public class TestJob : IJob
    {
        public static string TriggerCron => "0/15 * * ? * MON-FRI";
        private readonly ILogger _logger;
        public TestJob(ILogger<TestJob> logger)
        {
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation(string.Format($"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}[{DateTime.Now:yyyy-MM-dd hh:mm:ss:ffffff}]任务执行！", DateTime.Now));
            //do some thing
            //send me a message
            return Task.CompletedTask;
        }
    }
}
