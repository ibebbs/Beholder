using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Service.State
{
    public interface IProvider
    {
        Task<Snapshot> GetStateSnapshotAsync(CancellationToken cancellationToken);
        Task SetStateSnapshotAsync(Snapshot snapshot, CancellationToken cancellationToken);
    }

    public class Provider : IProvider
    {
        private readonly Persistence.IProvider _persistenceProvider;
        private readonly IOptions<Configuration> _options;

        public Provider(Persistence.IProvider persistenceProvider, IOptions<Configuration> options)
        {
            _persistenceProvider = persistenceProvider;
            _options = options;
        }

        public async Task<Snapshot> GetStateSnapshotAsync(CancellationToken cancellationToken)
        {
            if (await _persistenceProvider.BlobExistsAsync(_options.Value.ConnectionString, _options.Value.SnapshotContainer, "Snapshot.json", cancellationToken))
            {
                using (var stream = await _persistenceProvider.GetBlobContentAsync(_options.Value.ConnectionString, _options.Value.SnapshotContainer, "Snapshot.json", cancellationToken))
                { 
                    var snapshot = await JsonSerializer.DeserializeAsync<Snapshot>(stream);

                    return snapshot;
                }
            }
            else
            {
                return Snapshot.Empty;
            }
        }

        public async Task SetStateSnapshotAsync(Snapshot snapshot, CancellationToken cancellationToken)
        {
            Func<Stream, Task> serialization = stream => JsonSerializer.SerializeAsync(stream, snapshot);

            await _persistenceProvider.SetBlobContentAsync(_options.Value.ConnectionString, _options.Value.SnapshotContainer, "Snapshot.json", serialization, cancellationToken);
        }
    }
}
