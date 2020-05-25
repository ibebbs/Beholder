using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Beholder.Persistence.Blob
{
    public interface IStore
    {
        Task<Uri> SaveImage(IImage image);
    }

    public class Store : IStore
    {
        private readonly IOptions<Configuration> _options;
        private readonly ILogger<Store> _logger;

        public Store(IOptions<Configuration> options, ILogger<Store> logger)
        {
            _options = options;
            _logger = logger;
        }

        public async Task<Uri> SaveImage(IImage image)
        {
            var id = Guid.NewGuid().ToString();
            var fileName = $"{id}.png";

            var client = new BlobContainerClient(_options.Value.BlobConnectionString, _options.Value.BlobContainer);

            using (MemoryStream stream = new MemoryStream(image.Data))
            {
                _logger.LogInformation(0, "Persisting file {0}", fileName);

                await client.UploadBlobAsync(fileName, stream);

                _logger.LogInformation(1, "Persisted file {0}", fileName);

                var uri = client.GetBlobClient(fileName).Uri;

                return uri;
            }
        }
    }
}
