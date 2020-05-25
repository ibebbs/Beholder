using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Beholder.Snapshot.Provider
{
    public class Implementation : IProvider
    {
        private readonly HttpClient _httpClient;

        public Implementation(HttpClient httpClient, IOptions<Configuration> options)
        {
            _httpClient = httpClient;

            ConfigureHttpClient(options.Value);
        }

        private void ConfigureHttpClient(Configuration config)
        {
            _httpClient.BaseAddress = config.SnapshotUri;
        }

        public async Task<IImage> Get(string location)
        {
            var response = await _httpClient.GetAsync(string.Empty);

            response.EnsureSuccessStatusCode();

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);

                    return new Image(new Meta(DateTimeOffset.UtcNow, location),  memoryStream.ToArray());
                }
            }
        }
    }
}
