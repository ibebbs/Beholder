using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Face.Data
{
    public interface IProvider
    {
        Task<DateTimeOffset?> GetDateOfLastChangeAsync(CancellationToken cancellationToken);

        IAsyncEnumerable<Item> GetBalancedItems(CancellationToken cancellationToken);
    }

    public class Provider : IProvider
    {
        private readonly Persistence.IProvider _persistenceProvider;
        private readonly IOptions<Configuration> _options;
        private readonly ILogger<Provider> _logger;

        public Provider(Persistence.IProvider persistenceProvider, IOptions<Configuration> options, ILogger<Provider> logger)
        {
            _persistenceProvider = persistenceProvider;
            _options = options;
            _logger = logger;
        }

        private async Task<Item> CreateItem(string name, Stream source)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await source.CopyToAsync(ms);

                return new Item { Name = name, Image = ms.ToArray() };
            }
        }

        public async IAsyncEnumerable<Item> GetBalancedItems([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var allItems = await _persistenceProvider.GetItems(_options.Value.ConnectionString, _options.Value.CategorizedFacesContainerName, cancellationToken)
                .ToListAsync();

            var grouped = allItems
                .GroupBy(item => Path.GetDirectoryName(item))
                .Select(group => (Name: group.Key, Count: group.Count(), Items: group.AsEnumerable()))
                .GroupJoin(_options.Value.CategoriesToIgnore, tuple => tuple.Name.ToLower(), ignore => ignore.ToLower(), (tuple, ignored) => (tuple.Name, tuple.Count, tuple.Items, Include: !ignored.Any()))
                .Where(tuple => tuple.Include)
                .ToArray();

            var minItems = grouped.Min(group => group.Count);

            _logger.LogInformation("Balaned input comprises {0} items per category", minItems);

            foreach (var group in grouped)
            {
                foreach (var blobName in group.Items.Take(minItems))
                {
                    var item = await _persistenceProvider.GetBlobItemAsync(_options.Value.ConnectionString, _options.Value.CategorizedFacesContainerName, blobName, stream => CreateItem(group.Name, stream), cancellationToken);

                    yield return item;
                }
            }
        }

        public Task<DateTimeOffset?> GetDateOfLastChangeAsync(CancellationToken cancellationToken)
        {
            return _persistenceProvider.GetDateOfLastChangeAsync(_options.Value.ConnectionString, _options.Value.CategorizedFacesContainerName, cancellationToken);
        }
    }
}
