using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Lensman.Platform
{
    public class CustomHttpClientFactory : IHttpClientFactory
    {

        public static readonly HttpMessageHandler Handler = new Uno.UI.Wasm.WasmHttpHandler();

        private readonly ConcurrentDictionary<string, HttpClient> _activeHandlers;

        public CustomHttpClientFactory()
        {
            _activeHandlers = new ConcurrentDictionary<string, HttpClient>();
        }

        public HttpClient CreateClient(string name)
        {
            return _activeHandlers.GetOrAdd(name, _ => new HttpClient(Handler) { BaseAddress = new Uri("http://localhost:5000/") });
        }
    }
}
