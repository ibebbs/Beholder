using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Beholder.Persistence.Provider
{
    public class Implementation : IProvider
    {
        private readonly IOptions<Configuration> _options;
        private readonly ILogger<Implementation> _logger;

        public Implementation(IOptions<Configuration> options, ILogger<Implementation> logger)
        {
            _options = options;
            _logger = logger;
        }

        private async Task<Uri> SaveFace(IImage image)
        {
            var client = new BlobContainerClient(_options.Value.ConnectionString, _options.Value.UnknownFaceContainer);

            using (MemoryStream stream = new MemoryStream(image.Data))
            {
                var id = Guid.NewGuid().ToString();
                var fileName = $"{id}.png";

                _logger.LogInformation(0, "Persisting file {0}", fileName);

                await client.UploadBlobAsync(fileName, stream);

                _logger.LogInformation(1, "Persisted file {0}", fileName);

                var uri = client.GetBlobClient(fileName).Uri;

                return uri;
            }
        }

        public async Task<IPersistedRecognition> SaveRecognition(IRecognition recognition)
        {
            var uri = await SaveFace(recognition);

            return new PersistedRecognition(recognition.Data, recognition.Tags, uri.ToString());
        }
    }
}
