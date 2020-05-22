using FakeItEasy;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Tests.Service.State.Provider
{
    [TestFixture]
    public class Should
    {
        private const string ConnectionString = "BlahBlahConnectionString";
        private const string ContainerName = "BlahBlahContainerName";

        public (Examiner.Persistence.IProvider, Examiner.Service.State.Provider) CreateSubject(Examiner.Service.Configuration configuration = null)
        {
            configuration ??= new Examiner.Service.Configuration
            {
                ConnectionString = ConnectionString,
                SnapshotContainer = ContainerName
            };

            var options = A.Fake<IOptions<Examiner.Service.Configuration>>(builder => builder.ConfigureFake(fake => A.CallTo(() => fake.Value).Returns(configuration)));

            var persistenceProvider = A.Fake<Examiner.Persistence.IProvider>();

            var subject = new Examiner.Service.State.Provider(persistenceProvider, options);

            return (persistenceProvider, subject);
        }

        [Test]
        public async Task ReturnDefaultSnapshotIfPersistedSnapshotDoesNotExist()
        {
            (var persistenceProvider, var subject) = CreateSubject();

            A.CallTo(() => persistenceProvider.BlobExistsAsync(ConnectionString, ContainerName, A<string>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(false));
            var snapshot = await subject.GetStateSnapshotAsync(CancellationToken.None);

            Assert.That(snapshot, Is.SameAs(Examiner.Service.State.Snapshot.Empty));
        }

        [Test]
        public async Task ReturnDeserializedSnapshotIfPersistedSnapshotExists()
        {
            (var persistenceProvider, var subject) = CreateSubject();

            A.CallTo(() => persistenceProvider.BlobExistsAsync(ConnectionString, ContainerName, A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(true));
            A.CallTo(() => persistenceProvider.GetBlobContentAsync(ConnectionString, ContainerName, A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult(Helper.GetManifestResourceStream("Examiner.Tests.Service.State.Provider.Snapshot.json")));

            var snapshot = await subject.GetStateSnapshotAsync(CancellationToken.None);

            Assert.That(snapshot.LastTrainingDataUpdate, Is.EqualTo(DateTimeOffset.Parse("2020-05-21T10:15:00")));
        }

        [Test]
        public async Task PersistedTheSpecifiedSnapshot()
        {
            (var persistenceProvider, var subject) = CreateSubject();

            var snapshot = new Examiner.Service.State.Snapshot { LastTrainingDataUpdate = DateTimeOffset.Parse("2020-05-21T10:15:00") };

            await subject.SetStateSnapshotAsync(snapshot, CancellationToken.None);

            A.CallTo(() => persistenceProvider.SetBlobContentAsync(ConnectionString, ContainerName, A<string>.Ignored, A<Func<Stream, Task>>.Ignored, A<CancellationToken>.Ignored));
        }
    }
}
