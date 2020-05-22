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
        private readonly Func<Task<IEnumerable<IImage>>> _fetch;
        private readonly Func<IImage, Task<IEnumerable<IImage>>> _extractFaces;
        private readonly Func<IImage, Task<IEnumerable<IRecognition>>> _recogniseFaces;
        private readonly Func<string, IImage, Task<Uri>> _persistRecognised;
        private readonly Func<IImage, Task<Uri>> _persistUnrecognised;
        private readonly Func<IPersisted, Task> _notifyPersisted;

        public Implementation(
            Func<Task<IEnumerable<IImage>>> fetch,
            Func<IImage, Task<IEnumerable<IImage>>> extractFaces,
            Func<IImage, Task<IEnumerable<IRecognition>>> recogniseFaces,
            Func<string, IImage, Task<Uri>> persistRecognised,
            Func<IImage, Task<Uri>> persistUnrecognised,
            Func<IPersisted, Task> notifyPersisted)
        {
            _fetch = fetch;
            _extractFaces = extractFaces;
            _persistRecognised = persistRecognised;
            _persistUnrecognised = persistUnrecognised;
            _recogniseFaces = recogniseFaces;
            _notifyPersisted = notifyPersisted;
        }

        public Task<IEnumerable<IImage>> Fetch()
        {
            return _fetch();
        }

        public Task<IEnumerable<IImage>> ExtractFaces(IImage image)
        {
            return _extractFaces(image);
        }

        public Task<IEnumerable<IRecognition>> RecogniseFaces(IImage image)
        {
            return _recogniseFaces(image);
        }

        public Task<Uri> PersistRecognised(string name, IImage image)
        {
            return _persistRecognised(name, image);
        }

        public Task<Uri> PersistUnrecognised(IImage image)
        {
            return _persistUnrecognised(image);
        }

        public Task NotifyRecognition(IPersisted persisted)
        {
            return _notifyPersisted(persisted);
        }
    }
}
