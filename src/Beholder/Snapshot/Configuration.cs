using System;
using System.ComponentModel.DataAnnotations;

namespace Beholder.Snapshot
{
    public class Configuration
    {
        [Required]
        public Uri SnapshotUri { get; set; }
    }
}
