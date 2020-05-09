using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beholder.Tests.Face.Detector
{
    [TestFixture]
    public class Should
    {
        private Beholder.Face.Detector.Implementation CreateSubject()
        {
            return new Beholder.Face.Detector.Implementation();
        }

        [Test]
        public async Task DetectFaces()
        {
            using (var stream = Helper.GetManifestResourceStream("Beholder.Tests.Face.Detector.faces.jpg"))
            {
                var subject = CreateSubject();

                var bitmap = new Bitmap(stream);

                var result = await subject.ExtractFaces(bitmap);

                Assert.That(result.Count(), Is.EqualTo(7));

                //int count = 0;
                //foreach (var image in result)
                //{
                //    image.Save($"D:\\Temp\\Detected\\Image{count++}.bmp");
                //}
            }
        }
    }
}
