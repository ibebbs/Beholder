using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Beholder.Persistence.Provider
{
    public class Implementation : IProvider
    {
        private readonly Blob.IStore _blobStore;
        private readonly Data.IStore _dataStore;
        private readonly ILogger<Implementation> _logger;

        public Implementation(Blob.IStore blobStore, Data.IStore dataStore, ILogger<Implementation> logger)
        {
            _blobStore = blobStore;
            _dataStore = dataStore;
            _logger = logger;
        }

        public async Task<Uri> Save(IRecognition recognition)
        {
            var uri = await _blobStore.SaveImage(recognition);

            await _dataStore.SaveRecognition(recognition, uri);

            return uri;
        }
    }
}
