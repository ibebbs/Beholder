using Azure.Storage.Blobs;
using Examiner.Face.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Persistence
{
    public interface IProvider
    {
        Task<bool> BlobExistsAsync(string connectionString, string containerName, string blobName, CancellationToken cancellationToken);

        Task<Stream> GetBlobContentAsync(string connectionString, string containerName, string blobName, CancellationToken cancellationToken);

        Task SetBlobContentAsync(string connectionString, string containerName, string blobName, Func<Stream, Task> content, CancellationToken cancellationToken);

        Task<DateTimeOffset?> GetDateOfLastChangeAsync(string connectionString, string containerName, CancellationToken cancellationToken);

        IAsyncEnumerable<string> GetItems(string connectionString, string containerName, CancellationToken cancellationToken);
        
        Task<T> GetBlobItemAsync<T>(string connectionString, string containerName, string blobName, Func<Stream, Task<T>> createItem, CancellationToken cancellationToken);
    }

    public class Provider : IProvider
    {
        private readonly ILogger<Provider> _logger;

        public Provider(ILogger<Provider> logger)
        {
            _logger = logger;
        }

        private class ProgressHandler : IProgress<long>
        {
            private readonly string _action;
            private readonly ILogger<Provider> _logger;

            public ProgressHandler(string action, ILogger<Provider> logger)
            {
                _action = action;
                _logger = logger;
            }

            public void Report(long value)
            {
                _logger.LogInformation("Action {0} reports progress {1}", _action, value);
            }
        }

        public async Task<bool> BlobExistsAsync(string connectionString, string containerName, string blobName, CancellationToken cancellationToken)
        {
            _logger.LogInformation("BlobExistsAsync: {0}/{1}/{2}", connectionString, containerName, blobName);

            var client = new BlobClient(connectionString, containerName, blobName);

            var response = await client.ExistsAsync(cancellationToken).ConfigureAwait(false);

            return response.Value;
        }

        public async Task<Stream> GetBlobContentAsync(string connectionString, string containerName, string blobName, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetBlobContentAsync: {0}/{1}/{2}", connectionString, containerName, blobName);

            var client = new BlobClient(connectionString, containerName, blobName);

            var response = await client.DownloadAsync(cancellationToken).ConfigureAwait(false);

            return response.Value.Content;
        }

        public async Task SetBlobContentAsync(string connectionString, string containerName, string blobName, Func<Stream, Task> content, CancellationToken cancellationToken)
        {
            _logger.LogInformation("SetBlobContentAsync: {0}/{1}/{2}", connectionString, containerName, blobName);

            var client = new BlobClient(connectionString, containerName, blobName);

            using (var stream = new MemoryStream())
            {
                await content(stream);

                stream.Seek(0, SeekOrigin.Begin);

                await client.UploadAsync(stream, progressHandler: new ProgressHandler($"SetBlobContentAsync: {containerName}/{blobName}", _logger)).ConfigureAwait(false);
            }
        }

        public async Task<DateTimeOffset?> GetDateOfLastChangeAsync(string connectionString, string containerName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetDateOfLastChangeAsync: {0}/{1}", connectionString, containerName);

                var client = new BlobContainerClient(connectionString, containerName);

                var lastModified = await client.GetBlobsAsync(cancellationToken: cancellationToken)
                    .Select(item => item.Properties.LastModified)
                    .MaxAsync().ConfigureAwait(false);

                return lastModified;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async IAsyncEnumerable<string> GetItems(string connectionString, string containerName, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetItems: {0}/{1}", connectionString, containerName);

            var client = new BlobContainerClient(connectionString, containerName);

            await foreach (var blobItem in client.GetBlobsAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                yield return blobItem.Name;
            }
        }
        
        public async Task<T> GetBlobItemAsync<T>(string connectionString, string containerName, string blobName, Func<Stream, Task<T>> createItem, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetBlobItemAsync: {0}/{1}/{2}", connectionString, containerName, blobName);

            var client = new BlobClient(connectionString, containerName, blobName);

            var downloaded = await client.DownloadAsync().ConfigureAwait(false);

            using (var stream = downloaded.Value.Content)
            {
                var item = await createItem(stream).ConfigureAwait(false);

                return item;
            }
        }
    }
}
