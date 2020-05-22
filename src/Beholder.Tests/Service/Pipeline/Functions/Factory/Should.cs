using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
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
            var faceRecognizer = A.Fake<Beholder.Face.IRecognizer>();
            var persistenceProvider = A.Fake<Beholder.Persistence.IProvider>();
            var logger = A.Fake<ILogger>();
            var subject = new Beholder.Service.Pipeline.Functions.Factory.Implementation(snapshotProvider, faceDetector, faceRecognizer, persistenceProvider);

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

            var image = A.Fake<IImage>();
            await functions.ExtractFaces(image);

            A.CallTo(() => faceDetector.ExtractFaces(image)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task UseThePersistenceProviderToSaveRecognition()
        {
            (var snapshotProvider, var faceDetector, var persistenceProvider, var logger, var subject) = CreateSubject();

            var functions = await subject.Create(logger);

            var recognition = A.Fake<IRecognition>();
            await functions.PersistRecognition(recognition);

            A.CallTo(() => persistenceProvider.SaveRecognition(recognition)).MustHaveHappenedOnceExactly();
        }
    }
}
