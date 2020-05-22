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

        private async Task<Uri> SaveImage(string container, string fileName, IImage image)
        {
            var client = new BlobContainerClient(_options.Value.ConnectionString, container);

            using (MemoryStream stream = new MemoryStream(image.Data))
            {

                _logger.LogInformation(0, "Persisting file {0}", fileName);

                await client.UploadBlobAsync(fileName, stream);

                _logger.LogInformation(1, "Persisted file {0}", fileName);

                var uri = client.GetBlobClient(fileName).Uri;

                return uri;
            }
        }

        public Task<Uri> SaveRecognised(string name, IImage image)
        {
            var id = Guid.NewGuid().ToString();
            var fileName = $"{name}/{id}.png";

            return SaveImage(_options.Value.RecognisedFaceContainer, fileName, image);
        }

        public Task<Uri> SaveUnrecognised(IImage image)
        {
            var id = Guid.NewGuid().ToString();
            var fileName = $"{id}.png";

            return SaveImage(_options.Value.UnrecognisedFaceContainer, fileName, image);
        }
    }
}
