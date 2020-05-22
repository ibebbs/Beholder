using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Service
{
    public interface IProcessor
    {
        Task<bool> RequiresProcessingAsync(State.Snapshot snapshot, CancellationToken cancellationToken);

        Task<State.Snapshot> ProcessAsync(State.Snapshot snapshot, CancellationToken cancellationToken);
    }
}
