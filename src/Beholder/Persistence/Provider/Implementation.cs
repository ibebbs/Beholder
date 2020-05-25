using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Beholder.Persistence.Provider
{
    public class Implementation : IProvider
    {
        public Director.Client.IFacesClient _facesClient;
        private readonly ILogger<Implementation> _logger;

        public Implementation(Director.Client.IFacesClient facesClient, ILogger<Implementation> logger)
        {
            _facesClient = facesClient;
            _logger = logger;
        }

        public async Task<(Guid, Uri)> Save(IRecognition recognition)
        {
            var id = Guid.NewGuid();
            var fileName = "{id}.png";

            using (var stream = new MemoryStream(recognition.Data))
            {
                var face = await _facesClient.AddAsync(new Director.Client.FileParameter(stream, fileName), recognition.Meta.Location);

                var tag = recognition.Tags.OrderByDescending(tag => tag.Confidence).FirstOrDefault();

                if (tag != null)
                {
                    var persisted = await _facesClient.AddRecognitionAsync(face.Id, Director.Client.Recognisers.MLNET, tag.Name, tag.Confidence);
                }

                return (face.Id, new Uri(face.Uri));
            }
        }
    }
}
