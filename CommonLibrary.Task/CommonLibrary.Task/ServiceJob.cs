using CommonLibrary.Configuration;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.Utilities;

namespace CommonLibrary.Task
{
    [Serializable]
    public abstract class ServiceJob : IJob
    {
        public bool Test { get; protected internal set; }

        /// <summary>
        /// This method used by Quartz, not intendent to use manually. Calling this method directly would cause infinite loop
        /// </summary>
        /// <param name="context"></param>
        public virtual void Execute(IJobExecutionContext context)
        {
            Test = (context.Get("Test") as bool?).GetValueOrDefault();

            ExecuteInternal(context);
        }

        protected abstract void ExecuteInternal(IJobExecutionContext context);
    }

    [Serializable]
    public abstract class ServiceJob<TConfiguration> : ServiceJob
        where TConfiguration : IConfigurationManager, new()
    {
        private static readonly Logger logger = new Logger(typeof(ServiceJob<>));

        public TConfiguration Configuration { get; private set; }

        /// <summary>
        /// This method used by Quartz, not intendent to use manually. Calling this method directly would cause infinite loop
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(IJobExecutionContext context)
        {
            InitializeConfiguration();

            base.Execute(context);
        }

        private void InitializeConfiguration()
        {
            Configuration = new TConfiguration();
            if (!Configuration.LoadConfiguration())
            {
                logger.Error("Service configuration error");
                throw new InvalidOperationException("Service configuration error");
            }
        }
    }
}
