using System;
using System.Threading.Tasks;

namespace Beholder.Persistence.Data
{
    public interface IStore
    {
        Task SaveRecognition(IRecognition recognition, Uri blobUri);
    }

    public class Store : IStore
    {
        public Store()
        {
        }

        public Task SaveRecognition(IRecognition recognition, Uri blobUri)
        {
            throw new NotImplementedException();
        }
    }
}
