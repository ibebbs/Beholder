using System;

namespace Beholder
{
    public interface IImage
    {
        byte[] Data { get; }
    }

    public class Image : IImage 
    {
        public Image(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; }
    }
}
