using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Tests.Service.Processor
{
    [TestFixture]
    public class Should
    {
        private (Examiner.Face.Data.IProvider, Examiner.Face.Recognition.IEngine, Examiner.Face.Recognition.Model.IPersistor, Examiner.Service.Processor.Implementation) CreateSubject()
        {
            var faceDataProvider = A.Fake<Examiner.Face.Data.IProvider>();
            var recognitionEngine = A.Fake<Examiner.Face.Recognition.IEngine>();
            var modelPersistor = A.Fake<Examiner.Face.Recognition.Model.IPersistor>();

            var subject = new Examiner.Service.Processor.Implementation(faceDataProvider, recognitionEngine, modelPersistor);

            return (faceDataProvider, recognitionEngine, modelPersistor, subject);
        }

        [Test]
        public async Task RequireProcessingIfSnapshotLastUpdateIsOlderThanLatestFaceData()
        {
            (var faceDataProvider, var recognitionEngine, var modelPersistor, var subject) = CreateSubject();

            A.CallTo(() => faceDataProvider.GetDateOfLastChangeAsync(A<CancellationToken>.Ignored)).Returns(new DateTimeOffset(2020, 02, 17, 0, 0, 0, TimeSpan.Zero));

            var snapshot = new Examiner.Service.State.Snapshot { LastTrainingDataUpdate = new DateTimeOffset(2020, 02, 15, 0, 0, 0, TimeSpan.Zero) };

            var result = await subject.RequiresProcessingAsync(snapshot, CancellationToken.None);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task NotRequireProcessingIfSnapshotLastUpdateIsEqualToLatestFaceData()
        {
            (var faceDataProvider, var recognitionEngine, var modelPersistor, var subject) = CreateSubject();

            A.CallTo(() => faceDataProvider.GetDateOfLastChangeAsync(A<CancellationToken>.Ignored)).Returns(new DateTimeOffset(2020, 02, 17, 0, 0, 0, TimeSpan.Zero));

            var snapshot = new Examiner.Service.State.Snapshot { LastTrainingDataUpdate = new DateTimeOffset(2020, 02, 17, 0, 0, 0, TimeSpan.Zero) };

            var result = await subject.RequiresProcessingAsync(snapshot, CancellationToken.None);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetBalancedFaceDataWhenProcessing()
        {
            (var faceDataProvider, var recognitionEngine, var modelPersistor, var subject) = CreateSubject();

            var snapshot = new Examiner.Service.State.Snapshot { LastTrainingDataUpdate = new DateTimeOffset(2020, 02, 17, 0, 0, 0, TimeSpan.Zero) };

            var result = await subject.ProcessAsync(snapshot, CancellationToken.None);

            A.CallTo(() => faceDataProvider.GetBalancedItems(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task TrainRecognitionModelOnBalancedDataWhenProcessing()
        {
            (var faceDataProvider, var recognitionEngine, var modelPersistor, var subject) = CreateSubject();

            var snapshot = new Examiner.Service.State.Snapshot { LastTrainingDataUpdate = new DateTimeOffset(2020, 02, 17, 0, 0, 0, TimeSpan.Zero) };
            var balancedItems = A.Fake<IAsyncEnumerable<Examiner.Face.Data.Item>>();

            A.CallTo(() => faceDataProvider.GetBalancedItems(A<CancellationToken>.Ignored)).Returns(balancedItems);

            var result = await subject.ProcessAsync(snapshot, CancellationToken.None);

            A.CallTo(() => recognitionEngine.TrainAsync(balancedItems, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task PersistRecognitionModelWhenProcessing()
        {
            (var faceDataProvider, var recognitionEngine, var modelPersistor, var subject) = CreateSubject();

            var snapshot = new Examiner.Service.State.Snapshot { LastTrainingDataUpdate = new DateTimeOffset(2020, 02, 17, 0, 0, 0, TimeSpan.Zero) };
            var model = A.Fake<Examiner.Face.Recognition.IModel>();

            A.CallTo(() => recognitionEngine.TrainAsync(A<IAsyncEnumerable<Examiner.Face.Data.Item>>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(model));

            var result = await subject.ProcessAsync(snapshot, CancellationToken.None);

            A.CallTo(() => modelPersistor.SaveModelAsync(model, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}
