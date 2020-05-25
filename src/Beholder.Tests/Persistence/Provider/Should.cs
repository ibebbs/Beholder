using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Beholder.Tests.Persistence.Provider
{
    [TestFixture]
    public class Should
    {

        private (Beholder.Persistence.Blob.IStore, Beholder.Persistence.Data.IStore, Beholder.Persistence.Provider.Implementation) CreateSubject()
        {
            var blobStore = A.Fake<Beholder.Persistence.Blob.IStore>();
            var dataStore = A.Fake<Beholder.Persistence.Data.IStore>();
            var logger = A.Fake<ILogger<Beholder.Persistence.Provider.Implementation>>();

            var subject = new Beholder.Persistence.Provider.Implementation(blobStore, dataStore, logger);

            return (blobStore, dataStore, subject);
        }

        [Test]
        public async Task UseBlobStoreToPersistImage()
        {
            (var blobStore, var dataStore, var subject) = CreateSubject();

            var recognition = A.Fake<IRecognition>();

            await subject.Save(recognition);

            A.CallTo(() => blobStore.SaveImage(recognition)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task UseDataStoreToPersistRecognition()
        {
            (var blobStore, var dataStore, var subject) = CreateSubject();

            var uri = new Uri("http://blobstore.local/face.png");
            var recognition = A.Fake<IRecognition>();
            A.CallTo(() => blobStore.SaveImage(recognition)).Returns(uri);

            await subject.Save(recognition);

            A.CallTo(() => dataStore.SaveRecognition(recognition, uri)).MustHaveHappenedOnceExactly();
        }
    }
}
