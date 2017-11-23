using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Task.Objects
{
    public class JobExecutionContextStub : IJobExecutionContext
    {
        public IScheduler Scheduler { get; }

        public ITrigger Trigger { get; }

        public ICalendar Calendar { get; }

        public bool Recovering { get; }

        public TriggerKey RecoveringTriggerKey { get; }

        public int RefireCount { get; }

        public JobDataMap MergedJobDataMap { get; }

        public IJobDetail JobDetail { get; }

        public IJob JobInstance { get; }

        public DateTimeOffset? FireTimeUtc { get; }

        public DateTimeOffset? ScheduledFireTimeUtc { get; }

        public DateTimeOffset? PreviousFireTimeUtc { get; }

        public DateTimeOffset? NextFireTimeUtc { get; }

        public string FireInstanceId { get; }

        public object Result { get; set; }

        public TimeSpan JobRunTime { get; }

        public JobExecutionContextStub(bool test = true)
        {
            MergedJobDataMap = new JobDataMap();
            MergedJobDataMap["Test"] = test;
        }

        public object Get(object key)
        {
            return MergedJobDataMap[(string)key];
        }

        public void Put(object key, object objectValue)
        {
            MergedJobDataMap[(string)key] = objectValue;
        }
    }
}
