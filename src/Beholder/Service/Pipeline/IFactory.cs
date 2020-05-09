using System.Threading.Tasks;

namespace Beholder.Service.Pipeline
{

    public interface IFactory
    {
        Task<IPipeline> CreateAsync();
    }
}
