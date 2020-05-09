using System.Threading.Tasks;

namespace Beholder.Persistence
{
    public interface IProvider
    {
        Task<string> SaveFace(IImage image);
    }
}
