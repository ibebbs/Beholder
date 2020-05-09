using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Beholder.Tests.Service
{
    [TestFixture]
    public class Should
    {
        private (Beholder.Service.Pipeline.IFactory, Beholder.Service.IProcessor, Beholder.Service.Implementation) CreateSubject(Beholder.Service.Configuration configuration = null)
        {
            var logger = A.Fake<ILogger<Beholder.Service.Implementation>>();
            var pipelineFactory = A.Fake<Beholder.Service.Pipeline.IFactory>();
            var processor = A.Fake<Beholder.Service.IProcessor>();
            var subject = new Beholder.Service.Implementation(pipelineFactory, processor, logger);

            return (pipelineFactory, processor, subject);
        }

        [Test]
        public async ValueTask ConstructAPipelineWhenStarted()
        {
            (var pipelineFactory, var processor, var subject) = CreateSubject();

            await subject.StartAsync(CancellationToken.None);

            A.CallTo(() => pipelineFactory.CreateAsync()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async ValueTask RunTheProcessorWhenStarted()
        {
            (var pipelineFactory, var processor, var subject) = CreateSubject();

            var pipeline = A.Fake<Beholder.Service.IPipeline>();
            A.CallTo(() => pipelineFactory.CreateAsync()).Returns(Task.FromResult(pipeline));

            await subject.StartAsync(CancellationToken.None);

            A.CallTo(() => processor.RunAsync(pipeline, A<CancellationToken>.Ignored)).MustHaveHappened();
        }

        [Test]
        public async ValueTask CancelTheCancellationTokenWhenStopped()
        {
            (var pipelineFactory, var processor, var subject) = CreateSubject();

            CancellationToken cancellationToken = default;

            A.CallTo(() => processor.RunAsync(A<Beholder.Service.IPipeline>.Ignored, A<CancellationToken>.Ignored)).Invokes(call => cancellationToken = call.GetArgument<CancellationToken>(1));

            await subject.StartAsync(CancellationToken.None);
            await subject.StopAsync(CancellationToken.None);

            Assert.That(cancellationToken.IsCancellationRequested, Is.True);
        }

        [Test]
        public async ValueTask WaitForPipelineToCompleteWhenStopped()
        {
            (var pipelineFactory, var processor, var subject) = CreateSubject();

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            A.CallTo(() => processor.RunAsync(A<Beholder.Service.IPipeline>.Ignored, A<CancellationToken>.Ignored)).Returns(tcs.Task);

            await subject.StartAsync(CancellationToken.None);

            var stopping = subject.StopAsync(CancellationToken.None);

            Assert.That(stopping.IsCompleted, Is.False);

            tcs.SetResult(null);

            Assert.That(stopping.IsCompleted, Is.True);
        }

        [Test]
        public async ValueTask WaitForCancellationTokenWhenStopped()
        {
            (var pipelineFactory, var processor, var subject) = CreateSubject();

            CancellationTokenSource cts = new CancellationTokenSource();
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            A.CallTo(() => processor.RunAsync(A<Beholder.Service.IPipeline>.Ignored, A<CancellationToken>.Ignored)).Returns(tcs.Task);

            await subject.StartAsync(CancellationToken.None);

            var stopping = subject.StopAsync(cts.Token);

            Assert.That(stopping.IsCompleted, Is.False);

            cts.Cancel();

            Assert.That(stopping.IsCompleted, Is.True);
        }
    }
}
