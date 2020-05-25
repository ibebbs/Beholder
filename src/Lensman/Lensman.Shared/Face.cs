using Windows.UI.Xaml.Media.Imaging;

namespace Lensman.Shared
{
    public class Face
    {
        public Face(string name, BitmapImage bitmap)
        {
            Name = name;
            Bitmap = bitmap;
        }

        public string Name { get; }
        public BitmapImage Bitmap { get; }
    }
}
