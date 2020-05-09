using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Beholder.Tests.Service.Pipeline
{
    [TestFixture]
    public class Should
    {
        private (Beholder.Service.Pipeline.IFunctions, Beholder.Image.IFactory, Beholder.Service.Pipeline.Implementation) CreateSubject(Beholder.Service.Configuration configuration = null)
        {
            var pipelineFunctions = A.Fake<Beholder.Service.Pipeline.IFunctions>();
            var imageFactory = A.Fake<Beholder.Image.IFactory>();
            var logger = A.Fake<ILogger<Beholder.Service.Pipeline.Implementation>>();
            var subject = new Beholder.Service.Pipeline.Implementation(pipelineFunctions, imageFactory, logger);

            return (pipelineFunctions, imageFactory, subject);
        }

        [Test]
        public void LimitQueuedStartCallsToTwo()
        {
            (var functions, var imageFactory, var subject) = CreateSubject();

            A.CallTo(() => functions.Fetch()).Returns(new TaskCompletionSource<IEnumerable<IImage>>().Task);

            var task1 = subject.StartAsync(CancellationToken.None);
            var task2 = subject.StartAsync(CancellationToken.None);
            var task3 = subject.StartAsync(CancellationToken.None);

            Assert.That(task1.IsCompleted, Is.True);
            Assert.That(task2.IsCompleted, Is.True);
            Assert.That(task3.IsCompleted, Is.False);
        }

        [Test]
        public async Task CallFetchWhenStartAsyncCalled()
        {
            (var functions, var imageFactory, var subject) = CreateSubject();

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            A.CallTo(() => functions.Fetch()).Invokes(call => tcs.SetResult(null));

            await subject.StartAsync(CancellationToken.None);

            await tcs.Task;

            A.CallTo(() => functions.Fetch()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task NotAllowConcurrentCallsToFetch()
        {
            (var functions, var imageFactory, var subject) = CreateSubject();

            A.CallTo(() => functions.Fetch()).Returns(new TaskCompletionSource<IEnumerable<IImage>>().Task);

            await subject.StartAsync(CancellationToken.None);
            await subject.StartAsync(CancellationToken.None);

            // Give TPL a chance to invoke the Fetch funtion twice
            await Task.Delay(TimeSpan.FromMilliseconds(10));

            A.CallTo(() => functions.Fetch()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task PassTheResultOfFetchToExtractFaces()
        {
            (var functions, var imageFactory, var subject) = CreateSubject();

            var image = A.Fake<IImage>();

            A.CallTo(() => functions.Fetch()).Returns(Task.FromResult(new[] { image }.AsEnumerable()));
            A.CallTo(() => functions.ExtractFaces(A<IImage>.Ignored)).Returns(Task.FromResult(Enumerable.Empty<IImage>()));

            await subject.StartAsync(CancellationToken.None);
            await subject.WaitForCompletion();

            A.CallTo(() => functions.ExtractFaces(image)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task CloneTheResultOfFetchToExtractFaces()
        {
            (var functions, var imageFactory, var subject) = CreateSubject();

            var fetchedImage = A.Fake<IImage>();
            var faceImage = A.Fake<IImage>();

            A.CallTo(() => functions.Fetch()).Returns(Task.FromResult(new[] { fetchedImage }.AsEnumerable()));
            A.CallTo(() => functions.ExtractFaces(fetchedImage)).Returns(Task.FromResult(new[] { faceImage }.AsEnumerable()));

            await subject.StartAsync(CancellationToken.None);
            await subject.WaitForCompletion();

            A.CallTo(() => imageFactory.Clone(faceImage)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task PassTheResultOfExtractFacesToPersistFaces()
        {
            (var functions, var imageFactory, var subject) = CreateSubject();

            var image = A.Fake<IImage>();

            A.CallTo(() => functions.Fetch()).Returns(Task.FromResult(new[] { image }.AsEnumerable()));
            A.CallTo(() => functions.ExtractFaces(A<IImage>.Ignored)).Returns(Task.FromResult(new[] { image }.AsEnumerable()));

            await subject.StartAsync(CancellationToken.None);
            await subject.WaitForCompletion();

            A.CallTo(() => functions.PersistFace(image)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task PassTheResultOfExtractFacesToRecogniseFaces()
        {
            (var functions, var imageFactory, var subject) = CreateSubject();

            var image = A.Fake<IImage>();

            A.CallTo(() => functions.Fetch()).Returns(Task.FromResult(new[] { image }.AsEnumerable()));
            A.CallTo(() => functions.ExtractFaces(A<IImage>.Ignored)).Returns(Task.FromResult(new[] { image }.AsEnumerable()));
            A.CallTo(() => functions.RecogniseFaces(A<IImage>.Ignored)).Returns(Task.FromResult(Enumerable.Empty<IRecognition>()));

            await subject.StartAsync(CancellationToken.None);
            await subject.WaitForCompletion();

            A.CallTo(() => functions.RecogniseFaces(A<IImage>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task PassTheResultOfRecogniseFacesToPersistFaces()
        {
            (var functions, var imageFactory, var subject) = CreateSubject();

            var image = A.Fake<IImage>();
            var recognition = A.Fake<IRecognition>();

            A.CallTo(() => functions.Fetch()).Returns(Task.FromResult(new[] { image }.AsEnumerable()));
            A.CallTo(() => functions.ExtractFaces(A<IImage>.Ignored)).Returns(Task.FromResult(new[] { image }.AsEnumerable()));
            A.CallTo(() => functions.RecogniseFaces(A<IImage>.Ignored)).Returns(Task.FromResult(new[] { recognition }.AsEnumerable()));

            await subject.StartAsync(CancellationToken.None);
            await subject.WaitForCompletion();

            A.CallTo(() => functions.PersistFacialRecognition(recognition)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task PassTheResultOfRecogniseFacesToNotifyFaces()
        {
            (var functions, var imageFactory, var subject) = CreateSubject();

            var image = A.Fake<IImage>();
            var recognition = A.Fake<IRecognition>();

            A.CallTo(() => functions.Fetch()).Returns(Task.FromResult(new[] { image }.AsEnumerable()));
            A.CallTo(() => functions.ExtractFaces(A<IImage>.Ignored)).Returns(Task.FromResult(new[] { image }.AsEnumerable()));
            A.CallTo(() => functions.RecogniseFaces(A<IImage>.Ignored)).Returns(Task.FromResult(new[] { recognition }.AsEnumerable()));

            await subject.StartAsync(CancellationToken.None);
            await subject.WaitForCompletion();

            A.CallTo(() => functions.NotifyFacialRecognition(recognition)).MustHaveHappenedOnceExactly();
        }
    }
}
