using System;

namespace Examiner.Service.State
{
    public class Snapshot
    {
        public static readonly Snapshot Empty = new Snapshot();

        public DateTimeOffset? LastTrainingDataUpdate { get; set; }
    }
}
