using System.Drawing;

namespace Beholder.Image
{
    public interface IFactory
    {
        IImage Create(Bitmap source);

        IImage Clone(IImage source);
    }
}
