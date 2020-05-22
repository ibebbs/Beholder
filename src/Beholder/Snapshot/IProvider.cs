using System.Drawing;
using System.Threading.Tasks;

namespace Beholder.Snapshot
{
    public interface IProvider
    {
        Task<IImage> Get();
    }
}
