using FakeItEasy;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Beholder.Tests.Snapshot.Provider
{
    [TestFixture]
    public class Should
    {
        private const string SnapshotUri = "http://ipcam.local/tmpfs/snap.jpg?usr=admin&pwd=admin";

        private static Beholder.Snapshot.Configuration Configuration = new Beholder.Snapshot.Configuration
        {
            SnapshotUri = new Uri(SnapshotUri)
        };


        private Beholder.Snapshot.Provider.Implementation CreateSubject(HttpClient httpClient)
        {
            var options = A.Fake<IOptions<Beholder.Snapshot.Configuration>>(builder => builder.ConfigureFake(fake => A.CallTo(() => fake.Value).Returns(Configuration)));

            return new Beholder.Snapshot.Provider.Implementation(httpClient, options);
        }

        [Test]
        public async Task RequestSnapshotFromConfiguredUrl()
        {
            using (var stream = Helper.GetManifestResourceStream("Beholder.Tests.Snapshot.Provider.image.jpeg"))
            {
                var server = new MockHttpMessageHandler();
                server.Expect(SnapshotUri).Respond("image/jpeg", stream);

                var subject = CreateSubject(server.ToHttpClient());

                await subject.Get("Test");

                server.VerifyNoOutstandingExpectation();
            }
        }


        [Test]
        public void ThrowIfServerResponseWithNonSuccessErrorCode()
        {
            var server = new MockHttpMessageHandler();
            server.When("*").Respond(HttpStatusCode.NotFound);

            var subject = CreateSubject(server.ToHttpClient());

            Assert.ThrowsAsync<HttpRequestException>(() => subject.Get(string.Empty));
        }

        [Test]
        public async Task ReturnBitmapFromConfiguredUrl()
        {
            using (var stream = Helper.GetManifestResourceStream("Beholder.Tests.Snapshot.Provider.image.jpeg"))
            {
                var server = new MockHttpMessageHandler();
                server.Expect(SnapshotUri).Respond("image/jpeg", stream);

                var subject = CreateSubject(server.ToHttpClient());

                var image = await subject.Get(string.Empty);

                using (var received = new MemoryStream(image.Data))
                {
                    using (var bitmap = Bitmap.FromStream(received))
                    {
                        Assert.That(bitmap.Width, Is.EqualTo(626));
                        Assert.That(bitmap.Height, Is.EqualTo(626));
                    }
                }
            }
        }
    }
}
