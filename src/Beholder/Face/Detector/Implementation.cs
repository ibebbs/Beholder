using DlibDotNet;
using DlibDotNet.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Rect = DlibDotNet.Rectangle;
using Rectangle = System.Drawing.Rectangle;

namespace Beholder.Face.Detector
{
    public class Implementation : IDetector
    {
        private readonly FrontalFaceDetector _faceDetector;
        private readonly ILogger<Implementation> _logger;

        public Implementation(ILogger<Implementation> logger)
        {
            _faceDetector = Dlib.GetFrontalFaceDetector();
            _logger = logger;
        }

        private static byte[] ExtractFace(Rect face, Bitmap source)
        {
            using (var target = new Bitmap((int)face.Width, (int)face.Height))
            {
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(source, new Rectangle(0, 0, target.Width, target.Height), new Rectangle(face.Left, face.Top, (int)face.Width, (int)face.Height), GraphicsUnit.Pixel);
                }

                using (var memoryStream = new MemoryStream())
                {
                    target.Save(memoryStream, ImageFormat.Png);

                    return memoryStream.ToArray();
                }
            }
        }

        public Task<IEnumerable<IImage>> ExtractFaces(IImage image)
        {
            using (var stream = new MemoryStream(image.Data))
            {
                using (var bitmap = new Bitmap(stream))
                {
                    using (var dlibImage = bitmap.ToArray2D<RgbPixel>())
                    {
                        var faces = _faceDetector
                            .Operator(dlibImage)
                            .Select(face => ExtractFace(face, bitmap))
                            .Select(data => new Image(image.Meta, data))
                            .ToArray();

                        _logger.LogInformation("Found {0} faces", faces.Length);

                        return Task.FromResult<IEnumerable<IImage>>(faces);
                    }
                }
            }
        }
    }
}
