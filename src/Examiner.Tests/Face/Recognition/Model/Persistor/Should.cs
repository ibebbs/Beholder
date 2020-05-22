using FakeItEasy;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Tests.Face.Recognition.Model.Persistor
{
    [TestFixture]
    public class Should
    {
        private const string ConnectionString = "BlahBlahConnectionString";
        private const string CategorizedFacesContainerName = "BlahBlahCategorizedFacesName";
        private const string FaceRecognitionModelContainerName = "BlahBlahFaceRecognitionModelName";

        public (Examiner.Persistence.IProvider, Examiner.Face.Recognition.Model.Persistor subject) CreateSubject(Examiner.Face.Configuration configuration = null)
        {
            configuration ??= new Examiner.Face.Configuration
            {
                ConnectionString = ConnectionString,
                CategorizedFacesContainerName = CategorizedFacesContainerName,
                FaceRecognitionModelContainerName = FaceRecognitionModelContainerName
            };

            var options = A.Fake<IOptions<Examiner.Face.Configuration>>(builder => builder.ConfigureFake(fake => A.CallTo(() => fake.Value).Returns(configuration)));
            var persistenceProvider = A.Fake<Examiner.Persistence.IProvider>();
            var subject = new Examiner.Face.Recognition.Model.Persistor(persistenceProvider, options);

            return (persistenceProvider, subject);
        }

        [Test]
        public async Task PersistToConfiguredContainer()
        {
            (var persistenceProvider, var subject) = CreateSubject();

            var model = A.Fake<Examiner.Face.Recognition.IModel>();

            await subject.SaveModelAsync(model, CancellationToken.None);

            A.CallTo(() => persistenceProvider.SetBlobContentAsync(ConnectionString, FaceRecognitionModelContainerName, A<string>.Ignored, A<Func<Stream, Task>>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }
    }
}
