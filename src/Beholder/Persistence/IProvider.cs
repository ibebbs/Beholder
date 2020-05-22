using System.Threading.Tasks;

namespace Beholder.Persistence
{
    public interface IProvider
    {
        Task<IPersistedRecognition> SaveRecognition(IRecognition recognition);
    }
}
