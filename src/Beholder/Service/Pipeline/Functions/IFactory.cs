using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Beholder.Service.Pipeline.Functions
{
    public interface IFactory
    {
        Task<IFunctions> Create(ILogger logger);
    }
}
