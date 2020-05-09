using System.Collections.Generic;
using System.Drawing;

namespace Beholder
{
    public class Tag
    {
        public string Name { get; }

        public float Confidence { get; }
    }

    public interface IRecognition
    {
        Bitmap Source { get; }

        IEnumerable<Tag> Tags { get; }
    }

    public class Recognition : IRecognition
    {
        public Bitmap Source { get; }

        public IEnumerable<Tag> Tags { get; }
    }
}
