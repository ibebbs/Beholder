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
                fake.PersistFace,
                fake.RecogniseFaces,
                fake.PersistFacialRecognition,
                fake.NotifyFacialRecognition
            );

            return (fake, subject);
        }

        [Test]
        public void InvokeTheFetchFunctionWhenFetchIsCalled()
        {
            (var fake, var subject) = CreateSubject();

            subject.Fetch();

            A.CallTo(() => fake.Fetch()).MustHaveHappenedOnceExactly();
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
        public void InvokeThePersistFacesFunctionWhenExtractFacesIsCalled()
        {
            (var fake, var subject) = CreateSubject();

            var image = A.Fake<IImage>();

            subject.PersistFace(image);

            A.CallTo(() => fake.PersistFace(image)).MustHaveHappenedOnceExactly();
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
        public void InvokeThePersistFacialRecognitionFunctionWhenPersistFacialRecognitionIsCalled()
        {
            (var fake, var subject) = CreateSubject();

            var recognition = A.Fake<IRecognition>();

            subject.PersistFacialRecognition(recognition);

            A.CallTo(() => fake.PersistFacialRecognition(recognition)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void InvokeTheNotifyFacialRecognitionFunctionWhenNotifyFacialRecognitionIsCalled()
        {
            (var fake, var subject) = CreateSubject();

            var recognition = A.Fake<IRecognition>();

            subject.NotifyFacialRecognition(recognition);

            A.CallTo(() => fake.NotifyFacialRecognition(recognition)).MustHaveHappenedOnceExactly();
        }
    }
}
