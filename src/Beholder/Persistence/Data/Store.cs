using System;
using System.Linq;
using System.Threading.Tasks;

namespace Beholder.Persistence.Data
{
    public interface IStore
    {
        Task SaveRecognition(IRecognition recognition, Uri blobUri);
    }

    public class Store : IStore
    {
        public Director.Client.IFacesClient _facesClient;

        public Store(Director.Client.IFacesClient facesClient)
        {
            _facesClient = facesClient;
        }

        public async Task SaveRecognition(IRecognition recognition, Uri blobUri)
        {
            var face = await _facesClient.AddAsync(blobUri);

            var tag = recognition.Tags.OrderByDescending(tag => tag.Confidence).FirstOrDefault();

            if (tag != null)
            {
                var persisted = await _facesClient.AddRecognitionAsync(face.Id, Director.Client.Recognisers.MLNET, tag.Name, tag.Confidence);
            }
        }
    }
}
