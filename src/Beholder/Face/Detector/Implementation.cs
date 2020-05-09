using DlibDotNet;
using DlibDotNet.Extensions;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Rect = DlibDotNet.Rectangle;
using Rectangle = System.Drawing.Rectangle;

namespace Beholder.Face.Detector
{
    public class Implementation : IDetector
    {
        private FrontalFaceDetector _faceDetector;

        public Implementation()
        {
            _faceDetector = Dlib.GetFrontalFaceDetector();
        }

        private static Bitmap ExtractFace(Rect face, Bitmap source)
        {
            var target = new Bitmap((int)face.Width, (int)face.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(source, new Rectangle(0, 0, target.Width, target.Height), new Rectangle(face.Left, face.Top, (int)face.Width, (int)face.Height), GraphicsUnit.Pixel);
            }

            return target;
        }

        public Task<IEnumerable<Bitmap>> ExtractFaces(Bitmap source)
        {
            var dlibImage = source.ToArray2D<RgbPixel>();

            var faces = _faceDetector
                .Operator(dlibImage)
                .Select(face => ExtractFace(face, source))
                .ToArray();

            return Task.FromResult<IEnumerable<Bitmap>>(faces);
        }
    }
}
