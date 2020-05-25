using System;

namespace Beholder
{

    public class Meta
    {
        public Meta(DateTimeOffset at, string location)
        {
            At = at;
            Location = location;
        }

        public DateTimeOffset At { get; }

        public string Location { get; }
    }

    public interface IImage
    {
        Meta Meta { get; }

        byte[] Data { get; }
    }

    public class Image : IImage 
    {
        public Image(Meta meta, byte[] data)
        {
            Meta = meta;
            Data = data;
        }

        public Meta Meta { get; } 

        public byte[] Data { get; }
    }
}
