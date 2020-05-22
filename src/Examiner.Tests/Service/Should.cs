using FakeItEasy;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Tests.Service
{
    [TestFixture]
    public class Should
    {
        private (Examiner.Service.State.IProvider, Examiner.Service.IProcessor, IHostApplicationLifetime, Examiner.Service.Implementation) CreateSubject()
        {
            var stateProvider = A.Fake<Examiner.Service.State.IProvider>();
            var processor = A.Fake<Examiner.Service.IProcessor>();
            var hostLifetime = A.Fake<IHostApplicationLifetime>();

            var subject = new Examiner.Service.Implementation(stateProvider, processor, hostLifetime);

            return (stateProvider, processor, hostLifetime, subject);
        }

        [Test]
        public async Task LoadCurrentState()
        {
            (var stateProvider, var processor, var hostLifetime, var subject) = CreateSubject();

            await subject.StartAsync(CancellationToken.None);

            A.CallTo(() => stateProvider.GetStateSnapshotAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task StartProcessingIfProcessingRequired()
        {
            (var stateProvider, var processor, var hostLifetime, var subject) = CreateSubject();

            A.CallTo(() => processor.RequiresProcessingAsync(A<Examiner.Service.State.Snapshot>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));

            await subject.StartAsync(CancellationToken.None);

            A.CallTo(() => processor.ProcessAsync(A<Examiner.Service.State.Snapshot>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task NotStartProcessingIfProcessingNotRequired()
        {
            (var stateProvider, var processor, var hostLifetime, var subject) = CreateSubject();

            A.CallTo(() => processor.RequiresProcessingAsync(A<Examiner.Service.State.Snapshot>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(false));

            await subject.StartAsync(CancellationToken.None);

            A.CallTo(() => processor.ProcessAsync(A<Examiner.Service.State.Snapshot>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public async Task PersistStateIfProcessingRequired()
        {
            (var stateProvider, var processor, var hostLifetime, var subject) = CreateSubject();

            A.CallTo(() => processor.RequiresProcessingAsync(A<Examiner.Service.State.Snapshot>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));

            await subject.StartAsync(CancellationToken.None);

            A.CallTo(() => stateProvider.SetStateSnapshotAsync(A<Examiner.Service.State.Snapshot>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task NotPersistStateIfProcessingNotRequired()
        {
            (var stateProvider, var processor, var hostLifetime, var subject) = CreateSubject();

            A.CallTo(() => processor.RequiresProcessingAsync(A<Examiner.Service.State.Snapshot>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(true));

            await subject.StartAsync(CancellationToken.None);

            A.CallTo(() => stateProvider.SetStateSnapshotAsync(A<Examiner.Service.State.Snapshot>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task TerminateTheApplication()
        {
            (var stateProvider, var processor, var hostLifetime, var subject) = CreateSubject();

            await subject.StartAsync(CancellationToken.None);

            A.CallTo(() => hostLifetime.StopApplication()).MustHaveHappenedOnceExactly();
        }
    }
}
