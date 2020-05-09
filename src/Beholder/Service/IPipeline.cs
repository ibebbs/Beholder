using System;
using System.Threading;
using System.Threading.Tasks;

namespace Beholder.Service
{
    public interface IPipeline
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task WaitForCompletion();
    }
}
