using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Beholder.Tests.Persistence.Provider
{
    [TestFixture, Category("Manual")]
    public class Should
    {
        private static readonly Beholder.Persistence.Configuration Configuration = new Beholder.Persistence.Configuration
        {
            ConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;",
            UnrecognisedFaceContainer = "test"
        };

        private Beholder.Persistence.Provider.Implementation CreateSubject()
        {
            var options = A.Fake<IOptions<Beholder.Persistence.Configuration>>(builder => builder.ConfigureFake(fake => A.CallTo(() => fake.Value).Returns(Configuration)));
            var logger = A.Fake<ILogger<Beholder.Persistence.Provider.Implementation>>();

            var subject = new Beholder.Persistence.Provider.Implementation(options, logger);

            return subject;
        }

        [Test, Ignore("Only to be run manually")]
        public async Task PersistToConfiguredStore()
        {
            var subject = CreateSubject();

            var recognition = A.Fake<IRecognition>(builder => builder
                .ConfigureFake(fake => A.CallTo(() => fake.Data).ReturnsLazily(call => Helper.GetManifestResourceByteArray("Beholder.Tests.Face.Detector.faces.jpg"))
            ));

            var uri = await subject.SaveRecognised("test", recognition);

            Assert.That(uri.ToString(), Is.Not.Empty);
        }
    }
}
