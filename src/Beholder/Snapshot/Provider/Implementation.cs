using Microsoft.Extensions.Options;
using System;
using System.Drawing;
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

        public async Task<Bitmap> Get()
        {
            var response = await _httpClient.GetAsync(string.Empty);

            response.EnsureSuccessStatusCode();

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                Bitmap bitmap = new Bitmap(stream);

                return bitmap;
            }
        }
    }
}
