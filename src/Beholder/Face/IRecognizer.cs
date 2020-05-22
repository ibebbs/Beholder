namespace Beholder.Face
{
    public interface IRecognizer
    {
        IRecognition RecogniseFace(IImage image);
    }
}
