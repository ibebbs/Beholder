using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;

namespace Lensman.Platform
{
    public interface ISchedulers
    {
        IScheduler Default { get; }

        IScheduler Dispatcher { get; }
    }

    public partial class Schedulers : ISchedulers
    {
    }
}
