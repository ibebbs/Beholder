using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
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

        public async Task<string> SaveFace(IImage image)
        {
            var client = new BlobContainerClient(_options.Value.ConnectionString, _options.Value.FaceContainer);

            using (MemoryStream stream = new MemoryStream())
            {
                image.Bitmap.Save(stream, ImageFormat.Png);

                var id = Guid.NewGuid().ToString();
                var fileName = $"{id}.png";

                stream.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation(0, "Persisting file {0}", fileName);

                await client.UploadBlobAsync(fileName, stream);

                _logger.LogInformation(1, "Persisted file {0}", fileName);

                return fileName;
            }
        }
    }
}
