using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Beholder.Tests
{
    public class Should
    {
        public static IEnumerable<TestCaseData> ServiceTests
        {
            get
            {
                yield return new TestCaseData(typeof(Beholder.Face.IDetector))
                    .Returns(typeof(Beholder.Face.Detector.Implementation));
                yield return new TestCaseData(typeof(Beholder.Face.IRecognizer))
                    .Returns(typeof(Beholder.Face.Recognizer.Implementation));
                yield return new TestCaseData(typeof(Beholder.Persistence.IProvider))
                    .Returns(typeof(Beholder.Persistence.Provider.Implementation));
                yield return new TestCaseData(typeof(Beholder.Snapshot.IProvider))
                    .Returns(typeof(Beholder.Snapshot.Provider.Implementation));
                yield return new TestCaseData(typeof(Beholder.Service.Pipeline.Functions.IFactory))
                    .Returns(typeof(Beholder.Service.Pipeline.Functions.Factory.Implementation));
                yield return new TestCaseData(typeof(Beholder.Service.Pipeline.IFactory))
                    .Returns(typeof(Beholder.Service.Pipeline.Factory.Implementation));
            }
        }

        private static readonly Dictionary<string, string> Configuration = new Dictionary<string, string>
        {
            { "Snapshot:SnapshotUri", "http://ipcam.local/cgi?snapshot" },
            { "Face:Recognizer:ModelUri", "http://azurite.local:10000/devstoreaccount1/model/Model.zip" }
        };

        [Test, TestCaseSource(nameof(ServiceTests))]
        public Type BeAbleToResolve(Type serviceType)
        {
            var hostContext = new HostBuilderContext(new Dictionary<object, object>())
            {
                Configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(Configuration)
                    .Build()
            };

            var serviceProvider = Beholder.Program
                .ConfigureServices(hostContext, new ServiceCollection(), false)
                .BuildServiceProvider();

            var service = serviceProvider.GetService(serviceType);

            return service.GetType();
        }
    }
}
