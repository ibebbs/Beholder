using System.Drawing;

namespace Beholder.Face
{
    public interface IRecognizer
    {
        IRecognition RecogniseFace(Bitmap source);
    }
}
