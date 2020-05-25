using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private (Beholder.Service.Pipeline.IFunctions, Beholder.Service.Pipeline.Implementation) CreateSubject(Beholder.Service.Configuration configuration = null)
        {
            configuration ??= new Beholder.Service.Configuration();

            var options = A.Fake<IOptions<Beholder.Service.Configuration>>(builder => builder.ConfigureFake(fake => A.CallTo(() => fake.Value).Returns(configuration)));
            var pipelineFunctions = A.Fake<Beholder.Service.Pipeline.IFunctions>();
            var logger = A.Fake<ILogger<Beholder.Service.Pipeline.Implementation>>();
            var subject = new Beholder.Service.Pipeline.Implementation(pipelineFunctions, options, logger);

            return (pipelineFunctions, subject);
        }

        [Test]
        public void LimitQueuedStartCallsToTwo()
        {
            (var functions, var subject) = CreateSubject();

            A.CallTo(() => functions.Fetch(A<string>.Ignored)).Returns(new TaskCompletionSource<IEnumerable<IImage>>().Task);

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
            (var functions, var subject) = CreateSubject();

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            A.CallTo(() => functions.Fetch(A<string>.Ignored)).Invokes(call => tcs.SetResult(null));

            await subject.StartAsync(CancellationToken.None);

            await tcs.Task;

            A.CallTo(() => functions.Fetch(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task NotAllowConcurrentCallsToFetch()
        {
            (var functions, var subject) = CreateSubject();

            A.CallTo(() => functions.Fetch(A<string>.Ignored)).Returns(new TaskCompletionSource<IEnumerable<IImage>>().Task);

            await subject.StartAsync(CancellationToken.None);
            await subject.StartAsync(CancellationToken.None);

            // Give TPL a chance to invoke the Fetch funtion twice
            await Task.Delay(TimeSpan.FromMilliseconds(10));

            A.CallTo(() => functions.Fetch(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task PassTheResultOfFetchToExtractFaces()
        {
            (var functions, var subject) = CreateSubject();

            var image = A.Fake<IImage>();

            A.CallTo(() => functions.Fetch(A<string>.Ignored)).Returns(Task.FromResult(new[] { image }.AsEnumerable()));
            A.CallTo(() => functions.ExtractFaces(A<IImage>.Ignored)).Returns(Task.FromResult(Enumerable.Empty<IImage>()));

            await subject.StartAsync(CancellationToken.None);
            await subject.WaitForCompletion();

            A.CallTo(() => functions.ExtractFaces(image)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task PassTheResultOfExtractFacesToRecogniseFaces()
        {
            (var functions, var subject) = CreateSubject();

            var image = A.Fake<IImage>();

            A.CallTo(() => functions.Fetch(A<string>.Ignored)).Returns(Task.FromResult(new[] { image }.AsEnumerable()));
            A.CallTo(() => functions.ExtractFaces(A<IImage>.Ignored)).Returns(Task.FromResult(new[] { image }.AsEnumerable()));

            await subject.StartAsync(CancellationToken.None);
            await subject.WaitForCompletion();

            A.CallTo(() => functions.RecogniseFaces(image)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task PassTheResultOfRecogniseFacesToPersistRecognition()
        {
            (var functions, var subject) = CreateSubject();

            var recognition = A.Fake<IRecognition>();

            A.CallTo(() => functions.Fetch(A<string>.Ignored)).Returns(Task.FromResult(new[] { A.Fake<IImage>() }.AsEnumerable()));
            A.CallTo(() => functions.ExtractFaces(A<IImage>.Ignored)).Returns(Task.FromResult(new[] { A.Fake<IImage>() }.AsEnumerable()));
            A.CallTo(() => functions.RecogniseFaces(A<IImage>.Ignored)).Returns(Task.FromResult(new[] { recognition }.AsEnumerable()));

            await subject.StartAsync(CancellationToken.None);
            await subject.WaitForCompletion();

            A.CallTo(() => functions.PersistRecognition(recognition)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task PassTheResultOfPersistRecognitionToNotifyRecognition()
        {
            (var functions, var subject) = CreateSubject();

            var persistedRecognition = A.Fake<IPersistedRecognition>();

            A.CallTo(() => functions.Fetch(A<string>.Ignored)).Returns(Task.FromResult(new[] { A.Fake<IImage>() }.AsEnumerable()));
            A.CallTo(() => functions.ExtractFaces(A<IImage>.Ignored)).Returns(Task.FromResult(new[] { A.Fake<IImage>() }.AsEnumerable()));
            A.CallTo(() => functions.RecogniseFaces(A<IImage>.Ignored)).Returns(Task.FromResult(new[] { A.Fake<IRecognition>() }.AsEnumerable()));
            A.CallTo(() => functions.PersistRecognition(A<IRecognition>.Ignored)).Returns(Task.FromResult(persistedRecognition));

            await subject.StartAsync(CancellationToken.None);
            await subject.WaitForCompletion();

            A.CallTo(() => functions.NotifyRecognition(persistedRecognition)).MustHaveHappenedOnceExactly();
        }
    }
}
