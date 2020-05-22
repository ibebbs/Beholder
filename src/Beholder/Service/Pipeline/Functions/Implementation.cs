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
        private readonly Func<IRecognition, Task<IPersistedRecognition>> _persistFace;
        private readonly Func<IPersistedRecognition, Task> _notifyRecognition;

        public Implementation(
            Func<Task<IEnumerable<IImage>>> fetch,
            Func<IImage, Task<IEnumerable<IImage>>> extractFaces,
            Func<IImage, Task<IEnumerable<IRecognition>>> recogniseFaces,
            Func<IRecognition, Task<IPersistedRecognition>> persistFace,
            Func<IPersistedRecognition, Task> notifyFacialRecognition)
        {
            _fetch = fetch;
            _extractFaces = extractFaces;
            _persistFace = persistFace;
            _recogniseFaces = recogniseFaces;
            _notifyRecognition = notifyFacialRecognition;
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

        public Task<IPersistedRecognition> PersistRecognition(IRecognition image)
        {
            return _persistFace(image);
        }

        public Task NotifyRecognition(IPersistedRecognition recognition)
        {
            return _notifyRecognition(recognition);
        }
    }
}
