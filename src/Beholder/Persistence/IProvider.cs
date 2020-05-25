using System;
using System.Threading.Tasks;

namespace Beholder.Persistence
{
    public interface IProvider
    {
        Task<(Guid, Uri)> Save(IRecognition recognition);
    }
}
