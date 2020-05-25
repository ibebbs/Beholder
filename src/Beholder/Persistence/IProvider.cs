using System;
using System.Threading.Tasks;

namespace Beholder.Persistence
{
    public interface IProvider
    {
        Task<Uri> Save(IRecognition recognition);
    }
}
