using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Drawing;
using System.Threading.Tasks;

namespace Beholder.Tests.Service.Pipeline.Functions.Factory
{
    [TestFixture]
    public class Should
    {
        private (Beholder.Snapshot.IProvider, Beholder.Face.IDetector, Beholder.Persistence.IProvider, ILogger, Beholder.Service.Pipeline.Functions.Factory.Implementation) CreateSubject()
        {
            var snapshotProvider = A.Fake<Beholder.Snapshot.IProvider>();
            var faceDetector = A.Fake<Beholder.Face.IDetector>();
            var persistenceProvider = A.Fake<Beholder.Persistence.IProvider>();
            var imageFactory = A.Fake<Image.IFactory>();
            var logger = A.Fake<ILogger>();
            var subject = new Beholder.Service.Pipeline.Functions.Factory.Implementation(snapshotProvider, faceDetector, persistenceProvider, imageFactory);

            return (snapshotProvider, faceDetector, persistenceProvider, logger, subject);
        }

        [Test]
        public async Task UseTheSnapshotProviderToFetch()
        {
            (var snapshotProvider, var faceDetector, var persistenceProvider, var logger, var subject) = CreateSubject();

            var functions = await subject.Create(logger);

            await functions.Fetch();

            A.CallTo(() => snapshotProvider.Get()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task UseTheFaceDetectorToExtractFaces()
        {
            (var snapshotProvider, var faceDetector, var persistenceProvider, var logger, var subject) = CreateSubject();

            var functions = await subject.Create(logger);

            await functions.ExtractFaces(A.Fake<IImage>());

            A.CallTo(() => faceDetector.ExtractFaces(A<Bitmap>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task DisposeTheSourceImageAfterExtractingFaces()
        {
            (var snapshotProvider, var faceDetector, var persistenceProvider, var logger, var subject) = CreateSubject();

            var functions = await subject.Create(logger);

            var image = A.Fake<IImage>();

            await functions.ExtractFaces(image);

            A.CallTo(() => image.Dispose()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task UseThePersistenceProviderToSaveFace()
        {
            (var snapshotProvider, var faceDetector, var persistenceProvider, var logger, var subject) = CreateSubject();

            var functions = await subject.Create(logger);

            await functions.PersistFace(A.Fake<IImage>());

            A.CallTo(() => persistenceProvider.SaveFace(A<IImage>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task DisposeTheSourceImageAfterPersistingFace()
        {
            (var snapshotProvider, var faceDetector, var persistenceProvider, var logger, var subject) = CreateSubject();

            var functions = await subject.Create(logger);

            var image = A.Fake<IImage>();

            await functions.PersistFace(image);

            A.CallTo(() => image.Dispose()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task DisposeTheSourceImageAfterRecognisingFaces()
        {
            (var snapshotProvider, var faceDetector, var persistenceProvider, var logger, var subject) = CreateSubject();

            var functions = await subject.Create(logger);

            var image = A.Fake<IImage>();

            await functions.RecogniseFaces(image);

            A.CallTo(() => image.Dispose()).MustHaveHappenedOnceExactly();
        }
    }
}
