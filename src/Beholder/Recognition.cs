using System.Collections.Generic;

namespace Beholder
{
    public class Tag
    {
        public Tag(string name, float confidence)
        {
            Name = name;
            Confidence = confidence;
        }

        public string Name { get; }

        public float Confidence { get; }
    }

    public interface IRecognition : IImage
    {
        IEnumerable<Tag> Tags { get; }
    }

    public interface IPersistedRecognition : IRecognition
    {
        public string ImageUri { get; }
    }

    public class Recognition : Image, IRecognition
    {
        public Recognition(byte[] data, IEnumerable<Tag> tags) : base(data)
        {
            Tags = tags;
        }

        public IEnumerable<Tag> Tags { get; }
    }

    public class PersistedRecognition : Recognition, IPersistedRecognition
    {
        public PersistedRecognition(byte[] buffer, IEnumerable<Tag> tags, string imageUri) : base(buffer, tags)
        {
            ImageUri = imageUri;
        }

        public string ImageUri { get; }
    }
}
