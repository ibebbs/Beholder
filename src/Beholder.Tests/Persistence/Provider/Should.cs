using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Beholder.Tests.Persistence.Provider
{
    [TestFixture]
    public class Should
    {
        private (Director.Client.IFacesClient, Beholder.Persistence.Provider.Implementation) CreateSubject()
        {
            var facesClient = A.Fake<Director.Client.IFacesClient>();
            var logger = A.Fake<ILogger<Beholder.Persistence.Provider.Implementation>>();

            var subject = new Beholder.Persistence.Provider.Implementation(facesClient, logger);

            return (facesClient, subject);
        }
    }
}
