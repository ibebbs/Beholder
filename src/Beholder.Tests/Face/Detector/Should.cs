using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Beholder.Tests.Face.Detector
{
    [TestFixture]
    public class Should
    {
        private Beholder.Face.Detector.Implementation CreateSubject()
        {
            var logger = A.Fake<ILogger<Beholder.Face.Detector.Implementation>>();
            return new Beholder.Face.Detector.Implementation(logger);
        }

        [Test]
        public async Task DetectFaces()
        {
            var subject = CreateSubject();

            var image = A.Fake<IImage>(builder => builder
                .ConfigureFake(fake => A.CallTo(() => fake.Data).ReturnsLazily(call => Helper.GetManifestResourceByteArray("Beholder.Tests.Face.Detector.faces.jpg"))
            ));

            var result = await subject.ExtractFaces(image);

            Assert.That(result.Count(), Is.EqualTo(7));
        }
    }
}
