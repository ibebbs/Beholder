using System.Drawing;

namespace Beholder.Image.Factory
{
    public class Implementation : IFactory
    {
        public IImage Create(Bitmap source)
        {
            return new Image.Implementation(source);
        }

        public IImage Clone(IImage source)
        {
            return Create((Bitmap)source.Data.Clone());
        }
    }
}
