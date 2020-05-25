using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beholder.Service.Pipeline.Functions
{
    /// <summary>
    /// A simple facade for interacting with other components
    /// </summary>
    public class Implementation : IFunctions
    {
        private readonly Func<string, Task<IEnumerable<IImage>>> _fetch;
        private readonly Func<IImage, Task<IEnumerable<IImage>>> _extractFaces;
        private readonly Func<IImage, Task<IEnumerable<IRecognition>>> _recogniseFaces;
        private readonly Func<IRecognition, Task<IPersistedRecognition>> _persistRecognition;
        private readonly Func<IPersisted, Task> _notifyPersisted;

        public Implementation(
            Func<string, Task<IEnumerable<IImage>>> fetch,
            Func<IImage, Task<IEnumerable<IImage>>> extractFaces,
            Func<IImage, Task<IEnumerable<IRecognition>>> recogniseFaces,
            Func<IRecognition, Task<IPersistedRecognition>> persistRecognition,
            Func<IPersisted, Task> notifyPersisted)
        {
            _fetch = fetch;
            _extractFaces = extractFaces;
            _persistRecognition = persistRecognition;
            _recogniseFaces = recogniseFaces;
            _notifyPersisted = notifyPersisted;
        }

        public Task<IEnumerable<IImage>> Fetch(string location)
        {
            return _fetch(location);
        }

        public Task<IEnumerable<IImage>> ExtractFaces(IImage image)
        {
            return _extractFaces(image);
        }

        public Task<IEnumerable<IRecognition>> RecogniseFaces(IImage image)
        {
            return _recogniseFaces(image);
        }

        public Task<IPersistedRecognition> PersistRecognition(IRecognition recognition)
        {
            return _persistRecognition(recognition);
        }

        public Task NotifyRecognition(IPersisted persisted)
        {
            return _notifyPersisted(persisted);
        }
    }
}
