using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Face.Recognition
{
    public interface IEngine
    {
        Task<IModel> TrainAsync(IAsyncEnumerable<Data.Item> source, CancellationToken cancellationToken);
    }
}
