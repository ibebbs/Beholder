using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Beholder.Tests.Service.Pipeline.Functions
{
    [TestFixture]
    public class Should
    {
        /// <summary>
        /// Fake IFunctions is used as a convenient container of functions which can be asserted
        /// </summary>
        /// <returns></returns>
        private (Beholder.Service.Pipeline.IFunctions, Beholder.Service.Pipeline.Functions.Implementation) CreateSubject()
        {
            var fake = A.Fake<Beholder.Service.Pipeline.IFunctions>();

            var subject = new Beholder.Service.Pipeline.Functions.Implementation(
                fake.Fetch,
                fake.ExtractFaces,
                fake.RecogniseFaces,
                fake.PersistRecognition,
                fake.NotifyRecognition
            );

            return (fake, subject);
        }

        [Test]
        public void InvokeTheFetchFunctionWhenFetchIsCalled()
        {
            (var fake, var subject) = CreateSubject();

            subject.Fetch("Test");

            A.CallTo(() => fake.Fetch("Test")).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void InvokeTheExtractFacesFunctionWhenExtractFacesIsCalled()
        {
            (var fake, var subject) = CreateSubject();

            var image = A.Fake<IImage>();

            subject.ExtractFaces(image);

            A.CallTo(() => fake.ExtractFaces(image)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void InvokeThePersistRecognitionFunctionWhenPersistRecognitionIsCalled()
        {
            (var fake, var subject) = CreateSubject();

            var recognition = A.Fake<IRecognition>();

            subject.PersistRecognition(recognition);

            A.CallTo(() => fake.PersistRecognition(recognition)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void InvokeTheRecogniseFacesFunctionWhenRecogniseFacesIsCalled()
        {
            (var fake, var subject) = CreateSubject();

            var image = A.Fake<IImage>();

            subject.RecogniseFaces(image);

            A.CallTo(() => fake.RecogniseFaces(image)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void InvokeTheNotifyFacialRecognitionFunctionWhenNotifyFacialRecognitionIsCalled()
        {
            (var fake, var subject) = CreateSubject();

            var recognition = A.Fake<IPersistedRecognition>();

            subject.NotifyRecognition(recognition);

            A.CallTo(() => fake.NotifyRecognition(recognition)).MustHaveHappenedOnceExactly();
        }
    }
}
