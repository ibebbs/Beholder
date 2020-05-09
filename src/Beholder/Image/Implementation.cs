using System.Drawing;

namespace Beholder.Image
{
    public class Implementation : IImage
    {
        public Implementation(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        public void Dispose()
        {
            Bitmap.Dispose();
        }

        public Bitmap Bitmap { get; }
    }
}
