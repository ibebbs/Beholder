using System.Threading;
using System.Threading.Tasks;

namespace Beholder.Service
{
    public interface IProcessor
    {
        Task RunAsync(IPipeline pipeline, CancellationToken cancellationToken);
    }
}
