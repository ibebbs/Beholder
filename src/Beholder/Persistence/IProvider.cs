using System;
using System.Threading.Tasks;

namespace Beholder.Persistence
{
    public interface IProvider
    {
        Task<Uri> SaveRecognised(string name, IImage image);

        Task<Uri> SaveUnrecognised(IImage image);
    }
}
