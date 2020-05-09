using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beholder.Tests.Service.Processor
{
    public class Should
    {
        private Beholder.Service.Processor.Implementation CreateSubject()
        {
            var logger = A.Fake<ILogger<Beholder.Service.Processor.Implementation>>();

            return new Beholder.Service.Processor.Implementation(logger);
        }

        [Test]
        public void CallPipelineStart()
        {
            var subject = CreateSubject();

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            var pipeline = A.Fake<Beholder.Service.IPipeline>();
            A.CallTo(() => pipeline.StartAsync(A<CancellationToken>.Ignored)).Returns(tcs.Task);

            var task = subject.RunAsync(pipeline, CancellationToken.None);

            A.CallTo(() => pipeline.StartAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void PassTheCancellationTokenToPipelineStart()
        {
            var subject = CreateSubject();

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            CancellationTokenSource cts = new CancellationTokenSource();

            var pipeline = A.Fake<Beholder.Service.IPipeline>();
            A.CallTo(() => pipeline.StartAsync(A<CancellationToken>.Ignored)).Returns(tcs.Task);

            var task = subject.RunAsync(pipeline, cts.Token);

            A.CallTo(() => pipeline.StartAsync(cts.Token)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void WaitForPipelineStartTaskToCompleteBeforeCallingItASecondTime()
        {
            var subject = CreateSubject();

            TaskCompletionSource<object> tcs1 = new TaskCompletionSource<object>();
            TaskCompletionSource<object> tcs2 = new TaskCompletionSource<object>();

            var pipeline = A.Fake<Beholder.Service.IPipeline>();
            A.CallTo(() => pipeline.StartAsync(A<CancellationToken>.Ignored)).Returns(tcs1.Task).Once().Then.Returns(tcs2.Task);

            var task = subject.RunAsync(pipeline, CancellationToken.None);

            A.CallTo(() => pipeline.StartAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();

            tcs1.SetResult(null);

            A.CallTo(() => pipeline.StartAsync(A<CancellationToken>.Ignored)).MustHaveHappenedTwiceExactly();
        }

        [Test]
        public void StopCallingPipelineStartWhenCancellationTokenCancelled()
        {
            var subject = CreateSubject();

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            CancellationTokenSource cts = new CancellationTokenSource();

            var pipeline = A.Fake<Beholder.Service.IPipeline>();
            A.CallTo(() => pipeline.StartAsync(A<CancellationToken>.Ignored)).Returns(tcs.Task);

            var task = subject.RunAsync(pipeline, cts.Token);

            A.CallTo(() => pipeline.StartAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();

            cts.Cancel();

            tcs.SetResult(null);

            A.CallTo(() => pipeline.StartAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void CallWaitForCompletionAfterCancellationTokenCancelled()
        {
            var subject = CreateSubject();

            TaskCompletionSource<object> startTask = new TaskCompletionSource<object>();
            TaskCompletionSource<object> completionTask = new TaskCompletionSource<object>();
            CancellationTokenSource cts = new CancellationTokenSource();

            var pipeline = A.Fake<Beholder.Service.IPipeline>();
            A.CallTo(() => pipeline.StartAsync(A<CancellationToken>.Ignored)).Returns(startTask.Task);
            A.CallTo(() => pipeline.WaitForCompletion()).Returns(completionTask.Task);

            var task = subject.RunAsync(pipeline, cts.Token);

            cts.Cancel();

            startTask.SetResult(null);

            A.CallTo(() => pipeline.WaitForCompletion()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void WaitForPipelineCompletionAfterCancellationTokenCancelled()
        {
            var subject = CreateSubject();

            TaskCompletionSource<object> startTask = new TaskCompletionSource<object>();
            TaskCompletionSource<object> completionTask = new TaskCompletionSource<object>();
            CancellationTokenSource cts = new CancellationTokenSource();

            var pipeline = A.Fake<Beholder.Service.IPipeline>();
            A.CallTo(() => pipeline.StartAsync(A<CancellationToken>.Ignored)).Returns(startTask.Task);
            A.CallTo(() => pipeline.WaitForCompletion()).Returns(completionTask.Task);

            var task = subject.RunAsync(pipeline, cts.Token);

            cts.Cancel();

            startTask.SetResult(null);

            Assert.That(task.IsCompleted, Is.False);

            completionTask.SetResult(null);

            Assert.That(task.IsCompleted, Is.True);
        }
    }
}
