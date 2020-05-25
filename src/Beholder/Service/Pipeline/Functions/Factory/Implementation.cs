using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beholder.Service.Pipeline.Functions.Factory
{
    public class Implementation : IFactory
    {
        private readonly Persistence.IProvider _persistenceProvider;
        private readonly Snapshot.IProvider _snapshotProvider;
        private readonly Face.IDetector _faceDetector;
        private readonly Face.IRecognizer _faceRecognizer;

        public Implementation(Snapshot.IProvider snapshotProvider, Face.IDetector faceDetector, Face.IRecognizer faceRecognizer, Persistence.IProvider persistenceProvider)
        {
            _snapshotProvider = snapshotProvider;
            _faceDetector = faceDetector;
            _persistenceProvider = persistenceProvider;
            _faceRecognizer = faceRecognizer;
        }

        private async Task<IEnumerable<IImage>> Fetch(string location, ILogger logger)
        {
            try
            {
                var image = await _snapshotProvider.Get(location);

                return new[] { image };
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error retrieving snapshot. See inner exception for details");

                return Enumerable.Empty<IImage>();
            }
        }

        private Task<IEnumerable<IImage>> ExtractFaces(IImage image, ILogger logger)
        {
            return _faceDetector.ExtractFaces(image);
        }

        private Task<IEnumerable<IRecognition>> RecogniseFace(IImage image, ILogger logger)
        {
            var recognition = _faceRecognizer.RecogniseFace(image);

            return Task.FromResult<IEnumerable<IRecognition>>(new[] { recognition });
        }

        private async Task<IPersistedRecognition> PersistRecognition(IRecognition recognition, ILogger logger)
        {
            var tag = recognition.Tags.OrderByDescending(tag => tag.Confidence).FirstOrDefault();

            var name = tag?.Name;
            var confidence = tag?.Confidence ?? 0f;

            var uri = await _persistenceProvider.Save(recognition);

            return new PersistedRecognition(name, confidence, uri.ToString());
        }

        public Task<IFunctions> Create(ILogger logger)
        {
            var functions = new Functions.Implementation(
                location => Fetch(location, logger),
                image => ExtractFaces(image, logger),
                image => RecogniseFace(image, logger),
                recognition => PersistRecognition(recognition, logger),
                persistedRecognition => Task.CompletedTask);

            return Task.FromResult<IFunctions>(functions);
        }
    }
}
