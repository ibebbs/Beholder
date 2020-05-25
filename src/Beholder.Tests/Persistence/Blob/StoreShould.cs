using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Beholder.Tests.Persistence.Blob
{
    [TestFixture, Category("Manual")]
    public class StoreShould
    {
        private static readonly Beholder.Persistence.Configuration Configuration = new Beholder.Persistence.Configuration
        {
            BlobConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;",
            BlobContainer = "test"
        };

        private Beholder.Persistence.Blob.Store CreateSubject()
        {
            var options = A.Fake<IOptions<Beholder.Persistence.Configuration>>(builder => builder.ConfigureFake(fake => A.CallTo(() => fake.Value).Returns(Configuration)));
            var logger = A.Fake<ILogger<Beholder.Persistence.Blob.Store>>();

            var subject = new Beholder.Persistence.Blob.Store(options, logger);

            return subject;
        }

        [Test, Ignore("Only to be run manually")]
        public async Task PersistToConfiguredStore()
        {
            var subject = CreateSubject();

            var image = A.Fake<IImage>(builder => builder
                .ConfigureFake(fake => A.CallTo(() => fake.Data).ReturnsLazily(call => Helper.GetManifestResourceByteArray("Beholder.Tests.Face.Detector.faces.jpg"))
            ));

            var uri = await subject.SaveImage(image);

            Assert.That(uri.ToString(), Is.Not.Empty);
        }
    }
}
