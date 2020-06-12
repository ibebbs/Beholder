using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lensman.Data
{
    public interface IProvider
    {
        IAsyncEnumerable<Director.Client.Face> UnrecognisedBy(Guid userId, Func<long, long> incrementPage = null);

        Task<Director.Client.Face> UnrecognisedBy(Guid userId);

        Task<Director.Client.Recognition> Recognise(Guid faceId, Guid recogniserId, string label, float confidence);
    }

    public class Provider : IProvider
    {
        private const long ItemsPerPage = 25;

        private readonly Director.Client.IFacesClient _client;

        public Provider(Director.Client.IFacesClient client)
        {
            _client = client;
        }

        public async IAsyncEnumerable<Director.Client.Face> UnrecognisedBy(Guid userId, Func<long, long> incrementPage = null)
        {
            incrementPage ??= p => p + 1;

            long page = 0;
            int resultCount;

            do
            {
                page = incrementPage(page);

                var results = await _client.GetUnrecognisedByAsync(userId, page, ItemsPerPage).ConfigureAwait(false);

                resultCount = results.Count;

                foreach (var face in results)
                {
                    yield return face;
                }

            } while (resultCount == ItemsPerPage);
        }

        public async Task<Director.Client.Face> UnrecognisedBy(Guid userId)
        {
            var results = await _client.GetUnrecognisedByAsync(userId, 1, 1).ConfigureAwait(false);

            return results.FirstOrDefault();
        }

        public async Task<Director.Client.Recognition> Recognise(Guid faceId, Guid recogniserId, string label, float confidence)
        {
            var result = await _client.AddRecognitionAsync(faceId, recogniserId, label, confidence).ConfigureAwait(false);

            return result;
        }
    }
}
