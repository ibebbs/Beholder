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

        private async Task<IEnumerable<IImage>> Fetch(ILogger logger)
        {
            try
            {
                var image = await _snapshotProvider.Get();

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

        private Task<Uri> PersistRecognised(string name, IImage recognition, ILogger logger)
        {
            return _persistenceProvider.SaveRecognised(name, recognition);
        }

        private Task<Uri> PersistUnrecognised(IImage image, ILogger logger)
        {
            return _persistenceProvider.SaveUnrecognised(image);
        }

        public Task<IFunctions> Create(ILogger logger)
        {
            var functions = new Functions.Implementation(
                () => Fetch(logger),
                image => ExtractFaces(image, logger),
                image => RecogniseFace(image, logger),
                (name, image) => PersistRecognised(name, image, logger),
                image => PersistUnrecognised(image, logger),
                persistedRecognition => Task.CompletedTask);

            return Task.FromResult<IFunctions>(functions);
        }
    }
}
