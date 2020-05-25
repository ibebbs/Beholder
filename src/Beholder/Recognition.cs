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

    public interface IPersisted
    {
        public string ImageUri { get; }
    }

    public interface IPersistedRecognition : IPersisted
    {
        public string Name { get; }

        public float Confidence { get; }
    }

    public class Recognition : Image, IRecognition
    {
        public Recognition(Meta meta, byte[] data, IEnumerable<Tag> tags) : base(meta, data)
        {
            Tags = tags;
        }

        public IEnumerable<Tag> Tags { get; }
    }

    public class Persisted : IPersisted
    {
        public Persisted(string imageUri)
        {
            ImageUri = imageUri;
        }

        public string ImageUri { get; }
    }

    public class PersistedRecognition : IPersistedRecognition
    {
        public PersistedRecognition(string name, float confidence, string imageUri)
        {
            Name = name;
            Confidence = confidence;
            ImageUri = imageUri;
        }

        public string Name { get; }

        public float Confidence { get; }

        public string ImageUri { get; }
    }
}
