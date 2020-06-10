using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace Lensman.Platform
{
    public partial class Schedulers : ISchedulers
    {
        private static readonly Lazy<IScheduler> DispatchScheduler = new Lazy<IScheduler>(() => new SynchronizationContextScheduler(SynchronizationContext.Current));

        public IScheduler Default => Scheduler.Default;

        public IScheduler Dispatcher => DispatchScheduler.Value;
    }
}
