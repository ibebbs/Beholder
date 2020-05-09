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
        private readonly Image.IFactory _imageFactory;

        public Implementation(Snapshot.IProvider snapshotProvider, Face.IDetector faceDetector, Persistence.IProvider persistenceProvider, Image.IFactory imageFactory)
        {
            _snapshotProvider = snapshotProvider;
            _faceDetector = faceDetector;
            _persistenceProvider = persistenceProvider;
            _imageFactory = imageFactory;
        }

        private async Task<IEnumerable<IImage>> Fetch(ILogger logger)
        {
            try
            {
                var bitmap = await _snapshotProvider.Get();

                return new[] { _imageFactory.Create(bitmap) };
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error retrieving snapshot. See inner exception for details");

                return Enumerable.Empty<IImage>();
            }
        }

        private async Task<IEnumerable<IImage>> ExtractFaces(IImage source, ILogger logger)
        {
            var bitmaps = await _faceDetector.ExtractFaces(source.Bitmap);

            var images = bitmaps.Select(_imageFactory.Create).ToArray();

            source.Dispose();

            return images;
        }

        private async Task PersistFace(IImage source, ILogger logger)
        {
            await _persistenceProvider.SaveFace(source);

            source.Dispose();
        }

        private Task<IEnumerable<IRecognition>> RecogniseFace(IImage source, ILogger logger)
        {
            source.Dispose();

            return Task.FromResult(Enumerable.Empty<IRecognition>());
        }

        public Task<IFunctions> Create(ILogger logger)
        {
            var functions = new Functions.Implementation(
                () => Fetch(logger),
                image => ExtractFaces(image, logger),
                image => PersistFace(image, logger),
                image => RecogniseFace(image, logger),
                recognition => Task.CompletedTask,
                recognition => Task.CompletedTask);

            return Task.FromResult<IFunctions>(functions);
        }
    }
}
